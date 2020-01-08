using System;
using System.Collections.Generic;
using System.IO;

namespace epam_task4.Models
{
    internal class FileNameSaleModel
    {
        private static object locker = new object();

        internal string FullPath { get; set; }
        internal string FileName { get; set; }        
        internal string Extention { get; set; }

        internal string Manager { get; set; }
        internal DateTime DTG { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        private FileNameSaleModel()
        {
        }

        /// <summary>
        /// Create FileNameSaleModel object
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileNameStruct"></param>
        /// <returns></returns>
        internal static FileNameSaleModel CreateInstance(string path, IDictionary<string, string> fileNameStruct)
        {
            lock (locker)
            {
                if (string.IsNullOrWhiteSpace(path) 
                    || fileNameStruct?.Count < 1)
                { return null; }

                FileInfo fileInf = new FileInfo(path);
                if (!fileInf.Exists) { return null; }

                FileNameSaleModel fnsm = new FileNameSaleModel
                {
                    FullPath = fileInf.FullName,
                    FileName = fileInf.Name.ToLower(),
                    Extention = fileInf.Extension,
                    Manager = fileNameStruct["Manager"],
                    DTG = GetDTG(fileNameStruct["DTG"])
                };
                if (string.IsNullOrWhiteSpace(fnsm.Manager) 
                    || fnsm.DTG == new DateTime())
                { fnsm = null; }
                return fnsm;
            }
        }


        /// <summary>
        /// Convert string to DateTime
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private static DateTime GetDTG(string date)
        {
            DateTime dtg = new DateTime();
            if (!string.IsNullOrWhiteSpace(date))
            {
                try
                {
                    dtg = DateTime.ParseExact(date, "dd.MM.yyyy"
                        , System.Globalization.CultureInfo.CurrentCulture);
                }
                catch (Exception) { }                
            }
            return dtg;
        }

    }
}
