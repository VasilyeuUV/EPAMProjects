using mediatekaLib.DataTypes;
using mediatekaLib.Interfaces;

namespace mediatekaLib.Models
{
    internal class MP3MetaInfoModel : IMetaInfoAudio
    {
        #region PROPERTIES
        //##########################################################################################################################################

        #endregion // PROPERTIES




        #region CTOR
        //##########################################################################################################################################

        public MP3MetaInfoModel(string fullName)
        {

        }



        #endregion // CTOR



        #region METHODS
        //##########################################################################################################################################


        public EnumFileCategory GetFileExtention()
        {
            throw new System.NotImplementedException();
        }

        public string GetGenre()
        {
            throw new System.NotImplementedException();
        }

        public string GetLicense()
        {
            throw new System.NotImplementedException();
        }

        public string GetOwner()
        {
            throw new System.NotImplementedException();
        }


        #endregion // METHODS
    }
}
