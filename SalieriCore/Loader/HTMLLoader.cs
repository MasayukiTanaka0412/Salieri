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
    public class HTMLLoader :WebLoader
    {
        public string sourceURL { get; set; }
        public HTMLLoader() : base()
        {

        }

        public string Load()
        {
            Console.WriteLine("Load start");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            var doc = default(IHtmlDocument);
            using (var client = new HttpClient())
            {
                Task<Stream> t = client.GetStreamAsync(new Uri(sourceURL));
                t.Wait();
                Stream stream = t.Result;

                Console.WriteLine("Loading!!");
                // AngleSharp.Parser.Html.HtmlParserオブジェクトにHTMLをパースさせる
                var parser = new HtmlParser();
                Task<IHtmlDocument> task = parser.ParseAsync(stream);
                task.Wait();
                doc = task.Result;

                Contents = new List<ContentDao>();

                IHtmlCollection<IElement> resultcollection = doc.GetElementsByTagName("a");
                foreach (IElement resultElement in resultcollection)
                {
                    Debug.WriteLine(resultElement.GetAttribute("href"));
                    ContentDao content = new ContentDao();
                    content.URL = resultElement.GetAttribute("href");
                    Contents.Add(content);
                }
                stream.Close();
            }
            LoadPages();
            return "success";
        }


    }
}

