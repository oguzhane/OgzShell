using OgzShell.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OgzShell.CLASSES
{
    public class AppExecute:Command
    {

        //çalıştırılacak olan exe nin yolu dizini
        public string Path { get; set; }

        //exeye gönderilecek parametreler dosyasına ek olarak gönderilecek parametreler
        public string Parameters { get; set; }

        public override void RunContext(IEnumerable<string> selectedItemPaths, string defaultParams = null)
        {
            var files = selectedItemPaths.ToList();
            if (files.Count < 1)
                return;

            System.Diagnostics.Process process = new System.Diagnostics.Process();

            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.UseShellExecute = true;
            //dosyaların bulunduğu dizin
            string currentDir = System.IO.Path.GetDirectoryName(files[0]);


            //Ek olarak gönderilecek parametreler
            startInfo.Arguments = string.IsNullOrWhiteSpace(Parameters) ? defaultParams : defaultParams + Parameters;

            startInfo.WorkingDirectory = currentDir;//exe nin çalışacağı dosyalrın bulunduğu dizin

            startInfo.FileName = Path;//exe yolu
            process.StartInfo = startInfo;

            if (System.IO.File.Exists(process.StartInfo.FileName))
                process.Start();

            process.Dispose();
        }
    }
}
