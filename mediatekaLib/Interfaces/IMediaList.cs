using System.Collections.Generic;

namespace mediatekaLib.Interfaces
{
    /// <summary>
    /// Интрефейс подборки
    /// </summary>
    public interface IMediaList :IElement
    {
        ICollection<IElement> Items { get; }
    }
}
