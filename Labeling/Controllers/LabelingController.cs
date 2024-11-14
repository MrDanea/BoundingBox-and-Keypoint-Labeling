using Microsoft.AspNetCore.Mvc;

namespace Labeling
{
    [Route("api/labeling")]
    [ApiController]
    public class LabelingController : ControllerBase
    {
        [HttpPost("RequestHandle")]
        public Document RequestHandle(List<Dictionary<string, double>> data)
        {
            Document response = new Document()
            {
                Status = "Success",
                Msg = "Data received successfully",
            };
            if (data == null)
            {
                response.Msg = "Fail";
                response.Status = "Error";
            }
            else
            {

            }
            return response.Msg;
        }
        [HttpPost("commitnupdate")]
        public ActionResult<Document> CommitnUpdate(Document data)
        {
            Document response = new Document()
            {
                Status = "Success",
                Msg = "Data received successfully",
            };
            if (data == null)
            {
                response.Msg = "Fail";
                response.Status = "Error";
            }
            else
            {
                DB.History.Insert(new Document()
                {
                    ObjectId = "20241211" + data.imageName,
                    classindex = "0",
                    width = data.width,
                    height = data.height,
                    centerX = data.centerX,
                    centerY = data.centerY,
                    imageWidth = data.imageWidth,
                    imageHeight = data.imageHeight,
                });
            }
            return response;
        }
        [HttpGet("downloadlabelfile")]
        public IActionResult DownloadLabelFile(string fileName)
        {
            string tempPath = Path.GetTempPath();
            var filePath = Path.Combine(tempPath, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "text/plain", fileName);
        }
        [HttpGet("getimages")]
        public async Task<IActionResult> GetImage(string doc)
        {
            var imageDirectory = Path.Combine(PathParam.Instance.ImagesPath, doc);

            if (!Directory.Exists(imageDirectory))
            {
                return NotFound("Directory not found.");
            }

            var imagePath = Path.Combine(imageDirectory, "635bdab2-image_6.jpg");

            if (!System.IO.File.Exists(imagePath))
            {
                return NotFound("File not found.");
            }

            var imageBytes = await System.IO.File.ReadAllBytesAsync(imagePath);
            return File(imageBytes, "image/jpeg", "635bdab2-image_6.jpg");
        }


    }
}
namespace System
{
    public partial class Document
    {
        public string? Status { get => GetString(nameof(Status)); set => Push(nameof(Status), value); }
        public string? Msg { get => GetString(nameof(Msg)); set => Push(nameof(Msg), value); }
        public List<Dictionary<string, double>>? ReceivedData { get => GetArray<List<Dictionary<string, double>>>(nameof(ReceivedData)) ; set => Push(nameof(ReceivedData), value); }
        public string? classindex { get => GetString(nameof(classindex)); set => Push(nameof(classindex), value); }
        public string? width { get => GetString(nameof(width)); set => Push(nameof(width), value); }
        public string? height { get => GetString(nameof(height)); set => Push(nameof(height), value); }
        public string? centerX { get => GetString(nameof(centerX)); set => Push(nameof(centerX), value); }
        public string? centerY { get => GetString(nameof(centerY)); set => Push(nameof(centerY), value); }
        public string? imageWidth { get => GetString(nameof(imageWidth)); set => Push(nameof(imageWidth), value); }
        public string? imageHeight { get => GetString(nameof(imageHeight)); set => Push(nameof(imageHeight), value); }
        public string? imageName { get => GetString(nameof(imageName)); set => Push(nameof(imageName), value); }

    }
}
