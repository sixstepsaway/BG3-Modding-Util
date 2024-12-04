using System.IO;
using System.Text;
using AdonisUI.Controls;

namespace BG3ModdingUtil
{
    public static class ExceptionHelper
    {
        public static void GetException(Exception e, string info, DirectoryInfo location)
        {
            StringBuilder sb = new();
            sb.AppendLine(info);
            sb.AppendLine(string.Format("Exception Message: {0}", e.Message));
            sb.AppendLine(string.Format("Exception Source: {0}", e.Source));
            sb.AppendLine(string.Format("Inner Exception: {0}", e.InnerException));
            sb.AppendLine(string.Format("Base Exception: {0}", e.GetBaseException().ToString()));
            sb.AppendLine(string.Format("Exception Data: {0}", e.Data));
            sb.AppendLine(string.Format("Exception Stack Trace: {0}", e.StackTrace));
            List<string> dirs = Directory.GetDirectories(location.FullName).ToList();
            List<string> files = Directory.GetFiles(location.FullName).ToList();
            StringBuilder filesandfolders = new();

            sb.AppendLine("Items in Location:");
            foreach (string folder in dirs)
            {
                DirectoryInfo dirInfo = new(folder);
                filesandfolders.AppendLine(string.Format("\tFolder: {0}\n\tSymlink: {1}", dirInfo.Name, dirInfo.Attributes.HasFlag(FileAttributes.ReparsePoint).ToString()));
            }
            foreach (string file in files)
            {
                FileInfo fileInfo = new(file);
                filesandfolders.AppendLine(string.Format("\tFile: {0}\n\tSymlink: {1}", fileInfo.Name, fileInfo.Attributes.HasFlag(FileAttributes.ReparsePoint).ToString()));
            }
            sb.AppendLine(filesandfolders.ToString());
            Directory.CreateDirectory(Globals.UtilLogsDirectory);

            string fileName = $"BGMU_Exception_Report_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log";
            string fullPath = Path.Combine(Globals.UtilLogsDirectory, fileName);
            using (FileStream fileStream = new(fullPath, FileMode.CreateNew))
            {
                using (StreamWriter streamWriter = new(fileStream))
                {
                    streamWriter.Write(sb.ToString());
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                fileStream.Close();
            }
            MessageBox.Show(info, buttons:MessageBoxButton.OK, icon:MessageBoxImage.Exclamation);
        }

        
    }
}
