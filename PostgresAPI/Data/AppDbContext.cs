using Google.Cloud.BigQuery.V2;
using Microsoft.EntityFrameworkCore;
using PostgresAPI.Models;

namespace PostgresAPI.Data
{
    public class AppDbContext : DbContext
    {
        private readonly BigQueryService _bigQueryService;

        public AppDbContext(DbContextOptions<AppDbContext> options, BigQueryService bigQueryService) : base(options)
        {
            _bigQueryService = bigQueryService;
        }

        #region Methods to read from BigQuery

        #region Joined

        #region single data points
        public async Task<int> GetTotalProspectsAsync()
        {
            string query = "SELECT COUNT(*) AS total_prospects FROM `Prospect.prospects`";
            var result = await _bigQueryService.QueryDataAsync(query);
            return Convert.ToInt32(result.First()["total_prospects"]);
        }

        public async Task<int> GetTotalProspectsThisMonthAsync()
        {
            string query = @"
        SELECT COUNT(*) AS total_prospects_month 
        FROM `Prospect.prospects` 
        WHERE DATE(join_date) >= DATE_SUB(CURRENT_DATE(), INTERVAL 1 MONTH)";
            var result = await _bigQueryService.QueryDataAsync(query);
            return Convert.ToInt32(result.First()["total_prospects_month"]);
        }

        public async Task<int> GetTotalProspectsThisWeekAsync()
        {
            string query = @"
        SELECT COUNT(*) AS total_prospects_week 
        FROM `Prospect.prospects` 
        WHERE DATE(join_date) >= DATE_SUB(CURRENT_DATE(), INTERVAL 1 WEEK)";
            var result = await _bigQueryService.QueryDataAsync(query);
            return Convert.ToInt32(result.First()["total_prospects_week"]);
        }

        public async Task<int> GetTotalProspectsTodayAsync()
        {
            string query = @"
        SELECT COUNT(*) AS total_prospects_today 
        FROM `Prospect.prospects` 
        WHERE DATE(join_date) = CURRENT_DATE()";
            var result = await _bigQueryService.QueryDataAsync(query);
            return Convert.ToInt32(result.First()["total_prospects_today"]);
        }
        #endregion

        #region Graphs

        public async Task<Dictionary<string, int>> GetJoinedProspectsPerMonthThisYearAsync()
        {
            string query = @"
        SELECT FORMAT_TIMESTAMP('%Y-%m', join_date) AS month_year,
               COUNT(*) AS prospect_count
        FROM `Prospect.prospects`
        WHERE EXTRACT(YEAR FROM join_date) = EXTRACT(YEAR FROM CURRENT_DATE())
        GROUP BY month_year
        ORDER BY month_year";

            var result = await _bigQueryService.QueryDataAsync(query);

            var monthlyProspects = new Dictionary<string, int>();

            foreach (var row in result)
            {
                monthlyProspects[row["month_year"].ToString()] = Convert.ToInt32(row["prospect_count"]);
            }

            // Fill in months that have not happened yet with 0
            var currentYear = DateTime.Now.Year;
            for (int month = 1; month <= DateTime.Now.Month; month++)
            {
                string monthKey = $"{currentYear}-{month:D2}"; // Format: YYYY-MM
                if (!monthlyProspects.ContainsKey(monthKey))
                {
                    monthlyProspects[monthKey] = 0; // No prospects joined in that month
                }
            }

            return monthlyProspects;
        }


        #endregion

        #endregion

        #region Converted

        public async Task<int> GetTotalEntitiesAsync()
        {
            string query = "SELECT COUNT(*) AS total_entities FROM `Prospect.entities`";
            var result = await _bigQueryService.QueryDataAsync(query);
            return Convert.ToInt32(result.First()["total_entities"]);
        }

        public async Task<int> GetTotalEntitiesThisMonthAsync()
        {
            string query = @"
        SELECT COUNT(*) AS total_entities_month 
        FROM `Prospect.entities` 
        WHERE DATE(created_at) >= DATE_SUB(CURRENT_DATE(), INTERVAL 1 MONTH)";
            var result = await _bigQueryService.QueryDataAsync(query);
            return Convert.ToInt32(result.First()["total_entities_month"]);
        }

        public async Task<int> GetTotalEntitiesThisWeekAsync()
        {
            string query = @"
        SELECT COUNT(*) AS total_entities_week 
        FROM `Prospect.entities` 
        WHERE DATE(created_at) >= DATE_SUB(CURRENT_DATE(), INTERVAL 1 WEEK)";
            var result = await _bigQueryService.QueryDataAsync(query);
            return Convert.ToInt32(result.First()["total_entities_week"]);
        }

        public async Task<int> GetTotalEntitiesTodayAsync()
        {
            string query = @"
        SELECT COUNT(*) AS total_entities_today 
        FROM `Prospect.entities` 
        WHERE DATE(created_at) = CURRENT_DATE()";
            var result = await _bigQueryService.QueryDataAsync(query);
            return Convert.ToInt32(result.First()["total_entities_today"]);
        }

        #endregion

  

        /*
         *         public async Task<long> GetRecentProspectsCountAsync()
        {
            // Define the query to count the prospects where join_date is within the last month
            string query = @"
        SELECT COUNT(*) AS prospect_count
        FROM `Prospect.prospects`
        WHERE DATE(join_date) >= DATE_SUB(CURRENT_DATE(), INTERVAL 1 MONTH)";

            // Execute the query and retrieve the results
            var results = await _bigQueryService.QueryDataAsync(query);

            // Extract the count from the result
            var row = results.FirstOrDefault();
            if (row != null)
            {
                return (long)row["prospect_count"];
            }

            return 0; // Return 0 if no result is found
        }
         */

        #endregion

        public DbSet<Prospect> prospects { get; set; }
        public DbSet<Entity> entities { get; set; }
        public DbSet<Reward> rewards { get; set; }
    }
}
