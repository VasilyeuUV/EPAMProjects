using efc.DataContexts;
using efc.DataModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace efc
{
    class Program
    {
        static void Main(string[] args)
        {
            Random RND = new Random(DateTime.Now.Millisecond);

            using (var context = new SalesContext()) { context.Dispose(); }     // as install DB


            for (int i = 0; i < 15; i++)
            {
                Repository.SaveToDB(RND);
            }

            DisplayData();


            Console.WriteLine("");
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }







        private static void DisplayData()
        {
            Console.WriteLine("");
            Console.WriteLine("MANAGERS:");

            var managers = Repository.Select<Manager>()
                .Include(m => m.Sales).ThenInclude(mp => mp.Product)
                .Include(m => m.Sales).ThenInclude(mc => mc.Client)
                .Include(m => m.Sales).ThenInclude(mf => mf.FileName)
                .ToList();

            foreach (Manager manager in managers)
            {
                Console.WriteLine("{0}.{1}:", manager.Id, manager.Name);
                foreach (Sale sale in manager.Sales)
                {
                    Console.WriteLine("- sale {0}: {1}; {2}.{3}; {4}.{5}-{6}; {7}.{8}; {9}.{10}"
                        , sale.Id
                        , sale.DTG.ToString("dd.MM.yyyy hh:mm")
                        , sale.Manager?.Id, sale.Manager?.Name
                        , sale.Product?.Id, sale.Product?.Name, sale.Product?.Cost
                        , sale.Client?.Id, sale.Client?.Name
                        , sale.FileName?.Id, sale.FileName?.Name
                        );
                }
                Console.WriteLine();


                //using (var context = new SalesContext())
                //{

                //    var managers = context.Managers
                //                  .Include(m => m.Sales).ThenInclude(mp => mp.Product)
                //                  .Include(m => m.Sales).ThenInclude(mc => mc.Client)
                //                  .Include(m => m.Sales).ThenInclude(mf => mf.FileName)
                //                  .ToList();

                //    foreach (Manager manager in managers)
                //    {
                //        Console.WriteLine("{0}.{1}:", manager.Id, manager.Name);
                //        foreach (Sale sale in manager.Sales)
                //        {
                //            Console.WriteLine("- sale {0}: {1}; {2}.{3}; {4}.{5}-{6}; {7}.{8}; {9}.{10}"
                //                , sale.Id
                //                , sale.DTG.ToString("dd.MM.yyyy hh:mm")
                //                , sale.Manager?.Id, sale.Manager?.Name
                //                , sale.Product?.Id, sale.Product?.Name, sale.Product?.Cost
                //                , sale.Client?.Id, sale.Client?.Name
                //                , sale.FileName?.Id, sale.FileName?.Name
                //                );
                //        }
                //        Console.WriteLine();

                //    }
                //}
            }
        }
    }
}
