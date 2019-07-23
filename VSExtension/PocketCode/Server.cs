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
        public Server()
        {
        }

        public void Run(Deploy dep, AsyncPackage package)
        {
            //---listen at the specified IP and port no.---
            IPAddress localAdd = IPAddress.Parse(SERVER_IP);
            TcpListener listener = new TcpListener(localAdd, PORT_NO);
            Console.WriteLine("Listening...");
            listener.Start();
            this.dte = dep?.dte;
            //---incoming client connected---
            TcpClient client = listener.AcceptTcpClient();
            NetworkStream nwStream = client.GetStream();


            while (true)
            {
                //---get the incoming data through a network stream---
                byte[] buffer = new byte[client.ReceiveBufferSize];
                //---read incoming stream---
                int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

                //---convert the data received into a string---
                string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                //this.SetDTE(package);
                //var mydelegate = new Action<object>(delegate(object param)
                Action mydelegate = new Action(delegate
                {
                   this.SetDTE(package);
                   Documents documents = this.dte?.Documents;
                   if(documents == null)
                   {
                       documents = dep?.dte?.Documents;
                   }
                   this.SendFilesText(documents, nwStream);
               });
                //mydelegate.Invoke();
                dep.Dispatcher.BeginInvoke(mydelegate);
                //result = result + "Exit\n";
                //nwStream.Write(Encoding.ASCII.GetBytes(result), 0, Encoding.ASCII.GetByteCount(result));
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
            foreach (Document doc in docs)
            {
                SerializableFile sendFile = new SerializableFile(doc);
                var ser = new DataContractJsonSerializer(typeof(SerializableFile));
                ser.WriteObject(stream, sendFile);
            }
        }

        private async void SetDTE(AsyncPackage package)
        {
            //await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
            this.dte = (package.GetServiceAsync(typeof(_DTE)))?.Result as DTE;
        }
    }
}
