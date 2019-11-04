using System;
using mediatekaLib.Interfaces;

namespace mediatekaLib.ClassesToBD
{
    /// <summary>
    /// Get file information from JSON
    /// </summary>
    public sealed class MediaFile : IElement
    {

        #region PROPERTIES
        //##########################################################################################################################################

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

        public MediaFile(Guid id, string name, string fullName)
        {
            this.Id = id;
            this.Name = name;
            this.FullName = fullName;
        }

        #endregion // CTOR





        #region METHODS
        //##########################################################################################################################################



        #endregion // METHODS


    }
}
