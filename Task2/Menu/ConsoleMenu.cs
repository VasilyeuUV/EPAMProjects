using System;

namespace Task2.Menu
{
    /// <summary>
    /// Console menu class
    /// </summary>
    internal class ConsoleMenu
    {
        string[] menuItems;
        static int counter = 0;

        public ConsoleMenu(string[] menuItems)
        {
            this.menuItems = menuItems;
        }


        /// <summary>
        /// Display Menu
        /// </summary>
        /// <returns>selected Menu item</returns>
        public int PrintMenu(string op)
        {
            ConsoleKeyInfo key;
            do
            {
                Console.Clear();

                Console.WriteLine(op);
                Console.WriteLine();

                // MENU
                if (counter >= menuItems.Length) { counter = menuItems.Length - 1; }

                for (int i = 0; i < menuItems.Length; i++)
                {
                    if (counter == i)
                    {
                        Console.BackgroundColor = ConsoleColor.Cyan;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine(menuItems[i]);
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                        Console.WriteLine(menuItems[i]);
                }
                key = Console.ReadKey();
                if (key.Key == ConsoleKey.UpArrow)
                {
                    if (--counter == -1) counter = menuItems.Length - 1;
                }
                if (key.Key == ConsoleKey.DownArrow)
                {
                    if (++counter == menuItems.Length) counter = 0;
                }
                if (key.Key == ConsoleKey.Escape)
                {
                    return menuItems.Length - 1;                // last menu must be Exit
                }
            }
            while (key.Key != ConsoleKey.Enter);
            return counter;
        }
    }
}
