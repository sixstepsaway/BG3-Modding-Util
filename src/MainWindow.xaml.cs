using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SSA.VirtualFileSystem;
using System.Drawing.Text;
using System.Runtime.CompilerServices;

namespace BG3ModdingUtil
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            base.DataContext = new MainViewModel();
        }
    }

    public class Globals{
        public static string AppLocalFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static string BG3Folder = System.IO.Path.Combine(AppLocalFolder, @"Larian Studios\Baldur's Gate 3");
        public static string BG3ModsFolder = System.IO.Path.Combine(BG3Folder, "Mods");        
        public static string ModSettingsFolder = System.IO.Path.Combine(BG3Folder, @"PlayerProfiles\Public");
        public static string ModDirectory = Environment.CurrentDirectory;
        public static string BG3LoadOrders = System.IO.Path.Combine(ModDirectory, "Load Orders");
        public static string AppLog = System.IO.Path.Combine(ModDirectory, "BGModUtil.log");
        public static string ModsFolder = System.IO.Path.Combine(ModDirectory, "Mods");
        public static string GameDataFolder = System.IO.Path.Combine(ModDirectory, "GameData");
        public static string GameDataBin = System.IO.Path.Combine(GameDataFolder, "bin");
        public static string GameDataData = System.IO.Path.Combine(GameDataFolder, "Data");
        public static string SteamFolder = File.ReadAllText(System.IO.Path.Combine(ModDirectory, "BG3Config.cfg"));
        public static string SteamBINFolder = System.IO.Path.Combine(SteamFolder, "bin");
        public static string SteamDataFolder = System.IO.Path.Combine(SteamFolder, "Data");
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        VFileSystem vfs = new();
        
        public List<string> modpaks = Directory.GetFiles(Globals.ModsFolder).ToList();
        public List<string> generaldataBINFolders = Directory.GetDirectories(Globals.GameDataBin).ToList();
        public List<string> generaldataBINFiles = Directory.GetFiles(Globals.GameDataBin).ToList();
        public List<string> generaldataDATAFiles = Directory.GetFiles(Globals.GameDataData).ToList();
        public List<string> generaldataDATAFolders = Directory.GetDirectories(Globals.GameDataData).ToList();
        public string ModSettingsFile = System.IO.Path.Combine(Globals.ModSettingsFolder, "modsettings.lsx");
        public string VanillaModSettings = System.IO.Path.Combine(Globals.BG3LoadOrders, "vanilla.lsx");
        public string ModdedModSettings = System.IO.Path.Combine(Globals.BG3LoadOrders, "modded.lsx");
        public string reshadeini = System.IO.Path.Combine(Globals.GameDataBin, "ReShade.ini");
        public string bgexe = System.IO.Path.Combine(Globals.GameDataBin, "bg3.exe");
        public string bgexeSteam = System.IO.Path.Combine(Globals.SteamBINFolder, "bg3.exe");
        public string bgdxexe = System.IO.Path.Combine(Globals.GameDataBin, "bg3_dx11.exe");
        public string bgdxexeSteam = System.IO.Path.Combine(Globals.SteamBINFolder, "bg3_dx11.exe");

        private string _steamfolder;        

        public string SteamFolder {
            get { return _steamfolder;}
            set { _steamfolder = value;
                RaisePropertyChanged("SteamFolder");
            }
        }
        private string _labelstring;        

        public string LabelString {
            get { return _labelstring;}
            set { _labelstring = value;
                RaisePropertyChanged("LabelString");
            }
        }

        private bool _includereshade;
        public bool IncludeReshade{
            get { return _includereshade;}
            set { _includereshade = value;
                RaisePropertyChanged("IncludeReshade");
            }
        }
        private bool _includeroot;
        public bool IncludeRoot{
            get { return _includeroot;}
            set { _includeroot = value;
                RaisePropertyChanged("IncludeRoot");
            }
        }

        private List<string> _modlistlist;
        public List<string> ModListList {
            get { return _modlistlist;}
            set { _modlistlist = value;
                RaisePropertyChanged("ModListList");
            }
        }

        private string _modlist;
        public string ModList{
            get { return _modlist; }
            set { _modlist = value; 
                RaisePropertyChanged("ModList"); 
            }
        }
        

        public MainViewModel(){
            ModListList = new();
            var mll = Directory.GetFiles(Globals.BG3LoadOrders).ToList();
            foreach (string f in mll){
                FileInfo fileInfo = new(f);
                if (fileInfo.Name != "vanilla.lsx"){
                    string n = fileInfo.Name.Replace(fileInfo.Extension, "");
                    ModListList.Add(n);
                }
            }
            ModList = ModListList[0];
            SteamFolder = Globals.SteamFolder;
            IncludeReshade = true;
            IncludeRoot = true;
            MoveReshadeFolder();
            LabelString = "";
        }

        public void LogError([CallerLineNumber] int lineNumber = 0){
            
        }        


        public ICommand VanillaClick
        {
            get { return new DelegateCommand(this.VanillaSwap); }
        }

        public ICommand ModdedClick
        {
            get { return new DelegateCommand(this.ModdedSwap); }
        }
        public ICommand testclick
        {
            get { return new DelegateCommand(this.test); }
        }

        private void test(){

        }

        private void MoveReshadeFolder(){
            if (File.Exists(reshadeini)){
                string ScreenshotsFolder = System.IO.Path.Combine(Globals.ModDirectory, "Screenshots");
                List<string> reshade = File.ReadAllLines(reshadeini).ToList();
                string rs = reshade.Where(x => x.Contains("SavePath")).First();
                int indx = reshade.IndexOf(rs);
                reshade[indx] = string.Format("SavePath={0}", ScreenshotsFolder);
                string[] arr = reshade.ToArray<string>();
                File.WriteAllLines(reshadeini, arr);
            }
            
        }


        private void VanillaSwap(){
            LabelString = "Swapping to Vanilla";
            RemoveInstall();            
            vfs.MakeSymbolicLink(VanillaModSettings, Globals.ModSettingsFolder, "modsettings.lsx");
            if (IncludeRoot == true){
                IncludePartyCamYes();
                LabelString = "Successfully swapped to Vanilla++.";
            } else if (IncludeReshade == true){
                IncludeReshadeYes();
                LabelString = "Successfully swapped to Vanilla+.";
            } else {
                LabelString = "Successfully removed all mods.";
            }
            
        }

        private void RemoveInstall(){
            List<string> steambinfolders = Directory.GetDirectories(Globals.SteamBINFolder).ToList();
            List<string> steambinfiles = Directory.GetFiles(Globals.SteamBINFolder).ToList();
            List<string> steamdatafolders = Directory.GetDirectories(Globals.SteamDataFolder).ToList();
            List<string> steamdatafiles = Directory.GetFiles(Globals.SteamDataFolder).ToList();
            List<string> modfiles = Directory.GetFiles(Globals.BG3ModsFolder).ToList();
            
            foreach (string file in steambinfiles){
                FileInfo fileInfo = new(file);
                FileAttributes attr = fileInfo.Attributes;
                if(attr.HasFlag(FileAttributes.ReparsePoint)){
                    vfs.RemoveSymbolicLink(file);
                }
            }
            foreach (string dir in steambinfolders){
                DirectoryInfo dirInfo = new(dir);
                FileAttributes attr = dirInfo.Attributes;
                if(attr.HasFlag(FileAttributes.ReparsePoint)){
                    vfs.RemoveJunction(dir);
                }
            }
            foreach (string file in steamdatafiles){
                FileInfo fileInfo = new(file);
                FileAttributes attr = fileInfo.Attributes;
                if(attr.HasFlag(FileAttributes.ReparsePoint)){
                    vfs.RemoveSymbolicLink(file);
                }
            }
            foreach (string dir in steamdatafolders){
                DirectoryInfo dirInfo = new(dir);
                FileAttributes attr = dirInfo.Attributes;
                if(attr.HasFlag(FileAttributes.ReparsePoint)){
                    vfs.RemoveJunction(dir);
                }
            }
            foreach (string file in modfiles){
                FileInfo fileInfo = new(file);
                FileAttributes attr = fileInfo.Attributes;
                if(attr.HasFlag(FileAttributes.ReparsePoint)){
                    vfs.RemoveSymbolicLink(file);
                }
            }
 
            if (File.Exists(ModSettingsFile)){
                File.Delete(ModSettingsFile);
            }
        }

        private void IncludeReshadeYes(){
            if (!File.Exists(reshadeini)){
                string reshadeshaders = System.IO.Path.Combine(Globals.GameDataBin, "reshade-shaders");
                string reshadepresets = System.IO.Path.Combine(Globals.GameDataBin, "reshade-presets");
                string reshadelog = System.IO.Path.Combine(Globals.GameDataBin, "Reshade.log");
                vfs.MakeJunction(reshadeshaders, Globals.SteamBINFolder);
                vfs.MakeJunction(reshadepresets, Globals.SteamBINFolder);
                vfs.MakeSymbolicLink(reshadeini, Globals.SteamBINFolder);
                vfs.MakeSymbolicLink(reshadelog, Globals.SteamBINFolder);
            }            
        }

        private void IncludePartyCamYes(){
            if (generaldataBINFolders.Count != 0){
                foreach (string dir in generaldataBINFolders){                
                    vfs.MakeJunction(dir, Globals.SteamBINFolder);
                }
            }
            if (generaldataBINFiles.Count != 0){
                foreach (string file in generaldataBINFiles){
                    FileInfo fileInfo = new(file);
                    if (fileInfo.Name != "DWrite.dll"){                    
                        vfs.MakeSymbolicLink(file, Globals.SteamBINFolder);
                    }
                }
            }
            
            if (generaldataDATAFolders.Count != 0){
                foreach (string dir in generaldataDATAFolders){
                    DirectoryInfo directoryInfo = new(dir);
                    if (directoryInfo.Name == "Mods"){
                        vfs.MakeJunction(dir, Globals.SteamDataFolder);
                    } else if (directoryInfo.Name == "PatchFiles"){
                        vfs.MakeJunction(dir, Globals.SteamDataFolder);
                    }                
                } 
            }
            if (generaldataDATAFiles.Count != 0){
                foreach (string file in generaldataDATAFiles){
                    FileInfo fileInfo = new(file);
                    if (fileInfo.Name == "PartyLimitBegonePatcher.bat"){
                        vfs.MakeSymbolicLink(file, Globals.SteamDataFolder);
                    }                
                } 
            }
        }

        private void RemoveReshade(){
            string reshadeinisteam = System.IO.Path.Combine(Globals.SteamBINFolder, "ReShade.ini");
            string reshadeshaders = System.IO.Path.Combine(Globals.SteamBINFolder, "reshade-shaders");
            string reshadepresets = System.IO.Path.Combine(Globals.SteamBINFolder, "reshade-presets");
            string reshadelog = System.IO.Path.Combine(Globals.SteamBINFolder, "Reshade.log");
            if (File.Exists(reshadelog)){
                vfs.RemoveSymbolicLink(reshadelog);
            }
            if (File.Exists(reshadeini)){
                vfs.RemoveSymbolicLink(reshadeinisteam);
            }
            if (Directory.Exists(reshadeshaders)){
                vfs.RemoveJunction(reshadeshaders);
            }
            if (Directory.Exists(reshadepresets)){
                vfs.RemoveJunction(reshadepresets);
            }
        }

        private void ModdedSwap(){  
            LabelString = "Loading mods...";
            modpaks = Directory.GetFiles(Globals.ModsFolder).ToList();
            generaldataBINFolders = Directory.GetDirectories(Globals.GameDataBin).ToList();
            generaldataBINFiles = Directory.GetFiles(Globals.GameDataBin).ToList();
            generaldataDATAFiles = Directory.GetFiles(Globals.GameDataData).ToList();
            generaldataDATAFolders = Directory.GetDirectories(Globals.GameDataData).ToList();
            RemoveInstall();
            foreach (string file in modpaks){
                FileInfo fileInfo = new(file);
                vfs.MakeSymbolicLink(fileInfo.FullName, Globals.BG3ModsFolder);
            }
            foreach (string dir in generaldataBINFolders){                
                vfs.MakeJunction(dir, Globals.SteamBINFolder);
            }    
            foreach (string file in generaldataBINFiles){
                FileInfo fileInfo = new(file);
                if (fileInfo.Name == "bg3.exe" || fileInfo.Name == "bg3_dx11.exe"){
                    FileAttributes attr = fileInfo.Attributes;
                    if (!attr.HasFlag(FileAttributes.ReparsePoint)){
                        vfs.MakeSymbolicLink(file, Globals.SteamBINFolder);
                    }
                } else {
                    vfs.MakeSymbolicLink(file, Globals.SteamBINFolder);
                }                
            }   
            foreach (string file in generaldataDATAFiles){
                vfs.MakeSymbolicLink(file, Globals.SteamDataFolder);
            }    
            foreach (string dir in generaldataDATAFolders){
                vfs.MakeJunction(dir, Globals.SteamDataFolder);
            }   
            if (File.Exists(ModSettingsFile)){
                File.Delete(ModSettingsFile);
            }
            vfs.MakeSymbolicLink(LoadOrder(ModList), Globals.ModSettingsFolder, "modsettings.lsx");         
            LabelString = string.Format("Loaded modlist: {0}!", ModList);
        }

        private string LoadOrder(string name){
            return System.IO.Path.Combine(Globals.BG3LoadOrders, string.Format("{0}.lsx", name));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }


    public class DelegateCommand : ICommand  
    {  
        public delegate void SimpleEventHandler();  
        private SimpleEventHandler handler;  
        private bool isEnabled = true;  
 
        public event EventHandler CanExecuteChanged;  
 
        public DelegateCommand(SimpleEventHandler handler)  
        {  
            this.handler = handler;  
        }  
 
        private void OnCanExecuteChanged()  
        {  
            if (this.CanExecuteChanged != null)  
            {  
                this.CanExecuteChanged(this, EventArgs.Empty);  
            }  
        }  
 
        bool ICommand.CanExecute(object arg)  
        {  
            return this.IsEnabled;  
        }  
 
        void ICommand.Execute(object arg)  
        {  
            this.handler();  
        }  
 
        public bool IsEnabled  
        {  
            get { return this.isEnabled; }  
 
            set 
            {  
                this.isEnabled = value;  
                this.OnCanExecuteChanged();  
            }  
        }       
    }  
}
