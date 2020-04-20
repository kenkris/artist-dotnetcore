using Amazon.DynamoDBv2;

namespace Lambda
{
    public class ArtistDB3Base
    {
        protected readonly AmazonDynamoDBClient DbClient = new AmazonDynamoDBClient();
        protected const string ArtistTable = "ArtistDB3";
        protected const string GSI = "SK-GSI-PK";
        protected const string GSI_DATA_INDEX = "SK-GSI-PK-Data-index";
    }
}