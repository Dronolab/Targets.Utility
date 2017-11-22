namespace Target.Utility.Models
{
    public class SelectionPoint
    {
        public int X { get; set; }
        public int Y { get; set; }

        public SelectionPoint(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
