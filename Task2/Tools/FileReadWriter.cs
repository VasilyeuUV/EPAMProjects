using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Task2.Tools
{
    internal static class FileReadWriter
        {

        #region SAVE_FILE

        internal static async void WriteTxtFileAsync(string text)
        {
            //Stream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt";//|All files (*.*)|*.*";
            //saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filename = saveFileDialog1.FileName;
                using (StreamWriter SR = new StreamWriter(filename))
                {
                    await SR.WriteAsync(text);
                }
            }
            MessageBox.Show("Файл сохранен");
        }





        #endregion




        #region READ_STREAM
        //###############################################################################################################################

        /// <summary>
        /// Get text from file next types: *.txt (...)
        /// </summary>
        /// <returns></returns>
        internal static string ReadFile(Stream fileStream)
            {

                // variants of read file content by it's extentions
                FileStream fs = fileStream as FileStream;
                string extension = (Path.GetExtension(fs.Name)).ToLower();
                switch (extension)
                {
                    case ".txt":
                        return ReadTextFile(fileStream);
                    default:
                        break;
                }
                return String.Empty; ;
            }


            /// <summary>
            /// Get file content from text file
            /// </summary>
            /// <param name="fileStream">file stream</param>
            /// <returns>string file content</returns>
            private static string ReadTextFile(Stream fileStream)
            {
                if (fileStream == null) { return String.Empty; }

                string fileContent = string.Empty;
                using (StreamReader reader = new StreamReader(fileStream, System.Text.Encoding.Default))
                {
                    try
                    {
                        fileContent = reader.ReadToEnd();
                    }
                    catch (Exception)
                    {
                        fileContent = ReadTextLines(fileStream);
                    }
                }
                return fileContent;
            }


            /// <summary>
            /// Read text by lines
            /// </summary>
            /// <param name="fileStream">File Stream</param>
            /// <returns>string all text by line</returns>
            private static string ReadTextLines(Stream fileStream)
            {
                if (fileStream == null) { return String.Empty; }

                StringBuilder fileContent = null;
                try
                {
                    using (StreamReader reader = new StreamReader(fileStream, System.Text.Encoding.Default))
                    {
                        while (reader.Peek() >= 0)
                        {
                            if (fileContent == null) { fileContent = new StringBuilder(); }
                            fileContent.Append(reader.ReadLine());
                            fileContent.Append(string.Format("\r\n"));
                        }
                    }
                }
                catch (Exception)
                {
                    fileContent.Append(string.Format("\r\nОшибка чтения файла!"));
                }
                return (fileContent == null) || (fileContent.Length < 1) ? string.Empty : fileContent.ToString();
            }


            #endregion // READ_STREAM
    }
}
