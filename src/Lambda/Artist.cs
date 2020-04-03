using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Lambda.Response;
using Newtonsoft.Json;

namespace Lambda
{
    public class Artist
    {
        private readonly AmazonDynamoDBClient _dbClient = new AmazonDynamoDBClient();
        private const string ArtistTable = "ArtistDB2";

        public async Task<APIGatewayProxyResponse> GetArtistById(APIGatewayProxyRequest request, ILambdaContext context)
        {
            if (!request.PathParameters.TryGetValue("id", out var artistId))
                return APIResponse.ClientError("Missing id param");

            return APIResponse.Ok(await _fetchArtistById(artistId));
        }

        public async Task<APIGatewayProxyResponse> GetAllArtist(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return APIResponse.Ok(await  _fetchAllArtist());
        }


        private async Task<ArtistModel> _fetchArtistById(string id)
        {
            var query = new QueryRequest
            {
                TableName = ArtistTable,
                KeyConditionExpression = "pk = :artistId",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":artistId", new AttributeValue { S = id } }
                }
            };

            var queryResult = await _dbClient.QueryAsync(query);

            if (queryResult.Items.Count != 1)
                return new ArtistModel();

            var item = queryResult.Items.First();
            return new ArtistModel
            {
                pk = item["pk"].S,
                sk_gsi_pk = item["sk_gsi_pk"].S,
                data_gsi_sk = item["data_gsi_sk"].S,
                name = item["name"].S
            };
        }

        private async Task<List<ArtistModel>> _fetchAllArtist()
        {
            var query = new QueryRequest
            {
                TableName = ArtistTable,
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
                result.Add(new ArtistModel
                {
                    pk = item["pk"].S,
                    sk_gsi_pk = item["sk_gsi_pk"].S,
                    data_gsi_sk = item["data_gsi_sk"].S,
                    name = item["name"].S
                });
            }

            return result;
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