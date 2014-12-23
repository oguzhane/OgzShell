using NLua;
using OgzShell.CLASSES;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OgzContext.CLASSES.ContextMenus
{
    //Apply Lua script for RightClick Context
    class DoLua
    {
        const string _SELECTEDPATHS = "_SELECTEDPATHS";
        const string _MENU = "_MENU";

        internal void DoShow(ref System.Windows.Forms.ContextMenuStrip cxtMenuStrip, IEnumerable<string> selectedPaths = null)
        {
            
            List<string> selectedItemPaths = new List<string>(); ;
            if (selectedPaths != null && selectedPaths.Count() > 0)
                selectedItemPaths = selectedPaths.ToList();

            var branchMenuList = HelperSmartOp.ToBranchMenuList(ref cxtMenuStrip);

            var newBranchMenuList = new List<OgzShell.Branch.BranchMenu>();
            bool isAnyScriptRunned = false;
            foreach (var branchMenu in branchMenuList)
            {

                if (branchMenu.Script == null || !(branchMenu.Script is OgzShell.Branch.ContextLua) || string.IsNullOrWhiteSpace((branchMenu.Script as OgzShell.Branch.ContextLua).Show))
                {
                    newBranchMenuList.Add(branchMenu);
                    continue; 
                }
                
                StringBuilder sbScript = new StringBuilder();

                #region Assign _SELECTEDPATHS
                //"_SELECTEDPATHS = {\"Sunday\", \"Monday\", \"Tuesday\", \"Wednesday\"}
                sbScript.Append(_SELECTEDPATHS + " = {");
                int selItemsCount = selectedItemPaths.Count;
                for (int i = 0; i < selItemsCount; i++)
                {
                    sbScript.Append(string.Format(Resource1.strLuaStringFormat + ", ", selectedItemPaths[i]));
                    //sbScript.Append(string.Format("[[\"{0}\"]], ", selectedItemPaths[i]));//**__
                }
                sbScript.Append("}");
                //sbScript.Append(string.Format(Resource1.strLuaStringFormat + "}} ", selectedItemPaths[selItemsCount - 1]));
                //sbScript.Append(string.Format("[[\"{0}\"]]}}", selectedItemPaths[selItemsCount - 1]));
                #endregion

                #region Assign _MENU
                sbScript.AppendLine();//End Current Line
                //polyline = {color="blue", thickness=2, npoints=4, {x=13, y=4}, items={{x=-10, y=0}, {x=-10, y=1}}}
                sbScript.Append(_MENU + " = {");
                sbScript.Append(string.Format("CanShowMenu={0}, ", branchMenu.CanShowMenu.ToString().ToLower()));
                

                var _script = branchMenu.Script as OgzShell.Branch.ContextLua;
                sbScript.Append(string.Format("Script={{Show=" + Resource1.strLuaStringFormat + "}}, ", _script.Show));
                //sbScript.Append(string.Format("Script={{Show=\"{0}\"}}, ", _script.Show));
                // Script=
                // Remove first char '{' and append to sbScript
                sbScript.Append(GetIBranchLuaTableString(branchMenu).Remove(0, 1));
                
                #endregion

                sbScript.AppendLine();//end current line

                // Append user lua script 
                sbScript.Append((branchMenu.Script as OgzShell.Branch.ContextLua).Show);

                //string finalScript = sbScript.ToString();
                //System.IO.File.WriteAllText(@"C:\OguzhanE\Projects\CSharp\WinForms\OgzShell\tst\luaScr.txt", finalScript);

                //Run Lua show script
                using (Lua lua = new Lua())
                {
                    
                    lua.LoadCLRPackage();
                    lua.DoString(sbScript.ToString());
                    
                    isAnyScriptRunned = true;
                    // Add 'New'(may be edit BranchMenu) BranchMenu to list
                    newBranchMenuList.Add(LuaTableToBranchMenu(lua.GetTable(_MENU)));
                }
            }

            if (isAnyScriptRunned && newBranchMenuList.Count > 0)
            {
                /// Apply Changes to ContextMenuStrip
                HelperSmartOp.UpdateContextMenuStrip(ref cxtMenuStrip, newBranchMenuList);
            }

        }

        static string GetIBranchLuaTableString(OgzShell.Branch.IBranch iBranch)
        {
            StringBuilder sbIBranch = new StringBuilder();
            sbIBranch.Append("{");
            sbIBranch.Append(string.Format("Text={0}, ", iBranch.Text != null ? string.Format(Resource1.strLuaStringFormat, iBranch.Text) : "nil"));
            //sbIBranch.Append(string.Format("Text={0}, ", iBranch.Text != null ? '"' + iBranch.Text + '"' : "nil"));
            sbIBranch.Append(string.Format("IsEnabled={0}, ", iBranch.IsEnabled.ToString().ToLower()));
            sbIBranch.Append(string.Format("ImagePath={0}, ", iBranch.ImagePath != null ? string.Format(Resource1.strLuaStringFormat, iBranch.ImagePath) : "nil"));
            //sbIBranch.Append(string.Format("ImagePath={0}, ", iBranch.ImagePath != null ? '"' + iBranch.ImagePath + '"' : "nil"));
            
            #region AppendExtData

            if (iBranch.ExtData != null && iBranch.ExtData is ExpandoObject)
            {
                sbIBranch.Append("ExtData={");
                var dictExtData = (IDictionary<string, object>)iBranch.ExtData;
                foreach (var i in dictExtData)
                {
                    if (i.Value == null)
                        sbIBranch.Append(i.Key + "=nil, ");
                    else if (i.Value is string)
                        sbIBranch.Append(string.Format("{0}=" + string.Format(Resource1.strLuaStringFormat, i.Value) + ", ", i.Key));
                    //sbIBranch.Append(string.Format("{0}=\"{1}\", ", i.Key, i.Value));
                    else if (i.Value is int || i.Value is double)
                        sbIBranch.Append(string.Format("{0}={1}, ", i.Key, i.Value));
                    else if (i.Value is bool)
                        sbIBranch.Append(string.Format("{0}={1}, ", i.Key, i.Value.ToString().ToLower()));
                }
                sbIBranch.Append("}, ");
            }
            else
                sbIBranch.Append("ExtData=nil, ");

            #endregion

            #region AppendCommands

            if (iBranch.Commands == null)
            {
                sbIBranch.Append("Commands=nil, ");                
                goto CommandsNil;
            }

            sbIBranch.Append("Commands={");
            foreach (var cmd in iBranch.Commands)
            {
                if (cmd == null) continue;

                if (cmd is PowerShellScript)
                {
                    var _pss = cmd as PowerShellScript;
                    sbIBranch.Append("{_type=\"PowerShellScript\", ");
                    sbIBranch.Append(string.Format("CanShowConsole={0}, ", _pss.CanShowConsole.ToString().ToLower()));
                    sbIBranch.Append(string.Format("Parameters={0}, ", _pss.Parameters != null ? string.Format(Resource1.strLuaStringFormat, _pss.Parameters) : "nil"));
                    //sbIBranch.Append(string.Format("Parameters={0}, ", _pss.Parameters != null ? '"' + _pss.Parameters + '"' : "nil"));
                    sbIBranch.Append(string.Format("Path={0}", _pss.Path != null ? string.Format(Resource1.strLuaStringFormat, _pss.Path) : "nil"));
                    //sbIBranch.Append(string.Format("Path={0}", _pss.Path != null ? '"' + _pss.Path + '"' : "nil"));
                    sbIBranch.Append("}, ");
                }
                else if (cmd is AppExecute)
                {
                    var _appExe = cmd as AppExecute;
                    sbIBranch.Append("{_type=\"AppExecute\", ");
                    sbIBranch.Append(string.Format("Parameters={0}, ", _appExe.Parameters != null ? string.Format(Resource1.strLuaStringFormat, _appExe.Parameters) : "nil"));
                    //sbIBranch.Append(string.Format("Parameters={0}, ", _appExe.Parameters != null ? '"' + _appExe.Parameters + '"' : "nil"));
                    sbIBranch.Append(string.Format("Path={0}", _appExe.Path != null ? string.Format(Resource1.strLuaStringFormat, _appExe.Path) : "nil"));
                    //sbIBranch.Append(string.Format("Path={0}", _appExe.Path != null ? '"' + _appExe.Path + '"' : "nil"));
                    sbIBranch.Append("}, ");
                }
                else if (cmd is BatchScript)
                {
                    var _bs = cmd as BatchScript;
                    sbIBranch.Append("{_type=\"BatchScript\", ");
                    sbIBranch.Append(string.Format("CanShowConsole={0}, ", _bs.CanShowConsole.ToString().ToLower()));
                    sbIBranch.Append(string.Format("Parameters={0}, ", _bs.Parameters != null ? string.Format(Resource1.strLuaStringFormat, _bs.Parameters) : "nil"));
                    //sbIBranch.Append(string.Format("Parameters={0}, ", _bs.Parameters != null ? '"' + _bs.Parameters + '"' : "nil"));
                    sbIBranch.Append(string.Format("Path={0}", _bs.Path != null ? string.Format(Resource1.strLuaStringFormat, _bs.Path) : "nil"));
                    //sbIBranch.Append(string.Format("Path={0}", _bs.Path != null ? '"' + _bs.Path + '"' : "nil"));
                    sbIBranch.Append("}, ");
                }
            }

            sbIBranch.Append("}, ");

            // skip assign commands
            CommandsNil:
                
            #endregion

            if (iBranch.Items != null)
            {
                sbIBranch.Append("Items={");

                for (int i = 0; i < iBranch.Items.Count; i++)
                {
                    var item = iBranch.Items[i];
                    sbIBranch.Append(GetIBranchLuaTableString(item) + ", ");
                }

                sbIBranch.Append("}, ");
            }
            else
                sbIBranch.Append("Items=nil, ");

            sbIBranch.Append("}");
            
            return sbIBranch.ToString();
        }

        static OgzShell.Branch.BranchMenu LuaTableToBranchMenu(LuaTable table)
        {
            #region OldImplementBranchMenu
            /*OgzShell.Branch.BranchMenu returnObj = new OgzShell.Branch.BranchMenu()
            {
                CanShowMenu = (bool)table["CanShowMenu"],
                ImagePath = (string)table["ImagePath"],
                IsEnabled =(bool)table["IsEnabled"],
                Text = (string)table["Text"],
            };
            returnObj.ExtData = new ExpandoObject();

            var extDataTable =  table["ExtData"];

            if(extDataTable != null && extDataTable is NLua.LuaTable)
            {
                IDictionary<string, object> _extData = (IDictionary<string, object>)returnObj.ExtData;
                foreach(var i in (NLua.LuaTable)extDataTable)
                {
                    var value = (extDataTable as NLua.LuaTable)[i];
                    if(value is bool || value is string || value is double || value is int )
                        _extData.Add(i.ToString(), value);
                }
            }*/
            
            #endregion

            OgzShell.Branch.IBranch returnObj = new OgzShell.Branch.BranchMenu()
            {
                CanShowMenu = (bool)table["CanShowMenu"],
                Script = new OgzShell.Branch.ContextLua()
                {
                    Show = (string)((table["Script"] as LuaTable)["Show"]),
                }
            };
            
            
            SetIBranchFromLuaTable(ref returnObj, ref table);

            return returnObj as OgzShell.Branch.BranchMenu;
        }

        static void SetIBranchFromLuaTable(ref OgzShell.Branch.IBranch iBranch, ref LuaTable table)
        {
            iBranch.Text = (string)table["Text"];
            iBranch.IsEnabled = (bool)table["IsEnabled"];
            iBranch.ImagePath = (string)table["ImagePath"];
            iBranch.ExtData = new ExpandoObject();

            #region Commands
            var commandsTable = table["Commands"];
            if (commandsTable != null && commandsTable is LuaTable)
            {
                iBranch.Commands = new List<Command>();
                foreach (var key in (commandsTable as LuaTable).Keys)
                {
                    var value = (commandsTable as LuaTable)[key];
                    if (value is LuaTable)
                    {
                        var _commandTable = value as LuaTable;

                        if (_commandTable["_type"] == null || !(_commandTable["_type"] is string))
                            continue;

                        string _typeOfCommand = (string)_commandTable["_type"];
                        if (_typeOfCommand == "PowerShellScript")
                        {
                            iBranch.Commands.Add(new PowerShellScript() { 
                                CanShowConsole = (bool)_commandTable["CanShowConsole"],
                                Parameters = (string)_commandTable["Parameters"],
                                Path = (string)_commandTable["Path"]
                            });
                        }
                        else if (_typeOfCommand == "AppExecute")
                        {
                            iBranch.Commands.Add(new AppExecute() {
                                Parameters = (string)_commandTable["Parameters"],
                                Path = (string)_commandTable["Path"]
                            });
                        }
                        else if (_typeOfCommand == "BatchScript")
                        {
                            iBranch.Commands.Add(new BatchScript() {
                                CanShowConsole = (bool)_commandTable["CanShowConsole"],
                                Parameters = (string)_commandTable["Parameters"],
                                Path = (string)_commandTable["Path"]
                            });
                        }
                        
                    }
                }
            }
            else
                iBranch.Commands = null;

            #endregion

            #region ExtData
            var extDataTable = table["ExtData"];

            if (extDataTable != null && extDataTable is LuaTable)
            {
                IDictionary<string, object> _extData = (IDictionary<string, object>)iBranch.ExtData;
                foreach (var i in (extDataTable as LuaTable).Keys)
                {
                    var value = (extDataTable as LuaTable)[i];
                    if (value is bool || value is string || value is double || value is int)
                        _extData.Add(i.ToString(), value);
                }
            }
            #endregion

            #region Items
            var itemsTable = table["Items"];
            if (itemsTable != null && itemsTable is LuaTable)
            {

                foreach (var key in (itemsTable as LuaTable).Keys)
                {
                    if (!((itemsTable as LuaTable)[key] is LuaTable))
                        continue;

                    LuaTable itemTable = (itemsTable as LuaTable)[key] as LuaTable;
                    OgzShell.Branch.IBranch branchMenuItem = new OgzShell.Branch.BranchMenuItem()
                    {

                    };

                    SetIBranchFromLuaTable(ref branchMenuItem, ref itemTable);
                    iBranch.Items.Add((branchMenuItem as OgzShell.Branch.BranchMenuItem));
                }

            }
            #endregion

        }

    }

}
