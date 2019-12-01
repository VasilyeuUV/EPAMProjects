using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task2.Tools
{

    /// <summary>
    /// Class for Display string
    /// </summary>
    internal static class ToDisplay
    {

        /// <summary>
        /// View operations Title
        /// </summary>
        /// <param name="title">text to view</param>
        /// <param name="clear">true if Console clear</param>
        internal static void ViewTitle(string title, bool clear = false)
        {
            if (!clear)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(title + ":");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }    
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(title + ":");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }


        /// <summary>
        /// View operation Title
        /// </summary>
        /// <param name="title"></param>
        internal static void ViewBody(string body)
        {
            Console.WriteLine(body);
            Console.WriteLine();
        }

        /// <summary>
        /// Show information if there are many objects
        /// </summary>
        internal static void ViewInfo(string v, int objCount, int viewCount = 0)
        {
            if (viewCount == 0) { viewCount = objCount; } 
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"(Shown {viewCount} from {objCount} {v})");
            Console.ForegroundColor = ConsoleColor.White;
        }


        /// <summary>
        /// Wait push key 
        /// </summary>
        /// <param name="str"></param>
        internal static void WaitForContinue(string str = "")
        {
            if (!String.IsNullOrEmpty(str.Trim()))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(str);
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.WriteLine();
            Console.WriteLine("Press key to continue");
            Console.ReadKey();
        }

        internal static void ViewConcordanceTableHeader()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(string.Format("     {0, -22} {1}           {2}", "WORDS", "N", "STRING NUMBERS"));
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
