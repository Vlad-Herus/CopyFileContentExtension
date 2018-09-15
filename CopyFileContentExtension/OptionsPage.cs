using CopyFileContentExtension.Services;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyFileContentExtension
{
    class OptionsPage : UIElementDialogPage
    {
        protected override System.Windows.UIElement Child
        {
            get
            {
                return new OptionsControl(Settings.Instance);
            }
        }
    }
}
