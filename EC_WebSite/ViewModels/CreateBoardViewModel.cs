﻿using EC_WebSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EC_WebSite.ViewModels
{
    public class CreateBoardViewModel
    {
        public string BoardName { get; set; }
        public ForumHeader Forum { get; set; }
    }
}
