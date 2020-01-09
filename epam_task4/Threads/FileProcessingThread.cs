using efdb.DataContexts;
using efdb.DataModels;
using epam_task4.Models;
using FileParser.Parsers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
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
        internal event EventHandler<bool> FileNamingErrorEvent;
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


            // PARSE FILE NAME
            this._fnsm = GetFileNameData(filePath);
            if (this._fnsm == null) { return; }

            this.Name = this._fnsm.FileName;
            this._thread.Name = this._fnsm.FileName;
            

            // PARSE FILE CONTENT
            var fileData = GetFileContentData(filePath);
            if (fileData == null || fileData.Count() < 1) { OnFileContentErrorEvent(); return; }

            
            // CHECK CORRECT FILE CONTENT PARSING
            FileName fileName = null;
            ICollection<TmpSale> tmpSales = null;
            using (var repo = new Repository())
            {
                try
                {
                    fileName = repo.Select<FileName>().FirstOrDefault(x => x.Name.Equals(this._fnsm.FileName.ToLower()));
                    tmpSales = GetTmpSalesData<TmpSale>(repo, fileName);
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
        /// Check if data is saved in database from file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private bool CheckIfDataSaved(string fName)
        {
            List<Sale> sales = null;
            using (var repo = new Repository())
            {
                try
                {
                    FileName fileName = repo.Select<FileName>().FirstOrDefault(x => x.Name.Equals(fName.ToLower()));
                    sales = GetTmpSalesData<Sale>(repo, fileName);
                }
                catch (Exception) { }
            }
            return sales != null;
        }

        private FileName GetFileNameFromDB(Repository repo, string fname)
        {
            try
            {
                return repo.Select<FileName>().FirstOrDefault(x => x.Name.Equals(fname.ToLower()));
            }
            catch (Exception)
            {
                return null;
            }            
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
        //private IEnumerable<IDictionary<string, string>> GetFileContentData(string path)
        private IEnumerable<IDictionary<string, string>> GetFileContentData(string path)
        {
            this._csvParser = new CSVParser();
            this._csvParser.ParsingCompleted += CsvParser_ParsingCompleted;
            this._csvParser.FieldParsed += _csvParser_FieldParsed;
            this._csvParser.ErrorParsing += _csvParser_ErrorParsing;           

            return this._csvParser.Parse(path);
        }





        #region PARSING
        //############################################################################################################

        /// <summary>
        /// Get file name data
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private FileNameSaleModel GetFileNameData(string filePath)
        {
            this.Name = filePath;

            var fileNameData = FileNameParser.Parse(filePath, this._fileNameStruct);
            if (fileNameData == null
                || fileNameData.Count() != this._fileNameStruct.Length)
            {
                OnFileNamingErrorEvent();
                return null;
            }

            FileNameSaleModel fnsm = FileNameSaleModel.CreateInstance(filePath, fileNameData);
            if (fnsm == null) { OnFileNamingErrorEvent(); return null; }
            if (CheckIfDataSaved(fnsm.FileName))
            {
                bool fileSaved = true;
                OnFileNamingErrorEvent(fileSaved);
                return null;
            }
            return fnsm;
        }

        #endregion // PARSING






        #region ON_THIS_EVENTS
        //############################################################################################################


        /// <summary>
        /// Thread work completed event
        /// </summary>
        /// <param name="abort">true - if thread was being aborted</param>
        /// <param name="isSaved">true - if data was saved to database</param>
        private void OnWorkCompleting(bool abort, bool isSaved = false)
        {
            this._abort = abort;
            this.Stop();
            if (abort && !isSaved) { DeleteTmpData(this._fnsm); }   // delete data from temporary table
            WorkCompleted?.Invoke(this, abort);
        }


        /// <summary>
        /// File naming error event        
        /// </summary>
        /// <param name="isSaved">
        /// true - file was saved to db earlier;
        /// false - (default) incorrect file name;
        /// </param>
        private void OnFileNamingErrorEvent(bool isSaved = false)
        {
            FileNamingErrorEvent?.Invoke(this, isSaved);
            OnWorkCompleting(true, isSaved);
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
        /// <param name="error"></param>
        private void _csvParser_ErrorParsing(object sender, FileParser.Enums.EnumErrors error)
        {
            this.Stop();
            switch (error)
            {
                case FileParser.Enums.EnumErrors.fileNameError:
                    break;
                case FileParser.Enums.EnumErrors.fileContentError:
                    break;
                case FileParser.Enums.EnumErrors.fileParseError:
                    break;
                default:
                    break;
            }

            FileContentErrorEvent(this, EventArgs.Empty);
            
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
            this._csvParser.ErrorParsing -= _csvParser_ErrorParsing;
            this._csvParser = null;

            OnWorkCompleting(abort);
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
                    ICollection<TmpSale> tmpSales = GetTmpSalesData<TmpSale>(repo, fileData);
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
        private void DeleteTmpData(FileNameSaleModel fileData)
        {
            if (fileData == null) { return; }

            try
            {
                using (var repo = new Repository())
                {
                    FileName fileName = GetFileNameData(repo, fileData.FileName);
                    if (fileData == null) { return; }

                    List<TmpSale> tmpSales = GetTmpSalesData<TmpSale>(repo, fileName);
                    if (fileName != null 
                        && tmpSales != null && tmpSales.Count > 0 
                        && !repo.Delete(fileName))
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
        private List<T> GetTmpSalesData<T>(Repository repo, FileName fileName) where T : SaleBase
        {
            if (repo == null || fileName == null) { return null; }
            return repo.Select<T>()
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

            string clnt = parsedData["Client"];
            var client = repo.Select<Client>().FirstOrDefault(x => x.Name.Equals(clnt));
            sale.Client = client ?? new Client() { Name = parsedData["Client"] };

            string prdct = parsedData["Product"];
            var product = repo.Select<Product>().FirstOrDefault(x => x.Name.Equals(prdct));
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
