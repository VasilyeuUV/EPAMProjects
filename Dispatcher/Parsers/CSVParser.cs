using FileParser.Models;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileParser.Parsers
{
    public class CSVParser
    {
        private bool _abort = false;   // for stop parsing

        public event EventHandler<SalesFieldDataModel> HeaderParsed;
        public event EventHandler<SalesFieldDataModel> FieldParsed;
        public event EventHandler<bool> ParsingCompleted;
        public event EventHandler ErrorParsing;

        private void OnErrorParsing()
        {
            this._abort = true;
            ErrorParsing?.Invoke(this, EventArgs.Empty);
            this.Stop();
            ParsingCompleted?.Invoke(this, this._abort);
        }


        public void Stop()
        {
            this._abort = true;
        }


        /// <summary>
        /// CSV file parsing process
        /// </summary>
        /// <param name="filePath">CSV file</param>
        /// <param name="delimiters">array of delimiters (string[])</param>
        /// <returns>string array of string fields</returns>
        public IEnumerable<SalesFieldDataModel> Parse(string filePath, string[] delimiters = null )
        {
            if (!System.IO.File.Exists(filePath)) 
            { 
                ErrorParsing?.Invoke(this, EventArgs.Empty);
                return null;
            }

            if (delimiters == null) { delimiters = new[] { "," }; }            
            try
            {
                using (TextFieldParser tfp = new TextFieldParser(filePath))
                {
                    tfp.TextFieldType = FieldType.Delimited;
                    tfp.SetDelimiters(delimiters);

                    List<SalesFieldDataModel> lstFields = new List<SalesFieldDataModel>();
                    int count = 0;
                    while (!tfp.EndOfData && !this._abort)
                    {
                        string[] fields = tfp.ReadFields();

                        SalesFieldDataModel sfdm = CreateSFDM(fields);
                        if (sfdm == null) 
                        { 
                            OnErrorParsing();
                            return null;
                        }

                        if (++count == 1) { HeaderParsed?.Invoke(this, sfdm); continue; }
                        else { FieldParsed?.Invoke(this, sfdm); }

                        lstFields.Add(sfdm);
                    }
                    ParsingCompleted?.Invoke(this, this._abort);
                    return lstFields.Count() < 1 ? null : lstFields;
                }
            }
            catch (Exception ex)
            {
                OnErrorParsing();
                return null;
            }
        }

        /// <summary>
        /// Create new SFDM
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        private SalesFieldDataModel CreateSFDM(string[] fields)
        {
            if (fields == null || fields.Length != 4) { return null; }

            return new SalesFieldDataModel()
            {
                DTG = fields[0],
                Client = fields[1],
                Product = fields[2],
                Cost = fields[3]
            };
        }
    }
}
