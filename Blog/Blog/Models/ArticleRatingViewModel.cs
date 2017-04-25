using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    public class ArticleRatingViewModel
    {
        public int ArticleId { get; set; }

        public string User { get; set; }

        public int Rating { get; set; }
    }
}