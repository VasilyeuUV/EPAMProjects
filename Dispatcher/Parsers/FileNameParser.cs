﻿using Dispatcher.Interfaces;
using Dispatcher.Models;
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
                    case 0: return ManagerFileNameDataModel.CreateInstance(fileInf);
                    default: return null;
                }
            }
            return null;
        }
    }
}
