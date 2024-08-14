namespace WebsiteBanHang.Service
{
    using Google.Cloud.Dialogflow.V2;
    using Google.Protobuf;
    using Grpc.Core;
    using System.IO;

    public class DialogflowService
    {
        private readonly SessionsClient _sessionsClient;
        private readonly string _projectId;

        public DialogflowService(string projectId, string jsonPath)
        {
            _projectId = projectId;

            var builder = new SessionsClientBuilder
            {
                CredentialsPath = jsonPath
            };
            _sessionsClient = builder.Build();
        }

        public string DetectIntent(string sessionId, string query)
        {
            var sessionName = SessionName.FromProjectSession(_projectId, sessionId);
            var textInput = new TextInput { Text = query, LanguageCode = "vi" };
            var queryInput = new QueryInput { Text = textInput };

            try
            {
                var response = _sessionsClient.DetectIntent(sessionName, queryInput);
                return response.QueryResult.FulfillmentText;
            }
            catch (RpcException ex)
            {
                // Log and handle the exception
                Console.WriteLine($"Error detecting intent: {ex.Status.Detail}");
                throw;
            }
        }

    }

}
