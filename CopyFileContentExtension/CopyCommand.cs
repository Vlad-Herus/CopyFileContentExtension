using CopyFileContentExtension.Services;
using Microsoft;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CopyFileContentExtension
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class CopyCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("b19477f2-7633-4310-b57c-8fe4ad522e69");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package m_Package;
        private readonly SelectionService m_Selection;
        private readonly FileContentClipboard m_ContentCopy;

        private CopyCommand(Package package)
        {
            this.m_Package = package ?? throw new ArgumentNullException(nameof(package));
            OleMenuCommandService commandService = ServiceProvider.GetService((typeof(IMenuCommandService))) as OleMenuCommandService;
            Assumes.Present(commandService);
            m_Selection = new SelectionService();
            m_ContentCopy = new FileContentClipboard();
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            OleMenuCommand menuItem = new OleMenuCommand(this.Execute, menuCommandID);
            menuItem.BeforeQueryStatus += Before_Execute;
            menuItem.Visible = false;
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static CopyCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.m_Package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new CopyCommand(package);
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
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                var selectedItems = m_Selection.GetFullFilePathAsync(this.ServiceProvider);
                m_ContentCopy.CopyContents(selectedItems);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Execute failed : {ex.ToString()}");
            }
        }

        private void Before_Execute(object sender, EventArgs e)
        {
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                if (sender is OleMenuCommand menuCommand)
                {
                    var selectedItems = m_Selection.GetFullFilePathAsync(this.ServiceProvider);

                    if (selectedItems.All(path => FilePathSatiesfiesSettings(Path.GetFileName(path))))
                    {
                        menuCommand.Visible = true;
                    }
                    else
                    {
                        menuCommand.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Before_Execute failed : {ex.ToString()}");
            }
        }

        private bool FilePathSatiesfiesSettings(string filePath)
        {
            var patterns = (Settings.Instance.FIlePatterns ?? "").Split(',');

            return patterns.Any(pattern => LikeOperator.LikeString(filePath, pattern, Microsoft.VisualBasic.CompareMethod.Text));
        }
    }
}
