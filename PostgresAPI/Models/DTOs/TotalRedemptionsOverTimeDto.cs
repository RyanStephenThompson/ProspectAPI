namespace PostgresAPI.Models.DTOs
{
    public class TotalRedemptionsOverTimeDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int TotalRedemptions { get; set; }
    }

}
