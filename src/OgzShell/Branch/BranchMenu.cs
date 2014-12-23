using OgzShell.CLASSES;
using System;
using System.Text;
using System.Threading.Tasks;

namespace OgzShell.Branch
{
    //ShellRightClick için açıklama=> bu sınıf dallardan oluşur her bir dal bir sağ tıktaki bir basamağı ifade eder.
    public class BranchMenu : IBranch
    {

        //menu gösterilsin mi?
        bool canShowMenu = true;
        public bool CanShowMenu
        {
            get
            {
                return canShowMenu;
            }
            set
            {
                if (value != canShowMenu)
                    canShowMenu = value;
            }
        }

        //yazı
        public string Text { get; set; }

        //resmin yolu
        public string ImagePath { get; set; }

        //etkin veya değil//**
        bool isEnabled = true;
        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }
            set
            {
                if (value != isEnabled)
                    isEnabled = value;
            }
        }


        //Çalıştırılacak komut SmartOperation sınıfında ToolStripClick eventinde kullanılır.
        public System.Collections.Generic.List<Command> Commands { get; set; }

        BranchMenuItem root = null;

        public BranchMenu()
        {
            root = new BranchMenuItem(this);
        }

        BranchMenuItemCollection items = null;
        
        //ShellRightClick in alt basamakları
        public BranchMenuItemCollection Items {
            get {
                if (items == null)
                {
                    items = new BranchMenuItemCollection(root);
                }
                return items;
            }
        }

        public LuaScript Script { get; set; }

        public dynamic ExtData { get; set; }

        public string GetTreeIndexString()
        { return "0"; }

    }
}
