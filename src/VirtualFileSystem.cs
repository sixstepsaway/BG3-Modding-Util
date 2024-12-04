using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG3ModdingUtil
{
    public static class VirtualFileSystem
    {                
        public static bool MakeSymbolicLinkAuto(string original, string destination)
        {
            FileInfo fileInfo = new(original);
            FileInfo destinfo = new(destination);
            destination = Path.Combine(destination, fileInfo.Name);
            if (File.Exists(destination))
            {
                try
                {
                    File.Move(destination, string.Format("{0}.disabled", destination));
                }
                catch (Exception e)
                {
                    string exceptionstring = string.Format("Caught exception disabling duplicate file: {0}\nException: {1}", fileInfo.Name, e.Message);
                    ExceptionHelper.GetException(e, exceptionstring, destinfo.Directory);
                    return false;
                }
            }
            try
            {
                File.CreateSymbolicLink(destination, original);
            }
            catch (Exception e)
            {
                string exceptionstring = string.Format("Caught exception making symbolic link: {0}\nException: {1}", fileInfo.Name, e.Message);
                ExceptionHelper.GetException(e, exceptionstring, destinfo.Directory);
                return false;
            }
            return true;
        }
        public static bool MakeSymbolicLinkDirect(string original, string destination)
        {
            FileInfo fileInfo = new(original);
            FileInfo destinfo = new(destination);            
            if (File.Exists(destination))
            {
                try
                {
                    File.Move(destination, string.Format("{0}.disabled", destination));
                }
                catch (Exception e)
                {
                    string exceptionstring = string.Format("Caught exception disabling duplicate file: {0}\nException: {1}", fileInfo.Name, e.Message);
                    ExceptionHelper.GetException(e, exceptionstring, destinfo.Directory);
                    return false;
                }
            }
            try
            {
                File.CreateSymbolicLink(destination, original);
            }
            catch (Exception e)
            {
                string exceptionstring = string.Format("Caught exception making symbolic link: {0}\nException: {1}", fileInfo.Name, e.Message);
                ExceptionHelper.GetException(e, exceptionstring, destinfo.Directory);
                return false;
            }
            return true;
        }

        public static bool RemoveSymbolicLink(string Item)
        {
            if (File.Exists(Item))
            {
                try
                {
                    File.Delete(Item);
                }
                catch (Exception e)
                {
                    string exceptionstring = string.Format("Caught exception deleting symbolic link: {0}\nException: {1}", Item, e.Message);
                    ExceptionHelper.GetException(e, exceptionstring, new FileInfo(Item).Directory);
                    return false;
                }
            }
            if (File.Exists(string.Format("{0}.disabled", Item)))
            {
                string og = string.Format("{0}.disabled", Item);
                string ren = og.Replace(".disabled", "");
                try
                {
                    File.Move(og, ren);
                }
                catch (Exception e)
                {
                    string exceptionstring = string.Format("Caught exception renaming disabled file: {0}\nException: {1}", Item, e.Message);
                    ExceptionHelper.GetException(e, exceptionstring, new FileInfo(Item).Directory);
                    return false;
                }
            }
            return true;
        }

        public static bool MakeJunction(string Original, string Destination)
        {
            DirectoryInfo destinfo = new(Destination);
            DirectoryInfo directoryInfo = new(Original);
            Destination = Path.Combine(Destination, directoryInfo.Name);
            if (Directory.Exists(Destination))
            {
                try
                {
                    Directory.Move(Destination, string.Format("{0}--DISABLED", Destination));
                }
                catch (Exception e)
                {
                    string exceptionstring = string.Format("Caught exception disabling duplicate folder: {0}\nException: {1}", directoryInfo.Name, e.Message);
                    ExceptionHelper.GetException(e, exceptionstring, destinfo);
                    return false;
                }
            }
            try
            {
                Directory.CreateSymbolicLink(Destination, Original);
            }
            catch (Exception e)
            {
                string exceptionstring = string.Format("Caught exception making junction: {0}\nException: {1}", directoryInfo.Name, e.Message);
                ExceptionHelper.GetException(e, exceptionstring, destinfo);
                return false;
            }
            return true;
        }

        public static bool RemoveJunction(string Item)
        {
            if (Directory.Exists(Item))
            {
                try
                {
                    Directory.Delete(Item);
                }
                catch (Exception e)
                {
                    string exceptionstring = string.Format("Caught exception removing junction: {0}\nException: {1}", Item, e.Message);
                    ExceptionHelper.GetException(e, exceptionstring, new FileInfo(Item).Directory);
                    return false;
                }
            }
            if (Directory.Exists(string.Format("{0}--DISABLED", Item)))
            {
                string og = string.Format("{0}--DISABLED", Item);
                string ren = og.Replace("--DISABLED", "");
                try
                {
                    if (new DirectoryInfo(Item).Attributes.HasFlag(FileAttributes.ReparsePoint))
                    {
                        Directory.Move(og, ren);
                    }
                    else
                    {
                        File.Move(og, ren);
                    }
                }
                catch (Exception e)
                {
                    string exceptionstring = string.Format("Caught exception renaming disabled folder: {0}\nException: {1}", Item, e.Message);
                    ExceptionHelper.GetException(e, exceptionstring, new FileInfo(Item).Directory);
                    return false;
                }
            }
            return true;
        }
    }
}
