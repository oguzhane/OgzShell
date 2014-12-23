using OgzShell.Branch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Windows.Forms;
using System.Dynamic;
namespace OgzContext.CLASSES.ContextMenus
{
    public static class HelperSmartOp
    {

        static JsonSerializerSettings jss = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,//abstract classlar sorun çıkartıyor sınıfın tipini belirterek (de)serialize et
        };

        /// <summary>
        /// BranchMenu yü Json formatına dönüştürür
        /// </summary>
        /// <param name="menu">BranchMenu</param>
        /// <param name="path">Jsonun formatındaki dosyanın kaydedileceği yer</param>
        public static void BranchMenuToJSON(BranchMenu menu, string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(JsonConvert.SerializeObject(menu, jss));
            }
        }

        /// <summary>
        /// Json formatındaki dosyayı okur ve BranchMenuye dönüştürür
        /// </summary>
        /// <param name="path">Okunacak dosya</param>
        /// <returns>Dönüştürülen BranchMenu</returns>
        public static BranchMenu JSONToBranchMenu(string path)
        {
            BranchMenu returnObj = null;
            using (StreamReader sr = new StreamReader(path))
            {
                returnObj = JsonConvert.DeserializeObject<BranchMenu>(sr.ReadToEnd(), jss);
            }
            return returnObj;
        }

        //BranchMenu yü ContextMenuStrip e dönüştür
        public static ContextMenuStrip BranchFilesToContextMenuStrip(string[] branchFiles)
        {
            ContextMenuStrip returnObj = new ContextMenuStrip();

            foreach (string file in branchFiles)
            {
                try
                {
                    // Read BranchMenu from JSON file
                    var branchMenu = HelperSmartOp.JSONToBranchMenu(file);

                    // Add new BranchMenu to ContextMenuStrip
                    AddBranchToContextMenuStrip(ref returnObj, branchMenu);
                }
                catch { }
            }

            return returnObj;
        }
        
        #region Update_ContextMenuStrip
        
        public static void UpdateContextMenuStrip(ref ContextMenuStrip cxtMenuStrip, string[] branchFiles)
        {
            cxtMenuStrip.Items.Clear();

            foreach (string file in branchFiles)
            {
                try
                {
                    // Read BranchMenu from JSON file
                    var branchMenu = HelperSmartOp.JSONToBranchMenu(file);
                    // Add new BranchMenu to ContextMenuStrip
                    AddBranchToContextMenuStrip(ref cxtMenuStrip, branchMenu);
                }
                catch { }
            }

            //RemoveAllItems(ref cxtMenuStrip);
        }

        public static void UpdateContextMenuStrip(ref ContextMenuStrip cxtMenuStrip, List<BranchMenu> branchMenus)
        {
            cxtMenuStrip.Items.Clear();

            foreach (var branchMenu in branchMenus)
            {
                AddBranchToContextMenuStrip(ref cxtMenuStrip, branchMenu);
            }
        }
        
        #endregion

        #region Private_Methods

        // adds New BranchMenu to Context
        static void AddBranchToContextMenuStrip(ref ContextMenuStrip cxtMenuStrip, BranchMenu branchMenu)
        {
            try
            {
                //ana branch
                var mainStripMenu = GetMainMenu(branchMenu);

                foreach (var item in branchMenu.Items)
                {
                    // child branch
                    var stripMenuItem = GetMenuItem(item);
                    mainStripMenu.DropDownItems.Add(stripMenuItem);
                    AddItemToStripMenuItem(item, stripMenuItem);
                }

                cxtMenuStrip.Items.Add(mainStripMenu);
            }
            catch { }
        }

        //BranchMenu den ToolStripMenuItem oluştur
        static ToolStripMenuItem GetMainMenu(BranchMenu menu)
        {
            var mainStripMenu = new ToolStripMenuItem();

            ApplyStripMenuItem(ref mainStripMenu, menu);

            mainStripMenu.Tag = menu;//SmartOperation stripMenuItem_Click kontrol et
            return mainStripMenu;
        }

        //BranchMenuItem dan ToolStripMenuItem oluştur
        static ToolStripMenuItem GetMenuItem(BranchMenuItem item)
        {
            var stripMenuItem = new ToolStripMenuItem();

            ApplyStripMenuItem(ref stripMenuItem, item);

            stripMenuItem.Tag = item;//SmartOperation stripMenuItem_Click kontrol et

            return stripMenuItem;
        }

        static void ApplyStripMenuItem(ref ToolStripMenuItem stripMenuItem, IBranch iBranch)
        {
            stripMenuItem.Text = iBranch.Text;
            stripMenuItem.Enabled = iBranch.IsEnabled;

            if (!string.IsNullOrWhiteSpace(iBranch.ImagePath))
            {
                try
                {
                    stripMenuItem.Image = System.Drawing.Image.FromFile(iBranch.ImagePath);
                }
                catch { }
            }

            if (iBranch.ExtData == null || !(iBranch.ExtData is ExpandoObject))
                return;

            /*var dictExtData = (IDictionary<string, object>)iBranch.ExtData;

            if (dictExtData.ContainsKey(ExtDataConsts.IsVisible) && dictExtData[ExtDataConsts.IsVisible] is bool)
                stripMenuItem.Visible = (bool)dictExtData[ExtDataConsts.IsVisible];*/



        }

        // Itemın Itemslarını ekle
        static void AddItemToStripMenuItem(BranchMenuItem item, ToolStripMenuItem stripMenuItem)
        {
            foreach (var i in item.Items)
            {
                var newItem = GetMenuItem(i);
                stripMenuItem.DropDownItems.Add(newItem);
                AddItemToStripMenuItem(i, newItem);
            }
        }

        #endregion

        public static List<BranchMenu> ToBranchMenuList(ref ContextMenuStrip contextMenu)
        {
            List<BranchMenu> returnObj = new List<BranchMenu>();
            foreach (var item in contextMenu.Items)
            {
                if (item is ToolStripMenuItem && (item as ToolStripMenuItem).Tag is BranchMenu)
                {
                    var mainStripMenu = item as ToolStripMenuItem;
                    var branchMenu = mainStripMenu.Tag as BranchMenu;
                    returnObj.Add(branchMenu);
                }
            }
            return returnObj;
        }

        #region BackupLoadPrepare_ContextMenu

        static List<ToolStripMenuItem> BackupCxtMenuStrip = new List<ToolStripMenuItem>();
        public static void BackupContextMenu(ref ContextMenuStrip contextMenu)
        {
            BackupCxtMenuStrip.Clear();
            for (int i = 0; i < contextMenu.Items.Count; i++)
                BackupCxtMenuStrip.Add(contextMenu.Items[i] as ToolStripMenuItem);
        }


        public static void LoadContextMenuBackup(ref ContextMenuStrip contextMenu, bool NotLoadifBackupCountIsZero = true)
        {
            if (NotLoadifBackupCountIsZero && BackupCxtMenuStrip.Count == 0)
                return;

            contextMenu.Items.Clear();
            foreach (var i in BackupCxtMenuStrip)
                contextMenu.Items.Add(i);

        }

        public static void PrepareContextMenuForRelease(ref ContextMenuStrip contextMenu)
        {
            for (int i = contextMenu.Items.Count - 1; i >= 0; i--)
            {
                if(contextMenu.Items[i].Tag is BranchMenu && ((contextMenu.Items[i].Tag as BranchMenu).CanShowMenu == false))
                {
                    contextMenu.Items.RemoveAt(i);
                    continue;
                }

                IDictionary<string, object> _extData = (IDictionary<string, object>)(contextMenu.Items[i].Tag as IBranch).ExtData;

                if (_extData.ContainsKey("IsVisible") && !(bool)_extData["IsVisible"])
                    contextMenu.Items.RemoveAt(i);
                else
                    prepareItemForRelease(contextMenu.Items[i] as ToolStripMenuItem);

            }
        }

        static void prepareItemForRelease(ToolStripMenuItem item)
        {
            for (int i = item.DropDown.Items.Count - 1; i >= 0; i--)
            {
                IDictionary<string, object> _extData = (IDictionary<string, object>)(item.DropDown.Items[i].Tag as IBranch).ExtData;

                if (_extData.ContainsKey("IsVisible") && !(bool)_extData["IsVisible"])
                    item.DropDown.Items.RemoveAt(i);
                else
                    prepareItemForRelease(item.DropDown.Items[i] as ToolStripMenuItem);

            }
        }

        #endregion

    }

    public static class ExtDataConsts
    {
        public const string IsVisible = "IsVisible";
    }
}
