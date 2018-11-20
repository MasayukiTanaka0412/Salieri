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
        static void Main(string[] args)
        {
            string mergedTextFile = @"c:\temp\merged.txt";
            string trainedFile = @"c:\temp\trained.bin";

            GoogleLoader loader = new GoogleLoader();
            if (args.Length >0)
            {
                loader.keywords = args[0];
            }
            else
            {
                Console.WriteLine("Input keywords and press enter");
                loader.keywords = Console.ReadLine();
            }

            Console.WriteLine("Start Salieri");
            Console.WriteLine("Scraping !!");
            
            //loader.keywords = "disclosure management software";
            string result = loader.Load();
            

            Console.WriteLine("Generating Corpus");
            Corpus corpus = new Corpus(loader.Contents);
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
                if(string.IsNullOrEmpty(searchkey))
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
    }
}
