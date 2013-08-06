using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quartz
{
    public class Mode
    {

        #region Accessors

        public Boolean Remove
        {
            get;
            set;
        }
        public String Value
        {
            get;
            set;
        }
        public String Param
        {
            get;
            set;
        }

        #endregion

        #region Constructor

        public Mode(String value, String param)
        {
            Value = value;
            Param = param;
            Remove = false;
        }

        public Mode(String value, String param, Boolean remove)
        {
            Value = value;
            Param = param;
            Remove = remove;
        }

        #endregion
    }
}
