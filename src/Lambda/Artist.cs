using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

namespace Lambda
{
    public class Artist
    {
        private readonly AmazonDynamoDBClient _dbClient = new AmazonDynamoDBClient();

        public async Task<APIGatewayProxyResponse> GetArtist(APIGatewayProxyRequest request, ILambdaContext context)
        {
            // TODO determine getAll or byId
            var result = await _getAllArtist();

            return new APIGatewayProxyResponse
            {
                Body = JsonConvert.SerializeObject(result),
                StatusCode = 200,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }

        private async Task<List<ArtistModel>> _getAllArtist()
        {
            var query = new QueryRequest
            {
                TableName = "ArtistDB2",
                IndexName = "sk_gsi_pk-data_gsi_sk-index",
                KeyConditionExpression = "sk_gsi_pk = :artistStatic",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":artistStatic", new AttributeValue { S = "Artist" } }
                }
            };
            var queryResult = await _dbClient.QueryAsync(query);

            var result = new List<ArtistModel>();
            foreach (var item in queryResult.Items)
            {

                Console.Out.WriteLine(item["pk"].S);

                result.Add(new ArtistModel
                {
                    pk = item["pk"].S
                });
            }

            return result;
        }

        private ArtistModel _getArtistById(string id)
        {
            return new ArtistModel();
        }
    }

    [DynamoDBTable("ArtistDB2")]
    public class ArtistModel
    {
        [DynamoDBHashKey] public string pk;
        [DynamoDBHashKey, DynamoDBRangeKey]public string sk_gsi_pk;
        [DynamoDBRangeKey] public string data_gsi_sk;
        public string name;
    }
}