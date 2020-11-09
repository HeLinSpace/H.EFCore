using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Videos.Models
{
    public class MoviesSyncView
    {
        public List<SelectListItem> Sources { get; set; }

        public SelectListItem SelectSources { get; set; }

        public string PageFrom { get; set; }

        public string PageTo { get; set; }

    }

    public class MoviesSyncQuery
    {
        public string Source { get; set; }

        public int? PageFrom { get; set; }

        public int? PageTo { get; set; }
    }

}
