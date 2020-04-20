using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using Lambda.Models;

namespace Lambda
{
    public class PersonService : ArtistDB3Base
    {

        // TODO implement API handlers to go directly into persons if needed
        // For now these will be called through ArtistService

        public async Task<List<PersonModel>> GetPersons()
        {
            var query = new QueryRequest
            {
                TableName = ArtistTable,
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

        public async Task<PersonModel> GetPersons(string id)
        {
            return new PersonModel();
        }
    }
}