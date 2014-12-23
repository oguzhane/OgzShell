OgzShell
=======
This is helper tool to windows explore's context menu. i wrote to dynamically add item to windows explore's right click menu.

**Features**

 - Add item to windows explore's right click menu by editing .ogz file.
 - Run batch file script, powershell script or execute program when item clicked.
 - When right clicked on selecteds, dynamically add/remove item to windows explore's right click menu using Lua Script.

**Downloads**

 [OgzShell-Release.zip](http://1drv.ms/1CwY0kS)

**Installation**
 

 - Download OgzShell-Release.zip
 - Extract the file contents wherever you choose, and navigate to the root directory of the OgzShell-Release
 - Then run tools\ServerManager.exe
 - Then load server \bin\OgzContext.dll(File > Load Server..) and install & register server (Server > register, install)
 
**Example Usage**

 content of the .ogz file

    {
      "CanShowMenu": true,
      "Text": "Ogz's Context",
      "IsEnabled": true,
      "Items": [
        {
          "Text": "Batch and AppExecute",
          "IsEnabled": true,
          "Commands": [
            {
              "$type": "OgzShell.CLASSES.BatchScript, OgzShell",
              "CanShowConsole": true,
              "Path": "C:\\OguzhanE\\Projects\\Temp\\examples\\ts.bat",
              "Parameters": " custom par"
            },
    		{
              "$type": "OgzShell.CLASSES.AppExecute, OgzShell",
              "Path": "C:\\Windows\\system32\\calc.exe",
              "Parameters": ""
            }
          ],
          "Items": [],
          "ExtData": {
            "$type": "System.Dynamic.ExpandoObject, System.Core",
            "IsVisible": true
          }
        },
        {
          "Text": "Run PS",
          "ImagePath": "C:\\OguzhanE\\Projects\\Temp\\examples\\Basket.png",
          "IsEnabled": true,
          "Commands": [
            {
              "$type": "OgzShell.CLASSES.PowerShellScript, OgzShell",
              "CanShowConsole": true,
              "Path": "C:\\OguzhanE\\Projects\\Temp\\examples\\test.ps1",
              "Parameters": " my pars"
            }
          ],
          "Items": [],
    	  "ExtData": {
            "$type": "System.Dynamic.ExpandoObject, System.Core",
            "IsVisible": false
          }
        }
      ],
      "Script": {
        "$type": "OgzShell.Branch.ContextLua, OgzShell",
        "Show": "_MENU.Items[2][\"ExtData\"]={IsVisible=true}"
      }
    }
  
  ![result of the .ogz file](http://i.imgur.com/HfWfC7f.png)  
