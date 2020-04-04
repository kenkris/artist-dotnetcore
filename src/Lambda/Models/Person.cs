using Amazon.DynamoDBv2.DataModel;
using Lambda.Enums;

namespace Lambda.Models
{
    [DynamoDBTable("ArtistDB2")]
    public class Person
    {
        [DynamoDBHashKey] public string pk;
        [DynamoDBHashKey, DynamoDBRangeKey]public string sk_gsi_pk;
        [DynamoDBRangeKey] public string data_gsi_sk;
        public string Name;
        public string PlaceOfBirth;
        public Gender Gender;
    }
}