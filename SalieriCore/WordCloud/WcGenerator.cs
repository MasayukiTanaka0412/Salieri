using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sparc.TagCloud;

namespace SalieriCore.WordCloud
{
    public class WcGenerator
    {
        public string Content { get; set; }

        private IEnumerable<TagCloudTag> tagList;

        public void Analyze()
        {
            var phrases = new List<string>();
            phrases.Add(Content);
            tagList = new TagCloudAnalyzer().ComputeTagCloud(phrases);
        }

        public IEnumerable<TagCloudTag> GetTagList()
        {
            return this.tagList;
        }

        public string GetTagListByString()
        {
            return string.Join(
                Environment.NewLine,
                tagList.Select(p => "[" + p.Count + "] \t" + p.Text).ToArray());
        }

    }
}
