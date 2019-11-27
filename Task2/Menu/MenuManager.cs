using System;

namespace Task2.Menu
{
    internal static class MenuManager
    {
        internal delegate void method();

        internal static void DisplayMainMenu()
        {
            string operation = "ВЫБОР ОПЕРАЦИИ";
            string[] items = { "Создать предметный указатель текста", "Выход" };
            method[] methods = new method[] { ConcordanceManager.ConcordanceDoWork, Back };
            SelectMenuItem(operation, items, methods);
        }


        /// <summary>
        /// Back to the up menu
        /// </summary>
        private static void Back()
        {
            Console.WriteLine("Работа завершена.");
        }


        /// <summary>
        /// Select menu item
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="items"></param>
        /// <param name="methods"></param>
        internal static void SelectMenuItem(string operation, string[] items, method[] methods)
        {
            ConsoleMenu menu = new ConsoleMenu(items);
            int menuResult;
            do
            {
                menuResult = menu.PrintMenu(operation);
                Console.WriteLine();
                methods[menuResult]();
            } while (menuResult != items.Length - 1);
        }


        /// <summary>
        /// Display 
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
            Console.WriteLine("Для продолжения нажмите любую клавишу");
            Console.ReadKey();
        }


    }
}
