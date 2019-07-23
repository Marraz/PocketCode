using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PocketCode
{
    [DataContract]
    class SerializableFile
    {
        [DataMember]
        public string Name { get; private set; }
        [DataMember]
        public string Data { get; private set; }

        public SerializableFile(Document doc)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            TextDocument textDocument = (TextDocument)doc?.Object("TextDocument");
            if (textDocument != null)
            {
                this.Name = doc.Name;
                this.Data = textDocument.StartPoint.CreateEditPoint().GetText(textDocument.EndPoint);
            }
        }
    }
}
