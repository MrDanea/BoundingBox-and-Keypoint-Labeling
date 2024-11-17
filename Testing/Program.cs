// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

Document doc = new Document()
{
    x = "1",
    y = "2",
    Visible = "3"
};
DocumentList list = new DocumentList();
list.Add(doc);
list.Add(doc);
list.Add(doc);

Document doc1 = new Document()
{
    x = "1",
    y = "2",    
    keypoints = list,
};
Console.WriteLine(list.ToString());

namespace System
{
    public partial class Document
    {
        public DocumentList? keypoints { get => GetArray<DocumentList>(nameof(keypoints)); set => Push(nameof(keypoints), value); }
        public string? Visible { get => GetString(nameof(Visible)); set => Push(nameof(Visible), value); }
        public string? x { get => GetString(nameof(x)); set => Push(nameof(x), value); }
        public string? y { get => GetString(nameof(y)); set => Push(nameof(y), value); }
    }
}