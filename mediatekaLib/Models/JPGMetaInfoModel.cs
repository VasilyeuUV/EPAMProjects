using mediatekaLib.DataTypes;
using mediatekaLib.Interfaces;
using System;

namespace mediatekaLib.Models
{
    public class JPGMetaInfoModel : IMetaInfoGraphic
    {
        #region PROPERTIES
        //##########################################################################################################################################

        #endregion // PROPERTIES




        #region CTOR
        //##########################################################################################################################################

        public JPGMetaInfoModel(string fullName)
        {

        }





        #endregion // CTOR



        #region METHODS
        //##########################################################################################################################################

        public EnumFileCategory GetFileExtention()
        {
            throw new NotImplementedException();
        }

        public string GetGenre()
        {
            throw new NotImplementedException();
        }

        public string GetLicense()
        {
            throw new NotImplementedException();
        }

        public string GetOwner()
        {
            throw new NotImplementedException();
        }


        #endregion // METHODS
    }
}
