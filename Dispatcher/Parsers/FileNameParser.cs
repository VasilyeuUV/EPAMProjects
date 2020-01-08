using System.Collections.Generic;

namespace Dispatcher.Parsers
{
    public class FileNameParser
    {               

        /// <summary>
        /// File name Parser
        /// </summary>
        /// <param name="filePath">file path</param>
        /// <param name="dataStruct"></param>
        /// <param name="delimiters"></param>
        /// <returns></returns>
        internal IDictionary<string, string> Parse(string filePath, string[] dataStruct, char[] delimiters = null)
        {
            if (string.IsNullOrWhiteSpace(filePath) || dataStruct?.Length < 1) { return null; }
            if (delimiters?.Length < 1) { delimiters = new[] { '_' }; }

            string[] fields = filePath.Split(delimiters);
            if (fields.Length < dataStruct.Length) { return null; }

            IDictionary<string, string> fileNameData = new Dictionary<string, string>();
            for (int i = 0; i < dataStruct.Length; i++)
            {
                fileNameData.Add(dataStruct[i], fields[i]);
            }

            return fileNameData.Count < 1 ? null : fileNameData;
        }
    }
}