using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SalieriCSVCreator
{
    class Program
    {
        static string inputDirectory;
        static string outputFile;

        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Please set input_dir and output_dir");
                Console.WriteLine("Press Enter to exit");
                Console.ReadLine();
                return;
            }

            inputDirectory = args[0];
            outputFile = args[1];

            Console.WriteLine("Input " + inputDirectory);
            Console.WriteLine("Output " + outputFile);

            using (StreamWriter sw = new StreamWriter(outputFile))
            {
                ReadAndMergeFiles(inputDirectory,sw);
            }

            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();
        }

        private static void ReadAndMergeFiles(string path, StreamWriter sw)
        {
            if (File.Exists(path))
            {
                try
                {
                    Console.WriteLine(path);
                    using (StreamReader sr = new StreamReader(path))
                    {
                        string content = sr.ReadToEnd();
                        content = content.Replace("\r", " ");
                        content = content.Replace("\n", " ");
                        content = content.Replace(",", " ");
                        content = content.Replace("\"", " ");
                        content = content.Replace("'", " ");

                        string filename = Path.GetFileName(path);

                        string line = filename + "," + content;
                        sw.WriteLine(line);

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            if (Directory.Exists(path))
            {
                foreach (string filename in Directory.GetFiles(path))
                {
                    ReadAndMergeFiles(filename,sw);
                }

                foreach (string dirname in Directory.GetDirectories(path))
                {
                    ReadAndMergeFiles(dirname,sw);
                }

            }
        }
    }
}
