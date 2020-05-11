using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using Lambda.Models;

namespace Lambda.Access
{
    public class PersonAccess : ArtistDB3Base
    {
        public async Task<List<PersonModel>> FetchPersons()
        {
            var query = new QueryRequest
            {
                TableName = ArtistTable,
                IndexName = GSI_DATA_INDEX,
                KeyConditionExpression = "#key = :personStatic",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#key", GSI }
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":personStatic", new AttributeValue { S = "Person" } }
                }
            };

            var queryResult = await DbClient.QueryAsync(query);

            var result = new List<PersonModel>();
            foreach (var item in queryResult.Items)
            {
                result.Add(new PersonModel()
                {
                    PK = item["PK"].S,
                    SK_GSI_PK = item["SK-GSI-PK"].S,
                    Data = item["Data"].S,
                    FirstName = item["FirstName"].S,
                    LastName = item["LastName"].S
                });
            }
            return result;
        }

        public async Task<PersonModel> FetchPerson(string id)
        {
            var query = new QueryRequest
            {
                TableName = ArtistTable,
                KeyConditionExpression = "PK = :personId",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":personId", new AttributeValue { S = id } }
                }
            };

            var queryResult = await DbClient.QueryAsync(query);

            if (queryResult.Items.Count != 1)
                return new PersonModel();

            var item = queryResult.Items.First();
            return new PersonModel()
            {
                PK = item["PK"].S,
                SK_GSI_PK = item["SK-GSI-PK"].S,
                Data = item["Data"].S,
                FirstName = item["FirstName"].S,
                LastName = item["LastName"].S
            };
        }
    }
}