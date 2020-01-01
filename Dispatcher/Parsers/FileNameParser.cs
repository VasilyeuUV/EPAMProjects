using FileParcer.Interfaces;
using FileParser.Models;
using System.IO;

namespace Dispatcher.Parsers
{
    internal class FileNameParser
    {               

        internal IFileNameData Parse(string filePath, int returnData = 0)
        {
            if (string.IsNullOrWhiteSpace(filePath)) { return null; }

            FileInfo fileInf = new FileInfo(filePath);
            if (fileInf.Exists)
            {
                switch (returnData)
                {
                    case 0: return SalesFileNameDataModel.CreateInstance(fileInf);
                    default: return null;
                }
            }
            return null;
        }
    }
}
