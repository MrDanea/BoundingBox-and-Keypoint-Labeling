using System;
using System.IO;
using System.IO.Compression;
namespace Labeling
{
    public class DatasetZipBuilder
    {
        private string sourceImagesPath;
        private string sourceLabelsPath;
        private string tempFolderPath;
        private string zipFilePath;

        public DatasetZipBuilder()
        {
            // Tạo thư mục tạm để xây dựng cấu trúc dữ liệu trước khi nén
            //tempFolderPath = Path.Combine(Path.GetTempPath(), "datasets_temp");
            tempFolderPath = "Storage\\datasets\\";
            //zipFilePath = Path.Combine(Directory.GetCurrentDirectory(), "datasets.zip");
            zipFilePath = "Storage\\Zip\\";
        }

        // Thiết lập đường dẫn chứa ảnh nguồn
        public DatasetZipBuilder SetSourceImagesPath(string imagesPath)
        {
            sourceImagesPath = imagesPath;
            return this;
        }

        // Thiết lập đường dẫn chứa file label nguồn
        public DatasetZipBuilder SetSourceLabelsPath(string labelsPath)
        {
            sourceLabelsPath = labelsPath;
            return this;
        }

        // Tạo cấu trúc thư mục cần thiết trong thư mục tạm
        private void CreateDatasetStructure()
        {
            // Tạo thư mục gốc datasets, images, labels
            Directory.CreateDirectory(Path.Combine(tempFolderPath, "datasets", "images"));
            Directory.CreateDirectory(Path.Combine(tempFolderPath, "datasets", "labels"));
        }

        // Sao chép ảnh vào thư mục images trong thư mục tạm
        private void CopyImages()
        {
            var imagesDestination = Path.Combine(tempFolderPath, "datasets", "images");
            FolderHandler.CopyTextFilesToDirectory(Directory.GetFiles(sourceImagesPath, "*.jpg"), imagesDestination); // Copy tất cả ảnh JPG từ source
        }

        // Sao chép file label vào thư mục labels trong thư mục tạm
        private void CopyLabels()
        {
            var labelsDestination = Path.Combine(tempFolderPath, "datasets", "labels");
            FolderHandler.CopyTextFilesToDirectory(Directory.GetFiles(sourceLabelsPath, "*.txt"), labelsDestination); // Copy tất cả file TXT từ source
        }

        // Xây dựng file ZIP cuối cùng
        public void Build()
        {
            // Xóa file zip cũ nếu có
            if (File.Exists(zipFilePath))
            {
                File.Delete(zipFilePath);
            }

            // Tạo cấu trúc thư mục và sao chép file vào
            CreateDatasetStructure();
            //CopyImages();
            CopyLabels();

            // Tạo file zip từ thư mục tạm
            ZipFile.CreateFromDirectory(tempFolderPath, zipFilePath);
            Console.WriteLine("File zip đã được tạo thành công tại: " + zipFilePath);

            // Xóa thư mục tạm
            Directory.Delete(tempFolderPath, true);
        }
    }

}
