using SalieriCore.Word2VecHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Word2Vec.Net;
using System.IO;

namespace SalieriWeb.Controllers.api
{
    public class Word2VecController : ApiController
    {
        public IEnumerable<string> Get()
        {
            return new string[] { "Plese set keywords" };
        }

        public IEnumerable<BestWord> Get(string keyword)
        {
            string trainfilepath = Path.GetTempPath() + "corpus.txt";
            string outfilepath = Path.GetTempPath() + "trained.bin";

            W2VHelper w2CHelper = new W2VHelper();
            w2CHelper.TrainFile = trainfilepath;
            w2CHelper.OutputFile = outfilepath;
            w2CHelper.Train();

            return w2CHelper.GetBestWordsByDistance(keyword);
        }
    }
}
