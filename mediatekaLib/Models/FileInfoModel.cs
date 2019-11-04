using mediatekaLib.DataTypes;
using System;
using System.IO;

namespace mediatekaLib.Models
{
    public class FileInfoModel
    {


        #region PROPERTIES
        //##########################################################################################################################################


        public EnumFileCategory FileCategory { get; private set; }


        /// <summary>
        /// File information
        /// </summary>
        public FileInfo FileInfo { get; private set; }


        /// <summary>
        /// Meta information
        /// </summary>
        public dynamic MetaInfo { get; private set; } 



        #endregion // PROPERTIES




        #region CTOR
        //##########################################################################################################################################
        public FileInfoModel(string fileName)
        {
            this.FileInfo = new FileInfo(fileName);            
            this.MetaInfo = GetMetaInfo(this.FileInfo);
            this.FileCategory = this.MetaInfo.FileExtention == null
                                ? DictSupportedTypes.GetFileCategory(this.FileInfo.Extension)
                                : this.MetaInfo.FileExtention;

        }




        #endregion // CTOR





        #region METHODS
        //##########################################################################################################################################


        /// <summary>
        /// Get MetaInfo from File
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        private dynamic GetMetaInfo(FileInfo fileInfo)
        {
            switch (DictSupportedTypes.GetFileCategory(fileInfo.Extension.ToLower()))
            {
                case EnumFileCategory.Audio:
                    return new MetaInfoAudioModel(fileInfo);
                case EnumFileCategory.Video:
                    return new MetaInfoVideoModel(fileInfo);
                case EnumFileCategory.Graphic:
                    return new MetaInfoGraphicModel(fileInfo);
                default:
                    return null;
            }
        }



        #endregion // METHODS

    }
}