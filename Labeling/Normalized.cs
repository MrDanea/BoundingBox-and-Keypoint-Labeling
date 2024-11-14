namespace Labeling
{
    public class Normalized
    {
        private double ImageWidth { get; set; }
        private double ImageHeight { get; set; }

        public double BoxCenterX { get; set; }
        public double BoxCenterY { get; set; }
        public double BoxWidth { get; set; }
        public double BoxHeight { get; set; }

        public double KeypointX { get; set; }
        public double KeypointY { get; set; }
        public List<Dictionary<string, double>> KeypointsList { get; set; }

        private double _normalizedBoxCenterX;
        private double _normalizedBoxCenterY;
        private double _normalizedBoxWidth;
        private double _normalizedBoxHeight;

        private List<Dictionary<string, double>> _normalizedKeypointList;
        private double _normalizedKeypointX => (KeypointX - (BoxCenterX - BoxWidth / 2)) / BoxWidth;
        private double _normalizedKeypointY => (KeypointY - (BoxCenterY - BoxHeight / 2)) / BoxHeight;

        public double NormalizedBoxCenterX { get { return Math.Round(BoxCenterX / ImageWidth, 6); } }
        public double NormalizedBoxCenterY { get { return Math.Round(BoxCenterY / ImageHeight, 6); } }
        public double NormalizedBoxWidth { get { return Math.Round(BoxWidth / ImageWidth, 6); } }
        public double NormalizedBoxHeight { get { return Math.Round(BoxHeight / ImageHeight, 6); ; } }
        public List<Dictionary<string, double>> NormalizedKeypointList
        {
            get { return _normalizedKeypointList; }
            set { value = NormalizedKeypoints(); }
        }
        public Normalized(double imageWidth, double imageHeight)
        {
            this.ImageWidth = imageWidth;
            this.ImageHeight = imageHeight;
        }

        public List<Dictionary<string, double>> NormalizedKeypoints()
        {
            List<Dictionary<string, double>> keyValuePairs = new List<Dictionary<string, double>>();
            foreach(var keypoint in KeypointsList)
            {
                double xp;
                double yp;
                if(keypoint.TryGetValue("x", out xp) && keypoint.TryGetValue("y", out yp))
                {
                    keyValuePairs.Add(new Dictionary<string, double>() 
                    {
                        {"x", xp },
                        {"y", yp }
                    });
                }
            }
            return keyValuePairs;
        }


    }

}
