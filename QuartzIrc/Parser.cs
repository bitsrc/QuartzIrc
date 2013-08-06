using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quartz
{
    /// <summary>
    /// Parses IRC commands into prefix|command|parameters
    /// </summary>
    static class Parser
    {
        
        /// <summary>
        /// Parses an IRC command
        /// </summary>
        /// <param name="message">The raw command from the IRC server</param>
        /// <returns>An IrcMessage object containing the prefix, command, and parameters</returns>
        static public IrcMessage ParseIrcMessage(String message)
        {
            String prefix = String.Empty;
            String command = String.Empty;
            String[] parameters = new String[] { };
            String trailing = String.Empty;

            //We add one to skip the space for a prefixed command, so this will set a non-prefixed to 0
            int prefixEnd = -1;
            int trailingStart;
            String[] boom;

            //Logger.Trace("Parsing: {0}", message);

            //Messages that have a prefix begin with :
            // :prophet.tx.us.seersirc.net 376 TestBot :End of /MOTD command.
            // ^
            if (message.StartsWith(":"))
            {
                //Prefix ends at the first space
                prefixEnd = message.IndexOf(' ');
                prefix = message.Substring(1, prefixEnd - 1);
            }

            //Parameters will start with a " :"
            // :prophet.tx.us.seersirc.net 376 TestBot :End of /MOTD command.
            //                                        ^
            trailingStart = message.IndexOf(" :");
            if (trailingStart > 0)
            {
                trailing = message.Substring(trailingStart + 2);
            }
            else
            {
                trailingStart = message.Length;
            }

            //Split the remaining string
            // :prophet.tx.us.seersirc.net 376 TestBot :End of /MOTD command.
            //                             |<-------->|
            boom = message.Substring(prefixEnd + 1, trailingStart - prefixEnd - 1).Split(' ');
            
            //Command will always be the first item
            command = boom.First();

            //Grab additional parameters if present
            if (boom.Length > 1)
            {
                parameters = boom.Skip(1).ToArray();
            }

            //Attach the trailing parameter as a single entry
            if (!String.IsNullOrEmpty(trailing))
            {
                parameters = parameters.Concat(new String[] { trailing }).ToArray();
            }

            //Logger.Trace("Prefix: {0} Command: {1} Parameters: {2}", prefix, command, String.Join(", ", parameters));

            return new IrcMessage(prefix, command, parameters);
        }
    }

    /// <summary>
    /// Container for data from parsing an IRC command
    /// </summary>
    class IrcMessage
    {
        #region Fields

        #endregion

        #region Accessors
        /// <summary>
        /// Prefix of an IRC command. :prophet.tx.us.seersirc.net 376 TestBot :End of /MOTD command.
        ///                            ^------------------------^ 
        /// </summary>
        public String Prefix
        {
            get;
            private set;
        }

        /// <summary>
        /// Command portion of an IRC command. :prophet.tx.us.seersirc.net 376 TestBot :End of /MOTD command.
        ///                                                                ^-^
        /// </summary>
        public String Command
        {
            get;
            private set;
        }


        /// <summary>
        /// The parameters given in an IRC command. :prophet.tx.us.seersirc.net 376 TestBot :End of /MOTD command.
        ///                                                                         ^----------------------------^
        /// </summary>
        public String[] Parameters
        {
            get;
            private set;
        }

        #endregion


        /// <summary>
        /// Populates IrcMessage data.
        /// </summary>
        /// <param name="prefix">The prefix to an IRC command</param>
        /// <param name="command">The command portion of an IRC command</param>
        /// <param name="parameters">The parameters given to an IRC command</param>
        public IrcMessage(String prefix, String command, String[] parameters)
        {
            Prefix = prefix;
            Command = command;
            Parameters = parameters;
        }
    }
}
