using System.Drawing;

namespace Labeling
{
    public class Identify
    {
        public Identify() { }

        private bool _isIdentified = false;
        public bool IsIdentified
        {
            get 
            { 
                return _isIdentified; 
            }
        }
        private bool _isAdmin = false;
        public bool IsAdmin
        {
            get
            {
                return _isAdmin;
            }
        }
        private string _username { get; set; }
        public string Username { get { return _username; } }
        public void login(string username, string password) 
        {
            this._username = username;
            if (username == "admin" && password == "1") { this._isAdmin = true; _isIdentified = false; return; }
            if (DB.Identify != null)
            {
                if (DB.Identify.Find(username) != null)
                {
                    if (DB.Identify.Find(username).Password == password)
                    {
                        this._isAdmin = false; _isIdentified = true;
                    }
                }
            }
        }
        public void regis(string username, string password) 
        {
            if(DB.Identify.Find(username) == null)
            {
                DB.Identify.Insert(new Document()
                {
                    ObjectId = username,
                    UserID = username,
                    Password = password,
                });
                Directory.CreateDirectory(Path.Combine(PathParam.Instance.ImagesPath, username));
                Directory.CreateDirectory(Path.Combine(PathParam.Instance.LabelsPath, username));
                Directory.CreateDirectory(Path.Combine(PathParam.Instance.ZipPath, username));
            }
        }
    }
}
