using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Videos.Models
{
    public class MoviesView
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        public List<SelectListItem> Sources { get; set; }

        public SelectListItem PlaySources { get; set; }


        public List<SelectListItem> Line { get; set; }

        public SelectListItem PlayLine{ get; set; }

        public string Stars { get; set; }
        public string PlayUrl { get; set; }

        public string Description { get; set; }

    }
}
