using System;
using Task2.Menu;

namespace Task2
{
    class Program
    {
        delegate void method();

        [STAThread]
        static void Main(string[] args)
        {
            MenuManager.DisplayMainMenu();
        }
    }
}
