using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuartzIrc
{
    /// <summary>
    /// Representation of a user on IRC
    /// </summary>
    public class User
    {
        #region Fields
        /// <summary>
        /// Channels this user is known to be in. This is effectively channels that the user and the client have in common.
        /// </summary>
        public Dictionary<String, Channel> Channels;
        #endregion

        #region Properties
        
        /// <summary>
        /// The user's best known hostmask. Ident and host are not guaranteed to be accurate.
        /// </summary>
        public Hostmask Hostmask
        {
            get;
            set;
        }

        /// <summary>
        /// User's current nickname
        /// </summary>
        public String Nick
        {
            get
            {
                return Hostmask.Nick;
            }
            set
            {
                Hostmask.Nick = value;
            }
        }

        /// <summary>
        /// User's most recent known ident, not guaranteed to be accurate.
        /// </summary>
        public String Ident
        {
            get
            {
                return Hostmask.Ident;
            }
            set
            {
                Hostmask.Ident = value;
            }
        }

        /// <summary>
        /// User's most recently known hostname, not guaranteed to be accurate.
        /// </summary>
        public String Host
        {
            get
            {
                return Hostmask.Host;
            }
            set
            {
                Hostmask.Host = value;
            }
        }
        #endregion

        #region Constructor
        #endregion

        #region Members

        public override String ToString()
        {
            return Hostmask;
        }

        public static implicit operator string(User u)
        {
            return u.ToString();
        }

        #endregion

        #region Events
        #endregion

        #region EventHandlers
        #endregion
    }
}
