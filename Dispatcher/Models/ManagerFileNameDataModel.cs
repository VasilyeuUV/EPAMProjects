using Dispatcher.Interfaces;
using System;
using System.IO;

namespace Dispatcher.Models
{
    internal class ManagerFileNameDataModel : IFileNameData
    {
        public string FileName { get; private set; } = "";
        public string FileExtention { get; private set; } = "";

        internal string Manager { get; private set; } = "";
        internal DateTime DTG { get; private set; }


        /// <summary>
        /// CTOR
        /// </summary>
        private ManagerFileNameDataModel(System.IO.FileInfo fileInf)
        {
            this.FileName = fileInf.Name;
            this.FileExtention = fileInf.Extension;
            this.Manager = GetManager(this.FileName);
            this.DTG = GetDate(this.FileName);
        }


        internal static IFileNameData CreateInstance(FileInfo fileInf)
        {
            if (fileInf == null) { return null; }
            return new ManagerFileNameDataModel(fileInf);
        }


        /// <summary>
        /// Get Manager name from file name
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <returns>Manager name</returns>
        private string GetManager(string fileName)
        {
            var data = fileName.Split('_');
            if (data.Length > 0)
            {
                return data[0];
            }
            return string.Empty;     
        }

        /// <summary>
        /// Get DateTime from file name
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <returns>DateTime</returns>
        private DateTime GetDate(string fileName)
        {
            //Antonov_01.12.2019_1.csv
            var data = fileName.Split('_');
            if (data.Length > 1)
            {
                DateTime dtg;
                try
                {
                    dtg = DateTime.ParseExact(data[1], "dd.MM.yyyy", System.Globalization.CultureInfo.CurrentCulture);
                }
                catch (Exception)
                {
                    dtg = new DateTime();
                }                
                return dtg;
            }
            return new DateTime();            
        }

    }
}
