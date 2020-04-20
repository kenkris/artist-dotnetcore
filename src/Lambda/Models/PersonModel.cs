using Amazon.DynamoDBv2.DataModel;
using Lambda.Enums;

namespace Lambda.Models
{
    [DynamoDBTable("ArtistDB2")]
    public class PersonModel : BaseModel
    {
        public string FirstName;
        public string LastName;
        public string PlaceOfBirth; // TODO implement in db
        public Gender Gender; // TODO implement in db
    }
}