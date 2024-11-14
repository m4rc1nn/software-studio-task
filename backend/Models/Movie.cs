namespace Models
{
    public class Movie
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Director { get; set; }
        public required int Year { get; set; }
        public required int Rate { get; set; }
        public int? ExternalApiId { get; set; }
    }
}
