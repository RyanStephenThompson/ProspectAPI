using Google.Apis.Auth.OAuth2;
using Google.Cloud.BigQuery.V2;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PostgresAPI.Data
{
    public class BigQueryService
    {
        private readonly BigQueryClient _client;

        public BigQueryService(string projectId, string credentialPath)
        {
            string location = "africa-south1";
            // Load credentials from the specified file
            var credential = GoogleCredential.FromFile(credentialPath);

            // Create BigQuery client with the credentials and project ID
            _client = BigQueryClient.Create(projectId, credential);
        }

        public async Task<IEnumerable<BigQueryRow>> QueryDataAsync(string query)
        {
            var results = await _client.ExecuteQueryAsync(query, parameters: null);
            return results;
        }
    }
}
