using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Maxim.Helpers
{
    public static class FileManager
    {
        public static string Save(string root, string folder, IFormFile file)
        {
            var guid = Guid.NewGuid().ToString();

            string newFileName = guid + (file.FileName.Length > 64 ? file.FileName.Substring(file.FileName.Length - 64, 64) :file.FileName);

            string path = Path.Combine(root, folder, newFileName);

            using(FileStream stream = new FileStream(newFileName, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return newFileName;
        }

        public static void Delete(string root, string folder, string fileName)
        {
            string path = Path.Combine(root, folder, fileName);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
