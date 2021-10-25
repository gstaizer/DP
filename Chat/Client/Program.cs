using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Client
{
    class Program
    {
        public static void StartClient(string host, int port, string message)
        {
            try
            {
                IPAddress ipAddress = IPAddress.Loopback;
                if (host != "localhost")
                {
                    ipAddress = IPAddress.Parse(host);
                }
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // CREATE
                Socket sender = new Socket(
                    ipAddress.AddressFamily,
                    SocketType.Stream, 
                    ProtocolType.Tcp);

                try
                {
                    // CONNECT
                    sender.Connect(remoteEP);

                    // SEND
                    int bytesSent = sender.Send(Encoding.UTF8.GetBytes(message));

                    // RECEIVE
                    byte[] buf = new byte[1024];
                    StringBuilder builder = new StringBuilder();
                    do
                    {
                        int bytes = sender.Receive(buf, buf.Length, 0);
                        builder.Append(Encoding.UTF8.GetString(buf, 0, bytes));
                    }
                    while (sender.Available > 0);

                    var messages = JsonSerializer.Deserialize<List<string>>(builder.ToString());

                    foreach (var m in messages)
                    {
                        Console.WriteLine(m);
                    }

                    // RELEASE
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Invalid parameters count. Correct: <host> <port> <message>");
                return;
            }

            StartClient(args[0],  Int32.Parse(args[1]), args[2]);
        }
    }
}