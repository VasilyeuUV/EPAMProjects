using System;

namespace epam_task4.ConsoleMenu
{
    internal class Task4Menu
    {
        string[] menuItems;
        int counter = 0;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="menuItems"></param>
        public Task4Menu(string[] menuItems)
        {
            this.menuItems = menuItems;
        }


        /// <summary>
        /// Temporary Control TaxiStation
        /// </summary>
        /// <param name="taxiStation"></param>
        /// <returns></returns>
        public int PrintMenu()
        {
            ConsoleKeyInfo key;
            do
            {
                Console.Clear();

                Console.WriteLine("SELECT CLIENT");
                Console.WriteLine();

                // MENU
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
                    counter--;
                    if (counter == -1) counter = menuItems.Length - 1;
                }
                if (key.Key == ConsoleKey.DownArrow)
                {
                    counter++;
                    if (counter == menuItems.Length) counter = 0;
                }
                if (key.Key == ConsoleKey.Escape)
                {
                    return menuItems.Length - 1;
                }
            }
            while (key.Key != ConsoleKey.Enter);
            return counter;
        }
    }
}
