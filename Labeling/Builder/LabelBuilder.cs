using System.Text;
namespace Labeling
{
    public class Label
    {
        public int ClassIndex { get; private set; }
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Width { get; private set; }
        public double Height { get; private set; }
        public int Visible { get; private set; }
        public List<Dictionary<string, double>> Keypoints { get; private set; }

        public Label(int classIndex, double x, double y, double width, double height, List<Dictionary<string, double>> keypoints)
        {
            ClassIndex = classIndex;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Keypoints = keypoints;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{ClassIndex} {X} {Y} {Width} {Height}");
            foreach (var keypoint in Keypoints)
            {
                double xp;
                double yp;
                if (keypoint.TryGetValue("x", out xp) && keypoint.TryGetValue("y", out yp))
                {
                    sb.Append($" {xp} {yp}");
                }
            }
            return sb.ToString();
        }
    }
    public class LabelBuilder
    {
        private int _classIndex;
        private double _x;
        private double _y;
        private double _width;
        private double _height;
        private List<Dictionary<string, double>> _keypoints = new List<Dictionary<string, double>>();

        public LabelBuilder SetClassIndex(int classIndex)
        {
            _classIndex = classIndex;
            return this;
        }

        public LabelBuilder SetBoundingBox(double x, double y, double width, double height)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
            return this;
        }

        public LabelBuilder AddKeypoint(double px, double py, double visible)
        {
            _keypoints.Add(new Dictionary<string, double>()
            {
                {"x", px},
                {"y", py},
                {"visible",  visible}
            });
            return this;
        }

        public Label Build()
        {
            return new Label(_classIndex, _x, _y, _width, _height, _keypoints);
        }
    }

}
