using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace epam_task4.ConsoleMenu
{
    internal static class Display
    {
        private static object locker = new object();

        /// <summary>
        /// Wait push key 
        /// </summary>
        /// <param name="str"></param>
        internal static ConsoleKeyInfo WaitForContinue(string str = "", ConsoleColor color = ConsoleColor.Green)
        {
            lock (locker)
            {
                if (!String.IsNullOrEmpty(str.Trim()))
                {
                    Console.ForegroundColor = color;
                    Console.WriteLine(str);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine();
                Console.WriteLine("Press any key to continue");
                return Console.ReadKey();
            }

        }

        internal static void Message(string str, ConsoleColor color = ConsoleColor.White)
        {
            lock (locker)
            {
                if (!string.IsNullOrWhiteSpace(str))
                {
                    Console.ForegroundColor = color;
                    Console.WriteLine(str);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }
    }
}
