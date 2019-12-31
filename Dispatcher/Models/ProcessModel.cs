using Dispatcher.Models;
using System.IO;

namespace FileParser.Models
{
    internal class ProcessModel
    {
        private FileInfo _filePath = null;
        internal FileInfo FilePath { get => _filePath; }

        /// <summary>
        /// CTOR
        /// </summary>
        private ProcessModel(string filePath)
        {
            this._filePath = new FileInfo(filePath);
        }

        

        internal static ProcessModel CreateProcess(string filePath)
        {
            if (!File.Exists(filePath)) { return null; }
            return new ProcessModel(filePath);
        }

        internal void Start()
        {
            string fileName = this._filePath.Name.ToLower();
            FileNameDataModel fileNameData = FileNameDataModel.CreateInstance(fileName);

            // Parsing file context
            switch (Path.GetExtension(fileName))
            {
                case ".csv":                    
                    break;
                default:
                    break;
            }
        }


    }
}
