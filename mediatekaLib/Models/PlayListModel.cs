using mediatekaLib.ClassesToBD;
using mediatekaLib.Interfaces;
using System;
using System.Collections.Generic;

namespace mediatekaLib.Models
{

    /// <summary>
    /// PlayList for Play
    /// </summary>
    public sealed class PlayListModel : IElement
    {

        #region PROPERTIES
        //##########################################################################################################################################


        public string Name { get; set; }

        public Guid Id { get; }



        /// <summary>
        /// Audio files list
        /// </summary>
        public ICollection<MediaFileModel> AudioList { get; private set; } = new List<MediaFileModel>();

        /// <summary>
        /// Video files List
        /// </summary>
        public ICollection<MediaFileModel> VideoList { get; private set; } = new List<MediaFileModel>();

        /// <summary>
        /// Graphic files list
        /// </summary>
        public ICollection<MediaFileModel> GraphicList { get; private set; } = new List<MediaFileModel>();

               
        #endregion // PROPERTIES




        #region CTOR
        //##########################################################################################################################################

        public PlayListModel(PlayList playList)
        {
            this.Id = playList.Id;
            this.Name = playList.Name;
            CreatePlayLists(playList.Items);            
        }

               
        #endregion // CTOR





        #region METHODS
        //##########################################################################################################################################

        private void CreatePlayLists(ICollection<IElement> items)
        {
            foreach (var item in items)
            {
                MediaFileModel media = new MediaFileModel(((MediaFile)item).FullName);
                switch (media.File.FileCategory)
                {
                    case DataTypes.EnumFileCategory.Audio:
                        this.AudioList.Add(media);
                        break;
                    case DataTypes.EnumFileCategory.Video:
                        this.VideoList.Add(media);
                        break;
                    case DataTypes.EnumFileCategory.Graphic:
                        this.GraphicList.Add(media);
                        break;
                    default:
                        break;
                }
            }
        }






        #endregion // METHODS

    }
}
