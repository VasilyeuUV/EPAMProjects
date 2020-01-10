using efdb.DataModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;

namespace efdb.DataContexts
{
    public sealed class Repository : IDisposable
    {
        private bool disposed = false;                      // Flag: Has Dispose already been called?
        private readonly SalesContext _context = null;

        private static object locker = new object();
        private static AutoResetEvent waitHandler = new AutoResetEvent(true);

        /// <summary>
        /// CTOR
        /// </summary>
        public Repository()
        {
            this._context = new SalesContext();
        }

        public void Dispose()
        {
            Dispose(true);                     
        }

        private void Dispose(bool disposing)
        {
            if (disposed) { return; }
            if (disposing)      // Free any managed objects
            {
                this._context.Dispose();
            }
            disposed = true;
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

            lock (locker) { return this._context.Set<TEntity>(); }
            

            /*
            For use:
            static void Main()
            {
                 var managers = Select<Manager>()
                    .Include(m => m.Sales.Select(mp => mp.Product)
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
            lock (locker)
            {
                var dbSet = this._context.Set<TEntity>();
                try
                {
                    dbSet.Add(entity);
                    this._context.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Insert some Entities
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        public bool Inserts<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            lock (locker)
            {
                if (entities?.Count() > 0)
                {
                    foreach (TEntity entity in entities)
                    {
                        if (!this.Insert(entity)) { return false; }
                    }
                    return true;
                }
                return false;
            }

        }


        /// <summary>
        /// Delete any entity
        /// </summary>
        /// <typeparam name = "TEntity" ></ typeparam >
        /// < param name="entity"></param>
        public bool Delete<TEntity>(TEntity entity) where TEntity : class
        {
            lock (locker)
            {
                var dbSet = this._context.Set<TEntity>();

                try
                {
                    dbSet.Remove(entity);
                    this._context.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }


        public bool Deletes<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            lock (locker)
            {
                if (entities?.Count() > 0)
                {
                    foreach (TEntity entity in entities)
                    {
                        if (!this.Delete(entity)) { return false; }
                    }
                    return true;
                }
                return false;
            }
        }



        #endregion // CRUD


        #region GET_FROM_BD_OR_NEW  
        //#################################################################################################################
        private static Product GetProduct(SalesContext context, string name)
        {
            lock (locker)
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
        }

        private static Manager GetManager(SalesContext context, string name)
        {
            lock (locker)
            {
                var manager = context.Managers.FirstOrDefault(x => x.Name.Equals(name));

                return manager == null
                    ? new Manager() { Name = name }
                    : manager;
            }
        }

        private static FileName GetFileName(SalesContext context, string name)
        {
            lock (locker)
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
        }

        private static Client GetClient(SalesContext context, string name)
        {
            lock (locker)
            {
                var client = context.Clients.FirstOrDefault(x => x.Name.Equals(name));

                return client == null
                    ? new Client() { Name = name }
                    : client;
            }
        }



        #endregion // GET_FROM_BD_OR_NEW

    }
}
