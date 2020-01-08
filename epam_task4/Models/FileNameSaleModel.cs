using System;
using System.Collections.Generic;
using System.IO;

namespace epam_task4.Models
{
    internal class FileNameSaleModel
    {
        private static object locker = new object();

        internal string FullName { get; set; }
        internal string Name { get; set; }        
        internal string Extention { get; set; }

        internal string Manager { get; set; }
        internal DateTime DTG { get; set; }

        private FileNameSaleModel()
        {
        }


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
                    FullName = fileInf.FullName,
                    Name = fileInf.Name.ToLower(),
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



        private static DateTime GetDTG(string date)
        {
            
        }

    }
}
