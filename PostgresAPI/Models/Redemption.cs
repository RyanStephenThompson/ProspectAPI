
using System.ComponentModel.DataAnnotations;

namespace PostgresAPI.Models
{
    public class Redemption
    {
        [Key]
        public int redemption_id { get; set; }

        public int reward_id { get; set; }

        public int ddid { get; set; }

        public String category { get; set; }

        public DateTime date { get; set; }
    }
}
