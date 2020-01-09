using efdb.DataContexts;
using efdb.DataModels;
using epam_task4.Enums;
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
        


        private string[] _fileNameStruct = null;
        private string[] _fileDataStruct = null;

        FileNameSaleModel _fnsm = null;
        //List<SaleModel> _lstSM = null;
        CSVParser _csvParser = null;

        private readonly bool _checkProductsDB = false;        
        private readonly bool _checkManagersDB = false;
        private readonly bool _isSaveProcessedFile = true;

        internal string Name { get; private set; }

        private bool _abort = false;
        private bool Abort
        {
            get => _abort;
            set
            {
                _abort = value;
                OnWorkCompleting(_abort);
            }
        }

        internal event EventHandler<bool> WorkCompleted;
        internal event EventHandler<string> ErrorEvent;


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
        /// Stop this thread
        /// </summary>
        internal void Stop()
        {
            this._csvParser?.Stop();
            this.Abort = true;
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
            this._csvParser = null;
            if (fileData == null || fileData.Count() < 1) 
            { 
                OnErrorEvent(EnumErrors.fileContentError); 
                return; 
            }
            
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
                OnErrorEvent(EnumErrors.saveToDbError);                
                return;
            }

            SaveTmpData(fileName.Name);
            OnWorkCompleting(this.Abort);
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

                     


        #region FILENAME_PARSING
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
                OnErrorEvent(EnumErrors.fileNameError);
                return null;
            }

            FileNameSaleModel fnsm = FileNameSaleModel.CreateInstance(filePath, fileNameData);
            if (fnsm == null) 
            {
                OnErrorEvent(EnumErrors.fileNameError);
                return null; 
            }


            if (CheckIfDataSaved(fnsm.FileName))
            {
                OnErrorEvent(EnumErrors.fileWasSaved);
                return null;
            }
            return fnsm;
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
                    FileName fileName = GetFileNameFromDB(repo, fName);
                    sales = GetTmpSalesData<Sale>(repo, fileName);
                }
                catch (Exception) { }
            }
            return sales != null;
        }


        /// <summary>
        /// Get filename from DB
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="fname"></param>
        /// <returns></returns>
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


        #endregion // PARSING






        #region ON_THIS_EVENTS
        //############################################################################################################


        /// <summary>
        /// Thread work completed event
        /// </summary>
        /// <param name="abort">true - if thread was being aborted</param>
        /// <param name="deleteTmpData">true - if data was saved to database</param>
        private void OnWorkCompleting(bool abort)
        {
            WorkCompleted?.Invoke(this, abort);
        }

        /// <summary>
        /// File Content Error Event
        /// </summary>
        private void OnErrorEvent(EnumErrors error, int n = 0)
        {
            bool deleteTmpData = true;
            switch (error)
            {
                case EnumErrors.fileError:
                    ErrorEvent?.Invoke(this, "Error File");
                    break;
                case EnumErrors.fileNameError:
                    ErrorEvent?.Invoke(this, "Error file name");
                    break;
                case EnumErrors.fileContentError:
                    ErrorEvent?.Invoke(this, "Error file content");
                    break;
                case EnumErrors.managerError:
                    ErrorEvent?.Invoke(this, "Error getting manager information");
                    break;
                case EnumErrors.productError:
                    ErrorEvent?.Invoke(this, "Error getting product information");
                    break;
                case EnumErrors.dateError: 
                    ErrorEvent?.Invoke(this, "Error DateTime information"); 
                    break;
                case EnumErrors.costError: 
                    ErrorEvent?.Invoke(this, "Error cost converting"); 
                    break;
                case EnumErrors.fileWasSaved: 
                    ErrorEvent?.Invoke(this, "File was saved earlier");
                    deleteTmpData = false;
                    break;
                case EnumErrors.saveToDbError:
                    string[] msg =
                    {
                        "The amount of data processed is not equal to the amount of recorded data",
                        "Error saving parsed field to DB",
                        "Error saving data from temporary table",
                        "Error deleting temporary data after saving",
                        "Error accessing database while saving temporary data",
                        "Error deleting temporary data from the database",
                        "Error accessing database while deleting temporary data"
                    };
                    ErrorEvent?.Invoke(this, msg[n]);
                    break;
                default: return;
            }            
            if (deleteTmpData) { DeleteTmpData(this._fnsm); }
            Stop();
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
                    OnErrorEvent(EnumErrors.saveToDbError, 1); 
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
            switch (error)
            {
                case FileParser.Enums.EnumErrors.fileError:         OnErrorEvent(EnumErrors.fileError);          break;
                case FileParser.Enums.EnumErrors.fileNameError:     OnErrorEvent(EnumErrors.fileNameError);      break;
                case FileParser.Enums.EnumErrors.fileContentError:  OnErrorEvent(EnumErrors.fileContentError);   break;
                case FileParser.Enums.EnumErrors.fileParseError:    OnErrorEvent(EnumErrors.fileContentError);   break;
                default: return;
            }
            Stop();
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
                        OnErrorEvent(EnumErrors.saveToDbError, 2);
                    }
                    if (!repo.Deletes(tmpSales))
                    {
                        OnErrorEvent(EnumErrors.saveToDbError, 3);
                    }
                }
            }
            catch (Exception ex)
            {
                OnErrorEvent(EnumErrors.saveToDbError, 4);
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
                        OnErrorEvent(EnumErrors.saveToDbError, 5);
                    }
                }
            }
            catch (Exception ex)
            {
                OnErrorEvent(EnumErrors.saveToDbError, 6);
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



        /// <summary>
        /// Get form DB
        /// </summary>
        /// <typeparam name="T">EntityBase class</typeparam>
        /// <param name="repo">Repository</param>
        /// <param name="name">finding name</param>
        /// <param name="checkBD">receive only from the database, do not create new ones</param>
        /// <returns></returns>
        private T GetFromDB<T>(Repository repo, string name, bool checkBD = false) where T : EntityBase, new()
        {
            if (repo == null || string.IsNullOrWhiteSpace(name)) { return null; }

            T data = repo.Select<T>().FirstOrDefault(x => x.Name.Equals(name));
            if (checkBD && data == null)
            {
                var error = (typeof(T) == typeof(Manager))
                        ? EnumErrors.managerError
                        : EnumErrors.productError;
                OnErrorEvent(error);
                return null;
            }
            return data ?? new T() { Name = name };
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
        private TmpSale ConvertToSale(Repository repo, IDictionary<string, string> parsedData)
        {
            if (parsedData == null
                || string.IsNullOrWhiteSpace(parsedData["Client"])
                || string.IsNullOrWhiteSpace(parsedData["Cost"])
                || string.IsNullOrWhiteSpace(parsedData["DTG"])
                || string.IsNullOrWhiteSpace(parsedData["Product"])
                )
            {
                OnErrorEvent(EnumErrors.fileContentError);
                return null; 
            }

            TmpSale sale = new TmpSale();

            try { sale.Sum = Convert.ToInt32(parsedData["Cost"]); }
            catch (Exception) { OnErrorEvent(EnumErrors.costError); }

            try
            {                
                sale.DTG = DateTime.ParseExact(parsedData["DTG"], "dd.MM.yyyy HH:mm:ss"
                           , System.Globalization.CultureInfo.CurrentCulture);
                if (sale.DTG.Date != this._fnsm.DTG.Date) { throw new Exception(); }
            }
            catch (Exception) { OnErrorEvent(EnumErrors.dateError); }

            sale.Manager = GetFromDB<Manager>(repo, this._fnsm.Manager, this._checkManagersDB);
            sale.Client = GetFromDB<Client>(repo, parsedData["Client"]);
            sale.Product = GetFromDB<Product>(repo, parsedData["Product"], this._checkProductsDB);
            if (sale.Product.Cost == 0) { sale.Product.Cost = sale.Sum; }
            sale.FileName = GetFromDB<FileName>(repo, this._fnsm.FileName);
            if (sale.FileName.DTG == new DateTime()) { sale.FileName.DTG = this._fnsm.DTG; }

            if (this.Abort == true) { sale = null; }
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
