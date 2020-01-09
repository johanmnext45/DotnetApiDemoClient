using System;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers.NewtonsoftJson;
using Newtonsoft.Json.Linq;
namespace ApiApplication {
  class Api {
    static string ApiKey = Settings.ApiKey;
    static string Secret = Settings.Secret;
    static void Main(string[] args) {
      var client = new RestClient("https://docfox-demo.herokuapp.com/api/v2/");
      client.Timeout = -1;
      client.UseNewtonsoftJson();
      var request = new RestRequest("authentications/new", DataFormat.Json);
      request.RequestFormat = DataFormat.Json;
      request.AddHeader("X-Client-Api-Key", ApiKey);
      request.AddHeader("Content-Type", "application/vnd.api+json");
      request.AddHeader("Accept-Language", "en-US,en");
      request.AddHeader("Accept", "application/vnd.api+json");

      var response = client.Get(request);
      dynamic result = JObject.Parse(response.Content);
      string nonce = result.data.attributes.nonce;
      Console.WriteLine($"Nonce: {nonce}");

      Auth auth = new Auth(Secret, ApiKey);

      string signature = auth.Signature(nonce);
      Console.WriteLine($"Signature: {signature}");

      request = new RestRequest("tokens/new", DataFormat.Json);
      request.RequestFormat = DataFormat.Json;
      request.AddHeader("X-Client-Signature", signature);
      request.AddHeader("X-Client-Api-Key", ApiKey);
      request.AddHeader("Content-Type", "application/vnd.api+json");
      request.AddHeader("Accept-Language", "en-US,en");
      request.AddHeader("Accept", "application/vnd.api+json");

      response = client.Get(request);
      result = JObject.Parse(response.Content);

      string token = result.data.attributes.token;
      Console.WriteLine(token);

      request = new RestRequest("kyc_entity_templates?per_page=100");
      request.RequestFormat = DataFormat.Json;
      request.AddHeader("Content-Type", "application/vnd.api+json");
      request.AddHeader("Accept-Language", "en-US,en");
      request.AddHeader("Accept", "application/vnd.api+json");
      // request.AddHeader("Authorization", string.Format("Bearer {0}", token));
      // request.AddHeader("Authorization", token);
      client.Authenticator = new JwtAuthenticator(token);
      client.Authenticator.Authenticate(client, request);

      response = client.Get(request);
      result = JObject.Parse(response.Content);
      Console.WriteLine(result);
    }
  }
}
