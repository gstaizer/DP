using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Server
{
    class Program
    {
        public static void StartListening(int port)
        {
            // Привязываем сокет ко всем интерфейсам на текущей машинe
            IPAddress ipAddress = IPAddress.Any; 
            
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            // CREATE
            Socket listener = new Socket(
                ipAddress.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            List<string> messages = new List<string>();

            try
            {
                // BIND
                listener.Bind(localEndPoint);

                // LISTEN
                listener.Listen(10);

                while (true)
                {
                    // ACCEPT
                    Socket handler = listener.Accept();

                    byte[] buf = new byte[1024];
                    string data = null;
                    do
                    {
                        int bytes = handler.Receive(buf);
                        data += Encoding.UTF8.GetString(buf, 0, bytes);
                    }
                    while (handler.Available > 0);

                    Console.WriteLine("Message received: {0}", data);
                    messages.Add(data);

                    var jsonMessages = JsonSerializer.Serialize(messages);

                    // SEND
                    handler.Send(Encoding.UTF8.GetBytes(jsonMessages));

                    // RELEASE
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Invalid parameters count. Correct: <port>");
                return;
            }

            StartListening(Int32.Parse(args[0]));

            Console.Read();
        }
    }
}
