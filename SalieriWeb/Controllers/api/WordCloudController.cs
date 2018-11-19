using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using SalieriCore.Loader;
using SalieriCore.Corpus;
using SalieriCore.WordCloud;
using SalieriCore.Word2VecHelper;
using SalieriWeb.Models;
using Sparc.TagCloud;

namespace SalieriWeb.Controllers.api
{
    
    public class WordCloudController : ApiController
    {
        // GET: api/WordCloud
        public IEnumerable<string> Get()
        {
            return new string[] { "Plese set keywords"};
        }

        // GET: api/WordCloud/5
        public IEnumerable<TagCloudTag> Get(string keyword)
        {
            GoogleLoader loader = new GoogleLoader();
            loader.keywords = loader.keywords = keyword;
            string result = loader.Load();
            //result.Wait();

            Corpus corpus = new Corpus(loader.Contents);
            WcGenerator wc = new WcGenerator();
            wc.Content = corpus.MergedText;
            wc.Analyze();

            return wc.GetTagList();
        }

        
    }
}
