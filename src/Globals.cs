using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG3ModdingUtil
{
    public static class Globals
    {
        //AppData BG3 Folders
        public static string AppLocalFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static string BG3Folder = System.IO.Path.Combine(AppLocalFolder, @"Larian Studios\Baldur's Gate 3");
        public static string BG3ModsFolder = System.IO.Path.Combine(BG3Folder, "Mods");
        public static string BG3ModSettingsLsx = System.IO.Path.Combine(BG3Folder, @"PlayerProfiles\Public\modsettings.lsx");

        //Folders from this Tool

        public static string UtilDirectory = Environment.CurrentDirectory;
        public static string UtilLogsDirectory = System.IO.Path.Combine(UtilDirectory, "Logs");
        public static string UtilAppLog = System.IO.Path.Combine(UtilLogsDirectory, "BGModUtil.log");
        public static string UtilModProfilesFolder = System.IO.Path.Combine(UtilDirectory, "ModProfiles");
        public static string UtilVanillaProfileFolder = System.IO.Path.Combine(UtilDirectory, "Vanilla");

        //Folder containg BG3 Install Game files/.exe
        public static string SteamFolder = @"C:\Program Files (x86)\Steam\steamapps\common\Baldurs Gate 3";
        public static string SteamBINFolder = System.IO.Path.Combine(SteamFolder, "bin");
        public static string SteamDataFolder = System.IO.Path.Combine(SteamFolder, "Data");        
    }
}
