using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace OgzShell.Branch
{
    //Alt basamaklar kümesi
    public class BranchMenuItemCollection : ObservableCollection<BranchMenuItem>
    {

        BranchMenuItem currentItem;

        internal BranchMenuItemCollection(BranchMenuItem currentItem)
        {
            this.currentItem = currentItem;
            this.CollectionChanged -= BranchMenuItemCollection_CollectionChanged;
            this.CollectionChanged += BranchMenuItemCollection_CollectionChanged;
        }

        void BranchMenuItemCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add || e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                foreach (var x in e.NewItems)
                {
                    (x as BranchMenuItem).owner = currentItem;//Sahibini(üst basamağı) ata belirt.
                }
            }
        }

    }
}
