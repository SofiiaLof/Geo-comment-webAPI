namespace GeoComment.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Message { get; set; }

        public int minLon { get; set; }
        public int maxLon { get; set; }
        public int minLat { get; set; }
        public int maxLat { get; set; }

        public User User { get; set; }
    }
}
