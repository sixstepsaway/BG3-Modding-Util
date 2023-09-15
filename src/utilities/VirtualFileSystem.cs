using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SSA.VirtualFileSystem
{
    public class VFileSystem
    {
        //https://stackoverflow.com/questions/11777924/how-to-make-a-read-only-file
        //https://stackoverflow.com/questions/1199571/how-to-hide-file-in-c
        //https://learn.microsoft.com/en-us/dotnet/api/system.io.file.createsymboliclink?view=net-7.0
        //https://stackoverflow.com/questions/3387690/how-to-create-a-hardlink-in-c
        //https://github.com/usdAG/SharpLink   
   
        
        public void MakeSymbolicLink(string Original, string Destination){
            FileInfo fileInfo = new(Original);
            Destination = Path.Combine(Destination, fileInfo.Name);
            if (File.Exists(Destination)){
                try { 
                    File.Move(Destination, string.Format("{0}.disabled", Destination)); 
                } catch (Exception e) {
                    System.Windows.Forms.MessageBox.Show(string.Format("Caught exception disabling duplicate file: {0}", e.Message), fileInfo.Name);
                }
            }
            try {
                File.CreateSymbolicLink(Destination, Original);
            } catch (Exception e) {
                System.Windows.Forms.MessageBox.Show(string.Format("Caught exception making symbolic link: {0}", e.Message), fileInfo.Name);
            }
            
        }

        public void MakeSymbolicLink(string Original, string Destination, string AsName){
            FileInfo fileInfo = new(Original);
            Destination = Path.Combine(Destination, AsName);
            if (File.Exists(Destination)){
                try {
                    File.Move(Destination, string.Format("{0}.disabled", Destination));
                } catch (Exception e) {
                    System.Windows.Forms.MessageBox.Show(string.Format("Caught exception disabling duplicate file: {0}", e.Message), fileInfo.Name);
                }
            }
            try {
                File.CreateSymbolicLink(Destination, Original);
            } catch (Exception e) {
                System.Windows.Forms.MessageBox.Show(string.Format("Caught exception making symbolic link with custom name: {0}", e.Message), fileInfo.Name);
            }
            
        }

        public void RemoveSymbolicLink(string Item){
            if (File.Exists(Item)){
                try { 
                    File.Delete(Item); 
                } catch (Exception e) {
                    System.Windows.Forms.MessageBox.Show(string.Format("Caught exception deleting symbolic link: {0}", e.Message), Item);
                }
            }            
            if (File.Exists(string.Format("{0}.disabled", Item))){
                string og = string.Format("{0}.disabled", Item);
                string ren = og.Replace(".disabled", "");
                try {
                    File.Move(og, ren);
                } catch (Exception e) {
                    System.Windows.Forms.MessageBox.Show(string.Format("Caught exception renaming disabled file: {0}", e.Message), Item);
                }
            }
        }

        public void MakeJunction(string Original, string Destination){
            DirectoryInfo directoryInfo = new(Original);
            Destination = Path.Combine(Destination, directoryInfo.Name);
            if (Directory.Exists(Destination)){
                try { 
                    Directory.Move(Destination, string.Format("{0}--DISABLED", Destination)); 
                } catch (Exception e) {
                    System.Windows.Forms.MessageBox.Show(string.Format("Caught exception disabling duplicate folder: {0}", e.Message), directoryInfo.Name);
                }
            }
            try {
                Directory.CreateSymbolicLink(Destination, Original);
            } catch (Exception e) {
                System.Windows.Forms.MessageBox.Show(string.Format("Caught exception making junction: {0}", e.Message), directoryInfo.Name);
            }
        }

        public void RemoveJunction(string Item){
            if (Directory.Exists(Item)){
                try {
                    Directory.Delete(Item);
                } catch (Exception e) {
                    System.Windows.Forms.MessageBox.Show(string.Format("Caught exception removing junction: {0}", e.Message), Item);
                }
            }            
            if (Directory.Exists(string.Format("{0}--DISABLED", Item))){
                string og = string.Format("{0}--DISABLED", Item);
                string ren = og.Replace("--DISABLED", "");
                try {
                    File.Move(og, ren);
                } catch (Exception e) {
                    System.Windows.Forms.MessageBox.Show(string.Format("Caught exception renaming disabled folder: {0}", e.Message), Item);
                }
            }
        }
    }
}