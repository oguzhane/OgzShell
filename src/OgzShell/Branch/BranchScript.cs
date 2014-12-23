using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OgzShell.Branch
{
    public abstract class LuaScript
    {
    }

    //Script for Right Click menu
    public class ContextLua : LuaScript
    {
        public string Show { get; set; }
        
    }
}
