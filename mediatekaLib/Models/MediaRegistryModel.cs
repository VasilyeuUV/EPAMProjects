using System.Collections.Generic;

namespace mediatekaLib.Models
{

    /// <summary>
    /// All media files (1)
    /// </summary>
    internal class MediaRegistryModel
    {


        #region PROPERTIES
        //##########################################################################################################################################

        /// <summary>
        /// Registry mediafiles 
        /// </summary>
        public SortedSet<MediaFileModel> Items
        {
            get
            {
                return Items;
            }
            set
            {
                if (Items == null)
                {
                    Items = new SortedSet<MediaFileModel>(/*new ByFileId()*/);          // создаем с сортировкой по Id медиафайла
                }
            }
        }


        #endregion // PROPERTIES      

    }
}









/*
// Get a list of the files to use for the sorted set.
            IEnumerable<string> files1 =
                Directory.EnumerateFiles(@"\\archives\2007\media",
                "*", SearchOption.AllDirectories);

// Create a sorted set using the ByFileExtension comparer.
    var mediaFiles1 = new SortedSet<string>(new ByFileExtension());

    // Note that there is a SortedSet constructor that takes an IEnumerable,
    // but to remove the path information they must be added individually.
    foreach (string f in files1)
    {
        mediaFiles1.Add(f.Substring(f.LastIndexOf(@"\") + 1));
    }




// Defines a comparer to create a sorted set
// that is sorted by the file extensions.
public class ByFileExtension : IComparer<string>
{
string xExt, yExt;

CaseInsensitiveComparer caseiComp = new CaseInsensitiveComparer();

public int Compare(string x, string y)
{
// Parse the extension from the file name. 
xExt = x.Substring(x.LastIndexOf(".") + 1);
yExt = y.Substring(y.LastIndexOf(".") + 1);

// Compare the file extensions. 
int vExt = caseiComp.Compare(xExt, yExt);
if (vExt != 0)
{
    return vExt;
}
else
{
    // The extension is the same, 
    // so compare the filenames. 
    return caseiComp.Compare(x, y);
}
}
}



 */
