namespace System
{
    public partial class DB
    {
        public static void runDB()
        {
            string dir = Environment.CurrentDirectory;
            DB.AccountDB(dir + "/DataBase");
        }
    }
    public partial class DB
    {
        public static BsonData.MainDatabase Account { get; private set; }
        public static BsonData.Collection? Identify => Account.GetCollection(nameof(Identify));
        public static BsonData.Collection? History => Account.GetCollection(nameof(History));
        public static void AccountDB(string path)
        {
            Account = new BsonData.MainDatabase("AccountDB");
            Account.Connect(path);
            Account.StartStorageThread();
            if (DB.Identify != null)
            {
                if(DB.Identify.Find("admin") == null)
                {
                    DB.Identify.Insert(new Document() { ObjectId = "admin", UserID = "admin", Password = "1" });
                }
            }
        }
    }
    public partial class Document
    {
        public string ?UserID { get => GetString(nameof(UserID)); set => Push(nameof(UserID), value); }
        public string ?Password { get => GetString(nameof(Password)); set => Push(nameof(Password), value); }
        public string? StoragePath { get => GetString(nameof(StoragePath)); set => Push(nameof(StoragePath), value); }

    }
}
