using System.ComponentModel.DataAnnotations;

namespace PostgresAPI.Models
{
    public class Entity
    {
        public class Prospect
        {
            [Key]
            public int ddid { get; set; }

            public DateTime created_at { get; set; }

            public string product { get; set; }

        }
    }
}
