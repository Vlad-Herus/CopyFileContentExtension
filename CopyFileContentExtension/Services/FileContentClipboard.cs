using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CopyFileContentExtension.Services
{
    class FileContentClipboard
    {
        public void CopyContents(IEnumerable<string> sqlFiles)
        {
            List<string> lines = new List<string>();
            //int fileId = 0;
            foreach (var file in sqlFiles.OrderBy(s => s))
            {
                //lines.Add($"-- File number {fileId++}");
                lines.AddRange(File.ReadAllLines(file));
                lines.Add("\n\n\n");
            }

            var line = lines.Aggregate((s1, s2) => s1 + "\n" + s2);

            Clipboard.SetText(line);
        }
    }
}
