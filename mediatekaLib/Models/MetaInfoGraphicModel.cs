using mediatekaLib.BaseClasses;
using mediatekaLib.Interfaces;
using System.IO;

namespace mediatekaLib.Models
{
    public class MetaInfoGraphicModel : MetaInfoBase, IViewable
    {
        #region PROPERTIES
        //##########################################################################################################################################

        // метасвойства графических файлов


        #endregion // PROPERTIES




        #region CTOR
        //##########################################################################################################################################


        public MetaInfoGraphicModel(FileInfo fileInfo)
        {
            dynamic metaClassInfo;

            switch (fileInfo.Extension.ToLower())
            {
                case "jpg":
                    metaClassInfo = new JPGMetaInfoModel(fileInfo.FullName);
                    break;
                default:
                    metaClassInfo = null;
                    break;
            }


            if (metaClassInfo != null)
            {
                this.Owner = metaClassInfo.GetOwner();
                this.License = metaClassInfo.GetLicense();
                this.Genre = metaClassInfo.GetGenre();
                this.FileExtention = metaClassInfo.GetFileExtention();

                // ... другая метаинформации для графики
            }



        }



        #endregion // CTOR



        #region METHODS
        //##########################################################################################################################################


        public void Display(string file)
        {
            throw new System.NotImplementedException();
        }


        #endregion // METHODS

    }
}
