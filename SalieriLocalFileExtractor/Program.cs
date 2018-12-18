using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using org.apache.tika.metadata;
using TikaOnDotNet.TextExtraction;

namespace SalieriLocalFileExtractor
{
    class Program
    {
        static string inputDirectory;
        static string outputDirectory;

        static void Main(string[] args)
        {
            if(args.Length < 2)
            {
                Console.WriteLine("Please set input_dir and output_dir");
                Console.WriteLine("Press Enter to exit");
                Console.ReadLine();
                return;
            }

            inputDirectory = args[0];
            outputDirectory = args[1];

            Console.WriteLine("Input " + inputDirectory);
            Console.WriteLine("Output " + outputDirectory);
            //Console.WriteLine("Press Enter to execute");
            //Console.ReadLine();

            ReadFiles(inputDirectory);

        }

        private static void ReadFiles(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    Console.WriteLine(path);
                    var textExtractionResult = new TextExtractor().Extract(path, CreateCustomResult);
                    //Console.WriteLine(textExtractionResult.Metadata.ToString());
                    //Console.WriteLine(textExtractionResult.Text);
                    //Console.ReadLine();
                    using (StreamWriter sw = new StreamWriter(outputDirectory + @"\" + Path.GetFileName(path) + ".txt"))
                    {
                        foreach(string key in textExtractionResult.Metadata.Keys)
                        {
                            //sw.WriteLine("key:" + textExtractionResult.Metadata[key]);
                            foreach(string metavalue in textExtractionResult.Metadata[key])
                            {
                                sw.WriteLine(key + ":" + metavalue);
                            }
                        }

                        sw.WriteLine(textExtractionResult.Text);
                        sw.Flush();
                        sw.Close();
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            if (Directory.Exists(path))
            {
                foreach (string filename in Directory.GetFiles(path))
                {
                    ReadFiles(filename);
                }

                foreach (string dirname in Directory.GetDirectories(path))
                {
                    ReadFiles(dirname);
                }

            }
        }

        public static CustomResult CreateCustomResult(string text, Metadata metadata)
        {
            var metaDataDictionary = metadata.names().ToDictionary(name => name, metadata.getValues);
            return new CustomResult
            {
                Metadata = metaDataDictionary,
                Text = text,
            };
        }

    }

    public class CustomResult
    {
        public string Text { get; set; }
        public IDictionary<string, string[]> Metadata { get; set; }
    }
}
