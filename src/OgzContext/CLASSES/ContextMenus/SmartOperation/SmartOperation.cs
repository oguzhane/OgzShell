using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OgzContext.CLASSES.ContextMenus
{
    /*
    ..........v0.1.........:
    * Create dynamic items to windows shell right click
    * Supports Batch, Powershell script, App Execute
    * Add image to right click items
    */
    [ComVisible(true)]
    [COMServerAssociation(SharpShell.Attributes.AssociationType.Directory)]
    [COMServerAssociation(AssociationType.AllFiles)]
    public class SmartOperation : SharpContextMenu
    {
        public double Version = 0.1;

        ContextMenuStrip cxtMenuStrip = null;
        //OgzShell.Branch.BranchMenu branchMenu = null;

        public SmartOperation()
        {
            Load();
        }

        void Load()
        {
            string branchesPath = Path.Combine(System.IO.Path.GetDirectoryName(this.GetType().Assembly.Location), OgzContext.Resource1.strBranchFolderName);
            if (!Directory.Exists(branchesPath))
                Directory.CreateDirectory(branchesPath);
            string[] files = Directory.GetFiles(branchesPath, ("*" + Resource1.strBranchFileExt));

            // Create Context Menu Strip
            cxtMenuStrip = HelperSmartOp.BranchFilesToContextMenuStrip(files);
            CreateItemsClickEvent(cxtMenuStrip.Items);

            startBranchFilesListener(branchesPath);//branch dosyalarındaki değişimleri takip et
        }

        void Update()
        {
            string branchesPath = Path.Combine(System.IO.Path.GetDirectoryName(this.GetType().Assembly.Location), OgzContext.Resource1.strBranchFolderName);
            if (!Directory.Exists(branchesPath))
                Directory.CreateDirectory(branchesPath);
            string[] files = Directory.GetFiles(branchesPath, ("*" + Resource1.strBranchFileExt));

            DetachItemsClickEvents(cxtMenuStrip.Items);
            HelperSmartOp.UpdateContextMenuStrip(ref cxtMenuStrip, files);
            CreateItemsClickEvent(cxtMenuStrip.Items);
        }

        void DetachItemsClickEvents(ToolStripItemCollection items)
        {
            foreach (ToolStripMenuItem stripMenuItem in items)
            {
                stripMenuItem.Click -= stripMenuItem_Click;
                DetachItemsClickEvents(stripMenuItem.DropDownItems);
            }
        }

        DoLua _doLuaScript = new DoLua();

        
        //menu gösterilsin mi??**
        protected override bool CanShowMenu()
        {
            HelperSmartOp.LoadContextMenuBackup(ref cxtMenuStrip);

            // Firing Show Script
            _doLuaScript.DoShow(ref cxtMenuStrip, SelectedItemPaths);
            HelperSmartOp.BackupContextMenu(ref cxtMenuStrip);
            
            HelperSmartOp.PrepareContextMenuForRelease(ref cxtMenuStrip);
            // resume from here **
            CreateItemsClickEvent(cxtMenuStrip.Items);
            return true;
        }

        //Menu yü oluştur
        protected override ContextMenuStrip CreateMenu()
        {
            return cxtMenuStrip;
        }

        //context menu içindekilerin click eventini oluştur
        void CreateItemsClickEvent(ToolStripItemCollection items)
        {
            foreach (ToolStripMenuItem stripMenuItem in items)
            {
                stripMenuItem.Click -= stripMenuItem_Click;
                stripMenuItem.Click += stripMenuItem_Click;
                CreateItemsClickEvent(stripMenuItem.DropDownItems);
            }
        }

        //HelperSmartOp sınıfında tag işaretleniyor gerekli değer atanıyor.
        void stripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem;
            if (tsmi != null && tsmi.Tag is OgzShell.Branch.IBranch)
            {

                var cxtIBranch = tsmi.Tag as OgzShell.Branch.IBranch;
                try
                {
                    foreach (var cmd in cxtIBranch.Commands)
                        cmd.RunContext(SelectedItemPaths, OgzShell.Helpers.ContextHelper.GetDefaultParams(SelectedItemPaths.ToList(), cxtIBranch));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        #region FileTracker
        
        FileSystemWatcher fsw = new FileSystemWatcher();
        void startBranchFilesListener(string dir)
        {
            if (fsw != null)
            {
                fsw.Dispose();
                fsw = null;
                fsw = new FileSystemWatcher();
            }

            fsw.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime;

            fsw.Path = dir;
            fsw.IncludeSubdirectories = false;
            fsw.Filter = "*" + Resource1.strBranchFileExt;

            fsw.Deleted -= fsw_Changed;
            fsw.Deleted += fsw_Changed;

            fsw.Created -= fsw_Changed;
            fsw.Created += fsw_Changed;

            fsw.Changed -= fsw_Changed;
            fsw.Changed += fsw_Changed;

            fsw.EnableRaisingEvents = true;
            

        }


        void fsw_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed || e.ChangeType == WatcherChangeTypes.Created || e.ChangeType == WatcherChangeTypes.Deleted)
            {
                Update();
            }
            
        }
        
        #endregion

    }
}
