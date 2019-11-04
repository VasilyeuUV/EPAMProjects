using System.Collections.Generic;

namespace mediatekaLib.DataTypes
{
    public static class DictSupportedTypes
    {
        private static Dictionary<string, EnumFileCategory> _values = new Dictionary<string, EnumFileCategory>()
        {
            // AUDIO
            ["mp3"] = EnumFileCategory.Audio,

            // VIDEO
            ["avi"] = EnumFileCategory.Video,

            // GRAPHIC
            ["jpg"] = EnumFileCategory.Graphic
        };


        public static EnumFileCategory GetFileCategory(string ext)
        {
            string _ext = ext.ToLower();
            return _values[_ext];
        }
    }
}
