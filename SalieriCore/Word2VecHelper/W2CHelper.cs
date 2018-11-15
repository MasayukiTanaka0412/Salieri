using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Word2Vec.Net;

namespace SalieriCore.Word2VecHelper
{
    public class W2CHelper
    {
        private Word2VecBuilder builder;

        public string TrainFile { get; set; }

        public string OutputFile { get; set; }

        public Word2Vec.Net.Word2Vec W2C { get; set; }

        public W2CHelper()
        {
            builder = Word2VecBuilder.Create();
            
        }

        public void Train()
        {
            builder.WithTrainFile(TrainFile);
            builder.WithOutputFile(OutputFile);
            builder.WithDebug(2);
            builder.WithBinary(1);

            W2C = builder.Build();
            W2C.TrainModel();
        }

        public BestWord[] GetBestWordsByDistance(string keyword)
        {
            var distance = new Distance(OutputFile);
            BestWord[] bestwords = distance.Search(keyword);
            return bestwords;
        }

        public IEnumerable<string> GetBestWordsByDistanceInStringFormat(string keyword)
        {
            
            BestWord[] bestwords = GetBestWordsByDistance(keyword);
            List<string> result = new List<string>();
            foreach(BestWord best in bestwords)
            {
                result.Add(best.Word + "\t" + best.Distance);
            }

            return result;
        }
    }
}
