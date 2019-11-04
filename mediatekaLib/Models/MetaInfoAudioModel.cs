using mediatekaLib.BaseClasses;
using mediatekaLib.Interfaces;
using System.IO;

namespace mediatekaLib.Models
{
    public class MetaInfoAudioModel : MetaInfoBase, IPlayable
    {

        #region PROPERTIES
        //##########################################################################################################################################

        // метасвойства аудио файлов

        #endregion // PROPERTIES




        #region CTOR
        //##########################################################################################################################################

        public MetaInfoAudioModel(FileInfo fileInfo)
        {
            dynamic metaClassInfo;

            switch (fileInfo.Extension.ToLower())
            {
                case "mp3":
                    metaClassInfo = new AviMetaInfoModel(fileInfo.FullName);
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

                // ... другая метаинформации для аудио
            }

        }



        #endregion // CTOR



        #region METHODS
        //##########################################################################################################################################


        public void Play(StreamReader stream)
        {
            throw new System.NotImplementedException();
        }


        #endregion // METHODS


    }
}
