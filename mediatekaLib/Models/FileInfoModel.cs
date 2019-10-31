using mediatekaLib.DataTypes;
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
        public MetaInfoModel MetaInfo { get; private set; } 



        #endregion // PROPERTIES




        #region CTOR
        //##########################################################################################################################################
        public FileInfoModel(string fileName)
        {
            this.FileInfo = new FileInfo(fileName);            
            this.MetaInfo = new MetaInfoModel(fileName);
            this.FileCategory = this.MetaInfo.FileCategory;
        }



        #endregion // CTOR





        #region METHODS
        //##########################################################################################################################################



        #endregion // METHODS

    }
}