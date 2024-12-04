using BG3ModdingUtil.Views;
using BG3ModdingUtil.Views.UserControls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nucs.JsonSettings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BG3ModdingUtil.ViewModels
{
    public partial class RootViewModel: ObservableObject
    {
        [ObservableProperty]
        private UserControl _currentUserControl;

        private UserControl _settingsView;
        private UserControl _mainView;

        public RootViewModel()
        {
            ConfigSettings settings = JsonSettings.Load<ConfigSettings>();
            if(!Path.Exists(settings.BG3SteamFolder)&& !Path.Exists(settings.ModsFolder))
            {
                if(_settingsView == null)
                {
                    _settingsView = new SettingsView();
                    CurrentUserControl = _settingsView;
                }
                else
                {
                    CurrentUserControl = _settingsView;
                }
            }
            else
            {
                if(_mainView == null)
                {
                    _mainView = new MainView();
                    CurrentUserControl = _mainView;
                }
                else
                {
                    CurrentUserControl = _mainView;
                }
            }

        }



        [RelayCommand]
        public void ToggleView()
        {
            if(CurrentUserControl.GetType() == typeof(MainView))
            {
                if (_settingsView == null)
                {
                    _settingsView = new SettingsView();
                    CurrentUserControl = _settingsView;
                }
                else
                {
                    CurrentUserControl = _settingsView;
                }
            }
            else
            {
                if (_mainView == null)
                {
                    _mainView = new MainView();
                    CurrentUserControl = _mainView;
                }
                else
                {
                    CurrentUserControl = _mainView;
                }
            }
        }
    }
}
