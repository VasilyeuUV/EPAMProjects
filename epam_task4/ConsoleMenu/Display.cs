using efdb.DataContexts;
using efdb.DataModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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


        /// <summary>
        /// Display information from DB
        /// </summary>
        /// <param name="repo"></param>
        internal static void DisplayData<T>( int count = 20) where T: EntityBase
        {
            Message(typeof(T).ToString().ToUpper() + ":", ConsoleColor.Green);
            Console.WriteLine();

            Repository repo = new Repository();
            var results = repo.Select<T>()
                .Include(m => m.Sales.Select(mm => mm.Manager))
                .Include(m => m.Sales.Select(mc => mc.Client))
                .Include(m => m.Sales.Select(mp => mp.Product))
                .Include(m => m.Sales.Select(mf => mf.FileName))
                .ToList();

            int n = 0;
            foreach (var result in results)
            {
                Message($"{result.Id}.{result.Name}:", ConsoleColor.Yellow);
                foreach (Sale sale in result.Sales)
                {
                    Console.WriteLine("- sale {0, 2}: {1, 2}  | {2, 3}. {3, -10} | {4, 3}. {5, -10} - {6, 3} | {7, 3}. {8, -10} | {9, 3}. {10, 3}"
                        , sale.Id
                        , sale.DTG.ToString("dd.MM.yyyy HH:mm")
                        , sale.Manager?.Id, sale.Manager?.Name
                        , sale.Product?.Id, sale.Product?.Name, sale.Product?.Cost
                        , sale.Client?.Id, sale.Client?.Name
                        , sale.FileName?.Id, sale.FileName?.Name
                        );
                    if (++n > count) { break; }
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }



    }
}
