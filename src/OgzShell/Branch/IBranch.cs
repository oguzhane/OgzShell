using OgzShell.CLASSES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OgzShell.Branch
{
    public interface IBranch
    {
        System.Collections.Generic.List<Command> Commands { get; set; }
        
        string ImagePath { get; set; }

        bool IsEnabled { get; set; }//**
        BranchMenuItemCollection Items { get; }
        string Text { get; set; }
        dynamic ExtData { get; set; }
        string GetTreeIndexString();
    }
}
