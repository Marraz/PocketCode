using System;
using System.Net.Sockets;
using System.Text;

namespace ConsoleApp1
{
    class Program
    {
        const int PORT_NO = 5000;
        const string SERVER_IP = "127.0.0.1";
        static void Main(string[] args)
        {
            //---create a TCPClient object at the IP and port no.---
            TcpClient client = new TcpClient(SERVER_IP, PORT_NO);
            NetworkStream nwStream = client.GetStream();
            //---send the text---
            while (true)
            {
                string textToSend = Console.ReadLine();
                if (textToSend.Contains("exit"))
                {
                    client.Close();
                    break;
                }
                byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(textToSend);
                Console.WriteLine("Sending: " + textToSend);
                nwStream.Write(bytesToSend, 0, bytesToSend.Length);
                byte[] bytesToRead = new byte[client.ReceiveBufferSize];
                int bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
                Console.WriteLine(value: "Received : " + Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));
            }
        }
    }
}
