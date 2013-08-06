using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quartz
{
    /// <summary>
    /// Define configuration information for an IRC Client (IrcClient)
    /// </summary>
    public class IrcConfig
    {
        #region Fields
        #endregion

        #region Properties

        /// <summary>
        /// The address of the server to connect to
        /// </summary>
        public String Host
        {
            get;
            set;
        }

        /// <summary>
        /// The port to connect on
        /// </summary>
        public int Port
        {
            get;
            set;
        }

        /// <summary>
        /// Whether to use SSL or not when connecting
        /// </summary>
        public Boolean Ssl
        {
            get;
            set;
        }

        /// <summary>
        /// The nick to use when connecting
        /// </summary>
        public String Nick
        {
            get;
            set;
        }

        /// <summary>
        /// If the desired nick is taken, fallback to this nick
        /// </summary>
        public String AltNick
        {
            get;
            set;
        }

        /// <summary>
        /// Ident to use when connecting
        /// </summary>
        public String Ident {
            get;
            set;
        }

        /// <summary>
        /// Realname to use when connecting
        /// </summary>
        public String Realname
        {
            get;
            set;
        }

        /// <summary>
        /// The string to respond with when queried by a CTCP version
        /// </summary>
        public String Version
        {
            get;
            set;
        }

        //WebIRC
        /// <summary>
        /// Whether to fire a WebIRC command when connecting or not
        /// </summary>
        public Boolean WebIrc
        {
            get;
            set;
        }

        /// <summary>
        /// WebIRC Password, used to authenticate against a webirc config block using the WEBIRC command (CGI:IRC style)
        /// </summary>
        public String WebIrcPassword
        {
            get;
            set;
        }

        /// <summary>
        /// WebIRC user, used to authenticate against a webirc config block using the WEBIRC command (CGI:IRC style)
        /// </summary>
        public String WebIrcUser
        {
            get;
            set;
        }

        /// <summary>
        /// WebIRC Hostname, the host to spoof
        /// </summary>
        public String WebIrcHostname
        {
            get;
            set;
        }


        /// <summary>
        /// WebIRC IP, the IP to spoof
        /// </summary>
        public String WebIrcIp
        {
            get;
            set;
        }
        /// <summary>
        /// Password to connect to the server
        /// </summary>
        public String ServerPassword
        {
            get;
            set;
        }

        /// <summary>
        /// Password to identify to services with when connected.
        /// </summary>
        public String ServicesPassword
        {
            get;
            set;
        }

        /// <summary>
        /// Nickname of the services client to identify on connect
        /// </summary>
        public String ServicesNick
        {
            get;
            set;
        }

        /// <summary>
        /// Whether the client will attempt auto reconnection or not
        /// </summary>
        public Boolean Reconnect
        {
            get;
            set;
        }

        /// <summary>
        /// How many times to attempt to reconnect
        /// </summary>
        public int ReconnectAttempts
        {
            get;
            set;
        }

        /// <summary>
        /// How long to wait in between reconnection attempts in milliseconds
        /// </summary>
        public int ReconnectWaitTime
        {
            get;
            set;
        }

        /// <summary>
        /// Will the client respond to CTCP Ping requests
        /// </summary>
        public Boolean CtcpPing
        {
            get;
            set;
        }

        /// <summary>
        /// Will the client respond to CTCP Time requests
        /// </summary>
        public Boolean CtcpTime
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initiate a new Config object
        /// </summary>
        /// <param name="host">The address of the server you're connecting to</param>
        /// <param name="nick">The nick you would like to connect with</param>
        public IrcConfig(String host, String nick)
        {
            //TODO validate nick
            Host = host;
            Port = 6667;
            Ssl = false;
            Nick = nick;
            AltNick = String.Format("{0}_",nick);
            if (nick.Length > 9)
            {
                Ident = nick.Substring(0, 9);
            }
            else
            {
                Ident = nick;
            }
            Realname = nick;
            Version = "Quartz IRC Framework - Phate";
            WebIrc = false;
            WebIrcPassword = String.Empty;
            ServerPassword = String.Empty;
            ServicesPassword = String.Empty;
            Reconnect = true;
            ReconnectAttempts = 5;
            ReconnectWaitTime = 1000;
            CtcpPing = true;
            CtcpTime = true;
        }

        #endregion

        #region Members

        /// <summary>
        /// Enable WebIRC in the config
        /// </summary>
        /// <param name="webIrcPassword">The password to authenticate to the IRC server with</param>
        /// <param name="webIrcHostname">The hostname to apply to the user</param>
        /// <param name="webIrcIp">The IP to apply to the user</param>
        /// <param name="webIrcUser">The user to authenticate to the IRC server with (Optional)</param>
        public void AddWebIrc(String webIrcPassword, String webIrcUser, String webIrcHostname, String webIrcIp)
        {
            WebIrc = true;
            WebIrcPassword = webIrcPassword;
            WebIrcUser = webIrcUser;
            WebIrcHostname = webIrcHostname;
            WebIrcIp = webIrcIp;
        }

        /// <summary>
        /// Enable auto-reconnect in the config
        /// </summary>
        /// <param name="reconnectAttempts">How many times to attempt connecting</param>
        /// <param name="reconnectWaitTime">How long to wait in between connection attempts in milliseconds</param>
        public void AddReconnect(int reconnectAttempts, int reconnectWaitTime)
        {
            Reconnect = true;
            ReconnectAttempts = reconnectAttempts;
            ReconnectWaitTime = reconnectWaitTime;
        }

        #endregion
    }
}
