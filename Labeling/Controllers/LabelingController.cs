using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
                DocumentList? bBox = new DocumentList();
                DocumentList? kPoint = new DocumentList();
                if (data.string_boundingBoxes != null)
                {
                    string escapedJson = data.string_boundingBoxes;
                    string json = escapedJson.Replace("\"", "\"");
                    bBox = JsonConvert.DeserializeObject<DocumentList>(json);
                }
                if (data.string_keypoints != null)
                {
                    string escapedJson = data.string_keypoints;
                    string json = escapedJson.Replace("\"", "\"");
                    kPoint = JsonConvert.DeserializeObject<DocumentList>(json);
                }

                Document content = new Document()
                {
                    ObjectId = "" + data.imageName,
                    classindex = "0",
                    imageWidth = data.imageWidth,
                    imageHeight = data.imageHeight,
                    boundingBoxes = bBox,
                    keypoints = kPoint,
                };
                DB.History.Insert(content);
            }
            return response;
        }
        [HttpGet("downloadlabelfile")]
        public IActionResult DownloadLabelFile(string fileName, string username)
        {
            var filePath = Path.Combine(PathParam.Instance.LabelsPath, username , fileName + ".txt");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "text/plain", fileName);
        }
        [HttpPost("getimages")]
        public async Task<IActionResult> GetImages(Document doc)
        {
            if(doc.userName != null && doc.imageName != null)
            {
                var imageDirectory = Path.Combine(PathParam.Instance.ImagesPath, doc.userName);

                if (!Directory.Exists(imageDirectory))
                {
                    return NotFound("Directory not found.");
                }

                var imagePath = Path.Combine(imageDirectory, doc.imageName + ".jpg");

                if (!System.IO.File.Exists(imagePath))
                {
                    return NotFound("File not found.");
                }

                var imageBytes = await System.IO.File.ReadAllBytesAsync(imagePath);
                return File(imageBytes, "image/jpeg", doc.imageName);
            }
            return NotFound("Directory not found or File not found");
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
        public string? userName { get => GetString(nameof(userName)); set => Push(nameof(userName), value); }
        public DocumentList? boundingBoxes { get => GetArray<DocumentList>(nameof(boundingBoxes)); set => Push(nameof(boundingBoxes), value); }
        public DocumentList? keypoints { get => GetArray<DocumentList>(nameof(keypoints)); set => Push(nameof(keypoints), value); }
        public string? string_boundingBoxes { get => GetString(nameof(string_boundingBoxes)); set => Push(nameof(string_boundingBoxes), value); }
        public string? string_keypoints { get => GetString(nameof(string_keypoints)); set => Push(nameof(string_keypoints), value); }

        public string? visible { get => GetString(nameof(visible)); set => Push(nameof(visible), value); }
        public string? x { get => GetString(nameof(x)); set => Push(nameof(x), value); }
        public string? y { get => GetString(nameof(y)); set => Push(nameof(y), value); }


    }
}
