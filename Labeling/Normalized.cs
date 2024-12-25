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
        public Dictionary<string, double> KeypointsList { get; set; }

        private double _normalizedBoxCenterX;
        private double _normalizedBoxCenterY;
        private double _normalizedBoxWidth;
        private double _normalizedBoxHeight;

        private Dictionary<string, double> _normalizedKeypointList;
        private double _normalizedKeypointX => Math.Round(KeypointX / ImageWidth, 8);
        private double _normalizedKeypointY => Math.Round(KeypointX / ImageWidth, 8);

        public double NormalizedBoxCenterX { get { return Math.Round(BoxCenterX / ImageWidth, 8); } }
        public double NormalizedBoxCenterY { get { return Math.Round(BoxCenterY / ImageHeight, 8); } }
        public double NormalizedBoxWidth { get { return Math.Round(BoxWidth / ImageWidth, 8); } }
        public double NormalizedBoxHeight { get { return Math.Round(BoxHeight / ImageHeight, 8); ; } }
        public Dictionary<string, double> NormalizedKeypointList
        {
            get { return _normalizedKeypointList; }
            set { _normalizedKeypointList = NormalizedKeypoints(value); }
        }
        public Normalized(double imageWidth, double imageHeight)
        {
            this.ImageWidth = imageWidth;
            this.ImageHeight = imageHeight;
        }
        public T GetV<T>(Dictionary<string, T> dictionary, string key, T defaultValue = default)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary), "The dictionary cannot be null.");
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("The key cannot be null or empty.", nameof(key));
            }

            return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
        }

        public Dictionary<string, double> NormalizedKeypoints(Dictionary<string, double> Keypoints)
        {
            Dictionary<string, double> keyValuePairs = new Dictionary<string, double>();
            int n = (Keypoints.Count / 3) - 2;
            for (int i = 0; i < n; i++)
            {
                KeypointX = GetV(Keypoints, $"x{i}", 0);
                KeypointY = GetV(Keypoints, $"y{i}", 0);
                Keypoints[$"x{i}"] = _normalizedKeypointX;
                Keypoints[$"y{i}"] = _normalizedKeypointY;
            }
            return Keypoints;
        }


    }

}
