
using H.EF.Core;

namespace Videos.Entities
{
    public class UrlSource : BaseDBEntity
    {
        public string Id { get; set; }

        public string Url { get; set; }

        public string MovieId { get; set; }

        public string VideoSource { get; set; }
        public string Resource { get; set; }
    }
}
