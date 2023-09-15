I made this for a friend in like eight hours before we played so we'd have the same BG3 modded setup for one game, but be able to continue playing our vanilla game vanilla. I figured it might be useful for someone else too for similar reasons. Please read instructions on how to use it, or it won't work!  
  
![](https://i.imgur.com/qfLD9HW.png)
The app comes with:  
-   6 files in its root directory
-   3 folders
-   1 vanilla load order
  
Don't rename the folders, don't rename the vanilla load order.  
  
## Wait, this is an app and I don't know you! How do I know this isn't a virus?
  
[You can check out the source here.](https://github.com/sixstepsaway/BG3-Modding-Util)   

## Usage:  

-   Download and unzip into the same folder you have your [BG3 Mod Manager](https://github.com/LaughingLeader/BG3ModManager) in for ease of use.
-   Put mods you intend to install into the Mods folder    
-   If you're planning to install, for example, NativeMods, you put those into GameData/bin
-   You install root mods this way! It'll work. Instead of putting things directly into your steamapps BG3/Data and BG3/bin folders, put them into the GameData/Data and GameData/bin folders.
-   Reshade can be installed the same way. Just put dxgi.dll, ReShade.ini, and the reshade-shaders folder into the GameData/bin folder.     
-   I recommend making a reshade-presets folder too to keep things tidy.    
-   You can even install the Party Limit Patcher this way. Just plop the applicable files into GameData/bin. THIS INCLUDES BG3.EXE AND BG3.DX11.EXE.    
-   To get those files easily without overwriting your original ones: Open your steam BG3 bin folder. COPY the exes elsewhere. Follow the Party Limit Patcher install instructions from the GameData/Data folder, but use the steam exes. Let it create its new exes. Move its new exes into the GameData/bin folder. Delete its backup copies from your steam bin folder. Move YOUR backup copy back in there so it's fresh as the day it was installed. (You can probably use the backup exes the PLP creates but I am a paranoid android so I prefer to use my own copies. YMMV.) Now you have your patched version in your separate folder and can still use the unpatched version if you wish.    
-   Once you have your stuff set up and ready, go into Load Orders and make a copy of Vanilla (or, if you already have a load order, copy that into here). Name the file whatever you want. Seriously, you don't have to name it ModSettings. You can name it AstarionsCuteFangs and it'll work - I promise.    
-   Open BG3Config.cfg and change the contents to point at YOUR Baldur's Gate 3 steam folder. Default it will say C:\Program Files (x86)\Steam\steamapps\common\Baldurs Gate 3, so if yours is installed elsewhere, you want to change this to wherever that is, like E:\SteamStuff\common\Baldurs Gate 3 or whatever.     
-   Run BG3ModdingUtil.exe
-   You'll get two tickboxes, two buttons and a drop down. If you want to revert to pure vanilla (remove all mods and additions from your BG3 folders and revert to completely vanilla), untick both checks and click "Vanilla". It will say, "Successfully Removed All Mods". This will keep the link between ModSettings.lsx in your AppData folder, but everything else will be completely as it was. If you have Reshade installed and want Vanilla but with Reshade, tick "Include Reshade in Vanilla". If you want party size mods and NativeMods included, tick the second checkbox. Click vanilla to install those where applicable.
-   Click the drop-down below the Modded button and pick the Load Order file you just created. AstarionsCuteFangs, for example.    
-   Click "Modded"
-   The app will tell you it successfully loaded AstarionsCuteFangs.
-   Open your BG3 Mod Manager.
-   You should now see your mods in the mod manager!    
-   Use the mod manager as you already would.    
-   When you're ready to uninstall, press the Vanilla button, with or without the checkboxes checked.  
  
## Very Very Simple Instructions:  
-   Mods go in /Mods
-   If a mod says "Install into your /bin folder" or "Install into your /Data folder", it goes into GameData/bin or GameData/Data respectively
-   Don't rename vanilla.lsx, but you can name any other load orders whatever you wish.
-   Run app.
-   Pick your load order from the drop-down.
-   Click Modded.
-   It will give you a confirmation.
-   Load your mod manager. Continue as normal. When you want to remove the mods, run "Vanilla". 

## Uninstalling: 
-   Uncheck the checkboxes and click Vanilla. It will say "Successfully Removed All Mods". Go into Load Orders and copy vanilla.lsx. Go to %AppData%\Local\Larian Studios\Baldur's Gate 3\Player Profiles\public\. Delete modsettings.lsx. Paste vanilla.lsx in and rename it modsettings.lsx.      
-   Delete BG3Config.cfg and the five BG3ModdingUtil files from wherever you put them. Continue however you like.  

![](https://storage.ko-fi.com/cdn/kofi3.png)  
 
## Notes:
-   If it throws an exception about permissions or just plain crashes, run it as an administrator. My friend had to, I didn't, I don't know why.
-   It's built on net 7.0, so you need that if you don't have it.      
-   Reshade screenshots will save to the Screenshots folder. All other edits made to ReShade.ini will stick except changing this folder. Delete ReShade.ini from GameData/bin and put it into the steam BG3 bin folder directly if you really want to change this :)
-   Symlinks mean if Steam updates something that affects the files, the originals in the GameData/bin folder will be updated too. If you're modding I recommend turning off your automatic steam updates _anyway_, but just be aware if Steam updates the exe in your GameData/bin folder, for example, it won't have updated the real exe. So make sure to return to vanilla before running any updates. It's safer that way, anyway.
-   
## Something isn't working!
  
If you have a problem let me know and I'll try and fix it. The app should throw pop up an exception, so share that if you can. Please use the [issue tracker](https://github.com/sixstepsaway/BG3-Modding-Util/issues).
