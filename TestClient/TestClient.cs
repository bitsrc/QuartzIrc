using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using QuartzIrc;

namespace TestClient
{
    class TestClient
    {
        IrcClient Client;

        public TestClient()
        {
            IrcConfig config = new IrcConfig("irc.seersirc.net","TestClient");
            config.Port = 6697;
            config.Ssl = true;
            config.Realname = "QuartzIrc Framework - Phate408";

            Client = new IrcClient(config);

            Client.EventRawIn += new IrcRawEventHandler(OnRaw);
        }

        public void Start()
        {
            Client.Start();
        }

        public void Send(string s) {
            Client.Raw(s);
        }

        public void Quit()
        {
            Client.Quit();
        }

        public void OnRaw(IrcClient sender, String text)
        {
            Console.WriteLine("{0}", text);
        }
    }
}
