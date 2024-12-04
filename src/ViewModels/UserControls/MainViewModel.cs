using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nucs.JsonSettings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace BG3ModdingUtil.ViewModels.UserControls
{
    public partial class MainViewModel: ObservableObject
    {
        #region Properties
        private const string VANILLA_PROFILE = "Vanilla";
        private const string BG3_DX11 = "bg3_dx11.exe";
        private const string BG3_VULKAN = "bg3.exe";
        private const string RESHADE_CONFIG = "ReShade.ini";
        private const string MOD_PAKS = "Mods";
        private const string GAME_DATA = "GameData";
        private const string BIN = "Bin";


        [ObservableProperty]
        private List<string> _modProfiles;
        [ObservableProperty]
        private int _modSelectedIndex =0;
        [ObservableProperty]
        private string _bg3SteamPath = Globals.SteamFolder;
        [ObservableProperty]
        private string _textStatus = "";

        #endregion

        public MainViewModel()
        {
            InitPaths();
            CreateFolders();
            GetModProfileNames();
        }

        private void InitPaths()
        {
            ConfigSettings settings = JsonSettings.Load<ConfigSettings>();

            Globals.UtilModProfilesFolder = settings.ModsFolder;
            Globals.SteamFolder = settings.BG3SteamFolder;
            Globals.SteamBINFolder = Path.Combine(settings.BG3SteamFolder, "bin");
            Globals.SteamDataFolder = Path.Combine(settings.BG3SteamFolder, "Data");

            Directory.CreateDirectory(Globals.UtilModProfilesFolder);
            Directory.CreateDirectory(Globals.UtilVanillaProfileFolder);

            

            Bg3SteamPath = Globals.SteamFolder;
        }

        private void CreateFolders()
        {
            //Vanilla
            Directory.CreateDirectory(Path.Combine(Globals.UtilVanillaProfileFolder, MOD_PAKS));
            Directory.CreateDirectory(Path.Combine(Globals.UtilVanillaProfileFolder, GAME_DATA));
            Directory.CreateDirectory(Path.Combine(Globals.UtilVanillaProfileFolder, BIN));

            //ModProfile
            Directory.CreateDirectory(Path.Combine(Globals.UtilModProfilesFolder, "Template", MOD_PAKS));
            Directory.CreateDirectory(Path.Combine(Globals.UtilModProfilesFolder, "Template", GAME_DATA));
            Directory.CreateDirectory(Path.Combine(Globals.UtilModProfilesFolder, "Template", BIN));

            using (FileStream fileStream = File.Create(Path.Combine(Globals.UtilModProfilesFolder, "Template", "template.lsx")))
            {
                Assembly.GetExecutingAssembly().GetManifestResourceStream("BG3ModdingUtil.template.lsx").CopyTo(fileStream);
            }
        }

        private void GetModProfileNames()
        {
            var modProfiles = GetModProfiles();
            List<string> names = new();
            foreach ( var modProfile in modProfiles )
            {
                if(modProfile == VANILLA_PROFILE)
                {
                    names.Add(VANILLA_PROFILE);
                    continue;
                }
                names.Add(Path.GetFileName(Path.GetDirectoryName(modProfile)));
            }
            ModProfiles = names;
        }

        private IEnumerable<string> GetModProfiles()
        {
            List<string> modProfiles = [VANILLA_PROFILE];
            var subfolders = Directory.GetDirectories(Globals.UtilModProfilesFolder);
            foreach (var subfolder in subfolders)
            {
                var lsxConfig = Directory.GetFiles(subfolder, "*.lsx", SearchOption.TopDirectoryOnly).FirstOrDefault();
                if (lsxConfig != null)
                {
                    modProfiles.Add(lsxConfig);
                }
            }
            return modProfiles;
        }

        [RelayCommand]
        private void ApplyChanges()
        {
            var entry = ModProfiles[ModSelectedIndex];
            if(entry == null) return;

            if(entry == "Vanilla")
            {
                TextStatus = "Applying Vanilla Profile";
                UseVanillaProfile();
                TextStatus = "Done Applying Vanilla";
            }
            else
            {
                TextStatus = $"Applying {entry} Profile";
                UseModdedProfile();
                TextStatus = $"Done Applying {entry}";
            }
            
        }

        [RelayCommand]
        private void UseVanillaProfile()
        {
            RemoveInstall();
            var vanillaLsx = Path.Combine(Globals.UtilVanillaProfileFolder, $"{VANILLA_PROFILE}.lsx");
            bool state = VirtualFileSystem.MakeSymbolicLinkDirect(vanillaLsx, Globals.BG3ModSettingsLsx);
            if(state == false) return;

            //TODO: include section for IncludeRoot and IncludeReshade
            ConfigSettings settings = JsonSettings.Load<ConfigSettings>();
            if (settings.UseVanillaReshade)
            {
                IncludeReshadeVanilla();
            }
            if(settings.UseVanillaPartyCam)
            {
                IncludePartyCamVanilla();
            }
        }

        [RelayCommand]
        private void UseModdedProfile()
        {
            var modLsx = GetModProfiles().ToArray()[ModSelectedIndex];
            var modDir = Path.GetDirectoryName(modLsx);
            if(string.IsNullOrEmpty(modDir))return;
            var modPaks = Directory.GetFiles(Path.Combine(modDir, MOD_PAKS));
            var profileBinFolders = Directory.GetDirectories(Path.Combine(modDir, BIN));
            var profileBinFiles = Directory.GetFiles(Path.Combine(modDir, BIN));
            var profileDataFolders = Directory.GetDirectories(Path.Combine(modDir, GAME_DATA));
            var profileDataFiles = Directory.GetFiles(Path.Combine(modDir, GAME_DATA));
            RemoveInstall();
            bool state;
            foreach (string file in modPaks)
            {
                FileInfo fileInfo = new(file);
                state = VirtualFileSystem.MakeSymbolicLinkAuto(fileInfo.FullName, Globals.BG3ModsFolder);
                if (state == false) return;
            }
            foreach (string dir in profileBinFolders)
            {
                state = VirtualFileSystem.MakeJunction(dir, Globals.SteamBINFolder);
                if (state == false) return;
            }
            foreach (string file in profileBinFiles)
            {
                FileInfo fileInfo = new(file);
                if (fileInfo.Name == BG3_VULKAN || fileInfo.Name == BG3_DX11)
                {
                    FileAttributes attr = fileInfo.Attributes;
                    if (!attr.HasFlag(FileAttributes.ReparsePoint))
                    {
                        state = VirtualFileSystem.MakeSymbolicLinkAuto(file, Globals.SteamBINFolder);
                        if (state == false) return;
                    }
                }
                else
                {
                    state = VirtualFileSystem.MakeSymbolicLinkAuto(file, Globals.SteamBINFolder);
                    if (state == false) return;
                }
            }
            foreach (string dir in profileDataFolders)
            {
                state = VirtualFileSystem.MakeJunction(dir, Globals.SteamDataFolder);
                if (state == false) return;
            }
            foreach (string file in profileDataFiles)
            {
                state = VirtualFileSystem.MakeSymbolicLinkAuto(file, Globals.SteamDataFolder);
                if (state == false) return;
            }
            if (File.Exists(Globals.BG3ModSettingsLsx))
            {
                File.Delete(Globals.BG3ModSettingsLsx);
            }
            state = VirtualFileSystem.MakeSymbolicLinkDirect(modLsx, Globals.BG3ModSettingsLsx);
            if (state == false) return;

        }

        private void IncludeReshadeVanilla()
        {
            var profileBin = Path.Combine(Globals.UtilVanillaProfileFolder, BIN);
            var reshadeIni = Path.Combine(profileBin, RESHADE_CONFIG);
            bool state;
            if (!File.Exists(reshadeIni))
            {
                string reshadeshaders = Path.Combine(profileBin, "reshade-shaders");
                string reshadepresets = Path.Combine(profileBin, "reshade-presets");
                string reshadelog = Path.Combine(profileBin, "Reshade.log");
                state = VirtualFileSystem.MakeJunction(reshadeshaders, Globals.SteamBINFolder);
                if (state == false) return;
                state = VirtualFileSystem.MakeJunction(reshadepresets, Globals.SteamBINFolder);
                if (state == false) return;
                state = VirtualFileSystem.MakeSymbolicLinkAuto(reshadeIni, Globals.SteamBINFolder);
                if (state == false) return;
                state = VirtualFileSystem.MakeSymbolicLinkAuto(reshadelog, Globals.SteamBINFolder);
                if (state == false) return;
            }
        }

        private void IncludePartyCamVanilla()
        {            
            var vanillaBinFolders = Directory.GetDirectories(Path.Combine(Globals.UtilVanillaProfileFolder, BIN)).ToList();
            var vanillaDataFiles = Directory.GetFiles(Path.Combine(Globals.UtilVanillaProfileFolder, GAME_DATA)).ToList();
            bool state;
            if (vanillaBinFolders.Count != 0)
            {
                foreach (string dir in vanillaBinFolders)
                {
                    state = VirtualFileSystem.MakeJunction(dir, Globals.SteamBINFolder);
                    if (state == false) return;
                }
            }
            if (vanillaBinFolders.Count != 0)
            {
                foreach (string file in vanillaBinFolders)
                {
                    FileInfo fileInfo = new(file);
                    if (fileInfo.Name != "DWrite.dll")
                    {
                        state = VirtualFileSystem.MakeSymbolicLinkAuto(file, Globals.SteamBINFolder);
                        if (state == false) return;
                    }
                }
            }

            if (vanillaDataFiles.Count != 0)
            {
                foreach (string dir in vanillaDataFiles)
                {
                    DirectoryInfo directoryInfo = new(dir);
                    if (directoryInfo.Name == "Mods")
                    {
                        state = VirtualFileSystem.MakeJunction(dir, Globals.SteamDataFolder);
                        if (state == false) return;
                    }
                    else if (directoryInfo.Name == "PatchFiles")
                    {
                        state = VirtualFileSystem.MakeJunction(dir, Globals.SteamDataFolder);
                        if (state == false) return;
                    }
                }
            }
            if (vanillaDataFiles.Count != 0)
            {
                foreach (string file in vanillaDataFiles)
                {
                    FileInfo fileInfo = new(file);
                    if (fileInfo.Name == "PartyLimitBegonePatcher.bat")
                    {
                        state = VirtualFileSystem.MakeSymbolicLinkAuto(file, Globals.SteamDataFolder);
                        if (state == false) return;
                    }
                }
            }
        }

        private void RemoveInstall()
        {
            List<string> steambinfolders = Directory.GetDirectories(Globals.SteamBINFolder).ToList();
            List<string> steambinfiles = Directory.GetFiles(Globals.SteamBINFolder).ToList();
            List<string> steamdatafolders = Directory.GetDirectories(Globals.SteamDataFolder).ToList();
            List<string> steamdatafiles = Directory.GetFiles(Globals.SteamDataFolder).ToList();
            List<string> modfiles = Directory.GetFiles(Globals.BG3ModsFolder).ToList();
            bool state;

            foreach (string file in steambinfiles)
            {
                FileInfo fileInfo = new(file);
                FileAttributes attr = fileInfo.Attributes;
                if (attr.HasFlag(FileAttributes.ReparsePoint))
                {
                    state = VirtualFileSystem.RemoveSymbolicLink(file);
                    if (state == false) return;
                }
            }
            foreach (string dir in steambinfolders)
            {
                DirectoryInfo dirInfo = new(dir);
                FileAttributes attr = dirInfo.Attributes;
                if (attr.HasFlag(FileAttributes.ReparsePoint))
                {
                    state = VirtualFileSystem.RemoveJunction(dir);
                    if (state == false) return;
                }
            }
            foreach (string file in steamdatafiles)
            {
                FileInfo fileInfo = new(file);
                FileAttributes attr = fileInfo.Attributes;
                if (attr.HasFlag(FileAttributes.ReparsePoint))
                {
                    state = VirtualFileSystem.RemoveSymbolicLink(file);
                    if (state == false) return;
                }
            }
            foreach (string dir in steamdatafolders)
            {
                DirectoryInfo dirInfo = new(dir);
                FileAttributes attr = dirInfo.Attributes;
                if (attr.HasFlag(FileAttributes.ReparsePoint))
                {
                    state = VirtualFileSystem.RemoveJunction(dir);
                    if (state == false) return;
                }
            }
            foreach (string file in modfiles)
            {
                FileInfo fileInfo = new(file);
                FileAttributes attr = fileInfo.Attributes;
                if (attr.HasFlag(FileAttributes.ReparsePoint))
                {
                    state = VirtualFileSystem.RemoveSymbolicLink(file);
                    if (state == false) return;
                }
            }

            if (File.Exists(Globals.BG3ModSettingsLsx))
            {
                File.Delete(Globals.BG3ModSettingsLsx);
            }
        }


    }
}
