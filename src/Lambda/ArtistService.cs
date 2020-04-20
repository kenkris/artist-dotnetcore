using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Lambda.Models;
using Lambda.Response;

namespace Lambda
{
    public class ArtistService : ArtistDB3Base
    {
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

        public async Task<APIGatewayProxyResponse> GetArtistMembers(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return APIResponse.NotImplemented();
        }

        public async Task<APIGatewayProxyResponse> GetArtistAlbums(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return APIResponse.NotImplemented();
        }


        private async Task<ArtistModel> _fetchArtistById(string id)
        {
            var query = new QueryRequest
            {
                TableName = ArtistTable,
                KeyConditionExpression = "PK = :artistId and #sk = :artistStatic",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#sk", GSI }
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":artistId", new AttributeValue { S = id } },
                    { ":artistStatic", new AttributeValue { S = "Artist" } }
                }
            };

            var queryResult = await DbClient.QueryAsync(query);

            if (queryResult.Items.Count != 1)
                return new ArtistModel();

            var item = queryResult.Items.First();
            return new ArtistModel
            {
                PK = item["PK"].S,
                SK_GSI_PK = item["SK-GSI-PK"].S,
                Data = item["Data"].S,
                Name = item["Name"].S
            };
        }

        private async Task<List<ArtistModel>> _fetchAllArtist()
        {
            var query = new QueryRequest
            {
                TableName = ArtistTable,
                IndexName = GSI_DATA_INDEX,
                KeyConditionExpression = "#key = :artistStatic",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#key", GSI }
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":artistStatic", new AttributeValue { S = "Artist" } }
                },

            };
            var queryResult = await DbClient.QueryAsync(query);

            var result = new List<ArtistModel>();
            foreach (var item in queryResult.Items)
            {
                result.Add(new ArtistModel
                {
                    PK = item["PK"].S,
                    SK_GSI_PK = item["SK-GSI-PK"].S,
                    Data = item["Data"].S,
                    Name = item["Name"].S
                });
            }

            return result;
        }

        // TODO Implement get artist member function
        private async Task<List<PersonModel>> _fetchArtistMembers(string id)
        {
            return new List<PersonModel>();
        }

        // TODO Implement get artist albums function
        private async Task<List<AlbumModel>> _fetchArtistAlbums(string id)
        {
            var query = new QueryRequest
            {
                TableName = ArtistTable,
                IndexName = GSI_DATA_INDEX,
                KeyConditionExpression = "#key = :albumStatic and data_gsi_sk = :artistId",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#key", GSI }
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":albumStatic", new AttributeValue { S = "Album" } },
                    { ":artistId", new AttributeValue { S = $"Artist#{id}"} }
                }
            };
            var queryResult = await DbClient.QueryAsync(query);

            var result = new List<AlbumModel>();
            foreach (var item in queryResult.Items)
            {
                result.Add(new AlbumModel
                {
                    pk = item["pk"].S,
                    sk_gsi_pk = item["sk_gsi_pk"].S,
                    data_gsi_sk = item["data_gsi_sk"].S,
                    name = item["name"].S,
                    recoredYear = item["recoredYear"].S
                });
            }

            return result;
        }
    }

    public class ArtistModel : BaseModel
    {
        public string Name;
    }
}