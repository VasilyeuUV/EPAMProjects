using System.IO;

namespace mediatekaLib.Interfaces
{
    /// <summary>
    /// поток
    /// </summary>
    public interface IMediaStreamable
    {
        StreamReader Stream { get; }
    }
}
