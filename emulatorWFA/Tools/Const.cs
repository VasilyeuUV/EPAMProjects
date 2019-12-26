using System;

namespace emulatorWFA.Tools
{
    internal static class Const
    {
        internal static Random RND = new Random(Convert.ToInt32(DateTime.Now.Ticks % int.MaxValue));
        internal static Random RND2 = new Random(DateTime.Now.Millisecond);
    }
}
