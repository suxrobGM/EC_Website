﻿using System;
using System.Collections.Generic;
using SuxrobGM.Sdk.Utils;
using EC_WebSite.Models.UserModel;

namespace EC_WebSite.Models.Blog
{
    public class Comment
    {
        public Comment()
        {
            Id = GeneratorId.GenerateLong();
            CreatedTime = DateTime.Now;
            Replies = new List<Comment>();
        }

        public string Id { get; set; }
        public string Text { get; set; }
        public DateTime? CreatedTime { get; set; }
        public string AuthorId { get; set; }
        public virtual User Author { get; set; }
        public string ArticleId { get; set; }
        public virtual Article Article { get; set; }
        public string ParentId { get; set; }
        public virtual Comment Parent { get; set; }
        public virtual ICollection<Comment> Replies { get; set; }       
    }
}