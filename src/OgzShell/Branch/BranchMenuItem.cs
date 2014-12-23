using System;
using System.Text;
using System.Threading.Tasks;
using OgzShell.CLASSES;

namespace OgzShell.Branch
{
    public class BranchMenuItem : IBranch
    {

        //yazı
        public string Text { get; set; }

        //resmin yolu
        public string ImagePath { get; set; }

        //etkin veya değil//**
        bool isEnabled = true;
        public bool IsEnabled
        {
            get {
                return isEnabled;
            }
            set {
                if (value != isEnabled)
                    isEnabled = value;
            }
        }


        //Çalıştırılacak komut SmartOperation sınıfında ToolStripClick eventinde kullanılır
        public System.Collections.Generic.List<Command> Commands { get; set; }

        //Sahip olan üst basamak(cascade) Al.
        public object GetParent()
        {
            if (owner != null && owner.branchMenu != null)
                return owner.branchMenu;
            else
                return owner;
        }

        internal BranchMenuItem owner = null;

        internal BranchMenu branchMenu = null;

        internal BranchMenuItem(BranchMenu branchMenu):this()
        {
            this.branchMenu = branchMenu;
        }

        public BranchMenuItem()
        { }

        BranchMenuItemCollection items = null;

        public BranchMenuItemCollection Items
        {
            get
            {
                if (items == null)
                {
                    items = new BranchMenuItemCollection(this);
                }
                return items;
            }
        }

        public dynamic ExtData { get; set; }

        public string GetTreeIndexString()
        {
            var treeIndexString = _getParentTreeIndexString();
            int currentIndex = _getIndex();
            if (currentIndex < 0)
                return treeIndexString;
            else
                return string.Format("{0}-{1}", treeIndexString, currentIndex);
        }
        string _getParentTreeIndexString()
        {
            var parent = this.GetParent();
            if (parent is BranchMenu)
                return (parent as BranchMenu).GetTreeIndexString();
            else
                return (parent as BranchMenuItem).GetTreeIndexString();
        }
        int _getIndex()
        {
            var parent = this.GetParent();
            if (parent is BranchMenu)
                return (parent as BranchMenu).Items.IndexOf(this);
            else
                return (parent as BranchMenuItem).Items.IndexOf(this);
        }

    }
}
