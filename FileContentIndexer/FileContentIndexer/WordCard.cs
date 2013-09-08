using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileContentIndexer
{
    public class WordCard
    {
        public string Key { get; set; }

        public IEnumerable<string> Values { get; set; }
    }
}
