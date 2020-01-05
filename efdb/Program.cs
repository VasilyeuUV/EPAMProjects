using efdb.DataContexts;
using efdb.DataModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;


namespace efdb
{
    class Program
    {
        static void Main(string[] args)
        {
            var repo = new Repository();
            Random RND = new Random(DateTime.Now.Millisecond);

            using (var context = new SalesContext()) { context.Dispose(); }     // as install DB


            //for (int i = 0; i < 30; i++)
            //{
            //    SaveToDB(repo, RND);
            //}
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
        }

        private static void DisplayData(Repository repo)
        {
            Console.WriteLine("RESULT:");
            Console.WriteLine("");

            var managers = repo.Select<Manager>()
                .Include(m => m.Sales.Select(mm => mm.Manager))
                .Include(m => m.Sales.Select(mc => mc.Client))
                .Include(m => m.Sales.Select(mp => mp.Product))
                .Include(m => m.Sales.Select(mf => mf.FileName))
                .ToList();

            foreach (var manager in managers)
            {
                Console.WriteLine("{0}.{1}:", manager.Id, manager.Name);
                foreach (Sale sale in manager.Sales)
                {
                    Console.WriteLine("- sale {0, 2}: {1, 2}  | {2, 3}. {3, -10} | {4, 3}. {5, -10} - {6, 3} | {7, 3}. {8, -10} | {9, 3}. {10, 3}"
                        , sale.Id
                        , sale.DTG.ToString("dd.MM.yyyy HH:mm")
                        , sale.Manager?.Id, sale.Manager?.Name
                        , sale.Product?.Id, sale.Product?.Name, sale.Product?.Cost
                        , sale.Client?.Id, sale.Client?.Name
                        , sale.FileName?.Id, sale.FileName?.Name
                        );
                }
                Console.WriteLine();
            }
        }







        //private static void SaveToDB(Random RND)
        //{
        //    using (SalesContext context = new SalesContext())
        //    {
        //        Sale sale = new Sale
        //        {
        //            Client = GetClient(context, "Client" + RND.Next(1, 15).ToString()),
        //            FileName = GetFileName(context, "fileNameManager02.csv"),
        //            Manager = GetManager(context, "Manager" + RND.Next(1, 15).ToString()),
        //            Product = GetProduct(context, "Product" + RND.Next(1, 15).ToString()),
        //            DTG = DateTime.Now.AddMinutes(-RND.Next(1, 10))
        //        };
        //        sale.Sum = sale.Product.Cost;

        //        context.Sales.Add(sale);

        //        try
        //        {
        //            context.SaveChanges();
        //        }
        //        catch (DbEntityValidationException ex)
        //        {
        //            foreach (DbEntityValidationResult validationError in ex.EntityValidationErrors)
        //            {
        //                Console.WriteLine("Object: " + validationError.Entry.Entity.ToString());
        //                Console.WriteLine("");
        //                foreach (DbValidationError err in validationError.ValidationErrors)
        //                {
        //                    Console.Write(err.ErrorMessage + "");
        //                }
        //            }
        //        }
        //        catch (SqlException ex)
        //        {
        //            DisplaySqlErrors(ex);
        //            //var sqlException = ex.GetBaseException() as SqlException;
        //            //if (sqlException?.Errors.Count > 0)
        //            //{
        //            //    switch (sqlException.Errors[0].Number)
        //            //    {
        //            //        case 547: // Foreign Key violation
        //            //        default:
        //            //            break;
        //            //    }

        //            //}


        //        }
        //        //catch (Exception e)
        //        //{
        //        //    Console.WriteLine(e.Message);
        //        //}

        //    }
        //}

        //private static Product GetProduct(SalesContext context, string name)
        //{
        //    var product = context.Products.FirstOrDefault(x => x.Name.Equals(name));
        //    return product == null
        //        ? new Product()
        //        {
        //            Name = name,
        //            Cost = 200
        //        }
        //        : product;
        //}

        //private static Manager GetManager(SalesContext context, string name)
        //{
        //    var manager = context.Managers.FirstOrDefault(x => x.Name.Equals(name));

