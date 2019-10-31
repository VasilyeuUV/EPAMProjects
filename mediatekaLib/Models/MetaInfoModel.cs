using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mediatekaLib.DataTypes;

namespace mediatekaLib.Models
{
    /// <summary>
    /// File meta information
    /// </summary>
    public class MetaInfoModel
    {

        #region FIELDS
        //##########################################################################################################################################

        #endregion // FIELDS




        #region PROPERTIES
        //##########################################################################################################################################

        /// <summary>
        /// File category: audio, video or graphic ...
        /// </summary>
        public EnumFileCategory FileCategory { get; private set; }


        #endregion // PROPERTIES




        #region CTOR
        //##########################################################################################################################################

        public MetaInfoModel(string fileName)
        {


            



        }



        #endregion // CTOR





        #region METHODS
        //##########################################################################################################################################



        #endregion // METHODS



    }
}
