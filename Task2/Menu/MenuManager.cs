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

        private static void Back()
        {
            Console.WriteLine("Работа завершена.");
        }

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
    }
}
