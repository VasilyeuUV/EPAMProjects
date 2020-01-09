using FileParser.Enums;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileParser.Parsers
{
    public class CSVParser
    {
        private const string FILE_EXTENTION = ".csv";
        private bool _abort = false;   // for stop parsing

        public event EventHandler<IDictionary<string, string>> FieldParsed;
        public event EventHandler<bool> ParsingCompleted;
        public event EventHandler<EnumErrors> ErrorParsing;

        private void OnErrorParsing(EnumErrors error)
        {            
            ErrorParsing?.Invoke(this, error);
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
            IEnumerable<IDictionary<string, string>> lstFields = null;
            FileInfo fileInf = new FileInfo(filePath);
            if (!fileInf.Exists || fileInf.Extension.ToLower() != FILE_EXTENTION) 
            {
                OnErrorParsing(EnumErrors.fileError);
            }
            else
            {
                if (delimiters == null || delimiters.Length < 1) { delimiters = new[] { "," }; }
                try
                {
                    lstFields = Parsing(delimiters, fileInf.FullName);
                }
                catch (Exception)
                {
                    OnErrorParsing(EnumErrors.fileParseError);
                }
            }

            ParsingCompleted?.Invoke(this, this._abort);
            return lstFields;
        }

        /// <summary>
        /// Parsing process
        /// </summary>
        /// <param name="delimiters"></param>
        /// <param name="filePath"></param>
        /// <param name="lstFields"></param>
        /// <returns></returns>
        private IEnumerable<IDictionary<string, string>> Parsing(string[] delimiters, string filePath)
        {
            int count = 0;
            string[] fieldNames = new string[0];
            List<IDictionary<string, string>> lstFields = new List<IDictionary<string, string>>();

            using (TextFieldParser tfp = new TextFieldParser(filePath))
            {
                tfp.TextFieldType = FieldType.Delimited;
                tfp.SetDelimiters(delimiters);
                
                while (!tfp.EndOfData && !this._abort)
                {
                    string[] fields = tfp.ReadFields();

                    if (++count == 1) { fieldNames = fields; }
                    else
                    {
                        IDictionary<string, string> dicField = ParseFields(fieldNames, fields);
                        if (dicField == null)
                        {
                            OnErrorParsing(EnumErrors.fileContentError);
                        }
                        else
                        {
                            lstFields.Add(dicField);
                            FieldParsed?.Invoke(this, dicField);
                        }
                    }
                }
                if (this._abort) { lstFields = null; }
            }
            return lstFields;
        }

        /// <summary>
        /// Parse fields
        /// </summary>
        /// <param name="fieldNames">Columns names</param>
        /// <param name="fields">Columns values</param>
        /// <returns></returns>
        private IDictionary<string, string> ParseFields(string[] fieldNames, string[] fields)
        {
            if (fieldNames == null || fieldNames.Length < 1 
                || fields == null || fields.Length < 1 
                || fieldNames.Length != fields.Length)
            {
                return null;
            }

            IDictionary<string, string> dicField = new Dictionary<string, string>();
            for (int i = 0; i < fields.Length; i++)
            {
                dicField.Add(fieldNames[i], fields[i]);
            }
            return dicField;
        }
    }
}
