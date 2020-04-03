using System.Collections.Generic;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;

namespace Lambda.Response
{
    public static class APIResponse
    {

        public static APIGatewayProxyResponse Ok(object data)
        {
            return new APIGatewayProxyResponse
            {
                Body = JsonConvert.SerializeObject(data),
                StatusCode = 200,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }

        public static APIGatewayProxyResponse ClientError(string errorMessage)
        {
            return new APIGatewayProxyResponse
            {
                Body = JsonConvert.SerializeObject(errorMessage),
                StatusCode = 400,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
    }
}