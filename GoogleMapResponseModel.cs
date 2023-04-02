using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleMapPostCode
{
    public class distanceResponse
    {

        public class distance
        {
            public string text { get; set; } = string.Empty;
            public int value { get; set; } = 0;
        }

        public class duration
        {
            public string text { get; set; } = string.Empty;
            public int value { get; set; } = 0;

        }

        public class element
        {
            public distance? distance{ get; set; }
            public duration? duration { get; set; }
            public string? status { get; set; }

        }
        public class row
        {
            public element[]? elements { get; set; }
        }


        public string[]? destination_addresses { get; set; }
        public string[]? origin_addresses { get; set; }
        public row[]? rows { get; set; }
        public string? status { get; set; }

    }
}
