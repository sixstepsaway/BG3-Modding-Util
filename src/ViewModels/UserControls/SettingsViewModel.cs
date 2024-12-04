using BG3ModdingUtil.Views.UserControls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nucs.JsonSettings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace BG3ModdingUtil.ViewModels
{
    public partial class SettingsViewModel: ObservableObject
    {
        [ObservableProperty]
        public bool _useVanillaReshade = true;
        [ObservableProperty]
        public bool _useVanillaPartyCam = true;
        [ObservableProperty]
        public string _modsFolder = Path.Combine(Environment.CurrentDirectory, "ModProfiles");
        [ObservableProperty]
        public string _bg3SteamFolder = @"C:\Program Files (x86)\Steam\steamapps\common\Baldurs Gate 3";

        [RelayCommand]
        public void SaveSettings()
        {
            ConfigSettings settings = JsonSettings.Load<ConfigSettings>();
            if(settings == null) settings = JsonSettings.Construct<ConfigSettings>();

            settings.UseVanillaReshade = UseVanillaReshade;
            settings.UseVanillaPartyCam = UseVanillaPartyCam;
            settings.ModsFolder = ModsFolder;
            settings.BG3SteamFolder = Bg3SteamFolder;
            settings.Save();
        }

    }
}
