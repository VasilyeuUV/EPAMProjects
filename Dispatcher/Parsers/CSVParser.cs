using FileParser.Models;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FileParser.Parsers
{
    public class CSVParser
    {
        private bool _abort = false;   // for stop parsing

        public event EventHandler<SalesFieldDataModel> HeaderParsed;
        public event EventHandler<SalesFieldDataModel> FieldParsed;
        public event EventHandler<bool> ParsingCompleted;
        public event EventHandler ErrorParsing;


        /// <summary>
        /// CSV file parsing process
        /// </summary>
        /// <param name="filePath">CSV file</param>
        /// <param name="delimiters">array of delimiters (string[])</param>
        /// <returns>string array of string fields</returns>
        public IEnumerable<SalesFieldDataModel> Parse(string filePath, string[] delimiters = null )
        {
            if (!System.IO.File.Exists(filePath)) { return null; }

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

                        SalesFieldDataModel sale = null;
                        if (fields.Length == 4)
                        {
                            sale = new SalesFieldDataModel()
                            {
                                DTG = fields[0],
                                Client = fields[1],
                                Product = fields[2],
                                Cost = fields[3]
                            };
                        }
                        else 
                        {
                            ErrorParsing(this, EventArgs.Empty);
                            return null;
                        }

                        if (++count == 1) 
                        {
                            HeaderParsed(this, sale);
                            continue;
                        }
                        else { FieldParsed(this, sale); }
                        lstFields.Add(sale);
                    }
                    ParsingCompleted(this, this._abort);
                    return lstFields.Count() < 1 ? null : lstFields;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void Stop()
        {
            this._abort = true;
        }

    }
}
