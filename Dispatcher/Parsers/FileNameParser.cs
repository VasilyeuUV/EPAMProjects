using System.Collections.Generic;

namespace FileParser.Parsers
{
    public static class FileNameParser
    {
        private static object locker = new object();

        /// <summary>
        /// File name Parser
        /// </summary>
        /// <param name="filePath">file path</param>
        /// <param name="dataStruct"></param>
        /// <param name="delimiters"></param>
        /// <returns></returns>
        public static IDictionary<string, string> Parse(string filePath, string[] dataStruct, char[] delimiters = null)
        {
            lock (locker)
            {
                if (string.IsNullOrWhiteSpace(filePath) || dataStruct?.Length < 1) { return null; }
                if (delimiters == null || delimiters.Length < 1) { delimiters = new[] { '_' }; }

                System.IO.FileInfo fileInf = new System.IO.FileInfo(filePath);
                string[] fields = fileInf.Name.Split(delimiters);
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
}