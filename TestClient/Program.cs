using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using QuartzIrc;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            TestClient client = new TestClient();
            Thread ircThread = new Thread(client.Start);
            ircThread.Start();
            while (true)
            {
                String input = Console.ReadLine();
                if (input == "exit")
                {
                    client.Quit();
                    break;
                }
                client.Send(input);
            }
        }
    }
}
