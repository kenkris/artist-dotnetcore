using Amazon.DynamoDBv2.DataModel;

namespace Lambda.Models
{
    public class AlbumModel
    {
        [DynamoDBHashKey] public string pk;
        [DynamoDBHashKey, DynamoDBRangeKey]public string sk_gsi_pk;
        [DynamoDBRangeKey] public string data_gsi_sk;
        public string name;
        public string recoredYear;
    }
}