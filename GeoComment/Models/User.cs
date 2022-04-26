namespace GeoComment.Models
{
    public class User
    {
        public int Id { get; set; }
        public string First_name { get; set; }

        public IList<Comment> Comments { get; set; }
    }
}
