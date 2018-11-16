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

namespace SalieriCore.Loader
{
    public class GoogleLoader
    {
        static string baseURL = "https://www.google.co.jp/search?num=100&lr=lang_en&q=";
        public string keywords { get; set;}

        public ICollection<ContentDao> Contents { get; set; }

        public GoogleLoader() { }

        public async Task<string> LoadAsync()
        {
            Console.WriteLine("Load Async start");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            var doc = default(IHtmlDocument);
            using (var client = new HttpClient())
            using (var stream = await client.GetStreamAsync(new Uri(baseURL + HttpUtility.UrlEncode(keywords))))
            {
                Console.WriteLine("Loading!!");
                // AngleSharp.Parser.Html.HtmlParserオブジェクトにHTMLをパースさせる
                var parser = new HtmlParser();
                doc = await parser.ParseAsync(stream);

                Contents = new List<ContentDao>();

                IHtmlCollection<IElement> resultcollection = doc.GetElementsByClassName("r");
                foreach(IElement resultElement in resultcollection)
                {
                    Debug.WriteLine(resultElement.InnerHtml);
                    ContentDao content = new ContentDao();

                    MatchCollection mc = Regex.Matches(resultElement.InnerHtml, "q=(?<url>.+)&amp;sa");
                    if(mc.Count> 0)
                    {
                        content.URL = mc[0].Groups["url"].Value;   
                    }
                    Contents.Add(content);
                }
            }
            LoadPages();
            return "success";
        }

        public string Load()
        {
            Console.WriteLine("Load start");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            var doc = default(IHtmlDocument);
            using (var client = new HttpClient())
            {
                Task<Stream> task = client.GetStreamAsync(new Uri(baseURL + HttpUtility.UrlEncode(keywords)));
                task.Wait();
                var parser = new HtmlParser();
                Task<IHtmlDocument>task2 = parser.ParseAsync(task.Result);
                task2.Wait();
                doc = task2.Result;

                Contents = new List<ContentDao>();

                IHtmlCollection<IElement> resultcollection = doc.GetElementsByClassName("r");
                foreach (IElement resultElement in resultcollection)
                {
                    Debug.WriteLine(resultElement.InnerHtml);
                    ContentDao content = new ContentDao();

                    MatchCollection mc = Regex.Matches(resultElement.InnerHtml, "q=(?<url>.+)&amp;sa");
                    if (mc.Count > 0)
                    {
                        content.URL = mc[0].Groups["url"].Value;
                    }
                    Contents.Add(content);
                }
            }
            
            LoadPages();
            return "success";
        }

        private void LoadPages()
        {
            foreach(ContentDao content in Contents)
            {
                if (!(string.IsNullOrEmpty(content.URL) || content.URL.Contains(",pdf")))
                {
                    try
                    {
                        using (var client = new HttpClient())
                        {
                            Task<Stream> resp = client.GetStreamAsync(new Uri(content.URL));
                            resp.Wait();

                            var doc = default(IHtmlDocument);
                            var parser = new HtmlParser();
                            doc = parser.Parse(resp.Result);
                            IHtmlCollection<IElement> elements = doc.GetElementsByTagName("body");
                            foreach (IElement element in elements)
                            {
                                if (!element.InnerHtml.Contains("%PDF"))
                                {
                                    content.Content = content.Content + " " + removeTag(element.InnerHtml);
                                }
                            }
                            resp.Result.Close();

                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Error:" + e.Message);
                        Debug.WriteLine(e.StackTrace);
                    }
                }
            }
        }

        private string removeTag(string intext)
        {
            string newStr = intext;
            newStr = Regex.Replace(newStr, @"\r", string.Empty);
            newStr = Regex.Replace(newStr, @"\n", string.Empty);
            //<script.*?>.*?</script>
            newStr = Regex.Replace(newStr, @"<script.*?>.*?</script>", string.Empty,RegexOptions.IgnoreCase);
            newStr = Regex.Replace(newStr, @"<style.*?>.*?</style>", string.Empty, RegexOptions.IgnoreCase );
            newStr = Regex.Replace(newStr, @"<.*?>", string.Empty);
            newStr = Regex.Replace(newStr, @"&.+?;", string.Empty);
            //newStr = Regex.Replace(newStr, @"^\s*\n", string.Empty,RegexOptions.Multiline);
            //newStr = Regex.Replace(newStr, @"^$", string.Empty, RegexOptions.Multiline);
            newStr = Regex.Replace(newStr, @"\s{2,}", " ");
            return newStr;
        }

    }
}
