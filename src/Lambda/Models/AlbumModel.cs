using Amazon.DynamoDBv2.DataModel;

namespace Lambda.Models
{
    public class AlbumModel : BaseModel
    {
        public string Name;
        public string YOR; // Year of release
    }
}