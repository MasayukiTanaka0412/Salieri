using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;

namespace SalieriSlackExtractor
{
    class Program
    {
        static string outputDir;
        static string endpoint;
        static string token;

        static void Main(string[] args)
        {
            if(args.Length < 3)
            {
                Console.WriteLine("Please specify endpoint, token, and channels");
                Console.WriteLine("Press Enter to exit");
                Console.ReadLine();
                return;
            }

            List<string> channels = new List<string>();
            outputDir = args[0];
            endpoint = args[1];
            token = args[2];
            Console.WriteLine(outputDir);
            Console.WriteLine(endpoint);
            Console.WriteLine(token);

            for (int i=3; i<args.Length; i++)
            {
                channels.Add(args[i]);
            }

            foreach (string channel in channels)
            {
                Console.WriteLine(channel);
                LoadData(channel);
            }


            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();

        }

        private static void LoadData(string channel)
        {
            var client = new HttpClient();
            try
            {
                string url = endpoint + "?token=" + token + "&channel=" + channel + "&count=1000";
                Console.WriteLine(url);

                Task<string> resp = client.GetStringAsync(new Uri(url));
                resp.Wait();
                string result = resp.Result;
                Console.WriteLine(result);

                SlackResponse slackresp = JsonConvert.DeserializeObject<SlackResponse>(result);
                foreach(SlackMessage msg in slackresp.messages)
                {
                    try
                    {
                        string filename = msg.client_msg_id + ".txt";
                        using (StreamWriter sw = new StreamWriter(Path.Combine(outputDir,filename)))
                        {
                            sw.WriteLine("Date:" + msg.Timestamp.ToLongDateString() +" " + msg.Timestamp.ToLongTimeString() );
                            sw.WriteLine("User:" + msg.user);
                            sw.WriteLine();
                            sw.WriteLine(msg.text);
                        }
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("Error!! " + e.Message);
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Error!! " + e.Message);
            }
        }
    }

    public class SlackResponse
    {
        public string ok { get; set; }
        public IList<SlackMessage> messages { get; set; }

    }

    public class SlackMessage
    {
        public string client_msg_id { get; set;}
        public string type { get; set; }
        public string ts { get; set; }
        public string user { get; set; }
        public string text { get; set; }

        public DateTime Timestamp
        {
            get
            {
                DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                return UNIX_EPOCH.AddSeconds(double.Parse(ts)).ToLocalTime();
            }
        }
    }
}
