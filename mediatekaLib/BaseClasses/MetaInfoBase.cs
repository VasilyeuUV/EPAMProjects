using mediatekaLib.DataTypes;

namespace mediatekaLib.BaseClasses
{
    public abstract class MetaInfoBase
    {

        #region PROPERTIES
        //##########################################################################################################################################

        /// <summary>
        /// File owner
        /// </summary>
        public string Owner { get; protected set; }

        /// <summary>
        /// File License
        /// </summary>
        public string License { get; protected set; }

        /// <summary>
        /// The mediafile Genre (жанр)
        /// </summary>
        public string Genre { get; protected set; }


        public EnumFileCategory FileExtention { get; protected set; }

        #endregion // PROPERTIES




        #region CTOR
        //##########################################################################################################################################


        #endregion // CTOR






        #region METHODS
        //##########################################################################################################################################




        #endregion // METHODS


    }
}
