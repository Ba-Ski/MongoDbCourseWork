using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbApplication.Structure
{
    class Comment
    {
        public string author { get; set; }
        public string content { get; set; }
        public DateTime date { get; set; }
    }
}
