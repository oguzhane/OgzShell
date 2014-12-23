using Newtonsoft.Json;
using OgzShell.Branch;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestOgz
{
    class Program
    {
        static void Main(string[] args)
        {

            BranchMenuBuilder();
            Console.ReadLine();
            return;
            #region MyRegion

            //BranchMenuBuilder();
            //return;
            //RunPowershellScript(@"C:\OguzhanE\Projects\CSharp\WinForms\OgzShell\tst\test.ps1", " zza");
            
            #endregion

            #region MyRegionProcess
            //ProcessStartInfo startInfo = new ProcessStartInfo();
            //startInfo.FileName = @"powershell.exe";
            //startInfo.Arguments = string.Format(" -noprofile -executionpolicy bypass -file \"C:\\Users\\unquiet\\Desktop\\test2.ps1\"");


            //startInfo.UseShellExecute = true;

            //Process process = new Process();
            //process.StartInfo = startInfo;
            //process.Start();
            //Console.WriteLine("aaaaaaaaaaaaaaaaaa");
            ////process.Dispose();
            //return; 
            #endregion


            #region FromJson
            //BranchMenu branchMenu = JSONToBranchMenu(@"C:\OguzhanE\Projects\CSharp\WinForms\OgzShell\tst\pp.ogz");
            //WriteTreeIndexToConsole(branchMenu);
            //return; 
            #endregion

            #region MyRegion
            //===========================================================
            //BranchMenu brMenu = new BranchMenu();
            //brMenu.Text = "a0";
            ////brMenu.ClickCommandType = OgzShell.CLASSES.ClickCommandTypes.BatchScript;
            ////brMenu.ClickCommand = new OgzShell.CLASSES.BatchScript();

            //for (int i = 0; i < 3; i++)
            //{
            //    brMenu.Items.Add(new BranchMenuItem()
            //    {
            //        Text = brMenu.Text + "_b" + i,
            //    });
            //    for (int j = 0; j < 2; j++)
            //    {
            //        brMenu.Items[i].Items.Add(new BranchMenuItem()
            //        {
            //            Text = brMenu.Items[i].Text + "_c" + j,
            //        });
            //    }
            //}

            //BranchMenuToJSON(brMenu, @"C:\OguzhanE\Projects\CSharp\WinForms\OgzShell\tst\pp.ogz");
            //return;
            //==============================================================
            //BranchMenu branchMenu = JSONToBranchMenu(@"C:\OguzhanE\Projects\CSharp\WinForms\OgzShell\tst\pp.ogz");

            //TestContext.Class1.IsSingleInstance();

            //Console.ReadLine();

            //return;
            //var psi = new ProcessStartInfo();
            //psi.WorkingDirectory = @"C:\OguzhanE\Projects\CSharp\WinForms\OgzShell\tst\Tools\Branches";
            //psi.FileName = @"C:\Users\Oguz\Desktop\Yeni Metin Belgesi.bat";
            //psi.Arguments = " pppp saldlasd kslahdkj            2          ";
            //Process.Start(psi); 
            #endregion

            Console.ReadLine();

        }


        static void BranchMenuBuilder()
        {
            BranchMenu branchMenu = new BranchMenu()
            {
                CanShowMenu = true,
                IsEnabled = true,
                Text = "Ogz's Context",
                Script = new ContextLua()
                {
                    Show = "_MENU.Items[2][\"ExtData\"]={IsVisible=true}"
                }
            };

            branchMenu.Items.Add(new BranchMenuItem()
            {
                ExtData = new ExpandoObject()
                {

                },

                IsEnabled = true,
                Text = "Run Batch",
                Commands = new List<OgzShell.CLASSES.Command>() { 
                    new OgzShell.CLASSES.BatchScript(){
                        CanShowConsole=true,
                        Parameters=" custom par",
                        Path=@"C:\OguzhanE\Projects\CSharp\WinForms\OgzShell\tst\ts.bat",
                    },
                }
            });

            branchMenu.Items[0].ExtData.IsVisible = false;

            branchMenu.Items.Add(new BranchMenuItem()
            {
                IsEnabled = false,
                Text = "Run PS",
                ImagePath=@"C:\Users\unquiet\Downloads\1415656688_wooman.png",
                Commands = new List<OgzShell.CLASSES.Command>(){
                    new OgzShell.CLASSES.PowerShellScript(){
                        CanShowConsole=true,
                        Parameters = " my pars",
                        Path=@"C:\OguzhanE\Projects\CSharp\WinForms\OgzShell\tst\test.ps1",
                    }
                }
            });

            BranchMenuToJSON(branchMenu, @"C:\OguzhanE\Projects\CSharp\WinForms\OgzShell\src\OgzContext\bin\Release\Branches\pp.ogz");
            Console.WriteLine("BrancMenu oluşturulup json formatında kaydedildi");
        }


        //polyline = {color="blue", thickness=2, npoints=4, {x=13, y=4}, items={{x=-10, y=0}, {x=-10, y=1}}}
        static string ToLuaTableFormat(object o)
        {
            StringBuilder sb = new StringBuilder();
            Type t = typeof(BranchMenu);
            PropertyInfo[] props = t.GetProperties();
            foreach (var i in props)
            {
                object v = i.GetValue(o);
                
                if(v!=null)
                    Console.WriteLine("--------Type--->" + v.GetType());
                else
                {
                    Console.WriteLine("--------Type---> __null__ => " + i.GetType());
                }
                Console.WriteLine(string.Format("{0}={1}", i.Name, v));
            }
            return sb.ToString();
        }

        private static void RunPowershellScript(string scriptFile, string scriptParameters)
        {
            RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();
            Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration);
            runspace.Open();
            RunspaceInvoke scriptInvoker = new RunspaceInvoke(runspace);
            Pipeline pipeline = runspace.CreatePipeline();
            Command scriptCommand = new Command(scriptFile);
            Collection<CommandParameter> commandParameters = new Collection<CommandParameter>();
            foreach (string scriptParameter in scriptParameters.Split(' '))
            {
                CommandParameter commandParm = new CommandParameter(null, scriptParameter);
                commandParameters.Add(commandParm);
                scriptCommand.Parameters.Add(commandParm);
            }
            pipeline.Commands.Add(scriptCommand);
            Collection<PSObject> psObjects;
            psObjects = pipeline.Invoke();
        }
        static void WriteTreeIndexToConsole(BranchMenu bMenu)
        {
            Console.WriteLine(bMenu.Text + " => " + bMenu.GetTreeIndexString());
            foreach (var i in bMenu.Items)
                WriteTreeIndexToConsole(i);
        }

        static void WriteTreeIndexToConsole(BranchMenuItem bMenuItem)
        {
            Console.WriteLine(bMenuItem.Text + " => " + bMenuItem.GetTreeIndexString());
            foreach (var i in bMenuItem.Items)
                WriteTreeIndexToConsole(i);
        }

        /// <summary>
        /// BranchMenu yü Json formatına dönüştürür
        /// </summary>
        /// <param name="menu">BranchMenu</param>
        /// <param name="path">Jsonun formatındaki dosyanın kaydedileceği yer</param>
        public static void BranchMenuToJSON(BranchMenu menu, string path)
        {
            JsonSerializerSettings jss = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto,//abstract classlar sorun çıkartıyor sınıfın tipini belirterek serialize et
            };

            if(File.Exists(path)) File.Delete(path);
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(JsonConvert.SerializeObject(menu, jss));
            }
        }

        /// <summary>
        /// Json formatındaki dosyayı okr vs BranchMenuye dönüştürür
        /// </summary>
        /// <param name="path">Okunacak ddosya</param>
        /// <returns>Dönüştürülen BranchMenu</returns>
        public static BranchMenu JSONToBranchMenu(string path)
        {
            JsonSerializerSettings jss = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto,
            };
            BranchMenu returnObj = null;
            using (StreamReader sr = new StreamReader(path))
            {
                returnObj = JsonConvert.DeserializeObject<BranchMenu>(sr.ReadToEnd(), jss);
            }
            return returnObj;
        }
    }

}
