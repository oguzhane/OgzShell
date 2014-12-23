using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestContext
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.ClassOfExtension, ".txt")]
    public class Class1 : SharpContextMenu
    {
        protected override bool CanShowMenu()
        {
            return true;
        }

        ToolStripMenuItem itemBaglanti;

        protected override System.Windows.Forms.ContextMenuStrip CreateMenu()
        {
            //yeni bir menu oluştur
            var menu = new ContextMenuStrip();
            itemBaglanti = new ToolStripMenuItem
            {
                Text = "Secilenleri Say",
            };
            itemBaglanti.Click += (s1, e1) =>
            {
                StringBuilder sb = new StringBuilder();
                foreach (var f in SelectedItemPaths)
                {
                    sb.AppendLine(System.IO.Path.GetFileName(f));//dosya isimlerini ekle
                }
                sb.AppendLine("-------------------------");
                sb.AppendLine(SelectedItemPaths.Count() + " tane dosya seçildi");
                MessageBox.Show(sb.ToString());
            };
            var item2 = new ToolStripMenuItem("Test2Yeahh");
            item2.Click += (s2, e2) => {
                MessageBox.Show("selamm");
            };
            item2.MouseHover += (s3, e3) => {
                MessageBox.Show("common");
            };
            menu.Items.Add(itemBaglanti);//baglatiyi ekle
            menu.Items.Add(item2);
            return menu;
        }


        public void ChangeText()
        {
            itemBaglanti.Text = "asdfgg";
        }

        public static void AccessInstance()
        {

            var instances = from t in System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                            where t.GetInterfaces().Contains(typeof(SharpContextMenu))
                                     && t.GetConstructor(Type.EmptyTypes) != null
                            select Activator.CreateInstance(t) as Class1;

            foreach (var instance in instances)
            {
                instance.ChangeText(); // where Foo is a method of ISomething
            }
        }

        static System.Threading.Mutex _m;

        public static bool IsSingleInstance()
        {
            try
            {
                // Try to open existing mutex.
                System.Threading.Mutex.OpenExisting("PERL");
            }
            catch
            {
                // If exception occurred, there is no such mutex.
                Class1._m = new System.Threading.Mutex(true, "PERL");

                // Only one instance.
                return true;
            }
            // More than one instance.
            return false;
        }
    }
}
