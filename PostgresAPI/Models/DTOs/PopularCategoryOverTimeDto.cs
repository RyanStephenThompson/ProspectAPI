namespace PostgresAPI.Models.DTOs
{
    public class PopularCategoryOverTimeDto
    {
        public string Category { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Count { get; set; }
    }

}
