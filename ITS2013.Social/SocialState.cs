using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS2013.Social
{
    public class SocialState
    {
        public string ClientId { get; set; }
        public string AccessTokenQuery { get; set; }
        public SocialUser Me { get; set; }
    }
}
