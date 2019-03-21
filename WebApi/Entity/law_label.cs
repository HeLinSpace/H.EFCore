using H.EF.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Entity
{
    public class law_label : BaseDBEntity
    {
        public string created_by { get; set; }
        public string updated_by { get; set; }
        public string remark { get; set; }
        public DateTime created_time { get; set; }
        public DateTime updated_time { get; set; }
        public int is_delete { get; set; }
        public string law_id { get; set; }
        public string label { get; set; }
        public string type { get; set; }
    }
}
