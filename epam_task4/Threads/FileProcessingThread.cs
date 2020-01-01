using epam_task4.Models;
using FileParser.Models;
using FileParser.Parsers;
using System;
using System.Collections.Generic;
using System.Threading;

namespace epam_task4.Threads
{
    internal class FileProcessingThread
    {
        private Thread _thread = null;
        private bool _abort = false;

        private SalesFileNameDataModel _fileNameData = null;
        CSVParser _csvParser = null;


        private bool _checkProductsDB = false;        
        private bool _checkManagersDB = false;

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
            this._thread = new Thread(this.RunProcess);
        }


        /// <summary>
        /// Close this thread
        /// </summary>
        internal void Close()
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
            System.IO.FileInfo fileInf = new System.IO.FileInfo((string)obj);
            if (!fileInf.Exists) 
            { 
                FileNamingErrorEvent(this, EventArgs.Empty);
                return;
            }

            this._fileNameData = SalesFileNameDataModel.CreateInstance(fileInf);
            if (this._fileNameData == null
                || string.IsNullOrWhiteSpace(this._fileNameData.Manager)
                || this._fileNameData.DTG == new DateTime())
            {
                FileNamingErrorEvent(this, EventArgs.Empty);
                return;
            }



            this._csvParser = new CSVParser();
            this._csvParser.ParsingCompleted += CsvParser_ParsingCompleted;
            this._csvParser.HeaderParsed += CsvParser_HeaderParsed;
            this._csvParser.FieldParsed += CsvParser_FieldParsed;
            IEnumerable<SalesFieldDataModel> tableRows = this._csvParser.Parse(fileInf.FullName);



            WorkCompleted(this, this._abort);
        }



        /// <summary>
        /// Table row parsed event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CsvParser_FieldParsed(object sender, SalesFieldDataModel e)
        {
            SaleModel sale = new SaleModel();
            
            sale.Manager = this._fileNameData.Manager;           
            sale.SaleFileName = this._fileNameData.FileName;

            try
            {
                sale.SaleCost = Convert.ToInt32(e.Cost);
                sale.SaleDate = DateTime.ParseExact(e.DTG, "dd.MM.yyyy HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                if (sale.SaleDate.Date != this._fileNameData.DTG.Date) { throw new Exception(); }
            }
            catch (Exception)
            {
                FileContentErrorEvent(this, EventArgs.Empty);
                return;
            }
            sale.Product = e.Product;
            sale.Client = e.Client;

            if (SaveFieldData(sale))
            {
                // remove file
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
        /// Parse completed Event Handler 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CsvParser_ParsingCompleted(object sender, bool e)
        {
            // Command to Save data in DB
        }



        private bool SaveFieldData(SaleModel sale)
        {
            return false;
        }


    }
}
