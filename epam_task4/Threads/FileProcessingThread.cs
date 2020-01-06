using efdb.DataContexts;
using efdb.DataModels;
using epam_task4.Models;
using FileParser.Models;
using FileParser.Parsers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;

namespace epam_task4.Threads
{
    internal class FileProcessingThread
    {        
        private Thread _thread = null;
        private bool _abort = false;

        private SalesFileNameDataModel _fileNameData = null;
        CSVParser _csvParser = null;


        private readonly bool _checkProductsDB;        
        private readonly bool _checkManagersDB;

        internal string Name { get; private set; }

        internal event EventHandler<bool> WorkCompleted;
        internal event EventHandler FileNamingErrorEvent;
        internal event EventHandler WrongProductErrorEvent;
        internal event EventHandler FileContentErrorEvent;


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
        /// Stop this thread
        /// </summary>
        internal void Stop()
        {
            this._csvParser?.Stop();
            this._abort = true;            
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
        /// Manager job process
        /// </summary>
        /// <param name="obj">product list</param>
        private void RunProcess(object obj)
        {
            string filePath = (string)obj;
            FileInfo fileInf = new FileInfo(filePath);
            if (!fileInf.Exists) { FileNamingErrorEvent(this, EventArgs.Empty); return; }

            this._thread.Name = fileInf.Name;
            this.Name = fileInf.Name;

            this._fileNameData = GetFileNameData(fileInf);
            if (this._fileNameData == null) { FileNamingErrorEvent(this, EventArgs.Empty); return; }

            FileName fileName = null;
            using (var repo = new Repository())
            {
                fileName = repo.Select<FileName>().FirstOrDefault(x => x.Name.Equals(this._fileNameData.FileName));                
            }
            if (fileName != null) { WorkCompleted(this, this._abort); return; }

            IEnumerable<SalesFieldDataModel> tableRows = RunCSVParser(fileInf);

            WorkCompleted(this, this._abort);
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



        /// <summary>
        /// Convert parsedData to Sale object within the repository
        /// </summary>
        /// <param name="repo">Current repository</param>
        /// <param name="parsedData">parsed Data</param>
        /// <returns></returns>
        private Sale ConvertToSale(Repository repo, SalesFieldDataModel parsedData)
        {
            if (parsedData == null
                || string.IsNullOrWhiteSpace(parsedData.Client)
                || string.IsNullOrWhiteSpace(parsedData.Cost)
                || string.IsNullOrWhiteSpace(parsedData.DTG)
                || string.IsNullOrWhiteSpace(parsedData.Product)
                )
            { return null; }

            Sale sale = new Sale();
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



        private bool SaveFieldData(SaleModel sale)
        {
            return false;
        }




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
                Sale sale = ConvertToSale(repo, parsedData);
                if (sale == null) { return; }

                if (!repo.Insert(sale))
                {
                    Console.WriteLine("Error saving data");
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
            // throw new NotImplementedException();
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

            if (abort)
            {
                // delete data from temporary table
            }
            else
            {
                // save data to main table
            }
        }




        #endregion // CSV_PARSER_EVENTS











    }
}
