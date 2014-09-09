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

        /// <summary>
        /// The command invoked to cause this event
        /// </summary>
        public String Command
        {
            get;
            private set;
        }

        /// <summary>
        /// The full text of the command message
        /// </summary>
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

        /// <summary>
        /// Constructor for a command event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="target"></param>
        /// <param name="args"></param>
        /// <param name="command"></param>
        /// <param name="fullCommand"></param>
        public IrcEventArgs(String sender, String target, String[] args, String command, String fullCommand)
        {
            Sender = sender;
            Target = target;
            Args = args;
            Command = command;
            FullCommand = fullCommand;
        }

        #endregion
    }
}
