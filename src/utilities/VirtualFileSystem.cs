using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Text;
using BG3ModdingUtil;

namespace SSA.VirtualFileSystem
{
    public class VFileSystem
    {
        //https://stackoverflow.com/questions/11777924/how-to-make-a-read-only-file
        //https://stackoverflow.com/questions/1199571/how-to-hide-file-in-c
        //https://learn.microsoft.com/en-us/dotnet/api/system.io.file.createsymboliclink?view=net-7.0
        //https://stackoverflow.com/questions/3387690/how-to-create-a-hardlink-in-c
        //https://github.com/usdAG/SharpLink   

        public void GetException(Exception e, string info, DirectoryInfo location){
            StringBuilder sb = new();
            sb.AppendLine(info);
            string str = string.Format("Exception Message: {0}", e.Message);
            sb.AppendLine(str);
            str = string.Format("Exception Source: {0}", e.Source);
            sb.AppendLine(str);
            str = string.Format("Inner Exception: {0}", e.InnerException);
            sb.AppendLine(str);
            str = string.Format("Base Exception: {0}", e.GetBaseException().ToString());
            sb.AppendLine(str);
            str = string.Format("Exception Data: {0}", e.Data);
            sb.AppendLine(str);
            str = string.Format("Exception Stack Trace: {0}", e.StackTrace);
            sb.AppendLine(str);
            List<string> dirs = Directory.GetDirectories(location.FullName).ToList();
            List<string> files = Directory.GetFiles(location.FullName).ToList();
            StringBuilder filesandfolders = new();

            sb.AppendLine("Items in Location:");            
            foreach (string folder in dirs){
                DirectoryInfo dirInfo = new(folder);
                filesandfolders.AppendLine(string.Format("       Folder: {0}\n       Symlink: {1}", dirInfo.Name, dirInfo.Attributes.HasFlag(FileAttributes.ReparsePoint).ToString()));
            }          
            foreach (string file in files){
                FileInfo fileInfo = new(file);
                filesandfolders.AppendLine(string.Format("       File: {0}\n       Symlink: {1}", fileInfo.Name, fileInfo.Attributes.HasFlag(FileAttributes.ReparsePoint).ToString()));
            }
            sb.AppendLine(filesandfolders.ToString());
            Globals.MakeExceptionReport(sb);
        }
   
        
        public void MakeSymbolicLink(string Original, string Destination){
            FileInfo fileInfo = new(Original);
            FileInfo destinfo = new(Destination);
            Destination = Path.Combine(Destination, fileInfo.Name);
            if (File.Exists(Destination)){
                try { 
                    File.Move(Destination, string.Format("{0}.disabled", Destination)); 
                } catch (Exception e) {
                    string exceptionstring = string.Format("Caught exception disabling duplicate file: {0}\nException: {1}", fileInfo.Name, e.Message);
                    System.Windows.Forms.MessageBox.Show(exceptionstring);
                    GetException(e, exceptionstring, destinfo.Directory);
                }
            }
            try {
                File.CreateSymbolicLink(Destination, Original);
            } catch (Exception e) {
                string exceptionstring = string.Format("Caught exception making symbolic link: {0}\nException: {1}", fileInfo.Name, e.Message);
                System.Windows.Forms.MessageBox.Show(exceptionstring);
                GetException(e, exceptionstring, destinfo.Directory);
            }            
        }

        public void MakeSymbolicLink(string Original, string Destination, string AsName){
            FileInfo destinfo = new(Destination);
            FileInfo fileInfo = new(Original);
            Destination = Path.Combine(Destination, AsName);
            if (File.Exists(Destination)){
                try {
                    File.Move(Destination, string.Format("{0}.disabled", Destination));
                } catch (Exception e) {
                    string exceptionstring = string.Format("Caught exception disabling duplicate file: {0}\nException: {1}", fileInfo.Name, e.Message);
                    System.Windows.Forms.MessageBox.Show(exceptionstring);
                    GetException(e, exceptionstring, destinfo.Directory);
                }
            }
            try {
                File.CreateSymbolicLink(Destination, Original);
            } catch (Exception e) {
                string exceptionstring = string.Format("Caught exception making symbolic link: {0}\nException: {1}", fileInfo.Name, e.Message);
                System.Windows.Forms.MessageBox.Show(exceptionstring);
                GetException(e, exceptionstring, destinfo.Directory);
            }
            
        }

        public void RemoveSymbolicLink(string Item){
            if (File.Exists(Item)){
                try { 
                    File.Delete(Item); 
                } catch (Exception e) {
                    string exceptionstring = string.Format("Caught exception deleting symbolic link: {0}\nException: {1}", Item, e.Message);
                    System.Windows.Forms.MessageBox.Show(exceptionstring);
                    GetException(e, exceptionstring, new FileInfo(Item).Directory);
                }
            }            
            if (File.Exists(string.Format("{0}.disabled", Item))){
                string og = string.Format("{0}.disabled", Item);
                string ren = og.Replace(".disabled", "");
                try {
                    File.Move(og, ren);
                } catch (Exception e) {
                    string exceptionstring = string.Format("Caught exception renaming disabled file: {0}\nException: {1}", Item, e.Message);
                    System.Windows.Forms.MessageBox.Show(exceptionstring);
                    GetException(e, exceptionstring, new FileInfo(Item).Directory);
                }
            }
        }

        public void MakeJunction(string Original, string Destination){            
            DirectoryInfo destinfo = new(Destination);
            DirectoryInfo directoryInfo = new(Original);
            Destination = Path.Combine(Destination, directoryInfo.Name);
            if (Directory.Exists(Destination)){
                try { 
                    Directory.Move(Destination, string.Format("{0}--DISABLED", Destination)); 
                } catch (Exception e) {
                    string exceptionstring = string.Format("Caught exception disabling duplicate folder: {0}\nException: {1}", directoryInfo.Name, e.Message);
                    System.Windows.Forms.MessageBox.Show(exceptionstring);
                    GetException(e, exceptionstring, destinfo);
                }
            }
            try {
                Directory.CreateSymbolicLink(Destination, Original);
            } catch (Exception e) {         
                string exceptionstring = string.Format("Caught exception making junction: {0}\nException: {1}", directoryInfo.Name, e.Message);
                System.Windows.Forms.MessageBox.Show(exceptionstring);
                GetException(e, exceptionstring, destinfo);
            }
        }

        public void RemoveJunction(string Item){
            if (Directory.Exists(Item)){
                try {
                    Directory.Delete(Item);
                } catch (Exception e) {
                    string exceptionstring = string.Format("Caught exception removing junction: {0}\nException: {1}", Item, e.Message);
                    System.Windows.Forms.MessageBox.Show(exceptionstring);
                    GetException(e, exceptionstring, new FileInfo(Item).Directory);
                }
            }            
            if (Directory.Exists(string.Format("{0}--DISABLED", Item))){
                string og = string.Format("{0}--DISABLED", Item);
                string ren = og.Replace("--DISABLED", "");
                try {
                    if(new DirectoryInfo(Item).Attributes.HasFlag(FileAttributes.ReparsePoint)){
                        Directory.Move(og, ren);
                    } else {
                        File.Move(og, ren);
                    }                    
                } catch (Exception e) {
                    string exceptionstring = string.Format("Caught exception renaming disabled folder: {0}\nException: {1}", Item, e.Message);
                    System.Windows.Forms.MessageBox.Show(exceptionstring);
                    GetException(e, exceptionstring, new FileInfo(Item).Directory);
                }
            }
        }
    }
}