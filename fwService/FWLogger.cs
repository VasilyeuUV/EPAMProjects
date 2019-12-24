using System;
using System.IO;
using System.Threading;

namespace fwService
{
    internal class FWLogger
    {

        private FileSystemWatcher watcher;
        private object obj = new object();
        private bool enabled = true;                // work will continue as long as the this variable is true
        
        private string _logFile = @"D:\fwLogFile.txt";


        /// <summary>
        /// CTOR
        /// </summary>
        private FWLogger(string path)
        {

            watcher = new FileSystemWatcher(path);
            //watcher.Path = path;

            // these options are set by default
            watcher.NotifyFilter = NotifyFilters.LastWrite;          // Watch for changes in LastWrite times (Дата последней записи в файл или папку)
                                 //| NotifyFilters.FileName;          // Watch for renaming
                                 //| NotifyFilters.DirectoryName;     // Watch for changes in directories name
            watcher.IncludeSubdirectories = true;
            watcher.Filter = "*.csv";           // watch only csv files


            //watcher.Changed += Watcher_OnChanged;
            //watcher.Created += Watcher_OnCreated;
            //watcher.Deleted += Watcher_OnDeleted;
            //watcher.Renamed += Watcher_OnRenamed;
        }

        /// <summary>
        /// Create this instance
        /// </summary>
        /// <param name="watchedFolder"></param>
        /// <returns></returns>
        internal static FWLogger CreateInstance(string path)
        {
            if (Directory.Exists(path))
            {                
                return new FWLogger(path);
            }
            return null;
        }




        /// <summary>
        /// We will track changes in the folder through the FileSystemWatcher object.
        /// </summary>
        internal void Start()
        {
            
            watcher.Changed += Watcher_OnChanged;
            RecordEntry("add Changed event", "watcher");
            watcher.EnableRaisingEvents = true;         // Begin watching
            RecordEntry("Begin watching", "watcher");
            while (enabled)
            {
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Stop track changes in folder
        /// </summary>
        internal void Stop()
        {
            watcher.EnableRaisingEvents = false;
            watcher.Changed -= Watcher_OnChanged;
            RecordEntry("remove Changed event", "watcher");
            enabled = false;
            RecordEntry("Stop", "watcher");
        }

        /// <summary>
        /// Save event result
        /// </summary>
        /// <param name="fileEvent"></param>
        /// <param name="filePath"></param>
        private void RecordEntry(string fileEvent, string filePath)
        {
            lock (obj)
            {
                using (StreamWriter writer = new StreamWriter(_logFile, true))
                {
                    writer.WriteLine(string.Format("{0} file {1} was {2}",
                        DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), filePath, fileEvent));
                    writer.Flush();
                }
            }
        }


        private static bool IsFileReady(string filename)
        {
            // If the file can be opened for exclusive access it means that the file
            // is no longer locked by another process.
            try
            {
                using (FileStream inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return inputStream.Length > 0;
                }
                    
            }
            catch (Exception)
            {
                return false;
            }
        }





        #region EVENT HANDLERS
        //################################################################################################################

        /// <summary>
        /// Event actions if the file has been changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Watcher_OnChanged(object sender, FileSystemEventArgs e)
        {
            string filePath = e.FullPath;
            if (IsFileReady(filePath)) 
            {
                string fileEvent = "changed";                
                RecordEntry(fileEvent, filePath); 
            } 
            else
            {
                RecordEntry("0", filePath);
            }
        }


        /// <summary>
        /// Event actions if the file has been created (added)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Watcher_OnCreated(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "added";
            string filePath = e.FullPath;
            RecordEntry(fileEvent, filePath);
        }


        /// <summary>
        /// Event actions if the file has been removed (deleted)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Watcher_OnDeleted(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "deleted";
            string filePath = e.FullPath;
            RecordEntry(fileEvent, filePath);
        }

        /// <summary>
        /// Event actions if the file has been renamed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Watcher_OnRenamed(object sender, RenamedEventArgs e)
        {
            string fileEvent = "renamed to " + e.FullPath;
            string filePath = e.OldFullPath;
            RecordEntry(fileEvent, filePath);
        }

        #endregion //EVENT HANDLERS

    }
}