using System.ComponentModel.DataAnnotations;

namespace PostgresAPI.Models
{
    public class Reward
    {
        [Key]
        public int reward_id { get; set; }

        public String reward_name {  get; set; }

        public String category {  get; set; }

        public int reward_cost { get; set; }

    }
}
