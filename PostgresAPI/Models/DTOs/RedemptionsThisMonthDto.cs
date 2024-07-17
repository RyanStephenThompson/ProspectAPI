namespace PostgresAPI.Models.DTOs
{
    public class RedemptionsThisMonthDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int TotalRedemptions { get; set; }
    }

}
