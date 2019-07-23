using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;
using EnvDTE;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Windows.Threading;
using System.Runtime.CompilerServices;

namespace PocketCode
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class Deploy
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0xBEEF;
        public const int ToolbarButtonId = 0xDEAD;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("86b47887-c86c-4561-aaab-6030f407e2de");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="Deploy"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private Deploy(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static Deploy Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }
        public DTE dte;
        public Dispatcher Dispatcher;
        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in PocketCode's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
            
            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new Deploy(package, commandService);

            Instance.dte = (package.GetServiceAsync(typeof(_DTE)))?.Result as DTE;
            Instance.Dispatcher = Dispatcher.CurrentDispatcher;
            _ = Task.Run(() =>
              {
                  new Server().Run(Instance, package);
              });
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            DTE dte = (this.ServiceProvider.GetServiceAsync(typeof(_DTE)))?.Result as DTE;
            Document activeDoc = dte?.ActiveDocument;
            TextDocument textDocument = (TextDocument)activeDoc?.Object("TextDocument");
            if (textDocument != null)
            {
                string text = textDocument.StartPoint.CreateEditPoint().GetText(20);
                string message = $"The current doc is {activeDoc.Name} and have {text}";
                string title = "Deploy PocketCode";
                // Show a message box to prove we were here
                VsShellUtilities.ShowMessageBox(
                    this.package,
                    message,
                    title,
                    OLEMSGICON.OLEMSGICON_INFO,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }

            Documents documents = dte?.Documents;
            if (documents != null)
            {
                string title = "Show open docs";
                StringBuilder stringBuilder = new StringBuilder("Current documents: \n");
                foreach(Document doc in documents)
                {
                    stringBuilder.AppendLine($"{doc.Name}");
                }
                VsShellUtilities.ShowMessageBox(
                    this.package,
                    stringBuilder.ToString(),
                    title,
                    OLEMSGICON.OLEMSGICON_INFO,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }


            //SerializableFile sendFile = new SerializableFile(activeDoc);
            //var ms = new MemoryStream();
            //var ser = new DataContractJsonSerializer(typeof(SerializableFile));
            //ser.WriteObject(ms, sendFile);
        }
    }
}
