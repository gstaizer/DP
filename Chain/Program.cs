﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chain
{
    class Program
    {
        public static void Start(int listeningPort, string nextHost, int nextPort, bool initializer)
        {
            int X = Int32.Parse(Console.ReadLine());
            IPAddress ipAddress = IPAddress.Any; 
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, listeningPort);
            Socket listener = new Socket(
                ipAddress.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(10);

            IPAddress remoteIpAddress = IPAddress.Loopback;
                if (nextHost != "localhost")
                {
                    remoteIpAddress = IPAddress.Parse(nextHost);
                }
            IPEndPoint remoteEP = new IPEndPoint(remoteIpAddress, nextPort);
            Socket sender = new Socket(
                remoteIpAddress.AddressFamily,
                SocketType.Stream, 
                ProtocolType.Tcp);

            Connect(sender, remoteEP);
            Socket handler = listener.Accept();
            if (initializer)
            {
                RunInitiator(handler, sender, X);
            }
            else
            {
                RunCommonProccess(handler, sender, X);  
            }

            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }
        private static void RunInitiator(Socket handler, Socket sender, int X)
        {
            Send(sender, X);
            int Y = Receive(handler);
            Send(sender, Y);
            X = Receive(handler);
            Console.WriteLine(X);
        }
        private static void RunCommonProccess(Socket handler, Socket sender, int X)
        {
            int Y = Receive(handler);
            X = Math.Max(X, Y);
            Send(sender, X);
            X = Receive(handler);
            Send(sender, X);
            Console.WriteLine(X);
        }
        static void Main(string[] args)
        {
            if (args.Length == 3)
            {
                Start(Int32.Parse(args[0]), args[1], Int32.Parse(args[2]), false);
            }
            else
            {
                Start(Int32.Parse(args[0]), args[1], Int32.Parse(args[2]), true);
            }
            Console.ReadLine();
        }
        static void Connect(Socket sender, IPEndPoint remoteEP)
        {
            while (true)
            {
                try
                {
                    sender.Connect(remoteEP);
                    return;
                }
                catch (SocketException)
                {
                    Thread.Sleep(1000);
                }
            }
        }
        static void Send(Socket sender, int num)
        {
            try
            {
                byte[] msg = Encoding.UTF8.GetBytes(num.ToString());
                int bytesSent = sender.Send(msg);

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
        static int Receive(Socket handler)
        {
            try
            {
                byte[] buf = new byte[1024];
                string data = null;
                do
                {
                    int bytesRec = handler.Receive(buf);
                    data += Encoding.UTF8.GetString(buf, 0, bytesRec);
                }
                while (handler.Available>0);
                return Int32.Parse(data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return 0;
            }
        }
    }
}