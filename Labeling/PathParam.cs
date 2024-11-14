namespace Labeling
{
    public class PathParam
    {
        private string _dbpath = "";
        private string _imagespath = "";
        private string _labelpath = "";
        private string _zippath = "";
        public string DBPath { get { return _dbpath; } }
        public string ImagesPath { get { return _imagespath; } }
        public string LabelsPath { get { return _labelpath; } }
        public string ZipPath { get { return _zippath; } }
        private static PathParam? _instance;
        public static PathParam Instance { 
            get 
            { 
                if(_instance == null)
                {
                    _instance = new PathParam();
                }
                return _instance;
            } 
        }

        public PathParam() 
        {
            this._dbpath = Environment.CurrentDirectory;
            this._imagespath = Path.Combine(Path.GetTempPath(), "images");
            this._labelpath = Path.Combine(Path.GetTempPath(), "labels");
            this._zippath = Path.Combine(Path.GetTempPath(), "zip)");
        }

    }
}
