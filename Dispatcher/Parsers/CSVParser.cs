using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FileParser.Parsers
{
    public class CSVParser
    {
        private bool _abort = false;   // for stop parsing

        public event EventHandler<IDictionary<string, string>> FieldParsed;
        public event EventHandler<bool> ParsingCompleted;
        public event EventHandler ErrorParsing;

        private void OnErrorParsing()
        {            
            ErrorParsing?.Invoke(this, EventArgs.Empty);  
            Stop();            
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
        public IEnumerable<IDictionary<string, string>> Parse(string filePath, string[] delimiters = null )
        {
            System.IO.FileInfo fileInf = new System.IO.FileInfo(filePath);
            if (!fileInf.Exists || fileInf.Extension.ToLower() != ".csv") 
            {
                OnErrorParsing();
                ParsingCompleted?.Invoke(this, this._abort);
                return null;
            }

            if (delimiters == null) { delimiters = new[] { "," }; }

            List<IDictionary<string, string>> lstFields = new List<IDictionary<string, string>>();
            try
            {
                using (TextFieldParser tfp = new TextFieldParser(filePath))
                {
                    tfp.TextFieldType = FieldType.Delimited;
                    tfp.SetDelimiters(delimiters);

                    int count = 0;
                    string[] fieldNames = new string[0];
                    while (!tfp.EndOfData && !this._abort)
                    {
                        string[] fields = tfp.ReadFields();

                        if (++count == 1) { fieldNames = fields; }
                        else
                        {
                            IDictionary<string, string> dicField = ParseFields(fieldNames, fields);
                            if (dicField?.Count() < 1) { OnErrorParsing(); }
                            else { lstFields.Add(dicField); }                            
                        }
                    }
                    ParsingCompleted?.Invoke(this, this._abort);
                    if (this._abort) { lstFields = null; }
                    return lstFields?.Count() < 1 ? null : lstFields;
                }
            }
            catch (Exception)
            {
                OnErrorParsing();
                return null;
            }
        }

        /// <summary>
        /// Parse fields
        /// </summary>
        /// <param name="fieldNames">Columns names</param>
        /// <param name="fields">Columns values</param>
        /// <returns></returns>
        private IDictionary<string, string> ParseFields(string[] fieldNames, string[] fields)
        {
            if (fieldNames?.Length < 1 
                || fields?.Length < 1 
                || fieldNames.Length != fields.Length)
            {
                OnErrorParsing();
                return null;
            }

            IDictionary<string, string> dicField = new Dictionary<string, string>();
            for (int i = 0; i < fields.Length; i++)
            {
                dicField.Add(fieldNames[i], fields[i]);
            }
            FieldParsed?.Invoke(this, dicField);

            return dicField;
        }
    }
}
