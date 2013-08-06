using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Quartz
{
    public delegate void IrcEventHandler(IrcClient sender, IrcEventArgs e);
    public delegate void IrcRawEventHandler(IrcClient sender, String raw);
    public delegate void IrcNumericHandler(IrcClient sender, IrcNumerics numeric, IrcEventArgs e);

    /// <summary>
    /// Base implementation of a simple IRC bot
    /// </summary>
    public class IrcClient
    {
        #region Fields

        /// <summary>
        /// Config object
        /// </summary>
        private IrcConfig config;

        /// <summary>
        /// Network wrapper
        /// </summary>
        private Network net;

        /// <summary>
        /// Logger
        /// </summary>
        //private Logger Logger;

        /// <summary>
        /// Numeric handlers
        /// </summary>
        private Dictionary<IrcNumerics, List<IrcNumericHandler>> numericHandlers;

        /// <summary>
        /// Channels the bot is currently in.
        /// </summary>
        public Dictionary<String, Channel> Channels
        {
            get;
            private set;
        }

        /// <summary>
        /// How many times have we tried to reconnect
        /// </summary>
        private int reconnectAttempts;

        /// <summary>
        /// Users the bot currently knows of. NOT IMPLEMENTED
        /// </summary>
        public Dictionary<String, User> Users
        {
            get;
            private set;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The client's current nick
        /// </summary>
        public String Nick
        {
            get
            {
                return Hostmask.Nick;
            }
        }

        /// <summary>
        /// The client's current ident
        /// </summary>
        public String Ident
        {
            get
            {
                return Hostmask.Ident;
            }
        }

        /// <summary>
        /// The client's current realname
        /// </summary>
        public String Realname
        {
            get;
            private set;
        }

        /// <summary>
        /// Client's current hostmask to the best of its knowledge
        /// </summary>
        public Hostmask Hostmask
        {
            get;
            private set;
        }

        /// <summary>
        /// Client's external IP
        /// </summary>
        public String Ip
        {
            get;
            private set;
        }

        /// <summary>
        /// Indicates whether the client is currently connected or not
        /// </summary>
        public Boolean Connected
        {
            get;
            private set;
        }

        /// <summary>
        /// When the bot connected to the server (UTC)
        /// </summary>
        public DateTime ConnectionTime
        {
            get;
            private set;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Instantiate a new client
        /// </summary>
        /// <param name="config">Configuration object, requires a minimum of server address and nickname</param>
        public IrcClient(IrcConfig config)
        {
            this.config = config;
            Hostmask = new Hostmask(config.Nick, config.Ident, "not.connected");
            Realname = config.Realname;
            net = new Network();
            //Logger = LogManager.GetCurrentClassLogger();
            Channels = new Dictionary<String, Channel>();
            Users = new Dictionary<String, User>();
            Connected = false;
            reconnectAttempts = 0;
            numericHandlers = new Dictionary<IrcNumerics, List<IrcNumericHandler>>();
        }

        #endregion

        #region Members

        /// <summary>
        /// Connect to the server and begin the main loop
        /// </summary>
        public void Start()
        {
            Connect();
            EventJoin += new IrcEventHandler(OnJoin);
            EventPart += new IrcEventHandler(OnPart);
            EventCTCPVersion += new IrcEventHandler(OnCTCPVersion);
            EventCTCPPing += new IrcEventHandler(OnCtcpPing);
            EventCTCPTime += new IrcEventHandler(OnCTCPTime);
            EventConnect += new IrcEventHandler(OnConnect);
            EventDisconnect += new IrcEventHandler(OnDisconnect);
            EventPublicMessage += new IrcEventHandler(OnPublicMessage);
            EventPublicNotice += new IrcEventHandler(OnPublicNotice);
            EventKick += new IrcEventHandler(OnKick);
            EventChannelMode += new IrcEventHandler(OnChannelMode);
            AddNumericHandler(IrcNumerics.Welcome, new IrcNumericHandler(OnWelcome));
            AddNumericHandler(IrcNumerics.Topic, new IrcNumericHandler(OnTopic));
            AddNumericHandler(IrcNumerics.NamesReply, new IrcNumericHandler(OnNamesReply));
            AddNumericHandler(IrcNumerics.NamesEnd, new IrcNumericHandler(OnNamesEnd));
            Run();
        }

        /// <summary>
        /// Perform the connection to the server
        /// </summary>
        private void Connect()
        {
            net.Connect(config.Host, config.Port, config.Ssl);
            if (config.WebIrc)
            {
                Raw("WEBIRC {0} {1} {2} {3}", config.WebIrcPassword, config.WebIrcUser, config.WebIrcHostname, config.WebIrcIp);
            }
            if (!String.IsNullOrEmpty(config.ServerPassword))
            {
                Raw("PASS {0}", config.ServerPassword);
            }
            Raw("NICK {0}", config.Nick);
            Raw("USER {0} 8 * :{1}", config.Ident, config.Realname);
        }

        /// <summary>
        /// Runs the main loop of the bot
        /// </summary>
        private void Run()
        {
            String incoming;
            int numeric;

            while (true)
            {
                incoming = net.Read();

                if (String.IsNullOrEmpty(incoming))
                {
                    //We are disconnected
                    if (EventDisconnect != null)
                    {
                        EventDisconnect(this, new IrcEventArgs("","",new String[0]));
                    }
                    //If reconnection is enabled
                    if (config.Reconnect)
                    {
                        //And we haven't exceeded the desired number of attempts yet
                        if (reconnectAttempts < config.ReconnectAttempts)
                        {
                            //Then delay and reconnect
                            reconnectAttempts++;
                            Thread.Sleep(config.ReconnectWaitTime);
                            Connect();
                        }
                    }
                    return;
                }

                Console.WriteLine(incoming);
                if (EventRawIn != null)
                {
                    EventRawIn(this, incoming);
                }
                /*Match match = Regex.Match(incoming, @"PING\s:(.+)");
                if (match.Success)
                {
                    net.Write("PONG :{0}", match.Groups[1]);
                }
                */
                IrcMessage message = Parser.ParseIrcMessage(incoming);
                switch (message.Command)
                {
                    case "PING":
                        Raw("PONG :{0}", message.Parameters[0]);
                        break;
                    case "PRIVMSG":
                        //It's addressed to a channel, it's public
                        if (message.Parameters.First().StartsWith("#"))
                        {
                            if (EventPublicMessage != null)
                            {
                                EventPublicMessage(this, new IrcEventArgs(message.Prefix, message.Parameters.First(), message.Parameters.Skip(1).ToArray()));
                            }
                            if (message.Parameters[1][0] == (char)1)
                            {
                                if (message.Parameters[1].Substring(1, 6) == "ACTION")
                                {
                                    if (EventPublicAction != null)
                                    {
                                        EventPublicAction(this, new IrcEventArgs(message.Prefix, message.Parameters.First(), message.Parameters[1].Split(' ').Skip(1).ToArray()));
                                    }
                                }
                            }
                        }
                        else
                        {
                            //If first character of message is character 1, then it's either CTCP or an action
                            if (message.Parameters[1][0] == (char)1)
                            {
                                if (message.Parameters[1].Substring(1, 4) == "PING")
                                {
                                    if (EventCTCPPing != null)
                                    {
                                        EventCTCPPing(this, new IrcEventArgs(message.Prefix, message.Parameters.First(), message.Parameters.Skip(1).ToArray()));
                                    }
                                }
                                else if (message.Parameters[1].Substring(1, 4) == "TIME")
                                {
                                    if (EventCTCPTime != null)
                                    {
                                        EventCTCPTime(this, new IrcEventArgs(message.Prefix, message.Parameters.First(), message.Parameters.Skip(1).ToArray()));
                                    }
                                }
                                else if (message.Parameters[1].Substring(1, 6) == "ACTION")
                                {
                                    if (EventPrivateAction != null)
                                    {
                                        EventPrivateAction(this, new IrcEventArgs(message.Prefix, message.Parameters.First(), message.Parameters[1].Split(' ').Skip(1).ToArray()));
                                    }
                                }
                                else if (message.Parameters[1].Substring(1, 7) == "VERSION")
                                {
                                    if (EventCTCPVersion != null)
                                    {
                                        EventCTCPVersion(this, new IrcEventArgs(message.Prefix, message.Parameters.First(), message.Parameters.Skip(2).ToArray()));
                                    }
                                }
                            }

                            //Logger.Trace("Firing PrivateMessage: Sender: {0} Target: {1} Args: {2}", message.Prefix, message.Params.First(), String.Join(",", message.Params.Skip(1).ToArray()));
                            if (EventPrivateMessage != null)
                            {
                                EventPrivateMessage(this, new IrcEventArgs(message.Prefix, message.Parameters.First(), message.Parameters.Skip(1).ToArray()));
                            }
                        }
                        break;
                    case "NOTICE":
                        if (message.Parameters.First().StartsWith("#"))
                        {
                            if (EventPublicNotice != null)
                            {
                                EventPublicNotice(this, new IrcEventArgs(message.Prefix, message.Parameters.First(), message.Parameters.Skip(1).ToArray()));
                            }
                        }
                        else
                        {
                            if (EventPrivateNotice != null)
                            {
                                EventPrivateNotice(this, new IrcEventArgs(message.Prefix, message.Parameters.First(), message.Parameters.Skip(1).ToArray()));
                            }
                        }
                        break;
                    case "JOIN":
                        if (EventJoin != null)
                        {
                            EventJoin(this, new IrcEventArgs(message.Prefix, message.Parameters[0], message.Parameters));
                        }
                        break;
                    case "PART":
                        if (EventPart != null)
                        {
                            EventPart(this, new IrcEventArgs(message.Prefix, message.Parameters[0], message.Parameters.Skip(1).ToArray()));
                        }
                        break;
                    case "QUIT":
                        if (EventQuit != null)
                        {
                            EventQuit(this, new IrcEventArgs(message.Prefix, message.Parameters[0], message.Parameters.Skip(1).ToArray()));
                        }
                        break;
                    case "MODE":
                        if (message.Parameters[0].StartsWith("#"))
                        {
                            if (EventChannelMode != null)
                            {
                                EventChannelMode(this, new IrcEventArgs(message.Prefix, message.Parameters[0], message.Parameters.Skip(1).ToArray()));
                            }
                        }
                        else
                        {
                            if (EventUserMode != null)
                            {
                                EventUserMode(this, new IrcEventArgs(message.Prefix, message.Parameters[0], message.Parameters.Skip(1).ToArray()));
                            }
                        }
                        break;
                    case "KICK":
                        if (EventKick != null)
                        {
                            EventKick(this, new IrcEventArgs(message.Prefix, message.Parameters[0], message.Parameters.Skip(1).ToArray()));
                        }
                        break;
                    case "INVITE":
                        if (EventInvite != null)
                        {
                            EventInvite(this, new IrcEventArgs(message.Prefix, message.Parameters[0], message.Parameters.Skip(1).ToArray()));
                        }
                        break;
                    default:
                        if (Int32.TryParse(message.Command, out numeric))
                        {
                            Console.WriteLine(Enum.GetName(typeof(IrcNumerics), numeric));
                            FireNumeric((IrcNumerics)numeric, new IrcEventArgs(message.Prefix, message.Parameters[0], message.Parameters.Skip(1).ToArray()));
                        }
                        break;
                }
            }
        }

        public void AddNumericHandler(IrcNumerics numeric, IrcNumericHandler handler)
        {
            if (!numericHandlers.ContainsKey(numeric))
            {
                numericHandlers[numeric] = new List<IrcNumericHandler>();
            }
            numericHandlers[numeric].Add(handler);
        }

        private void FireNumeric(IrcNumerics numeric, IrcEventArgs e)
        {
            if (numericHandlers.ContainsKey(numeric))
            {
                foreach (IrcNumericHandler handler in numericHandlers[numeric])
                {
                    handler(this, numeric, e);
                }
            }
        }

        /// <summary>
        /// Send a raw command to the server
        /// </summary>
        /// <param name="message">The string to be sent to the server.</param>
        /// <param name="args">Optional arguments for String.Format()</param>
        public void Raw(String message, params object[] args)
        {
            net.Write(message, args);

            if (EventRawOut != null)
            {
                EventRawOut(this, String.Format(message, args));
            }
        }

        /// <summary>
        /// Send a message to a channel or user
        /// </summary>
        /// <param name="target">Either a channel (#channel) or a user (Nick)</param>
        /// <param name="message">Message to be sent</param>
        public void Message(String target, String message)
        {
            Raw("PRIVMSG {0} :{1}", target, message);
        }

        /// <summary>
        /// Send a message to a channel or user
        /// </summary>
        /// <param name="target">Either a channel (#channel) or a user (Nick)</param>
        /// <param name="message">Message to be sent</param>
        /// <param name="args">Optional arguments for String.Format()</param>
        public void Message(String target, String message, params object[] args)
        {
            Raw("PRIVMSG {0} :{1}", target, String.Format(message, args));
        }

        /// <summary>
        /// Send a notice to a channel or user
        /// </summary>
        /// <param name="target">Either a channel (#channel) or a user (Nick)</param>
        /// <param name="message">Message to be sent</param>
        public void Notice(String target, String message)
        {
            Raw("NOTICE {0} :{1}", target, message);
        }

        /// <summary>
        /// Send a notice to a channel or user
        /// </summary>
        /// <param name="target">Either a channel (#channel) or a user (Nick)</param>
        /// <param name="message">Message to be sent</param>
        /// <param name="args">Optional arguments for String.Format()</param>
        public void Notice(String target, String message, params object[] args)
        {
            Raw("NOTICE {0} :{1}", target, String.Format(message, args));
        }

        /// <summary>
        /// Perform an action (/me) to a given target.
        /// </summary>
        /// <param name="target">Either a channel (#channel) or a user (Nick)</param>
        /// <param name="message">The message to be sent</param>
        public void Action(String target, String message)
        {
            Raw("PRIVMSG {0} :ACTION {1}", target, message);
        }


        /// <summary>
        /// Join a channel or channels
        /// </summary>
        /// <param name="channel">Either a single channel in the form "#channel" or multiple, using the form "#chan1,#chan2,#test"</param>
        public void Join(String channel)
        {
            //TODO Validate channel(s)
            Raw("JOIN {0}", channel);
        }

        /// <summary>
        /// Join a channel or channels
        /// </summary>
        /// <param name="channels">An array of channel names to join</param>
        public void Join(String[] channels)
        {
            //TODO Validate channels + number of channels
            Join(String.Join(",", channels));
        }

        /// <summary>
        /// Part a channel with no reason
        /// </summary>
        /// <param name="channel">Channel to part</param>
        public void Part(String channel)
        {
            Raw("PART {0}", channel);
        }

        /// <summary>
        /// Part a channel with a given reason
        /// </summary>
        /// <param name="channel">Channel to part</param>
        /// <param name="reason">Reason to use</param>
        public void Part(String channel, String reason)
        {
            Raw("PART {0} :{1}", channel, reason);
        }

        /// <summary>
        /// Invite another user into a channel
        /// </summary>
        /// <param name="nick">The user to invite</param>
        /// <param name="channel">The channel to invite the user to (#channel)</param>
        public void Invite(String nick, String channel)
        {
            Raw("INVITE {0} {1}", nick, channel);
        }

        /// <summary>
        /// Kick a user from a channel with the server's default reason
        /// </summary>
        /// <param name="channel">The channel to kick the user from (#channel)</param>
        /// <param name="nick">The nick to kick from the channel</param>
        public void Kick(String channel, String nick)
        {
            Raw("KICK {0} {1}", channel, nick);
        }

        /// <summary>
        /// Kick a user from a channel with a specified reason
        /// </summary>
        /// <param name="channel">The channel to kick the user from</param>
        /// <param name="nick">The nick to kick from the channel</param>
        /// <param name="reason">The reason to use when kicking</param>
        public void Kick(String channel, String nick, String reason)
        {
            Raw("KICK {0} {1} :{2}", channel, nick, reason);
        }

        /// <summary>
        /// Requests the topic for a channel
        /// </summary>
        /// <param name="channel">The channel to request the topic for</param>
        public void Topic(String channel)
        {
            Raw("TOPIC {0}");
        }

        /// <summary>
        /// Set a topic for a channel
        /// </summary>
        /// <param name="channel">The channel to set a topic on</param>
        /// <param name="topic">The topic to set</param>
        public void Topic(String channel, String topic)
        {
            Raw("TOPIC {0} :{1}", channel, topic);
        }

        #endregion

        #region Events

        /// <summary>
        /// A message sent to a channel
        /// </summary>
        public event IrcEventHandler EventPublicMessage;
        /// <summary>
        /// A message sent directly to the user.
        /// </summary>
        public event IrcEventHandler EventPrivateMessage;

        /// <summary>
        /// A CTCP ACTION sent to a channel
        /// </summary>
        public event IrcEventHandler EventPublicAction;
        /// <summary>
        /// A CTCP ACTION sent directly to the user
        /// </summary>
        public event IrcEventHandler EventPrivateAction;

        /// <summary>
        /// A notice sent to a channel
        /// </summary>
        public event IrcEventHandler EventPublicNotice;
        /// <summary>
        /// A notice sent directly to the user
        /// </summary>
        public event IrcEventHandler EventPrivateNotice;

        /// <summary>
        /// Triggered when a user (including self) joins a channel
        /// </summary>
        public event IrcEventHandler EventJoin;
        /// <summary>
        /// Triggered when a user parts a channel
        /// </summary>
        public event IrcEventHandler EventPart;
        /// <summary>
        /// Triggered when a user invites another user to a channel
        /// </summary>
        public event IrcEventHandler EventInvite;
        /// <summary>
        /// Triggered when someone is kicked from a channel
        /// </summary>
        public event IrcEventHandler EventKick;
        /// <summary>
        /// Triggered when someone disconnects from IRC
        /// </summary>
        public event IrcEventHandler EventQuit;
        
        /// <summary>
        /// Triggered when a channel mode is changed
        /// </summary>
        public event IrcEventHandler EventChannelMode;
        /// <summary>
        /// Triggered when self's mode is changed
        /// </summary>
        public event IrcEventHandler EventUserMode;
        
        /// <summary>
        /// Triggers upon receipt of a CTCP Version request
        /// </summary>
        public event IrcEventHandler EventCTCPVersion;
        /// <summary>
        /// Triggers upon receipt of a CTCP Ping request
        /// </summary>
        public event IrcEventHandler EventCTCPPing;
        /// <summary>
        /// Triggers upon receipt of a CTCP Time request
        /// </summary>
        public event IrcEventHandler EventCTCPTime;

        /// <summary>
        /// Triggers for every incoming line
        /// </summary>
        public event IrcRawEventHandler EventRawIn;
        /// <summary>
        /// Triggers for every outgoing line
        /// </summary>
        public event IrcRawEventHandler EventRawOut;

        /// <summary>
        /// Triggers on the bot connecting to the IRC server
        /// </summary>
        public event IrcEventHandler EventConnect;
        /// <summary>
        /// Triggers on the bot disconnecting from the IRC server
        /// </summary>
        public event IrcEventHandler EventDisconnect;

        #endregion

        #region EventHandlers

        protected void OnJoin(IrcClient sender, IrcEventArgs e)
        {
            if (Hostmask.ToNick(e.Sender) == sender.Nick)
            {
                //I want to access Channels here
                Console.WriteLine("Joined {0}", string.Join(" ", e.Args));
                foreach (string chan in e.Args)
                {
                    Channels[chan] = new Channel(this, chan);
                    //TODO: Get users
                    Channels[chan].PerformSelfJoin(sender, e);
                }
            }
            else
            {
                Channels[e.Args.First()].PerformJoin(this,e);
            }
        }

        protected void OnPart(IrcClient sender, IrcEventArgs e)
        {
            if (Hostmask.ToNick(e.Sender) == sender.Nick)
            {
                //I left a channel
                Channels[e.Target].PerformSelfPart(sender, e);
            }
            else
            {
                Channels[e.Target].PerformPart(sender, e);
            }
        }

        protected void OnCTCPVersion(IrcClient sender, IrcEventArgs e)
        {
            Raw("NOTICE {0} :{1}VERSION {2}{1}", Hostmask.ToNick(e.Sender), (char)1, config.Version);
        }

        protected void OnCtcpPing(IrcClient sender, IrcEventArgs e)
        {
            if (config.CtcpPing)
            {
                Raw("NOTICE {0} :{1}PING {2}{1}", Hostmask.ToNick(e.Sender), (char)1, e.Args[0].Split(' ').Last().TrimEnd((char)1));
            }
        }

        protected void OnCTCPTime(IrcClient sender, IrcEventArgs e)
        {
            if (config.CtcpTime)
            {
                Raw("NOTICE {0} :{1}TIME {2}{1}", Hostmask.ToNick(e.Sender), (char)1, DateTime.Now.ToString("F"));
            }
        }
        protected void OnConnect(IrcClient sender, IrcEventArgs e)
        {
            ConnectionTime = DateTime.UtcNow;
            reconnectAttempts = 0;
            Connected = true;
        }

        protected void OnDisconnect(IrcClient sender, IrcEventArgs e)
        {
            Connected = false;
        }

        protected void OnWelcome(IrcClient sender, IrcNumerics numeric, IrcEventArgs e)
        {
            Hostmask = new Hostmask(e.Args[0].Split(' ').Last());
            IPAddress[] addresses = Dns.GetHostAddresses(Hostmask.Host);
            Ip = addresses.First().ToString();

            if (EventConnect != null)
            {
                EventConnect(this, e);
            }
            Console.WriteLine("Welcome event received");
        }

        protected void OnTopic(IrcClient sender, IrcNumerics numeric, IrcEventArgs e)
        {
            Channels[e.Args.First()].PerformTopic(this, e);
        }

        protected void OnPublicMessage(IrcClient sender, IrcEventArgs e)
        {
            Channels[e.Target].PerformMessage(this, e);
        }

        protected void OnPublicNotice(IrcClient sender, IrcEventArgs e)
        {
            Channels[e.Target].PerformMessage(this, e);
        }

        protected void OnKick(IrcClient sender, IrcEventArgs e)
        {
            Channels[e.Target].PerformKick(sender, e);
        }

        protected void OnChannelMode(IrcClient sender, IrcEventArgs e)
        {
            Channels[e.Target].PerformMessage(sender, e);
        }

        private void OnNamesReply(IrcClient sender, IrcNumerics numeric, IrcEventArgs e)
        {
            
        }

        private void OnNamesEnd(IrcClient sender, IrcNumerics numeric, IrcEventArgs e)
        {
            
        }

        

        #endregion
    }
}
