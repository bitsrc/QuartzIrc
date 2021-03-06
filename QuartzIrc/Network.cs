﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace QuartzIrc
{
    /// <summary>
    /// Provides network interaction wrapping
    /// </summary>
    class Network
    {

        #region Fields

        private TcpClient Client;
        private SslStream sslStream;
        private StreamReader sr;
        private StreamWriter sw;
        private bool secure;

        #endregion

        #region Constructor


        /// <summary>
        /// Constructor, sets up networking and logging
        /// </summary>
        public Network()
        {
            Client = new TcpClient();
        }

        #endregion

        #region Members

        /// <summary>
        /// Initiates the connection to the IRC server
        /// </summary>
        /// <param name="host">Hostname to connect to</param>
        /// <param name="port">Port to connect on</param>
        /// <param name="ssl">Whether to use SSL when connecting or not</param>
        public void Connect(String host, int port, bool ssl)
        {
            if (Client.Connected)
            {
                Client.Close();
                Client = new TcpClient();
            }
            //Logger.Trace("Connecting");
            Client.Connect(host, port);
            if (ssl)
            {
                secure = true;
                //Logger.Trace("Going SSL");
                sslStream = new SslStream(Client.GetStream(), true, new RemoteCertificateValidationCallback(ValidateServerCertificate));
                //Logger.Trace("Authenticating SSL");

                //We have to authenticate to use the connection.
                try
                {
                    sslStream.AuthenticateAsClient(host);
                }
                catch (Exception e)
                {
                    //Logger.Error("SSL Authentication failed");
                    Console.WriteLine("Exception: {0}", e.Message);
                    if (e.InnerException != null)
                    {
                        Console.WriteLine("Inner exception: {0}", e.InnerException.Message);
                    }
                    Console.WriteLine("Authentication failed - closing the connection.");
                    Client.Close();
                    return;
                }

                sr = new StreamReader(sslStream);
            }
            else
            {
                secure = false;
                sr = new StreamReader(Client.GetStream());
                sw = new StreamWriter(Client.GetStream());
            }
        }


        public void Disconnect()
        {
            Client.Close();
        }

        /// <summary>
        /// Read a line from the connection, blocking.
        /// </summary>
        /// <returns>First line in the network buffer</returns>
        public String Read()
        {
            /* byte[] buffer = new byte[2048];
            int bytes = -1;
            bytes = sslStream.Read(buffer, 0, buffer.Length);
            StringBuilder serverMessage = new StringBuilder();
            serverMessage.Append(Encoding.UTF8.GetChars(buffer, 0, bytes));
            Logger.Trace("Read: {0}", serverMessage.ToString());
            return serverMessage.ToString(); */
            String serverMessage = sr.ReadLine();
            //Logger.Trace("Read: {0}", serverMessage);
            return serverMessage;
        }


        /// <summary>
        /// Send a line to the IRC server
        /// </summary>
        /// <param name="message">Line to send, will have a newline appended if one doesn't exist already</param>
        /// <param name="args">Arguments for String.Format() upon the message</param>
        public void Write(String message, params object[] args)
        {
            message = String.Format(message, args);
            
            //IRC Server (Unreal at least) requires a newline to process the command
            if (!message.EndsWith("\n"))
            {
                message += "\n";
            }

            //Logger.Trace("Write: {0}", message);

            if (secure)
            {
                //SSL wants bytes
                byte[] bytes = Encoding.ASCII.GetBytes(message);
                sslStream.Write(bytes);
                sslStream.Flush();
            }
            else
            {
                sw.Write(message);
                sw.Flush();
            }
        }

        #endregion

        #region Static Functions

        //Always return true. We don't really care about certificate strength.
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        #endregion
    }
}
