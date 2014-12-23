using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OgzShell.CLASSES
{
    //komutlar
    public abstract class Command
    {
        //komutu çalıştır implement edilmiş sınıftaki Run methodundaki komutları çalıştırır. ShellRightClickte kullanılacak method
        public abstract void RunContext(IEnumerable<string> selectedItemPaths, string defaultParams = null);
    }
}
