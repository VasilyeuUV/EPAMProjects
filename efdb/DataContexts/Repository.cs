using efdb.DataModels;
using System;
using System.Linq;

namespace efdb.DataContexts
{
    public class Repository
    {
        private readonly SalesContext _context = null;

        /// <summary>
        /// CTOR
        /// </summary>
        public Repository()
        {
            this._context = new SalesContext();
        }              


        #region CRUD  
        //#################################################################################################################

        /// <summary>
        /// Select from DB
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public IQueryable<TEntity> Select<TEntity>() where TEntity : class
        {
            // Here we can specify various context settings
            // for example: save to log
            //context.GetService<ILoggerFactory>().AddProvider(new SalesLoggerProvider());

            // Loading data using the universal Set method
            return this._context.Set<TEntity>().AsQueryable();

            /*
            For use:
            static void Main()
            {
                 var managers = Select<Manager>()
                    .Include(m => m.Sales).ThenInclude(mp => mp.Product)
                    .Where(c => c.id > 25)
                    .ToList();
            }
            */
        }


        /// <summary>
        /// Insert Entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        public bool Insert<TEntity>(TEntity entity) where TEntity : class
        {
            var dbSet = this._context.Set<TEntity>();
            try
            {
                //context.GetService<ILoggerFactory>().AddProvider(new SalesLoggerProvider());
                dbSet.Add(entity);
                this._context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                return false;
            }
        }


        #endregion // CRUD

















        #region GET_FROM_BD_OR_NEW  
        //#################################################################################################################
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














        ///// <summary>
        ///// Insert some Entities
        ///// </summary>
        ///// <typeparam name="TEntity"></typeparam>
        ///// <param name="entities"></param>
        //public static void Inserts<TEntity>(IEnumerable<TEntity> entities, DbContext context = null) 
        //    where TEntity : class
        //{
            
        //    if (entities?.Count() > 0)
        //    {
        //        //if (context == null) { context = new SalesContext(GetConnection()); }
        //        using (context ?? new SalesContext(GetConnection()))
        //        {
        //            // Отключаем отслеживание и проверку изменений для оптимизации вставки множества полей
        //            //context.Configuration.AutoDetectChangesEnabled = false;
        //            //context.Configuration.ValidateOnSaveEnabled = false;

        //            //context.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s));

        //            foreach (TEntity entity in entities)
        //            { context.Entry(entity).State = EntityState.Added; }
        //            bool result = SaveToDB(context);

        //            //context.Configuration.AutoDetectChangesEnabled = true;
        //            //context.Configuration.ValidateOnSaveEnabled = true;

        //            context.Dispose();
        //        }
        //    }







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
        //}


        ///// <summary>
        ///// Update any entity
        ///// </summary>
        ///// <typeparam name="TEntity"></typeparam>
        ///// <param name="entity"></param>
        ///// <param name="context"></param>
        //public static void Update<TEntity>(TEntity entity, DbContext context = null)
        //    where TEntity : class
        //{
        //    //if (context == null) { context = new SalesContext(); }

        //    // Настройки контекста
        //    //context.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s));

        //    using (context ?? new SalesContext(GetConnection()))
        //    {
        //        context.Entry<TEntity>(entity).State = EntityState.Modified;
        //        bool result = SaveToDB(context);
        //        context.Dispose();
        //    }
        //}


        ///// <summary>
        ///// Delete any entity
        ///// </summary>
        ///// <typeparam name="TEntity"></typeparam>
        ///// <param name="entity"></param>
        //public void Delete<TEntity>(TEntity entity, DbContext context = null)
        //    where TEntity : class
        //{
        //    //if (context == null) { context = new SalesContext(); }
        //    //context.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s));

        //    using (context ?? new SalesContext(GetConnection()))
        //    {
        //        context.Entry<TEntity>(entity).State = EntityState.Deleted;
        //        bool result = SaveToDB(context);
        //        context.Dispose();
        //    }
        //}


    }
}
