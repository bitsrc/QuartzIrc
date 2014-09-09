using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuartzIrc
{
    /// <summary>
    /// Arguments for an IRC event
    /// </summary>
    public class IrcEventArgs
    {

        #region Accessors

        /// <summary>
        /// The source of the event. Typically a server (guardian.il.us.seersirc.net) or a user (Nick or Nick!ident@host)
        /// </summary>
        public String Sender
        {
            get;
            private set;
        }

        /// <summary>
        /// The intended recipient of the event, typically a user (Nick) or channel (#channel)
        /// </summary>
        public String Target
        {
            get;
            private set;
        }

        /// <summary>
        /// Arguments received from the event
        /// </summary>
        public String[] Args
        {
            get;
            private set;
        }

        public String FullCommand
        {
            get;
            private set;
        }

        #endregion

        #region Constructor


        /// <summary>
        /// It's a pretty standard constructor
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="target">The recipient of the event</param>
        /// <param name="args">Parameters of the event</param>
        public IrcEventArgs(String sender, String target, String[] args)
        {
            Sender = sender;
            Target = target;
            Args = args;
        }

        public IrcEventArgs(String sender, String target, String[] args, String fullCommand)
        {
            Sender = sender;
            Target = target;
            Args = args;
            FullCommand = fullCommand;
        }

        #endregion
    }
}
