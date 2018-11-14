using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalieriCore.Loader;
using SalieriCore.Corpus;
using SalieriCore.WordCloud;

namespace SalieriCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start Salieri");
            Console.WriteLine("Scraping !!");
            GoogleLoader loader = new GoogleLoader();
            loader.keywords = "disclosure management software";
            Task<string> result = loader.LoadAsync();
            result.Wait();

            Console.WriteLine("Generating Corpus");
            Corpus corpus = new Corpus(loader.Contents);
            //Console.WriteLine(corpus.MergedText);

            WcGenerator wc = new WcGenerator();
            wc.Content = corpus.MergedText;
            wc.Analyze();
            Console.WriteLine(wc.GetTagListByString());


            Console.WriteLine("Completed");
            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }
    }
}
