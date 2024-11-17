using Microsoft.Extensions.Primitives;
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
        public Dictionary<string, double> Keypoints { get; private set; }

        public Label(int classIndex, double x, double y, double width, double height, Dictionary<string, double> keypoints)
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
            int n = (Keypoints.Count / 3) - 2;
            for(int i = 0; i < n; i++)
            {
                sb.Append($" {GetV(Keypoints, $"x{i}", 0)}");
                sb.Append($" {GetV(Keypoints, $"y{i}", 0)}");
                sb.Append($" {GetV(Keypoints, $"visible{i}", 0)}");
            }
            return sb.ToString();
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
    }
    public class LabelBuilder
    {
        private int _classIndex;
        private double _x;
        private double _y;
        private double _width;
        private double _height;
        private Dictionary<string, double> _keypoints = new Dictionary<string, double>();

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

        public LabelBuilder AddKeypoint(Dictionary<string, double> dic)
        {
            _keypoints = dic;
            return this;
        }

        public Label Build()
        {
            return new Label(_classIndex, _x, _y, _width, _height, _keypoints);
        }
    }

}
