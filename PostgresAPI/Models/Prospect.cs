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

        public DateTime join_date { get; set; }
        public DateTime last_active_date { get; set; }
        public bool converted { get; set; }

        public string gender { get; set; }
        
    }
}
