using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
