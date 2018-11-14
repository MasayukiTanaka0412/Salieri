using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalieriCore.Dao;

namespace SalieriCore.Corpus
{
    public class  Corpus
    {
       
        public string MergedText { get; set; }
        public Corpus(ICollection<ContentDao> daoCollection)
        {
            foreach(ContentDao dao in daoCollection)
            {
                if (!string.IsNullOrEmpty(dao.Content))
                {
                    MergedText = MergedText + " " + dao.Content;
                }
            }
        }
    }
}
