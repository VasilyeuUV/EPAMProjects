using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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



        public static IQueryable<TEntity> Select<TEntity>(DbContext context = null) 
            where TEntity : class
        {
            if (context == null) { context = new SalesContext(GetConnection()); }

            // Здесь мы можем указывать различные настройки контекста,
            // например выводить в отладчик сгенерированный SQL-код
            //context.Database.Log =
            //    (s => System.Diagnostics.Debug.WriteLine(s));

            // Загрузка данных с помощью универсального метода Set
            return context.Set<TEntity>();

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
        public static void Insert<TEntity>(TEntity entity, DbContext context = null) 
            where TEntity : class
        {
            //if (context == null) { context = new SalesContext(); }
            using (context ?? new SalesContext(GetConnection()))
            {
                //context.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s));
                context.Entry(entity).State = EntityState.Added;
                context.SaveChanges();
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
                    context.SaveChanges();

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
                context.SaveChanges();
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
                context.SaveChanges();
                context.Dispose();
            }
        }


    }
}
