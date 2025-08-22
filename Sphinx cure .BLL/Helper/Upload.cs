

using Microsoft.AspNetCore.Http;

namespace Sphinx_cure_.BLL.Helper
{
    public static class Upload
    {
        private static readonly string[] AllowedExtensions = { ".pdf", ".doc", ".docx" };

        public static string UploadFile(string FolderName, IFormFile File, string? subFolder = null)
        {
            try
            {
                var ext = Path.GetExtension(File.FileName).ToLower();
                if (!AllowedExtensions.Contains(ext))
                    throw new Exception("File type not allowed. Only PDF or Word files are allowed.");


                string FolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", FolderName);


                if (!string.IsNullOrEmpty(subFolder))
                    FolderPath = Path.Combine(FolderPath, subFolder);


                if (!Directory.Exists(FolderPath))
                    Directory.CreateDirectory(FolderPath);


                string FileName = Guid.NewGuid() + ext;
                string FinalPath = Path.Combine(FolderPath, FileName);

                using (var Stream = new FileStream(FinalPath, FileMode.Create))
                {
                    File.CopyTo(Stream);
                }

                string relativePath = string.IsNullOrEmpty(subFolder)
                    ? FileName
                    : Path.Combine(subFolder, FileName);

                return relativePath.Replace("\\", "/"); 
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string RemoveFile(string FolderName, string fileName)
        {
            try
            {
                string FullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", FolderName, fileName);
                if (File.Exists(FullPath))
                {
                    File.Delete(FullPath);
                    return "File Deleted";
                }
                return "File Not Found";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

    }
}
