using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dispatcher.Parsers
{
    internal class CSVParser
    {
        private bool _abort = false;   // for stop parsing

        internal event EventHandler<string[]> HeaderParsed;
        internal event EventHandler<string[]> FieldParsed;
        internal event EventHandler<bool> ParsingCompleted;

        /// <summary>
        /// CSV file parsing process
        /// </summary>
        /// <param name="filePath">CSV file</param>
        /// <param name="delimiters">array of delimiters (string[])</param>
        /// <returns>string array of string fields</returns>
        internal IEnumerable<string[]> Parse(string filePath, string[] delimiters = null )
        {
            if (!System.IO.File.Exists(filePath)) { return null; }

            if (delimiters == null) { delimiters = new[] { "," }; }            
            try
            {
                using (TextFieldParser tfp = new TextFieldParser(filePath))
                {
                    tfp.TextFieldType = FieldType.Delimited;
                    tfp.SetDelimiters(delimiters);

                    List<string[]> lstFields = new List<string[]>();
                    while (!tfp.EndOfData && !this._abort)
                    {
                        string[] fields = tfp.ReadFields();
                        if (fields.Length == 1) { HeaderParsed(this, fields); }
                        else { FieldParsed(this, fields); }
                        lstFields.Add(fields);
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

        internal void Stop()
        {
            this._abort = true;
        }

    }
}
