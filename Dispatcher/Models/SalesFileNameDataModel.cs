using FileParcer.Interfaces;
using System;
using System.IO;
using System.Text;

namespace FileParser.Models
{
    public class SalesFileNameDataModel : IFileNameData
    {
        public string FileName { get; private set; } = "";
        public string FileExtention { get; private set; } = "";
        public string Path { get; private set; } = "";


        public string Manager { get; private set; } = "";
        public DateTime DTG { get; private set; }

        


        /// <summary>
        /// CTOR
        /// </summary>
        private SalesFileNameDataModel(System.IO.FileInfo fileInf)
        {
            this.FileName = fileInf.Name.ToLower();
            this.FileExtention = fileInf.Extension.ToLower();
            this.Manager = GetManager(this.FileName);
            this.DTG = GetDate(this.FileName);
        }


        public static SalesFileNameDataModel CreateInstance(FileInfo fileInf)
        {
            if (fileInf == null) { return null; }
            return new SalesFileNameDataModel(fileInf);
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
                var name = new StringBuilder(data[0]);
                name[0] = char.ToUpper(name[0]);
                return name.ToString();
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
