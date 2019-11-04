using mediatekaLib.Interfaces;
using mediatekaLib.Models;
using System;
using System.IO;

namespace mediatekaLib.Models
{

    /// <summary>
    /// MediaFile
    /// </summary>
    public sealed class MediaFileModel : IElement
    {


        #region PROPERTIES
        //##########################################################################################################################################

        /// <summary>
        /// File metainformation
        /// </summary>
        public FileInfoModel File { get; private set; }


        // NEXT PROPERTIES WILL BE SAVED TO JSON FILE

            
        /// <summary>
        /// File Id
        /// </summary>
        public Guid Id { get; private set; }

        
        /// <summary>
        /// File name
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// File path
        /// </summary>
        public string FullName { get; private set; }


        #endregion // PROPERTIES




        #region CTOR
        //##########################################################################################################################################

        /// <summary>
        /// added new file
        /// </summary>
        /// <param name="fileName"></param>
        public MediaFileModel(string fileName)
        {
            this.Id = Guid.NewGuid();
            this.File = new FileInfoModel(fileName);
            this.Name = this.File.FileInfo.Name;
            this.FullName = this.File.FileInfo.FullName;
        }






        #endregion // CTOR





        #region METHODS
        //##########################################################################################################################################


        #endregion // METHODS



        /* Для списка файлов (вариант)
         var files = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)
            .Select(f => new FileInfo(f)).ToArray();           
         */

    }
}

