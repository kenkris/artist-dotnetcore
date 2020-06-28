using System;
using System.Collections.Generic;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

namespace Lambda
{
    public class Tester
    {
        public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var result = new TesterResult
            {
                Message = "Hi from tester",
                Created = DateTime.Now
            };

            return new APIGatewayProxyResponse
            {
                Body = JsonConvert.SerializeObject(result),
                StatusCode = 200,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };;
        }
    }

    public class TesterResult
    {
        public string Message;
        public DateTime Created;
    }
}