namespace Talabat.APIs.Helper
{
    public class FileSettings
    {
        public static string UploadFile(IFormFile file, string FolderName)
        {
            // 1.Get Located Folder path
            string FolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", FolderName);

            // 2.Get File Name, Guid adds string in the beginning to make it unique
            string FileName = $"{Guid.NewGuid()}{file.FileName}";

            // 3.Creating File Path
            string FilePath = Path.Combine(FolderPath, FileName);

            // 4.Save File As Streams
            using (var sf = new FileStream(FilePath, FileMode.Create))
                file.CopyTo(sf);

            string name = $"images/{FolderName}/{FileName}";

            return name;
        }

        public static void DeleteFile(string FileName, string FolderName)
        {
            var FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", FolderName, FileName);
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
        }
    }

}
