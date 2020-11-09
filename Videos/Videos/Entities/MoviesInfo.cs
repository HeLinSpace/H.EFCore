using H.EF.Core;
using System;

namespace Videos.Entities
{
    public class MoviesInfo : BaseDBEntity
    {
        public string Name { get; set; }

        public string Stars { get; set; }

        public string Type { get; set; }

        public string Area { get; set; }

        public string Year { get; set; }

        public string ImageUrl { get; set; }

        public string Description { get; set; }

        public string Resource { get; set; }

        public DateTime CreateTime { get; set; }

    }
}
