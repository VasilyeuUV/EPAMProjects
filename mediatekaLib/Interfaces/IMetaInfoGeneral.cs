using mediatekaLib.DataTypes;

namespace mediatekaLib.Interfaces
{
    public interface IMetaInfoGeneral
    {

        /// <summary>
        /// Get file owner information
        /// </summary>
        /// <returns></returns>
        string GetOwner();

        /// <summary>
        /// Get file license information
        /// </summary>
        /// <returns></returns>
        string GetLicense();

        /// <summary>
        /// Get file genre information
        /// </summary>
        /// <returns></returns>
        string GetGenre();

        /// <summary>
        /// Get file extention from metainformation
        /// </summary>
        /// <returns></returns>
        EnumFileCategory GetFileExtention();




        // ... Методы для работы с мета информацией


    }
}
