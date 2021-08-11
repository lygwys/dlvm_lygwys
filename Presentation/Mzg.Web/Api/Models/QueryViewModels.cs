﻿using System;
using System.Collections.Generic;

namespace Mzg.Web.Models
{
    public class QueryViewLayoutConfigModel
    {
        public List<object> SortColumns { get; set; }
        public List<object> Rows { get; set; }
        public List<Guid> ClientResources { get; set; }
    }
}