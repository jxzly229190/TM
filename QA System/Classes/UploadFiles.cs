using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using QA_System.DataProvider;

namespace QA_System.Classes
{
    public class UploadFiles
    {
        internal DataBase db = new DataBase();

        // Retrieve the filer root location of Document directory
        public string DocumentsRootDirectory
        {
            get { return HttpContext.Current.Session["session_AttachmentDir"].ToString(); }
        }

        public void SaveUploadFiles(Stream stream, string documentPathAndFilename, out string fileExt, out Guid documentGuid)
        {
            documentGuid = Guid.NewGuid();
            fileExt = string.Empty;
             // Create the directory if it doesn't already exist
            CreateDirectory();
            string documentPath = DocumentsRootDirectory;
            // Get just the document Filename
            string documentFilename = documentGuid + Path.GetExtension(documentPathAndFilename);
            fileExt = GetFileExt(documentFilename);

            // Save the document data to disk
            this.Save(documentPath + documentFilename, stream);

            db.createFile(documentGuid, documentPathAndFilename, fileExt);
            // Return our document ID
        }

        // returns the file extension
        public static string GetFileExt(string fileName)
        {
            string fileExt = String.Empty;

            if (fileName.Contains("."))
                fileExt = fileName.Substring(fileName.LastIndexOf('.') + 1);

            return fileExt;
        }

        public void CreateDirectory()
        {
            if (!Directory.Exists(DocumentsRootDirectory))
                Directory.CreateDirectory(DocumentsRootDirectory);
        }

        // Save the file to disk
        public void Save(string documentPathAndFilename, Stream stream)
        {
            Stream s = stream;
            byte[] binaryData = new byte[s.Length];
            long bytesRead = s.Read(binaryData, 0, binaryData.Length);
            s.Close();

            using (FileStream fs = File.Create(documentPathAndFilename))
                fs.Write(binaryData, 0, binaryData.Length);
        }

        // Get a text file from disk
        public string Get(string documentPathAndFilename)
        {
            string data = null;

            try
            {
                StreamReader stream = new StreamReader(documentPathAndFilename);
                data = stream.ReadToEnd();
                stream.Close();
            }
            catch { }

            return data;
        }

       
    }
}