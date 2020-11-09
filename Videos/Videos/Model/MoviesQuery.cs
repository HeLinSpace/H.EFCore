using H.EF.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Videos.Models
{
    public class MoviesQuery: BaseQuery
    {
        public int page { get; set; } = 1;
    }
}
