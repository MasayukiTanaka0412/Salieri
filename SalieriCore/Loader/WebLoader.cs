using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using System.Diagnostics;
using SalieriCore.Dao;
using System.IO;
using System.Net;
using System.Threading;
using NMeCab;
using System.Web.Hosting;

namespace SalieriCore.Loader
{
    public abstract class WebLoader
    {
        public ICollection<ContentDao> Contents { get; set; }

        public MeCabTagger Tagger { get; set; }

        public WebLoader()
        {
            MeCabParam param = new MeCabParam();
            param.DicDir = HostingEnvironment.ApplicationPhysicalPath + @"App_Data\ipadic";
            Tagger = MeCabTagger.Create(param);
        }


        protected void LoadPages()
        {
            List<Task> tasks = new List<Task>();
            Dictionary<Task<string>, ContentDao> taskDic = new Dictionary<Task<string>, ContentDao>();
            //ServicePointManager.DefaultConnectionLimit = 200;
            foreach (ContentDao content in Contents)
            {
                if (!(string.IsNullOrEmpty(content.URL) || content.URL.Contains(",pdf")))
                {
                    Console.WriteLine("Loading!! " + content.URL);
                    var client = new HttpClient();
                    try
                    {
                        client.Timeout = TimeSpan.FromSeconds(20);
                        //Task<Stream> resp = client.GetStreamAsync(new Uri(content.URL));
                        Task<string> resp = client.GetStringAsync(new Uri(content.URL));
                        tasks.Add(resp);
                        taskDic.Add(resp, content);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error!! " + e.Message);
                    }
                }
            }

            Task[] taskArray = tasks.ToArray();
            tasks = null;
            Task t = Task.WhenAll(taskArray);
            try
            {
                Console.WriteLine("Waiting load tasks to complete");
                t.Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error!! " + e.Message);
            }

            List<Task> parsTasks = new List<Task>();
            //foreach (Task<string> task in tasks)
            for(int i =0; i < taskArray.Length;i++)
            {
                Task<string> task = (Task<string>)taskArray[i];
                if (!(task.IsCanceled || task.IsFaulted))
                {
                    ContentDao content = taskDic[task];
                    var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                    var token = cts.Token;

                    var pT = Task.Run(() =>
                    {
                        Console.WriteLine("Parsing!! " + content.URL);
                        try
                        {
                            var doc = default(IHtmlDocument);
                            var parser = new HtmlParser();
                            
                            doc = parser.Parse(task.Result);
                            IHtmlCollection<IElement> elements = doc.GetElementsByTagName("body");
                            foreach (IElement element in elements)
                            {
                                if (!element.InnerHtml.Contains("%PDF"))
                                {
                                    content.Content = content.Content + " " + removeTag(element.InnerHtml);
                                }
                            }
                            doc.Dispose();
                            doc = null;
                            parser = null;
                            Tokenize(content);
                            taskDic.Remove(task);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error!! " + e.Message);
                        }
                    }, token);
                    taskArray[i] = null;
                    //pT.Wait();
                    parsTasks.Add(pT);
                }
            }
            Task t2 = Task.WhenAll(parsTasks.ToArray());
            t2.Wait();
        }

        protected string removeTag(string intext)
        {
            string newStr = intext;
            newStr = Regex.Replace(newStr, @"\r", string.Empty);
            newStr = Regex.Replace(newStr, @"\n", string.Empty);
            //<script.*?>.*?</script>
            newStr = Regex.Replace(newStr, @"<script.*?>.*?</script>", string.Empty, RegexOptions.IgnoreCase);
            newStr = Regex.Replace(newStr, @"<style.*?>.*?</style>", string.Empty, RegexOptions.IgnoreCase);
            newStr = Regex.Replace(newStr, @"<.*?>", string.Empty);
            newStr = Regex.Replace(newStr, @"&.+?;", string.Empty);
            //newStr = Regex.Replace(newStr, @"^\s*\n", string.Empty,RegexOptions.Multiline);
            //newStr = Regex.Replace(newStr, @"^$", string.Empty, RegexOptions.Multiline);
            newStr = Regex.Replace(newStr, @"\s{2,}", " ");
            return newStr;
        }

        protected void Tokenize(ContentDao dao)
        {
            //Tagger.OutPutFormatType = "wakati";
            //string result = Tagger.Parse(dao.Content);
            //dao.Content = result;

            string result = string.Empty;
            MeCabNode node = Tagger.ParseToNode(dao.Content);
            while (node != null)
            {
                if (node.CharType > 0)
                {
                    //Console.WriteLine(node.Surface + "\t" + node.Feature);
                    string feature = node.Feature;
                    if (!(feature.StartsWith("助") ||
                            feature.StartsWith("フィラー") ||
                            feature.StartsWith("記号") ||
                            feature.Contains("接尾") ||
                            feature.Contains("非自立") ||
                            feature.Contains("接頭") ||
                            feature.StartsWith("その他")
                        ))
                    {
                        if (node.Surface.Length > 1 && !"する".Equals(node.Surface))
                        {
                            result = result + node.Surface + " ";
                        }
                    }
                }
                node = node.Next;
            }

            dao.Content = result;

        }
    }
}
