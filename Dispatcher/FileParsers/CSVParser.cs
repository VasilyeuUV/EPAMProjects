using Microsoft.VisualBasic.FileIO;
using System;
using System.Linq;

namespace Dispatcher.FileParsers
{
    internal class CSVParser
    {
        private bool _abort = false;   // for stop parsing

        internal event EventHandler<string> HeaderParsed;
        internal event EventHandler<string> FieldParsed;
        internal event EventHandler<bool> ParsingCompleted;


        /// <summary>
        /// CSV file parsing process
        /// </summary>
        /// <param name="filePath">CSV file</param>
        /// <param name="delimiters">array of delimiters (string[])</param>
        /// <returns>string array of string fields</returns>
        internal string[] Parse(string filePath, string[] delimiters = null )
        {
            if (!System.IO.File.Exists(filePath)) { return null; }

            if (delimiters == null) { delimiters = new[] { "," }; }            
            try
            {
                using (TextFieldParser tfp = new TextFieldParser(filePath))
                {
                    tfp.TextFieldType = FieldType.Delimited;
                    tfp.SetDelimiters(delimiters);

                    string[] fields = null;
                    while (!tfp.EndOfData && !this._abort)
                    {
                        fields = tfp.ReadFields();
                        if (fields.Length == 1) { HeaderParsed(this, fields.Last()); }
                        else { FieldParsed(this, fields.Last()); }
                    }
                    tfp.Dispose();
                    ParsingCompleted(this, this._abort);
                    return fields;
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
