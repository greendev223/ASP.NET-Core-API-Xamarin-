using System;
using System.Collections.Generic;
using System.Text;

namespace MukaiTablet2.Model
{
    public class RssData
    {
        public static readonly string RSS_URL = "http://blog.livedoor.jp/sugiura214/index.rdf";

        public string Title { get; set; }

        public string PublishDate { get; set; }

        public string PublishTime { get; set; }

        public string FeedUrl{ get; set; }
    }
}
