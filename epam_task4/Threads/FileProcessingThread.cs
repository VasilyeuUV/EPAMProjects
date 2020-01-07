using efdb.DataContexts;
using efdb.DataModels;
using FileParser.Models;
using FileParser.Parsers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;

namespace epam_task4.Threads
{
    internal sealed class FileProcessingThread
    {        
        private Thread _thread = null;
        private bool _abort = false;

        private SalesFileNameDataModel _fileNameData = null;
        CSVParser _csvParser = null;


        private readonly bool _checkProductsDB;        
        private readonly bool _checkManagersDB;

        internal string Name { get; private set; }

        internal event EventHandler<bool> WorkCompleted;
        internal event EventHandler<bool> FileNamingErrorEvent;
        internal event EventHandler FileContentErrorEvent;
        internal event EventHandler<string> SendMessageEvent;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="filePath"></param>
        internal FileProcessingThread()
        {
            Boolean.TryParse(ConfigurationManager.AppSettings["CheckManagers"], out this._checkManagersDB);
            Boolean.TryParse(ConfigurationManager.AppSettings["CheckProducts"], out this._checkProductsDB);

            this._thread = new Thread(this.RunProcess);  
            
        }

        /// <summary>
        /// Start this Thread
        /// </summary>
        /// <param name="products"></param>
        internal void Start(string filePath)
        {
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                this._thread.IsBackground = true;
                this._thread?.Start(filePath);
            }
        }


        /// <summary>
        /// Stop this thread
        /// </summary>
        internal void Stop()
        {
            this._csvParser?.Stop();
        }


        /// <summary>
        /// Manager job process
        /// </summary>
        /// <param name="obj">product list</param>
        private void RunProcess(object obj)
        {
            string filePath = (string)obj;
            FileInfo fileInf = new FileInfo(filePath);
            if (!fileInf.Exists) { OnFileNamingErrorEvent(false); return; }

            this._thread.Name = fileInf.Name;
            this.Name = fileInf.Name;

            this._fileNameData = GetFileNameData(fileInf);
            if (this._fileNameData == null) { OnFileNamingErrorEvent(false); return; }

            FileName fileName = null;
            using (var repo = new Repository())
            {
                try
                {
                    fileName = repo.Select<FileName>().FirstOrDefault(x => x.Name.Equals(this._fileNameData.FileName));
                }
                catch (Exception ex) { }                             
            }
            if (fileName != null) { OnFileNamingErrorEvent(true); return; }

            IEnumerable<SalesFieldDataModel> tableRows = RunCSVParser(fileInf);

            OnWorkCompleting(this._abort);
        }


        /// <summary>
        /// CSV Parser work process
        /// </summary>
        /// <param name="fileInf"></param>
        /// <returns></returns>
        private IEnumerable<SalesFieldDataModel> RunCSVParser(FileInfo fileInf)
        {
            this._csvParser = new CSVParser();
            this._csvParser.ParsingCompleted += CsvParser_ParsingCompleted;
            this._csvParser.HeaderParsed += CsvParser_HeaderParsed;
            this._csvParser.FieldParsed += CsvParser_FieldParsed;
            this._csvParser.ErrorParsing += CsvParser_ErrorParsing;
            return this._csvParser.Parse(fileInf.FullName);
        }





        #region ON_THIS_EVENTS
        //############################################################################################################


        /// <summary>
        /// Thread work completed event
        /// </summary>
        /// <param name="abort"> true if thread aborted</param>
        private void OnWorkCompleting(bool abort)
        {
            this._abort = abort;
            this.Stop();

            if (abort) { DeleteTmpData(this._fileNameData.FileName); }  // delete data from temporary table
            else { SaveTmpData(this._fileNameData.FileName); }          // save data from temporary table to main table

            WorkCompleted?.Invoke(this, abort);
        }


        /// <summary>
        /// File naming error event        
        /// </summary>
        /// <param name="isProcessed">
        /// true - if the file was processed earlier;
        /// false - if the file name is incorrect;
        /// </param>
        private void OnFileNamingErrorEvent(bool isProcessed)
        {
            FileNamingErrorEvent?.Invoke(this.Name, isProcessed);
            this.Stop();
        }


        /// <summary>
        /// File Content Error Event
        /// </summary>
        private void OnFileContentErrorEvent()
        {
            FileContentErrorEvent?.Invoke(this, EventArgs.Empty);
            this.Stop();
        }


        #endregion



        #region CSV_PARSER_EVENTS
        //############################################################################################################

        /// <summary>
        /// Table row parsed event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="parsedData"></param>
        private void CsvParser_FieldParsed(object sender, SalesFieldDataModel parsedData)
        {
            using (var repo = new Repository())
            {
                TmpSale sale = ConvertToSale(repo, parsedData);
                if (sale == null) { return; }

                if (!repo.Insert(sale))
                {
                    SendMessageEvent?.Invoke(this, "Error saving temporary data");
                }
            }
        }

        /// <summary>
        /// Table Header parsed event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CsvParser_HeaderParsed(object sender, SalesFieldDataModel e)
        {
        }


        /// <summary>
        /// On error parsing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CsvParser_ErrorParsing(object sender, EventArgs e)
        {
            FileContentErrorEvent(this, EventArgs.Empty);
            this.Stop();
        }



        /// <summary>
        /// Parse completed Event Handler 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CsvParser_ParsingCompleted(object sender, bool abort)
        {
            this._csvParser.ParsingCompleted -= CsvParser_ParsingCompleted;
            this._csvParser.HeaderParsed -= CsvParser_HeaderParsed;
            this._csvParser.FieldParsed -= CsvParser_FieldParsed;
            this._csvParser.ErrorParsing -= CsvParser_ErrorParsing;
            this._csvParser = null;            
        }


