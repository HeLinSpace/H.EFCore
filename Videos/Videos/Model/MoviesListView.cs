using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Videos.Models
{
    public class MoviesListView
    {
        public string MoviesContent { get; set; }

        public int Page { get; set; }

        public int Records { get; set; }

        public int Total { get; set; }
    }
}
