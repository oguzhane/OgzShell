using OgzShell.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OgzShell.CLASSES
{
    //batchkomutu için sınıf
    public class BatchScript : Command
    {
        //konsol ekranı gösterilsin mi?
        bool canShowConsole = true;
        public bool CanShowConsole
        {
            get { return canShowConsole; }
            set
            {
                if (value != canShowConsole)
                    canShowConsole = value;
            }
        }

        //çalıştırılacak olan batchscript in yolu dizini
        public string Path { get; set; }

        //scripte gönderilecek parametreler dosyasına ek olarak gönderilecek parametreler
        public string Parameters { get; set; }

        //ShellContext için çalıştır Bat dosyasını çalıştır.RightClick
        public override void RunContext(IEnumerable<string> selectedItemPaths, string defaultParams = null)
        {
            var files = selectedItemPaths.ToList();
            if (files.Count < 1)
                return;

            System.Diagnostics.Process process = new System.Diagnostics.Process();

            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.UseShellExecute = true;
            if (canShowConsole == false)//siyah ekran konsol görünmesin
            {
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;//hidden olacak
                startInfo.CreateNoWindow = true;//true olacak
            }

            //seçili dosyaların bulunduğu dizin
            string currentDir = System.IO.Path.GetDirectoryName(files[0]);

            //Ek olarak gönderilecek parametreler
            startInfo.Arguments = string.IsNullOrWhiteSpace(Parameters) ? defaultParams : defaultParams + Parameters;

            startInfo.WorkingDirectory = currentDir;//bath kodun çalışacağı dosyalrın bulunduğu dizin

            startInfo.FileName = Path;//bat dosya yolu
            process.StartInfo = startInfo;

            if (System.IO.File.Exists(process.StartInfo.FileName))
                process.Start();

            process.Dispose();

        }
    }
}
