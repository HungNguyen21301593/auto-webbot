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
        public ExecuteParams(Setting? setting = null, long? page = null, Post? post = null)
        {
            Setting = setting;
            Page = page;
            Post = post;
        }

        public Post? Post { get; set; }
        public long? Page { get; set; }
        public Setting? Setting { get; set; }
    }
}
