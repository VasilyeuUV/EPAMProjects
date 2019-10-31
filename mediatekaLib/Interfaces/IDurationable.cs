using System;

namespace mediatekaLib.Interfaces
{
    /// <summary>
    /// Продолжительность медиафайлов
    /// </summary>
    interface IDurationable
    {
        TimeSpan Duration { get; set; }
    }
}
