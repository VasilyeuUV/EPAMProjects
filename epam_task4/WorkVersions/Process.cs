using epam_task4.ConsoleMenu;
using epam_task4.Threads;
using System;
using System.Collections.Generic;

namespace epam_task4.WorkVersions
{
    public static class Process
    {
        private static object locker = new object();
        internal static List<FileProcessingThread> lstThread = new List<FileProcessingThread>();

        private static readonly string[] FILE_NAME_STRUCT = { "Manager", "DTG" };
        private static readonly string[] FILE_DATA_STRUCT = { "DTG", "Client", "Product", "Sum" };

        /// <summary>
        /// Run file handler
        /// </summary>
        /// <param name="file"></param>
        public static void StartProcessing(string file)
        {
            lock (locker)
            {
                FileProcessingThread fileHandler = CreateFileHandlerThread();
                try
                {
                    if (fileHandler.Start(file))
                    {
                        Display.Message($"{file}: Processing of file starting");
                        lstThread.Add(fileHandler);
                        Display.Message($"Number of file handler threads - {lstThread.Count}", ConsoleColor.Blue);
                    }
                    else
                    {
                        Display.Message($"{file}: can't starting");
                    }
                }
                catch (Exception)
                {
                    Display.Message($"{file}: Error starting");
                }
            }
        }

        private static FileProcessingThread CreateFileHandlerThread()
        {
            lock (locker)
            {
                FileProcessingThread fileHandler = new FileProcessingThread(fns: FILE_NAME_STRUCT, fds: FILE_DATA_STRUCT);
                fileHandler.WorkCompleted += FileHandler_WorkCompleted;
                fileHandler.ErrorEvent += FileHandler_ErrorEvent;
                return fileHandler;
            }
        }




        #region FILE_PROCESSING_THREAD_EVENTS
        //##################################################################################################

        private static void FileHandler_WorkCompleted(object sender, bool aborted)
        {
            lock (locker)
            {
                if (aborted) { Display.Message($"{(sender as FileProcessingThread)?.Name}: Processing aborted", ConsoleColor.Red); }
                else { Display.Message($"{(sender as FileProcessingThread)?.Name}: Processing completed", ConsoleColor.Green); }

                var fileHandler = sender as FileProcessingThread;
                if (fileHandler != null)
                {
                    fileHandler.WorkCompleted -= FileHandler_WorkCompleted;
                    fileHandler.ErrorEvent -= FileHandler_ErrorEvent;

                    lstThread.Remove(fileHandler);
                    Display.Message($"Number of file handler threads - {lstThread.Count}", ConsoleColor.Blue);
                }
            }
        }


        private static void FileHandler_ErrorEvent(object sender, string msg)
        {
            lock (locker)
            {
                Display.Message($"{(sender as FileProcessingThread)?.Name}: " + msg, ConsoleColor.Yellow);
            }
        }


        #endregion // FILE_PROCESSING_THREAD_EVENTS


    }
}
