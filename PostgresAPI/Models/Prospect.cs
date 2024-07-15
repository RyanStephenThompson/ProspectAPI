using System.ComponentModel.DataAnnotations;

namespace PostgresAPI.Models
{
    public class Prospect
    {
        [Key]
        public int ddid { get; set; }

        public String id { get; set; }

        public String first_name { get; set; }

        public String surname { get; set; }

        public String email { get; set; }

        public DateOnly join_date { get; set; }
        public DateOnly last_active_date { get; set; }
        public bool converted { get; set; }
        
    }
}
