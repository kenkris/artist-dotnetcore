using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using Lambda.Models;
using Newtonsoft.Json;

namespace Lambda.Access
{
    public class ArtistAccess : ArtistDB3Base
    {
        public async Task<ArtistModel> FetchArtistById(string id)
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

        public async Task<List<ArtistModel>> FetchAllArtist()
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

        public async Task<List<PersonModel>> FetchArtistMembers(string id)
        {
            var query = new QueryRequest
            {
                TableName = ArtistTable,
                KeyConditionExpression = "PK = :artistId and begins_with(#sk, :memberPersonStatic)",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#sk", GSI }
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":artistId", new AttributeValue { S = id } },
                    { ":memberPersonStatic", new AttributeValue { S = "MemberPerson#" } }
                }
            };

            var queryResult = await DbClient.QueryAsync(query);

            var result = new List<PersonModel>();
            var personAccess = new PersonAccess();
            foreach (var item in queryResult.Items)
            {
                var personId = "";
                try
                {
                    personId = item[GSI].S.Split("#")[1];
                }
                catch (Exception e)
                {
                    throw new Exception("Could not extract personId " + e.Message);
                }

                var person = await personAccess.FetchPerson(personId);
                if(!string.IsNullOrEmpty(person.PK))
                    result.Add(person);
            }

            return result;
        }

        public async Task<List<AlbumModel>> FetchArtistAlbums(string id)
        {
            var query = new QueryRequest
            {
                TableName = ArtistTable,
                IndexName = GSI_DATA_INDEX,
                KeyConditionExpression = "#key = :albumStatic and #sk = :artistId",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#key", GSI },
                    { "#sk" , "Data" }
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
                    PK = item["PK"].S,
                    SK_GSI_PK = item[GSI].S,
                    Data = item["Data"].S,
                    Name = item["Name"].S,
                    YOR = item["YOR"].S
                });
            }

            return result;
        }
    }
}