using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.Error;
using DatingApp.Helpers.HO;
using DatingApp.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace DatingApp.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IConfiguration _config;

        public PhotoService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<PhotoResultHO> AddPhotoAsync(IFormFile photo, string host)
        {
            var image = Image.FromStream(photo.OpenReadStream());
            var resized = new Bitmap(image, new Size(1280, 720));

            if (image.PropertyIdList.Contains(0x0112))
            {
                var orientation = (int)image.GetPropertyItem(0x0112).Value[0];
                switch (orientation)
                {
                    case 2:
                        resized.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        break;
                    case 3:
                        resized.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;
                    case 4:
                        resized.RotateFlip(RotateFlipType.Rotate180FlipX);
                        break;
                    case 5:
                        resized.RotateFlip(RotateFlipType.Rotate90FlipX);
                        break;
                    case 6:
                        resized.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                    case 7:
                        resized.RotateFlip(RotateFlipType.Rotate270FlipX);
                        break;
                    case 8:
                        resized.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                }
            }

            await using var imageStream = new MemoryStream();
            resized.Save(imageStream, ImageFormat.Jpeg);
            var imageBytes = imageStream.ToArray();

            var path = Path.Combine(Directory.GetCurrentDirectory(), _config["PhotosStorage"]);

            var prho = new PhotoResultHO();
            string filename = "";

            try
            {
                do
                {
                    filename = new Random().Next(100000000, 1000000000).ToString();
                } while (FileNameIsExist(filename, path));
            }
            catch (DirectoryNotFoundException ex)
            {
                prho.Error = new ApiException(500 ,ex.Message);
            }

            filename += ".jpg";

            var pathToSave = Path.Combine(path, filename);

            await File.WriteAllBytesAsync(pathToSave, imageBytes);

            prho.PhotoPath = host + "/photos/" + filename;

            return prho;
        }

        public string GetPhoto(string fileName)
        {
            var file = GetFile(fileName);

            return file.FullName;
        }

        public bool DeletePhoto(string fileName)
        {
            var file = GetFile(fileName);

            if (file == null) return false;

            

            File.Delete(file.FullName);

            return true;
        }

        private FileInfo GetFile(string fileName)
        {
            var directory = _config["PhotosStorage"];

            try
            {
                if (!FileNameIsExist(fileName, directory)) return null;
            }
            catch (DirectoryNotFoundException)
            {
                return null;
            }

            var dir = new DirectoryInfo(directory);

            var file = dir.GetFiles().FirstOrDefault(x => x.Name.Contains(fileName));

            return file;
        }

        private bool FileNameIsExist(string fileName, string path)
        {
            var directory = new DirectoryInfo(path);

            if (!directory.Exists) throw new DirectoryNotFoundException();

            return directory.GetFiles().Any(x => x.Name.Contains(fileName));
        }
    }
}
