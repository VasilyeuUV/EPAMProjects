using System.Collections.Generic;

namespace mediatekaLib.ClassesToBD
{
    public static class MediaPlayLists
    {

        #region PROPERTIES
        //##########################################################################################################################################

        /// <summary>
        /// Registry mediafiles 
        /// </summary>
        public static Dictionary<string, PlayList> Items
        {
            get
            {
                return Items;
            }
            set
            {
                if (Items == null)
                {
                    Items = new Dictionary<string, PlayList>();          // string - id
                }
            }
        }


        #endregion // PROPERTIES     



        #region METHODS
        //##########################################################################################################################################


        // добавление, удаление, ...


        #endregion // METHODS 

    }




}