        //    return manager == null
        //        ? new Manager() { Name = name }
        //        : manager;
        //}

        //private static FileName GetFileName(SalesContext context, string name)
        //{
        //    var fileName = context.Files.FirstOrDefault(x => x.Name.Equals(name));
        //    return fileName == null
        //        ? new FileName()
        //        {
        //            Name = name,
        //            DTG = DateTime.Now
        //        }
        //        : fileName;
        //}

        //private static Client GetClient(SalesContext context, string name)
        //{
        //    var client = context.Clients.FirstOrDefault(x => x.Name.Equals(name));

        //    return client == null
        //        ? new Client() { Name = name }
        //        : client;
        //}




        private static void DisplaySqlErrors(SqlException exception)
        {
            for (int i = 0; i < exception.Errors.Count; i++)
            {
                Console.WriteLine("Index #" + i + "\n" +
                    "Error: " + exception.Errors[i].ToString() + "\n");
            }
            Console.ReadLine();
        }

        private static void DisplayManagers()
        {
            Console.WriteLine("");
            Console.WriteLine("MANAGERS:");

            using (SalesContext context = new SalesContext())
            {
                context.Configuration.LazyLoadingEnabled = false;   // отключаем ленивую загрузку

                List<Manager> managers = context.Managers
                                                .Include("Sales")
                                                .ToList();
                //List<Sale> sales = managers.SelectMany(c => c.Sales)
                //        // Запрос к базе данных не выполняется,
                //        // т.к. данные уже были извлечены 
                //        // ранее с помощью прямой загрузки
                //        .ToList();
                
                foreach (Manager manager in managers)
                {
                    Console.WriteLine("{0}.{1}:", manager.Id, manager.Name);
                    foreach (Sale sale in manager.Sales)
                    {
                        Console.WriteLine("- sale {0}: {1}; {2}.{3}; {4}.{5}-{6}; {7}.{8}; {9}.{10}"
                            , sale.Id
                            , sale.DTG.ToString("dd.MM.yyyy hh:mm")
                            , sale.Manager.Id, sale.Manager.Name
                            , sale.Product.Id, sale.Product.Name, sale.Product.Cost
                            , sale.Client.Id, sale.Client.Name
                            , sale.FileName.Id, sale.FileName.Name
                            );
                    }
                    Console.WriteLine();
                }
            }
        }

        //private static void DisplayProducts()
        //{
        //    Console.WriteLine("");
        //    using (SalesContext context = new SalesContext())
        //    {
        //        var products = context.Products.OrderBy(x => x.Id);
        //        Console.WriteLine("PRODUCTS:");
        //        foreach (Product product in products)
        //        {
        //            Console.WriteLine("{0}.{1};", product.Id, product.Name);
        //        }
        //    }
        //}

        //private static void DisplayClients()
        //{
        //    Console.WriteLine("");
        //    using (SalesContext context = new SalesContext())
        //    {
        //        var clients = context.Clients.OrderBy(x => x.Id);
        //        Console.WriteLine("CLIENTS:");
        //        foreach (Client client in clients)
        //        {
        //            Console.WriteLine("{0}.{1};", client.Id, client.Name);
        //        }
        //    }
        //}

        private static void DisplaySales()
        {
            Console.WriteLine("");
            Console.WriteLine("SALES:");

            IOrderedQueryable<Sale> sales;
            using (SalesContext context = new SalesContext())
            {
                sales = GetAllSales(context);
            }
            foreach (Sale sale in sales)
            {
                Console.WriteLine("{0}: {1} - {2}.{3}; {4}.{5}-{6}; {7}.{8}; {9}.{10}"
                    , sale.Id
                    , sale.DTG.ToString("dd.MM.yyyy hh:mm")
                    , sale.Manager.Id, sale.Manager.Name
                    , sale.Product.Id, sale.Product.Name, sale.Product.Cost
                    , sale.Client.Id, sale.Client.Name
                    , sale.FileName.Id, sale.FileName.Name
                    );
            }
        }