        #endregion // CSV_PARSER_EVENTS




        #region DATA_MANAGMENT
        //############################################################################################################




        private void SaveTmpData(string fileName)
        {
            try
            {
                using (var repo = new Repository())
                {
                    FileName fileData = GetFileNameData(repo, fileName);
                    ICollection<TmpSale> tmpSales = GetTmpSalesData(repo, fileData);
                    IEnumerable<Sale> sales = ConvertToSaleList<Sale, TmpSale>(tmpSales);

                    if (!repo.Inserts(sales))
                    {
                        SendMessageEvent?.Invoke(this, "Error saving data from temporary table");
                    }
                    if (!repo.Deletes(tmpSales))
                    {
                        SendMessageEvent?.Invoke(this, "Error deleting temporary data after saving");
                    }
                }
            }
            catch (Exception ex)
            {
                SendMessageEvent?.Invoke(this, $"Error accessing database while saving temporary data: {ex.Message}");
            }
        }


        /// <summary>
        /// Delete TmpSales from DB by filename
        /// </summary>
        /// <param name="fileName"></param>
        private void DeleteTmpData(string fileName)
        {
            try
            {
                using (var repo = new Repository())
                {
                    FileName fileData = GetFileNameData(repo, fileName);
                    List<TmpSale> tmpSales = GetTmpSalesData(repo, fileData);

                    if (fileData != null && tmpSales?.Count > 0 && !repo.Delete(fileData))
                    {
                        SendMessageEvent?.Invoke(this, "Error deleting temporary data from the database");
                    }
                }
            }
            catch (Exception ex)
            {
                SendMessageEvent?.Invoke(this, $"Error accessing database while deleting temporary data: {ex.Message}");
            }
        }

        /// <summary>
        /// Get sales from temporary table by filename
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private List<TmpSale> GetTmpSalesData(Repository repo, FileName fileName)
        {
            if (repo == null || fileName == null) { return null; }
            return repo.Select<TmpSale>()
                       .Include(m => m.Manager)
                       .Include(p => p.Product)
                       .Include(c => c.Client)
                       .Where(f => f.FileName.Id == fileName.Id)
                       .ToList();
        }

        /// <summary>
        /// Get Filename object data
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private FileName GetFileNameData(Repository repo, string fileName)
        {
            if (repo == null || string.IsNullOrWhiteSpace(fileName)) { return null; }
            return repo.Select<FileName>()
                       .FirstOrDefault(x => x.Name.Equals(fileName));
        }
        

        private SalesFileNameDataModel GetFileNameData(FileInfo fileInf)
        {
            SalesFileNameDataModel fileNameData = SalesFileNameDataModel.CreateInstance(fileInf);
            if (fileNameData == null
                || string.IsNullOrWhiteSpace(fileNameData.Manager)
                || fileNameData.DTG == new DateTime()
                || fileNameData.FileName.Trim().Length < 16
                || fileNameData.FileExtention.Trim().Length < 4
                )
            {
                return null;
            }
            return fileNameData;
        }


        #endregion // DATA_MANAGMENT





        #region CONVERTERS
        //############################################################################################################


        /// <summary>
        /// Convert parsedData to Sale object within the repository
        /// </summary>
        /// <param name="repo">Current repository</param>
        /// <param name="parsedData">parsed Data</param>
        /// <returns></returns>
        private TmpSale ConvertToSale(Repository repo, SalesFieldDataModel parsedData)
        {
            if (parsedData == null
                || string.IsNullOrWhiteSpace(parsedData.Client)
                || string.IsNullOrWhiteSpace(parsedData.Cost)
                || string.IsNullOrWhiteSpace(parsedData.DTG)
                || string.IsNullOrWhiteSpace(parsedData.Product)
                )
            { return null; }

            TmpSale sale = new TmpSale();
            try
            {
                sale.Sum = Convert.ToInt32(parsedData.Cost);
                sale.DTG = DateTime.ParseExact(parsedData.DTG, "dd.MM.yyyy HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                if (sale.DTG.Date != this._fileNameData.DTG.Date) { throw new Exception(); }
            }
            catch (Exception) { return null; }

            var manager = repo.Select<Manager>().FirstOrDefault(x => x.Name.Equals(this._fileNameData.Manager));
            sale.Manager = manager ?? new Manager() { Name = this._fileNameData.Manager };

            var client = repo.Select<Client>().FirstOrDefault(x => x.Name.Equals(parsedData.Client));
            sale.Client = client ?? new Client() { Name = parsedData.Client };

            var product = repo.Select<Product>().FirstOrDefault(x => x.Name.Equals(parsedData.Product));
            sale.Product = product ?? new Product { Name = parsedData.Product, Cost = sale.Sum };

            var fileName = repo.Select<FileName>().FirstOrDefault(x => x.Name.Equals(this._fileNameData.FileName));
            sale.FileName = fileName ?? new FileName()
            {
                Name = this._fileNameData.FileName,
                DTG = this._fileNameData.DTG
            };
            return sale;
        }

        /// <summary>
        /// Convert TmpSale to Sale and back
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="lstFrom"></param>
        /// <returns></returns>
        private IEnumerable<T> ConvertToSaleList<T, U>(ICollection<U> lstFrom)
            where U : SaleBase
            where T : SaleBase, new()
        {
            ICollection<T> lstTo = new List<T>();
            if (lstFrom?.Count > 0)
            {
                foreach (var item in lstFrom)
                {
                    T sale = new T()
                    {
                        Client = item.Client,
                        DTG = item.DTG,
                        FileName = item.FileName,
                        Manager = item.Manager,
                        Product = item.Product,
                        Sum = item.Sum
                    };
                    lstTo.Add(sale);
                }
            }
            return lstTo;
        }

        #endregion



    }
}
