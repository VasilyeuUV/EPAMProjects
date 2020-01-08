using efdb.DataContexts;
using efdb.DataModels;
using epam_task4.Models;
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

        private string[] _fileNameStruct = null;
        private string[] _fileDataStruct = null;

        FileNameSaleModel _fnsm = null;
        //List<SaleModel> _lstSM = null;
        CSVParser _csvParser = null;

        private readonly bool _checkProductsDB = false;        
        private readonly bool _checkManagersDB = false;
        private readonly bool _isSaveProcessedFile = true;

        internal string Name { get; private set; }

        internal event EventHandler<bool> WorkCompleted;
        internal event EventHandler FileNamingErrorEvent;
        internal event EventHandler FileContentErrorEvent;
        internal event EventHandler SaveToDbErrorEvent;
        internal event EventHandler<string> SendMessageEvent;


        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="filePath"></param>
        internal FileProcessingThread(string[] fns, string[] fds)
        {
            Boolean.TryParse(ConfigurationManager.AppSettings["CheckManagers"], out this._checkManagersDB);
            Boolean.TryParse(ConfigurationManager.AppSettings["CheckProducts"], out this._checkProductsDB);
            Boolean.TryParse(ConfigurationManager.AppSettings["SaveProcessedFile"], out this._isSaveProcessedFile);

            //private static readonly string[] FILE_NAME_STRUCT = { "Manager", "DTG" };
            //private static readonly string[] FILE_DATA_STRUCT = { "DTG", "Client", "Product", "Sum" };
            this._fileDataStruct = fds;
            this._fileNameStruct = fns;

            this._thread = new Thread(this.RunProcess);              
        }


        /// <summary>
        /// Manager job process
        /// </summary>
        /// <param name="obj">product list</param>
        private void RunProcess(object obj)
        {
            string filePath = (string)obj;
            
            var fileNameData = FileNameParser.Parse(filePath, this._fileNameStruct);
            if (fileNameData?.Count() < 1) { OnFileNamingErrorEvent(); return; }

            this._fnsm = FileNameSaleModel.CreateInstance(filePath, fileNameData);
            if (this._fnsm == null) { OnFileNamingErrorEvent(); return; }

            this.Name = this._fnsm.FileName;
            this._thread.Name = this._fnsm.FileName;

            //this._lstSM = GetFromCSVParser(filePath);
            //if (_lstSM?.Count() < 1) { OnFileContentErrorEvent(); return; }

            var fileData = GetFromCSVParser(filePath);
            if (fileData?.Count() < 1) { OnFileContentErrorEvent(); return; }

            FileName fileName = null;
            ICollection<TmpSale> tmpSales = null;
            using (var repo = new Repository())
            {
                try
                {
                    fileName = repo.Select<FileName>().FirstOrDefault(x => x.Name.Equals(this._fnsm.FileName.ToLower()));
                    tmpSales = GetTmpSalesData(repo, fileName);
                }
                catch (Exception ) { }
            }

            if (fileData?.Count() != tmpSales?.Count)
            {
                SaveToDbErrorEvent?.Invoke(this, EventArgs.Empty);
                return;
            }

            SaveTmpData(fileName.Name);
            OnWorkCompleting(this._abort);
        }


        /// <summary>
        /// Start this Thread
        /// </summary>
        /// <param name="products"></param>
        internal bool Start(string filePath)
        {
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                this._thread.IsBackground = true;
                this._thread?.Start(filePath);
                return true;
            }
            return false;
        }


        /// <summary>
        /// Stop this thread
        /// </summary>
        internal void Stop()
        {
            this._csvParser?.Stop();
        }






        /// <summary>
        /// CSV Parser work process
        /// </summary>
        /// <param name="fileInf"></param>
        /// <returns></returns>
        //private IEnumerable<IDictionary<string, string>> GetFromCSVParser(string path)
        private IEnumerable<IDictionary<string, string>> GetFromCSVParser(string path)
        {
            this._csvParser = new CSVParser();
            this._csvParser.ParsingCompleted += CsvParser_ParsingCompleted;
            this._csvParser.FieldParsed += _csvParser_FieldParsed;
            this._csvParser.ErrorParsing += CsvParser_ErrorParsing;

            return this._csvParser.Parse(path);
        }







        #region ON_THIS_EVENTS
        //############################################################################################################


        /// <summary>
        /// Thread work completed event
        /// </summary>
        /// <param name="abort"></param>
        /// <param name="canDeleteTmpData"></param>
        private void OnWorkCompleting(bool abort)
        {
            this._abort = abort;
            this.Stop();
            if (abort) { DeleteTmpData(this._fnsm.FileName); } // delete data from temporary table
            WorkCompleted?.Invoke(this, abort);
        }


        /// <summary>
        /// File naming error event        
        /// </summary>
        /// <param name="canDelete">
        /// true - if the file was processed earlier;
        /// false - if the file name is incorrect;
        /// </param>
        private void OnFileNamingErrorEvent()
        {
            FileNamingErrorEvent?.Invoke(this.Name, EventArgs.Empty);
            OnWorkCompleting(true);
        }


        /// <summary>
        /// File Content Error Event
        /// </summary>
        private void OnFileContentErrorEvent()
        {
            FileContentErrorEvent?.Invoke(this, EventArgs.Empty);
            OnWorkCompleting(true);
        }


        #endregion



        #region CSV_PARSER_EVENTS
        //############################################################################################################
        private void _csvParser_FieldParsed(object sender, IDictionary<string, string> parsedData)
        {
            using (var repo = new Repository())
            {
                TmpSale sale = ConvertToSale(repo, parsedData);
                if (sale == null) { return; }                

                if (!repo.Insert(sale))
                {
                    SendMessageEvent?.Invoke(this, "Error saving temporary data");
                    this.Stop();
                }
            }
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
            this._csvParser.FieldParsed -= _csvParser_FieldParsed;
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
                    if (fileData == null) { return; }

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
        

        //private SalesFileNameDataModel GetFileNameData(FileInfo fileInf)
        //{
        //    SalesFileNameDataModel fileNameData = SalesFileNameDataModel.CreateInstance(fileInf);
        //    if (fileNameData == null
        //        || string.IsNullOrWhiteSpace(fileNameData.Manager)
        //        || fileNameData.DTG == new DateTime()
        //        || fileNameData.FileName.Trim().Length < 16
        //        || fileNameData.FileExtention.Trim().Length < 4
        //        )
        //    {
        //        return null;
        //    }
        //    return fileNameData;
        //}


        #endregion // DATA_MANAGMENT





        #region CONVERTERS
        //############################################################################################################


        /// <summary>
        /// Convert parsedData to Sale object within the repository
        /// </summary>
        /// <param name="repo">Current repository</param>
        /// <param name="parsedData">parsed Data</param>
        /// <returns></returns>
        private TmpSale ConvertToSale(Repository repo, IDictionary<string, string> parsedData)
        {
            if (parsedData == null
                || string.IsNullOrWhiteSpace(parsedData["Client"])
                || string.IsNullOrWhiteSpace(parsedData["Cost"])
                || string.IsNullOrWhiteSpace(parsedData["DTG"])
                || string.IsNullOrWhiteSpace(parsedData["Product"])
                )
            { return null; }

            TmpSale sale = new TmpSale();
            try
            {
                sale.Sum = Convert.ToInt32(parsedData["Cost"]);
                sale.DTG = DateTime.ParseExact(parsedData["DTG"], "dd.MM.yyyy HH:mm:ss"
                           , System.Globalization.CultureInfo.CurrentCulture);
                if (sale.DTG.Date != this._fnsm.DTG.Date) { throw new Exception(); }
            }
            catch (Exception) { return null; }

            var manager = repo.Select<Manager>().FirstOrDefault(x => x.Name.Equals(this._fnsm.Manager));
            sale.Manager = manager ?? new Manager() { Name = this._fnsm.Manager };

            var client = repo.Select<Client>().FirstOrDefault(x => x.Name.Equals(parsedData["Client"]));
            sale.Client = client ?? new Client() { Name = parsedData["Client"] };

            var product = repo.Select<Product>().FirstOrDefault(x => x.Name.Equals(parsedData["Product"]));
            sale.Product = product ?? new Product { Name = parsedData["Product"], Cost = sale.Sum };

            var fileName = repo.Select<FileName>().FirstOrDefault(x => x.Name.Equals(this._fnsm.FileName));
            sale.FileName = fileName ?? new FileName()
            {
                Name = this._fnsm.FileName,
                DTG = this._fnsm.DTG
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