        private static IOrderedQueryable<Sale> GetAllSales(SalesContext context)
        {
            var sales = context.Sales
                               .Include("Manager")
                               .Include("Product")
                               .Include("Client")
                               .Include("FileName")
                               .OrderBy(x => x.Id);            
            return sales;
        }

        private static IOrderedQueryable<Sale> GetSalesManager(SalesContext context, Manager manager)
        {
            var sales = context.Sales
                               .Include("Manager")
                               .Include("Product")
                               .Include("Client")
                               .Include("FileName")
                               .Where(x => x.Manager == manager)
                               .OrderBy(x => x.Id);
            return sales;
        }




















//        internal static void SaveToDB(Repository repo, Random rnd)
//{

//    string client = "Client" + rnd.Next(1, 15).ToString();
//    string fileName = "fileNameDefault.csv";
//    string manager = "Manager" + rnd.Next(1, 15).ToString();
//    string product = "Product" + rnd.Next(1, 15).ToString();


//    Sale sale = new Sale
//    {
//        Client = repo.Select<Client>().FirstOrDefault(x => x.Name.Equals(client)) ?? new Client() { Name = client },
//        FileName = repo.Select<FileName>().FirstOrDefault(x => x.Name.Equals(fileName)) ?? new FileName() { Name = fileName, DTG = DateTime.Now },
//        Manager = repo.Select<Manager>().FirstOrDefault(x => x.Name.Equals(manager)) ?? new Manager() { Name = manager },
//        Product = repo.Select<Product>().FirstOrDefault(x => x.Name.Equals(product)) ?? new Product() { Name = product, Cost = rnd.Next(100, 300) },
//        DTG = DateTime.Now.AddMinutes(-rnd.Next(1, 50))
//    };
//    sale.Sum = sale.Product.Cost;
//    repo.Insert(sale);


//    //using (SalesContext context = new SalesContext())
//    //{
//    //    Sale sale = new Sale
//    //    {
//    //        Client = GetClient(context, "Client" + rnd.Next(1, 15).ToString()),
//    //        FileName = GetFileName(context, "fileNameManager02.csv"),
//    //        Manager = GetManager(context, "Manager" + rnd.Next(1, 15).ToString()),
//    //        Product = GetProduct(context, "Product" + rnd.Next(1, 15).ToString()),
//    //        DTG = DateTime.Now.AddMinutes(-rnd.Next(1, 10))
//    //    };
//    //    sale.Sum = sale.Product.Cost;

//    //    context.TmpSales.Add(sale);
//    //    bool result = SaveToDB(context);
//    //    context.Dispose();
//    //    //    try
//    //    //    {
//    //    //        context.SaveChanges();
//    //    //    }
//    //    //    //catch (DbEntityValidationException ex)
//    //    //    //{
//    //    //    //    foreach (DbEntityValidationResult validationError in ex.EntityValidationErrors)
//    //    //    //    {
//    //    //    //        Console.WriteLine("Object: " + validationError.Entry.Entity.ToString());
//    //    //    //        Console.WriteLine("");
//    //    //    //        foreach (DbValidationError err in validationError.ValidationErrors)
//    //    //    //        {
//    //    //    //            Console.Write(err.ErrorMessage + "");
//    //    //    //        }
//    //    //    //    }
//    //    //    //}
//    //    //    //catch (SqlException ex)
//    //    //    //{
//    //    //    //    DisplaySqlErrors(ex);
//    //    //    //    //var sqlException = ex.GetBaseException() as SqlException;
//    //    //    //    //if (sqlException?.Errors.Count > 0)
//    //    //    //    //{
//    //    //    //    //    switch (sqlException.Errors[0].Number)
//    //    //    //    //    {
//    //    //    //    //        case 547: // Foreign Key violation
//    //    //    //    //        default:
//    //    //    //    //            break;
//    //    //    //    //    }

//    //    //    //    //}
//    //    //    //}
//    //    //    catch (Exception e)
//    //    //    {
//    //    //        Console.WriteLine(e.Message);
//    //    //    }

//    //}
//}

    }
}
