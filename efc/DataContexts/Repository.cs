using efc.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace efc.DataContexts
{
    public class Repository
    {

        private static DbContextOptions<SalesContext> GetConnection()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(System.IO.Directory.GetCurrentDirectory());   // установка пути к текущему каталогу
            builder.AddJsonFile("appsettings.json");                          // получаем конфигурацию из файла appsettings.json
            var config = builder.Build();                                     // создаем конфигурацию
            string connectionString = config.GetConnectionString("DefaultConnection");
            var optionsBuilder = new DbContextOptionsBuilder<SalesContext>();
            var options = optionsBuilder
                .UseSqlServer(connectionString)
                .Options;
            return options;
        }

        internal static void SaveToDB(Random rnd)
        {

            string client = "Client" + rnd.Next(1, 15).ToString();
            string fileName = "fileNameDefault.csv";
            string manager = "Manager" + rnd.Next(1, 15).ToString();
            string product = "Product" + rnd.Next(1, 15).ToString();


            //    Sale sale = new Sale
            //{
            //    Client = Repository.Select<Client>().FirstOrDefault(x => x.Name.Equals(client)) ?? new Client() { Name = client },
            //    FileName = Repository.Select<FileName>().FirstOrDefault(x => x.Name.Equals(fileName)) ?? new FileName() { Name = fileName, DTG = DateTime.Now },
            //    Manager = Repository.Select<Manager>().FirstOrDefault(x => x.Name.Equals(manager)) ?? new Manager() { Name = manager },
            //    //Product = GetProduct(context, "Product" + RND.Next(1, 15).ToString()),
            //    Product = Repository.Select<Product>().FirstOrDefault(x => x.Name.Equals(product)) ?? new Product() { Name = product, Cost = RND.Next(100, 300) },
            //    DTG = DateTime.Now.AddMinutes(-rnd.Next(1, 50))
            //};
            //sale.Sum = sale.Product.Cost;
            //Repository.Insert(sale);


            using (SalesContext context = new SalesContext())
            {
                Sale sale = new Sale
                {
                    Client = GetClient(context, "Client" + rnd.Next(1, 15).ToString()),
                    FileName = GetFileName(context, "fileNameManager02.csv"),
                    Manager = GetManager(context, "Manager" + rnd.Next(1, 15).ToString()),
                    Product = GetProduct(context, "Product" + rnd.Next(1, 15).ToString()),
                    DTG = DateTime.Now.AddMinutes(-rnd.Next(1, 10))
                };
                sale.Sum = sale.Product.Cost;

                context.Sales.Add(sale);
                bool result = SaveToDB(context);
                context.Dispose();
                //    try
                //    {
                //        context.SaveChanges();
                //    }
                //    //catch (DbEntityValidationException ex)
                //    //{
                //    //    foreach (DbEntityValidationResult validationError in ex.EntityValidationErrors)
                //    //    {
                //    //        Console.WriteLine("Object: " + validationError.Entry.Entity.ToString());
                //    //        Console.WriteLine("");
                //    //        foreach (DbValidationError err in validationError.ValidationErrors)
                //    //        {
                //    //            Console.Write(err.ErrorMessage + "");
                //    //        }
                //    //    }
                //    //}
                //    //catch (SqlException ex)
                //    //{
                //    //    DisplaySqlErrors(ex);
                //    //    //var sqlException = ex.GetBaseException() as SqlException;
                //    //    //if (sqlException?.Errors.Count > 0)
                //    //    //{
                //    //    //    switch (sqlException.Errors[0].Number)
                //    //    //    {
                //    //    //        case 547: // Foreign Key violation
                //    //    //        default:
                //    //    //            break;
                //    //    //    }

                //    //    //}
                //    //}
                //    catch (Exception e)
                //    {
                //        Console.WriteLine(e.Message);
                //    }

            }
        }

        #region GET_FROM_BD_OR_NEW  
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

        private static FileName GetFileName(SalesContext context, string name)
        {
            var fileName = context.Files.FirstOrDefault(x => x.Name.Equals(name));
            return fileName == null
                ? new FileName()
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


        #endregion // GET_FROM_BD_OR_NEW




        private static bool SaveToDB(DbContext context)
        {
            try
            {
                //context.GetService<ILoggerFactory>().AddProvider(new SalesLoggerProvider());
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                return false;
            }
        }


        public static IQueryable<TEntity> Select<TEntity>(DbContext context = null) 
            where TEntity : class
        {
            if (context == null) { context = new SalesContext(GetConnection()); }

            // Здесь мы можем указывать различные настройки контекста,
            // например выводить в отладчик сгенерированный SQL-код
            //context.Database.Log =
            //    (s => System.Diagnostics.Debug.WriteLine(s));
            //context.GetService<ILoggerFactory>().AddProvider(new SalesLoggerProvider());

            // Загрузка данных с помощью универсального метода Set
            return context.Set<TEntity>().AsQueryable();

            /*
             * USING
            static void Main()
            {
                 var customers = Repository.Select<Customer>()
                    .Include(c => c.Orders)
                    .Where(c => c.Age > 25)
                    .ToList();
            }
            */
        }

        /// <summary>
        /// Insert Entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        public static void Insert<TEntity>(TEntity entity, DbContext cntx = null) 
            where TEntity : class
        {
            //if (context == null) { context = new SalesContext(); }
            using (var context = cntx ?? new SalesContext(GetConnection()))
            {
                //context.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s));
                //context.Entry(entity).State = EntityState.Added;
                bool result = SaveToDB(context);
                context.Dispose();
            }
        }





        /// <summary>
        /// Insert some Entities
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        public static void Inserts<TEntity>(IEnumerable<TEntity> entities, DbContext context = null) 
            where TEntity : class
        {
            
            if (entities?.Count() > 0)
            {
                //if (context == null) { context = new SalesContext(GetConnection()); }
                using (context ?? new SalesContext(GetConnection()))
                {
                    // Отключаем отслеживание и проверку изменений для оптимизации вставки множества полей
                    //context.Configuration.AutoDetectChangesEnabled = false;
                    //context.Configuration.ValidateOnSaveEnabled = false;

                    //context.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s));

                    foreach (TEntity entity in entities)
                    { context.Entry(entity).State = EntityState.Added; }
                    bool result = SaveToDB(context);

                    //context.Configuration.AutoDetectChangesEnabled = true;
                    //context.Configuration.ValidateOnSaveEnabled = true;

                    context.Dispose();
                }
            }







            /*
             * USING
            static void Main()
            {
                Repository.Inserts(
                    new List<Customer>
                    {
                        new Customer {
                            FirstName = "Сидор",
                            LastName = "Сидоров",
                            Age = 23
                        },
                        new Customer {
                            FirstName = "Павел",
                            LastName = "Васин",
                            Age = 20
                        }                    });
}
            */
        }


        /// <summary>
        /// Update any entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="context"></param>
        public static void Update<TEntity>(TEntity entity, DbContext context = null)
            where TEntity : class
        {
            //if (context == null) { context = new SalesContext(); }

            // Настройки контекста
            //context.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s));

            using (context ?? new SalesContext(GetConnection()))
            {
                context.Entry<TEntity>(entity).State = EntityState.Modified;
                bool result = SaveToDB(context);
                context.Dispose();
            }
        }


        /// <summary>
        /// Delete any entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        public void Delete<TEntity>(TEntity entity, DbContext context = null)
            where TEntity : class
        {
            //if (context == null) { context = new SalesContext(); }
            //context.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s));

            using (context ?? new SalesContext(GetConnection()))
            {
                context.Entry<TEntity>(entity).State = EntityState.Deleted;
                bool result = SaveToDB(context);
                context.Dispose();
            }
        }


    }
}
