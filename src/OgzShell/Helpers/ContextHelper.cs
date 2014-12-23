using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OgzShell.Helpers
{
    public static class ContextHelper
    {
        public static string GetDefaultParams(List<string> paths, OgzShell.Branch.IBranch ibranch = null)
        {
            if (paths == null || paths.Count < 1) return null;

            //dosyaların bulunduğu dizin
            string currentDir = System.IO.Path.GetDirectoryName(paths[0]);

            StringBuilder sbArgs = new StringBuilder();

            sbArgs.Append(string.Format(ResShell.strDir + " \"{0}\"", currentDir));// /dir "currentdir" 

            if (ibranch != null)
            {
                sbArgs.Append(string.Format(ResShell.strTreeIndex + " {0}", ibranch.GetTreeIndexString()));
            }

            for(int j=0; j < paths.Count; j++){
                sbArgs.Append(string.Format(" \"{0}\"", System.IO.Path.GetFileName(paths[j])));
            }

            ////Argumentleri(dosya adlarını) bath koda göndermek için sıraya diz
            //for (int i = 0; i < paths.Count - 1; i++)
            //{
            //    sbArgs.Append(string.Format("\"{0}\" ", System.IO.Path.GetFileName(paths[i])));
            //}

            ////son gönderilen parametreden sonra boşluk olmaması için
            //sbArgs.Append(string.Format("\"{0}\"", System.IO.Path.GetFileName(paths[paths.Count - 1])));
            return sbArgs.ToString();
        }

    }
}
