using System.IO;

namespace ShampanPOS.uploads
{
    public class UploadHandler
    {
        public string Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return "No file uploaded.";
            }
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            string fileExtension = Path.GetExtension(file.FileName).ToLower();
            List<string> validExtensions = new List<string>() { ".jpg", ".jpeg", ".png", ".gif" };

            if (!validExtensions.Contains(fileExtension))
            {
                return $"Extension is not valid ({string.Join(',', validExtensions)})";
            }
            if (file.Length > (5 * 1024 * 1024))
            {
                return "Maximum size can be 5 MB";
            }
            string fileName = Guid.NewGuid().ToString() + fileExtension;
            string filePath = Path.Combine(uploadsFolder, fileName);
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return fileName; 
        }
        //public string Upload(IFormFile file)
        //{
        //    //extention
        //    List<string> validExtentions = new List<string>() { ".jpg", ".png", ".gif" }; string extention = Path.GetExtension(file.FileName);
        //    if (!validExtentions.Contains(extention))
        //    {
        //        return $"Extention is not valid ({string.Join(',', validExtentions)})";
        //    }
        //    //file size
        //    long size = file.Length;
        //    if (size > (5 * 1024 * 1024))
        //        return "Maximum size can be 5mb";
        //    //name changing
        //    string fileName = Guid.NewGuid().ToString() + extention;
        //    string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        //    using FileStream stream = new FileStream(Path.Combine(path,fileName) + fileName, FileMode.Create);
        //    file.CopyTo(stream);
        //    return fileName;
        //}
    }
}
