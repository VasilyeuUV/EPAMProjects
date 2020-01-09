using fwService.Constants;
using System;
using System.IO;
using System.Threading;

namespace fwService
{
    public class FileWatcherModel
    {
        private FileSystemWatcher _watcher;         // Watcher
        public FileSystemWatcher Watcher => _watcher;


        
        private bool enabled = true;                // work will continue as long as the this variable is true

        public event EventHandler<string> NewFileDetectedEvent;


        


        /// <summary>
        /// CTOR
        /// </summary>
        private FileWatcherModel()
        {
            try
            {
                _watcher = new FileSystemWatcher();                

                // these options are set by default
                _watcher.NotifyFilter = NotifyFilters.LastWrite;          // Watch for changes in LastWrite times (Дата последней записи в файл или папку)
                                    //| NotifyFilters.FileName;          // Watch for renaming
                                    //| NotifyFilters.DirectoryName;     // Watch for changes in directories name
                _watcher.IncludeSubdirectories = false;
                _watcher.Filter = "*.csv";           // watch only csv files


                _watcher.Changed += Watcher_OnChanged;
                //watcher.Created += Watcher_OnCreated;
                //watcher.Deleted += Watcher_OnDeleted;
                //watcher.Renamed += Watcher_OnRenamed;
            }
            catch (Exception e)
            {
                FWMessage.RecordEntry("Error created Watcher" + e.Message);
            }
        }

        /// <summary>
        /// Create this instance
        /// </summary>
        /// <param name="watchedFolder"></param>
        /// <returns></returns>
        public static FileWatcherModel CreateInstance(string path)
        {
            try
            {
                FileWatcherModel fwm = new FileWatcherModel();
                fwm._watcher.Path = path;
                FWMessage.RecordEntry($"watching folder {path}", "watcher");
                return fwm;
            }
            catch (Exception)
            {
                FWMessage.RecordEntry($"Error for watching folder {path}.", "watcher");
                return null;
            }
        }

        /// <summary>
        /// We will track changes in the folder through the FileSystemWatcher object.
        /// </summary>
        public void Start()
        {            
            _watcher.EnableRaisingEvents = true;         // Begin watching
            FWMessage.RecordEntry("Begin watching", "watcher");
            while (enabled)
            {
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Stop track changes in folder
        /// </summary>
        public void Stop()
        {
            _watcher.EnableRaisingEvents = false;
            enabled = false;
            FWMessage.RecordEntry("Stop", "watcher");
        }


        /// <summary>
        /// Chechk for file ready to use
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
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
                FWMessage.RecordEntry(fileEvent, filePath);
                NewFileDetectedEvent?.Invoke(this, filePath);

                Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");
            } 
            else
            {
                FWMessage.RecordEntry("0", filePath);
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
            FWMessage.RecordEntry(fileEvent, filePath);
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
            FWMessage.RecordEntry(fileEvent, filePath);
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
            FWMessage.RecordEntry(fileEvent, filePath);
        }

        #endregion //EVENT HANDLERS

    }
}