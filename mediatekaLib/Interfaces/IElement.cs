using mediatekaLib.DataTypes;
using System;

namespace mediatekaLib.Interfaces
{
    public interface IElement
    {

        /// <summary>
        /// File name
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// File Id
        /// </summary>
        Guid Id { get; }
    }
}
