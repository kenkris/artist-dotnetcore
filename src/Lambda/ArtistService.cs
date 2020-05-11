using System;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Lambda.Access;
using Lambda.Models;
using Lambda.Response;

namespace Lambda
{
    public class ArtistService
    {
        private ArtistAccess _artistAccess;
        public ArtistService()
        {
            _artistAccess = new ArtistAccess();
        }

        public async Task<APIGatewayProxyResponse> GetArtistById(APIGatewayProxyRequest request, ILambdaContext context)
        {
            if (!request.PathParameters.TryGetValue("id", out var artistId))
                return APIResponse.ClientError("Missing id param");

            return APIResponse.Ok(await _artistAccess.FetchArtistById(artistId));
        }

        public async Task<APIGatewayProxyResponse> GetAllArtist(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return APIResponse.Ok(await  _artistAccess.FetchAllArtist());
        }

        public async Task<APIGatewayProxyResponse> GetArtistMembers(APIGatewayProxyRequest request, ILambdaContext context)
        {
            if (!request.PathParameters.TryGetValue("id", out var artistId))
                return APIResponse.ClientError("Missing id parameter");

            return APIResponse.Ok(await _artistAccess.FetchArtistMembers(artistId));
        }

        public async Task<APIGatewayProxyResponse> GetArtistAlbums(APIGatewayProxyRequest request, ILambdaContext context)
        {
            if (!request.PathParameters.TryGetValue("id", out var artistId))
                return APIResponse.ClientError("Missing id parameter");

            return APIResponse.Ok(await _artistAccess.FetchArtistAlbums(artistId));
        }
    }

    public class ArtistModel : BaseModel
    {
        public string Name;
    }
}