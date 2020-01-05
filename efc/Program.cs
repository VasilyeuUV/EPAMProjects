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
            var repo = new Repository();
            Random RND = new Random(DateTime.Now.Millisecond);

            using (var context = new SalesContext()) { context.Dispose(); }     // as install DB


            for (int i = 0; i < 15; i++)
            {
                SaveToDB(repo, RND);
            }
            DisplayData(repo);


            Console.WriteLine("");
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }



        internal static void SaveToDB(Repository repo, Random rnd)
        {

            string client = "Client" + rnd.Next(1, 15).ToString();
            string fileName = "fileNameDefault.csv";
            string manager = "Manager" + rnd.Next(1, 15).ToString();
            string product = "Product" + rnd.Next(1, 15).ToString();


            Sale sale = new Sale
            {
                Client = repo.Select<Client>().FirstOrDefault(x => x.Name.Equals(client)) ?? new Client() { Name = client },
                FileName = repo.Select<FileName>().FirstOrDefault(x => x.Name.Equals(fileName)) ?? new FileName() { Name = fileName, DTG = DateTime.Now },
                Manager = repo.Select<Manager>().FirstOrDefault(x => x.Name.Equals(manager)) ?? new Manager() { Name = manager },
                Product = repo.Select<Product>().FirstOrDefault(x => x.Name.Equals(product)) ?? new Product() { Name = product, Cost = rnd.Next(100, 300) },
                DTG = DateTime.Now.AddMinutes(-rnd.Next(1, 50))
            };
            sale.Sum = sale.Product.Cost;
            repo.Insert(sale);


            //using (SalesContext context = new SalesContext())
            //{
            //    Sale sale = new Sale
            //    {
            //        Client = GetClient(context, "Client" + rnd.Next(1, 15).ToString()),
            //        FileName = GetFileName(context, "fileNameManager02.csv"),
            //        Manager = GetManager(context, "Manager" + rnd.Next(1, 15).ToString()),
            //        Product = GetProduct(context, "Product" + rnd.Next(1, 15).ToString()),
            //        DTG = DateTime.Now.AddMinutes(-rnd.Next(1, 10))
            //    };
            //    sale.Sum = sale.Product.Cost;

            //    context.TmpSales.Add(sale);
            //    bool result = SaveToDB(context);
            //    context.Dispose();
            //    //    try
            //    //    {
            //    //        context.SaveChanges();
            //    //    }
            //    //    //catch (DbEntityValidationException ex)
            //    //    //{
            //    //    //    foreach (DbEntityValidationResult validationError in ex.EntityValidationErrors)
            //    //    //    {
            //    //    //        Console.WriteLine("Object: " + validationError.Entry.Entity.ToString());
            //    //    //        Console.WriteLine("");
            //    //    //        foreach (DbValidationError err in validationError.ValidationErrors)
            //    //    //        {
            //    //    //            Console.Write(err.ErrorMessage + "");
            //    //    //        }
            //    //    //    }
            //    //    //}
            //    //    //catch (SqlException ex)
            //    //    //{
            //    //    //    DisplaySqlErrors(ex);
            //    //    //    //var sqlException = ex.GetBaseException() as SqlException;
            //    //    //    //if (sqlException?.Errors.Count > 0)
            //    //    //    //{
            //    //    //    //    switch (sqlException.Errors[0].Number)
            //    //    //    //    {
            //    //    //    //        case 547: // Foreign Key violation
            //    //    //    //        default:
            //    //    //    //            break;
            //    //    //    //    }

            //    //    //    //}
            //    //    //}
            //    //    catch (Exception e)
            //    //    {
            //    //        Console.WriteLine(e.Message);
            //    //    }

            //}
        }




        private static void DisplayData(Repository repo)
        {            

            Console.WriteLine("");
            Console.WriteLine("MANAGERS:");

            var managers = repo.Select<Manager>()
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
