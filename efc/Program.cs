using efc.DataContexts;
using efc.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;

namespace efc
{
    class Program
    {
        static void Main(string[] args)
        {
            Random RND = new Random(DateTime.Now.Millisecond);

            //for (int i = 0; i < 15; i++)
            //{
            //    SaveToDB(RND);
            //}

            DisplayManagers();
            //DisplayProducts();
            //DisplayClients();
            //DisplaySales();


            Console.WriteLine("");
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }


        private static void SaveToDB(Random RND)
        {
            using (SalesContext context = new SalesContext())
            {
                Sale sale = new Sale
                {
                    Client = GetClient(context, "Client" + RND.Next(1, 15).ToString()),
                    FileName = GetFileName(context, "fileNameManager02.csv"),
                    Manager = GetManager(context, "Manager" + RND.Next(1, 15).ToString()),
                    Product = GetProduct(context, "Product" + RND.Next(1, 15).ToString()),
                    DTG = DateTime.Now.AddMinutes(-RND.Next(1, 10))
                };
                sale.Sum = sale.Product.Cost;

                context.Sales.Add(sale);

                try
                {
                    context.SaveChanges();
                }
                //catch (DbEntityValidationException ex)
                //{
                //    foreach (DbEntityValidationResult validationError in ex.EntityValidationErrors)
                //    {
                //        Console.WriteLine("Object: " + validationError.Entry.Entity.ToString());
                //        Console.WriteLine("");
                //        foreach (DbValidationError err in validationError.ValidationErrors)
                //        {
                //            Console.Write(err.ErrorMessage + "");
                //        }
                //    }
                //}
                //catch (SqlException ex)
                //{
                //    DisplaySqlErrors(ex);
                //    //var sqlException = ex.GetBaseException() as SqlException;
                //    //if (sqlException?.Errors.Count > 0)
                //    //{
                //    //    switch (sqlException.Errors[0].Number)
                //    //    {
                //    //        case 547: // Foreign Key violation
                //    //        default:
                //    //            break;
                //    //    }

                //    //}
                //}
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
        }

        private static Product GetProduct(SalesContext context, string name)
        {
            var product = context.Products.FirstOrDefault(x => x.Name.Equals(name));
            return product == null
                ? new Product()
                {
                    Name = name,
                    Cost = 200
                }
                : product;
        }

        private static Manager GetManager(SalesContext context, string name)
        {
            var manager = context.Managers.FirstOrDefault(x => x.Name.Equals(name));

            return manager == null
                ? new Manager() { Name = name }
                : manager;
        }

        private static FileNameData GetFileName(SalesContext context, string name)
        {
            var fileName = context.Files.FirstOrDefault(x => x.Name.Equals(name));
            return fileName == null
                ? new FileNameData()
                {
                    Name = name,
                    DTG = DateTime.Now
                }
                : fileName;
        }

        private static Client GetClient(SalesContext context, string name)
        {
            var client = context.Clients.FirstOrDefault(x => x.Name.Equals(name));

            return client == null
                ? new Client() { Name = name }
                : client;

            //return context.Clients.Find(name) ?? new Client() { Name = name };

        }


        private static void DisplayManagers()
        {
            Console.WriteLine("");
            Console.WriteLine("MANAGERS:");

            var managers = Repository.Select<Manager>()
                .Include(x => x.Sales)
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
            }
        }


    }
}
