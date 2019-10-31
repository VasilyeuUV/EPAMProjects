using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mediatekaLib.Interfaces
{
    /// <summary>
    /// способность проигрывать
    /// </summary>
    interface IPlayable
    {
        /// <summary>
        /// поток для Play
        /// </summary>
        /// <param name="stream"></param>
        void Play(System.IO.StreamReader stream);
    }
}
