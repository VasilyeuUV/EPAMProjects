using System;
using System.Collections.Generic;
using mediatekaLib.Interfaces;

namespace mediatekaLib.ClassesToBD
{
    public sealed class PlayList : IMediaList
    {
        #region PROPERTIES
        //##########################################################################################################################################

        /// <summary>
        /// PlayList Id
        /// </summary>
        public Guid Id { get; private set; }


        /// <summary>
        /// PlayList Name
        /// </summary>
        public string Name { get; set; } = "Default PlayList";


        /// <summary>
        /// Collection MediaFileModel Id
        /// </summary>
        public ICollection<IElement> Items { get; private set; }


        #endregion // PROPERTIES


        #region CTOR
        //##########################################################################################################################################

        public PlayList() : this("Default PlayList") { }
        public PlayList(string name)
        {
            this.Name = name;
            this.Id = Guid.NewGuid();
            this.Items = new List<IElement>();
        }

        #endregion // CTOR


        #region METHODS
        //##########################################################################################################################################


        #endregion // METHODS













    }
}
