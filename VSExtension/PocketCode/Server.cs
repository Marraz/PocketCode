using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace PocketCode
{
    class Server
    {
        const int PORT_NO = 5000;
        const string SERVER_IP = "127.0.0.1";
        DTE dte;
        private AsyncPackage package;
        private DTE Dte
        {
            get
            {
                return this.dte ?? (package?.GetServiceAsync(typeof(_DTE)))?.Result as DTE;
            }
        }
        public Server()
        {
        }

        public void Run(Deploy dep, AsyncPackage package)
        {
            this.dte = dep?.dte;
            this.package = package;
            //---listen at the specified IP and port no.---
            IPAddress localAdd = IPAddress.Parse(SERVER_IP);
            TcpListener listener = new TcpListener(localAdd, PORT_NO);
            Console.WriteLine("Listening...");
            listener.Start();

            //---incoming client connected---
            TcpClient client;
            NetworkStream nwStream;
            //System.IO.IOException
            while (true)
            {
                client = listener.AcceptTcpClient();
                nwStream = client.GetStream();
                while (true)
                {
                    try
                    {
                        //---get the incoming data through a network stream---
                        byte[] buffer = new byte[client.ReceiveBufferSize];
                        //---read incoming stream---
                        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

                        //---convert the data received into a string---
                        string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                        dep.Dispatcher.Invoke((Action)delegate ()
                        {
                            try
                            {
                                Documents documents = this.Dte?.Documents ?? dep?.dte?.Documents;
                                this.SendFilesText(documents, nwStream);
                            }
                            catch (Exception) { } //Lets just ignore everything for now
                        });
                    }
                    catch (Exception e)
                    {
                        nwStream?.Dispose();
                        client?.Dispose();
                        break;
                    }
                }
            }
            client.Close();
            listener.Stop();
            Console.ReadLine();
        }

        public void RegisterUpdate(AsyncPackage package)
        {
            this.dte = (package.GetServiceAsync(typeof(_DTE)))?.Result as DTE;


        }

        public event EventHandler Update;
        public bool TerminateThread { get; set; }

        public void SendFilesText(Documents docs, Stream stream)
        {
            List<SerializableFile> sendFiles = new List<SerializableFile>();
            foreach (Document doc in docs)
            {
                sendFiles.Add(new SerializableFile(doc));
            }
            var ser = new DataContractJsonSerializer(typeof(List<SerializableFile>));
            ser.WriteObject(stream, sendFiles);

            string line = "Exit" + "\n";
            stream.Write(Encoding.ASCII.GetBytes(line), 0, Encoding.ASCII.GetByteCount(line));
        }
    }
}
