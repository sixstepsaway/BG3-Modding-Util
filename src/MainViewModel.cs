using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SSA.VirtualFileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG3ModdingUtil
{
    public partial class MainViewModel: ObservableObject
    {
        private readonly VFileSystem _vfs = new();

        private string _modSettingsFile;
        private string _vanillaModSettings;
        private string _moddedModSettings;
        private string _reshadeIni;
        private string _bgExe;
        private string _bgExeSteam;
        private string _bgDxExe;
        private string _bgDxExeSteam;
        private List<string> _modPaks;
        private List<string> _generalDataBinFolders;
        private List<string> _generalDataBinFiles;
        private List<string> _generalDataDataFiles;
        private List<string> _generalDataDataFolders;

        // Existing other properties...
        [ObservableProperty]
        private string _steamFolder;

        [ObservableProperty]
        private string _labelString = string.Empty;

        [ObservableProperty]
        private bool _includeReshade = true;

        [ObservableProperty]
        private bool _includeRoot = true;

        [ObservableProperty]
        private List<string> _modListList;

        [ObservableProperty]
        private string _modList;

        public MainViewModel()
        {
            InitializePaths();
            InitializeModLists();
            MoveReshadeFolder();
        }

        private void InitializePaths()
        {
            _modSettingsFile = Path.Combine(Globals.ModSettingsFolder, "modsettings.lsx");
            _vanillaModSettings = Path.Combine(Globals.BG3LoadOrders, "vanilla.lsx");
            _moddedModSettings = Path.Combine(Globals.BG3LoadOrders, "modded.lsx");
            _reshadeIni = Path.Combine(Globals.GameDataBin, "ReShade.ini");

            _bgExe = Path.Combine(Globals.GameDataBin, "bg3.exe");
            _bgExeSteam = Path.Combine(Globals.SteamBINFolder, "bg3.exe");
            _bgDxExe = Path.Combine(Globals.GameDataBin, "bg3_dx11.exe");
            _bgDxExeSteam = Path.Combine(Globals.SteamBINFolder, "bg3_dx11.exe");

            _modPaks = Directory.GetFiles(Globals.ModsFolder).ToList();
            _generalDataBinFolders = Directory.GetDirectories(Globals.GameDataBin).ToList();
            _generalDataBinFiles = Directory.GetFiles(Globals.GameDataBin).ToList();
            _generalDataDataFiles = Directory.GetFiles(Globals.GameDataData).ToList();
            _generalDataDataFolders = Directory.GetDirectories(Globals.GameDataData).ToList();
        }


        private void InitializeModLists()
        {
            ModList = Directory.GetFiles(Globals.BG3LoadOrders)
                   .Where(f => Path.GetFileName(f) != "vanilla.lsx")
                   .Select(f => Path.GetFileNameWithoutExtension(f))
                   .FirstOrDefault() ?? string.Empty;
            SteamFolder = Globals.SteamFolder;
        }

        private void MoveReshadeFolder()
        {
            if (!File.Exists(_reshadeIni)) return;
            string screenshotsFolder = Path.Combine(Globals.ModDirectory, "Screenshots");
            var reshadeLines = File.ReadAllLines(_reshadeIni).ToList();

            var savePathLine = reshadeLines.FirstOrDefault(x => x.Contains("SavePath"));
            if (savePathLine != null)
            {
                int index = reshadeLines.IndexOf(savePathLine);
                reshadeLines[index] = $"SavePath={screenshotsFolder}";
                File.WriteAllLines(_reshadeIni, reshadeLines);
            }
        }

        [RelayCommand]
        private void VanillaSwap()
        {
            LabelString = "Swapping to Vanilla";
            RemoveInstall();
            _vfs.MakeSymbolicLink(
                Path.Combine(Globals.BG3LoadOrders, "vanilla.lsx"),
                Globals.ModSettingsFolder,
                "modsettings.lsx"
            );

            LabelString = (IncludeRoot, IncludeReshade) switch
            {
                (true, _) => "Successfully swapped to Vanilla++.",
                (false, true) => "Successfully swapped to Vanilla+.",
                _ => "Successfully removed all mods."
            };
        }

        [RelayCommand]
        private void ModdedSwap()
        {
            LabelString = "Loading mods...";
            _modPaks = Directory.GetFiles(Globals.ModsFolder).ToList();
            _generalDataBinFolders = Directory.GetDirectories(Globals.GameDataBin).ToList();
            _generalDataBinFiles = Directory.GetFiles(Globals.GameDataBin).ToList();
            _generalDataDataFiles = Directory.GetFiles(Globals.GameDataData).ToList();
            _generalDataDataFolders = Directory.GetDirectories(Globals.GameDataData).ToList();
            RemoveInstall();
            foreach (string file in _modPaks)
            {
                FileInfo fileInfo = new(file);
                _vfs.MakeSymbolicLink(fileInfo.FullName, Globals.BG3ModsFolder);
            }
            foreach (string dir in _generalDataBinFolders)
            {
                _vfs.MakeJunction(dir, Globals.SteamBINFolder);
            }
            foreach (string file in _generalDataBinFiles)
            {
                FileInfo fileInfo = new(file);
                if (fileInfo.Name == "bg3.exe" || fileInfo.Name == "bg3_dx11.exe")
                {
                    FileAttributes attr = fileInfo.Attributes;
                    if (!attr.HasFlag(FileAttributes.ReparsePoint))
                    {
                        _vfs.MakeSymbolicLink(file, Globals.SteamBINFolder);
                    }
                }
                else
                {
                    _vfs.MakeSymbolicLink(file, Globals.SteamBINFolder);
                }
            }
            foreach (string file in _generalDataDataFiles)
            {
                _vfs.MakeSymbolicLink(file, Globals.SteamDataFolder);
            }
            foreach (string dir in _generalDataDataFolders)
            {
                _vfs.MakeJunction(dir, Globals.SteamDataFolder);
            }
            if (File.Exists(_modSettingsFile))
            {
                File.Delete(_modSettingsFile);
            }
            _vfs.MakeSymbolicLink(LoadOrder(ModList), Globals.ModSettingsFolder, "modsettings.lsx");
            LabelString = string.Format("Loaded modlist: {0}!", ModList);
        }


        private void RemoveInstall()
        {
            List<string> steambinfolders = Directory.GetDirectories(Globals.SteamBINFolder).ToList();
            List<string> steambinfiles = Directory.GetFiles(Globals.SteamBINFolder).ToList();
            List<string> steamdatafolders = Directory.GetDirectories(Globals.SteamDataFolder).ToList();
            List<string> steamdatafiles = Directory.GetFiles(Globals.SteamDataFolder).ToList();
            List<string> modfiles = Directory.GetFiles(Globals.BG3ModsFolder).ToList();

            foreach (string file in steambinfiles)
            {
                FileInfo fileInfo = new(file);
                FileAttributes attr = fileInfo.Attributes;
                if (attr.HasFlag(FileAttributes.ReparsePoint))
                {
                    _vfs.RemoveSymbolicLink(file);
                }
            }
            foreach (string dir in steambinfolders)
            {
                DirectoryInfo dirInfo = new(dir);
                FileAttributes attr = dirInfo.Attributes;
                if (attr.HasFlag(FileAttributes.ReparsePoint))
                {
                    _vfs.RemoveJunction(dir);
                }
            }
            foreach (string file in steamdatafiles)
            {
                FileInfo fileInfo = new(file);
                FileAttributes attr = fileInfo.Attributes;
                if (attr.HasFlag(FileAttributes.ReparsePoint))
                {
                    _vfs.RemoveSymbolicLink(file);
                }
            }
            foreach (string dir in steamdatafolders)
            {
                DirectoryInfo dirInfo = new(dir);
                FileAttributes attr = dirInfo.Attributes;
                if (attr.HasFlag(FileAttributes.ReparsePoint))
                {
                    _vfs.RemoveJunction(dir);
                }
            }
            foreach (string file in modfiles)
            {
                FileInfo fileInfo = new(file);
                FileAttributes attr = fileInfo.Attributes;
                if (attr.HasFlag(FileAttributes.ReparsePoint))
                {
                    _vfs.RemoveSymbolicLink(file);
                }
            }

            if (File.Exists(_modSettingsFile))
            {
                File.Delete(_modSettingsFile);
            }
        }

        private void IncludeReshadeYes()
        {
            if (!File.Exists(_reshadeIni))
            {
                string reshadeshaders = Path.Combine(Globals.GameDataBin, "reshade-shaders");
                string reshadepresets = Path.Combine(Globals.GameDataBin, "reshade-presets");
                string reshadelog = Path.Combine(Globals.GameDataBin, "Reshade.log");
                _vfs.MakeJunction(reshadeshaders, Globals.SteamBINFolder);
                _vfs.MakeJunction(reshadepresets, Globals.SteamBINFolder);
                _vfs.MakeSymbolicLink(_reshadeIni, Globals.SteamBINFolder);
                _vfs.MakeSymbolicLink(reshadelog, Globals.SteamBINFolder);
            }
        }

        private void IncludePartyCamYes()
        {
            if (_generalDataBinFolders.Count != 0)
            {
                foreach (string dir in _generalDataBinFolders)
                {
                    _vfs.MakeJunction(dir, Globals.SteamBINFolder);
                }
            }
            if (_generalDataBinFolders.Count != 0)
            {
                foreach (string file in _generalDataBinFolders)
                {
                    FileInfo fileInfo = new(file);
                    if (fileInfo.Name != "DWrite.dll")
                    {
                        _vfs.MakeSymbolicLink(file, Globals.SteamBINFolder);
                    }
                }
            }

            if (_generalDataDataFiles.Count != 0)
            {
                foreach (string dir in _generalDataDataFiles)
                {
                    DirectoryInfo directoryInfo = new(dir);
                    if (directoryInfo.Name == "Mods")
                    {
                        _vfs.MakeJunction(dir, Globals.SteamDataFolder);
                    }
                    else if (directoryInfo.Name == "PatchFiles")
                    {
                        _vfs.MakeJunction(dir, Globals.SteamDataFolder);
                    }
                }
            }
            if (_generalDataDataFiles.Count != 0)
            {
                foreach (string file in _generalDataDataFiles)
                {
                    FileInfo fileInfo = new(file);
                    if (fileInfo.Name == "PartyLimitBegonePatcher.bat")
                    {
                        _vfs.MakeSymbolicLink(file, Globals.SteamDataFolder);
                    }
                }
            }
        }

        private void RemoveReshade()
        {
            string reshadeinisteam = Path.Combine(Globals.SteamBINFolder, "ReShade.ini");
            string reshadeshaders = Path.Combine(Globals.SteamBINFolder, "reshade-shaders");
            string reshadepresets = Path.Combine(Globals.SteamBINFolder, "reshade-presets");
            string reshadelog = Path.Combine(Globals.SteamBINFolder, "Reshade.log");
            if (File.Exists(reshadelog))
            {
                _vfs.RemoveSymbolicLink(reshadelog);
            }
            if (File.Exists(_reshadeIni))
            {
                _vfs.RemoveSymbolicLink(reshadeinisteam);
            }
            if (Directory.Exists(reshadeshaders))
            {
                _vfs.RemoveJunction(reshadeshaders);
            }
            if (Directory.Exists(reshadepresets))
            {
                _vfs.RemoveJunction(reshadepresets);
            }
        }

        

        private string LoadOrder(string name)
        {
            return Path.Combine(Globals.BG3LoadOrders, string.Format("{0}.lsx", name));
        }

    }
}
