using Microsoft.JSInterop;
using System.Security.Policy;

namespace Labeling
{
    public class FileExport
    {
        private readonly IJSRuntime _jsRuntime;

        private string? filename { get; set; } 
        private string? filepath { get; set; }
        private string? imageName { get; set; }
        private string? content { get; set; }
        public FileExport(string filename, string content, IJSRuntime jsRuntime) 
        {
            _jsRuntime = jsRuntime;
            this.filename = filename;
            this.content = content;
        }
        public async Task<bool> Export()
        {
            if (filename != null)
            {
                try
                {
                    var url = Path.Combine(PathParam.Instance.LabelsPath, filename + ".txt");
                    await File.WriteAllTextAsync(url, content);
                    log.writeLog("The file has been successfully written!");
                    return true;
                }
                catch (Exception ex)
                {
                    await _jsRuntime.InvokeVoidAsync("showlog", "Error: " + ex.Message);
                    log.writeLog("Export failed. " + ex);
                    return false;
                }
            }
            return false;
        }

    }
    public class FolderHandler
    {
        public static void CreateFolderAndCopyImages(string[] imagePaths, string targetFolderPath, string destinationFolderPath)
        {
            if (!Directory.Exists(targetFolderPath))
            {
                Directory.CreateDirectory(targetFolderPath);
            }

            foreach (string imagePath in imagePaths)
            {
                string fileName = Path.GetFileName(imagePath);
                string destinationPath = Path.Combine(targetFolderPath, fileName);

                try
                {
                    File.Copy(imagePath, destinationPath, overwrite: true);
                    Console.WriteLine($"Đã sao chép {fileName} vào {targetFolderPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi khi sao chép {fileName}: {ex.Message}");
                }
            }

            // Tạo thư mục chứa cả thư mục ảnh đã sao chép nếu chưa tồn tại
            if (!Directory.Exists(destinationFolderPath))
            {
                Directory.CreateDirectory(destinationFolderPath);
            }

            // Sao chép toàn bộ thư mục ảnh vào thư mục đích cuối cùng
            string finalDestinationPath = Path.Combine(destinationFolderPath, Path.GetFileName(targetFolderPath));

            try
            {
                CopyDirectory(targetFolderPath, finalDestinationPath);
                Console.WriteLine($"Đã sao chép toàn bộ thư mục vào {destinationFolderPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi sao chép thư mục: {ex.Message}");
            }
        }

        // Hàm sao chép toàn bộ thư mục và các file con
        private static void CopyDirectory(string sourceDir, string destinationDir)
        {
            // Tạo thư mục đích
            Directory.CreateDirectory(destinationDir);

            // Sao chép tất cả file trong thư mục nguồn
            foreach (string filePath in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(filePath);
                string destinationPath = Path.Combine(destinationDir, fileName);
                File.Copy(filePath, destinationPath, overwrite: true);
            }

            // Sao chép tất cả các thư mục con trong thư mục nguồn
            foreach (string directoryPath in Directory.GetDirectories(sourceDir))
            {
                string dirName = Path.GetFileName(directoryPath);
                string destinationSubDir = Path.Combine(destinationDir, dirName);
                CopyDirectory(directoryPath, destinationSubDir);
            }
        }

        public static void CopyTextFilesToDirectory(string[] textFilePaths, string destinationFolderPath)
        {
            // Tạo thư mục đích nếu chưa tồn tại
            if (!Directory.Exists(destinationFolderPath))
            {
                Directory.CreateDirectory(destinationFolderPath);
            }

            // Sao chép từng file .txt vào thư mục đích
            foreach (string filePath in textFilePaths)
            {
                // Lấy tên file (filename) từ đường dẫn
                string fileName = Path.GetFileName(filePath);
                string destinationPath = Path.Combine(destinationFolderPath, fileName);

                try
                {
                    File.Copy(filePath, destinationPath, overwrite: true);
                    Console.WriteLine($"Đã sao chép {fileName} vào {destinationFolderPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi khi sao chép {fileName}: {ex.Message}");
                }
            }
        }
    }

}

