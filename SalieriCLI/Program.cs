using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalieriCore.Loader;
using SalieriCore.Corpus;
using SalieriCore.WordCloud;
using SalieriCore.Word2VecHelper;
using System.IO;

namespace SalieriCLI
{
    class Program
    {
        static string mergedTextFile = @"c:\temp\merged.txt";
        static string trainedFile = @"c:\temp\trained.bin";

        static void Main(string[] args)
        {
            Corpus corpus = null;
            if (args.Length <2 )
            {
                Console.WriteLine("Usage SalieriCLI Google keywords");
                Console.WriteLine("Or SalieriCLI HTML uri");
                Console.WriteLine("Press Enter to Exit");
                Console.ReadLine();
            }
            else
            {
                if ("Google".Equals(args[0]))
                {
                    corpus =LoadFromGoogle(args[1]);
                }
                else if ("HTML".Equals(args[0])){
                    corpus = LoadFromHTML(args[1]);
                }
            }

            //Console.WriteLine(corpus.MergedText);
            using (StreamWriter sw = new StreamWriter(mergedTextFile))
            {
                sw.WriteLine(corpus.MergedText);
                sw.WriteLine("");
            }


            WcGenerator wc = new WcGenerator();
            wc.Content = corpus.MergedText;
            wc.Analyze();
            Console.WriteLine(wc.GetTagListByString());

            Console.WriteLine("Vectorizing!!");
            W2VHelper w2CHelper = new W2VHelper();
            w2CHelper.TrainFile = mergedTextFile;
            w2CHelper.OutputFile = trainedFile;
            w2CHelper.Train();

            Console.WriteLine("Completed");

            while (true)
            {
                Console.WriteLine("Enter keyword to analyze and press enter");
                string searchkey = Console.ReadLine();
                if (string.IsNullOrEmpty(searchkey))
                {
                    break;
                }
                IEnumerable<string> bestwords = w2CHelper.GetBestWordsByDistanceInStringFormat(searchkey);
                foreach (string s in bestwords)
                {
                    Console.WriteLine(s);
                }

                Console.WriteLine("Enter keyword to analyze and press enter, or just press enter to exit");
            }
            Console.ReadLine();

        }

        private static Corpus LoadFromGoogle(string keywords)
        {
            GoogleLoader loader = new GoogleLoader();
            loader.keywords = keywords;

            Console.WriteLine("Start Salieri");
            Console.WriteLine("Scraping !!");

            //loader.keywords = "disclosure management software";
            string result = loader.Load();


            Console.WriteLine("Generating Corpus");
            Corpus corpus = new Corpus(loader.Contents);
            return corpus;
            
        }

        private static Corpus LoadFromHTML(string url)
        {
            Console.WriteLine("Loading from URL " + url);
            HTMLLoader loader = new HTMLLoader();
            loader.sourceURL = url;
            string result = loader.Load();

            Console.WriteLine("Generating Corpus");
            Corpus corpus = new Corpus(loader.Contents);
            return corpus;
        }
    }
}
