using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model
{
    public class ExecuteParams
    {
        public ExecuteParams(long? page = null, Post? post = null)
        {
            Page = page;
            Post = post;
        }

        public Post? Post { get; set; }
        public long? Page { get; set; }
    }
}
