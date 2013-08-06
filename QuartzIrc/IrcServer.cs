using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuartzIrc
{
    public class IrcServer
    {
        public Boolean Secure
        {
            get;
            set;
        }

        public String Host
        {
            get;
            set;
        }

        public int Port
        {
            get;
            set;
        }

        public String NetworkName
        {
            get;
            set;
        }

        public String ServerName
        {
            get;
            set;
        }


    }
}
