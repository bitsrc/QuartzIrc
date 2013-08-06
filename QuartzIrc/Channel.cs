using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace QuartzIrc
{
    /// <summary>
    /// Representation of a channel on IRC
    /// </summary>
    public class Channel
    {

        #region Fields
        /// <summary>
        /// Parent client
        /// </summary>
        private IrcClient Irc;

        /// <summary>
        /// Users currently in the channel. Don't change it.
        /// </summary>
        public Dictionary<String,User> users;

        #endregion

        #region Properties

        /// <summary>
        /// Name of the channel
        /// </summary>
        public String Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Channel's current topic
        /// </summary>
        public String Topic
        {
            get;
            internal set;
        }

        #endregion

        #region Constructor

        public Channel(IrcClient irc, String name)
        {
            Irc = irc;
            Name = name;
        }

        #endregion

        #region Members

        private void SetTopic(String topic)
        {
            Irc.Topic(Name, topic);
        }

        public override String ToString()
        {
            return Name;
        }

        public static implicit operator string(Channel c)
        {
            return c.ToString();
        }

        #endregion

        #region Events
        /// <summary>
        /// Triggered when a message is sent to the channel. Does not include notices.
        /// </summary>
        public event IrcEventHandler EventMessage;
        public void PerformMessage(IrcClient sender, IrcEventArgs e)
        {
            if (EventMessage != null)
                EventMessage(sender, e);
        }

        /// <summary>
        /// Triggered when a notice is sent to the channel
        /// </summary>
        public event IrcEventHandler EventNotice;
        public void PerformNotice(IrcClient sender, IrcEventArgs e)
        {
            if (EventNotice != null)
                EventNotice(sender, e);
        }

        /// <summary>
        /// Triggered when a user joins the channel
        /// </summary>
        public event IrcEventHandler EventJoin;
        public void PerformJoin(IrcClient sender, IrcEventArgs e)
        {
            if (EventJoin != null)
                EventJoin(sender, e);
            Console.WriteLine("{0} joined {1}",Hostmask.ToNick(e.Sender),this);
        }

        /// <summary>
        /// Triggered when the client joins the channel
        /// </summary>
        public event IrcEventHandler EventSelfJoin;
        public void PerformSelfJoin(IrcClient sender, IrcEventArgs e)
        {
            if (EventSelfJoin != null)
                EventSelfJoin(sender, e);
            Console.WriteLine("I joined: {0}", this);
        }

        /// <summary>
        /// Triggered when a user parts the channel
        /// </summary>
        public event IrcEventHandler EventPart;
        public void PerformPart(IrcClient sender, IrcEventArgs e)
        {
            if (EventPart != null)
                EventPart(sender, e);
        }

        /// <summary>
        /// Triggered when the client parts the channel
        /// </summary>
        public event IrcEventHandler EventSelfPart;
        public void PerformSelfPart(IrcClient sender, IrcEventArgs e)
        {
            if (EventSelfPart != null)
                EventSelfPart(sender, e);
        }

        /// <summary>
        /// Triggered when a user is kicked from the channel
        /// </summary>
        public event IrcEventHandler EventKick;
        public void PerformKick(IrcClient sender, IrcEventArgs e)
        {
            if (EventKick != null)
                EventKick(sender, e);
        }

        /// <summary>
        /// Triggered on a mode change for the channel
        /// </summary>
        public event IrcEventHandler EventMode;
        public void PerformMode(IrcClient sender, IrcEventArgs e)
        {
            if (EventMode != null)
                EventMode(sender, e);
        }

        /// <summary>
        /// Triggered when the channel's topic is changed
        /// </summary>
        public event IrcEventHandler EventTopic;
        public void PerformTopic(IrcClient sender, IrcEventArgs e)
        {
            Topic = string.Join(" ", e.Args.Skip(1).ToArray()); ;
            if (EventTopic != null)
                EventTopic(sender, e);
        }

        #endregion

        #region EventHandlers

        #endregion
    }
}
