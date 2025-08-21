

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
                // 1) تحقق من الامتداد
                var ext = Path.GetExtension(File.FileName).ToLower();
                if (!AllowedExtensions.Contains(ext))
                    throw new Exception("File type not allowed. Only PDF or Word files are allowed.");

                // 2) مجلد رئيسي
                string FolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", FolderName);

                // 3) مجلد فرعي إذا موجود (مثلاً لكل Patient ID)
                if (!string.IsNullOrEmpty(subFolder))
                    FolderPath = Path.Combine(FolderPath, subFolder);

                if (!Directory.Exists(FolderPath))
                    Directory.CreateDirectory(FolderPath);

                // 4) اسم الملف + GUID
                string FileName = Guid.NewGuid() + ext;
                string FinalPath = Path.Combine(FolderPath, FileName);

                // 5) حفظ الملف
                using (var Stream = new FileStream(FinalPath, FileMode.Create))
                {
                    File.CopyTo(Stream);
                }

                // 6) ارجاع المسار النسبي (FolderName/subFolder/FileName)
                string relativePath = string.IsNullOrEmpty(subFolder)
                    ? FileName
                    : Path.Combine(subFolder, FileName);

                return relativePath.Replace("\\", "/"); // تحويل / بدل \
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
