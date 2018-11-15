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

            Console.WriteLine("Start Salieri");
            Console.WriteLine("Scraping !!");
            GoogleLoader loader = new GoogleLoader();
            loader.keywords = "disclosure management software";
            Task<string> result = loader.LoadAsync();
            result.Wait();

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
            W2CHelper w2CHelper = new W2CHelper();
            w2CHelper.TrainFile = mergedTextFile;
            w2CHelper.OutputFile = trainedFile;
            w2CHelper.Train();

            Console.WriteLine("Completed");
            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }
    }
}
