using Amazon.DynamoDBv2.DataModel;

namespace Lambda.Models
{
    [DynamoDBTable("ArtistDB2")]
    public class BaseModel
    {
        [DynamoDBHashKey] public string PK;
        [DynamoDBHashKey, DynamoDBRangeKey]public string SK_GSI_PK;
        [DynamoDBRangeKey] public string Data;
    }
}