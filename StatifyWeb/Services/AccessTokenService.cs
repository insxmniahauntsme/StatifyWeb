using StatifyWeb.Interfaces;

namespace StatifyWeb.Services;

public class AccessTokenService : IAccessTokenService
{
	private readonly IHttpClientFactory _httpClientFactory;

	public AccessTokenService(IHttpClientFactory httpClientFactory)
	{
		_httpClientFactory = httpClientFactory;
	}

	public async Task SendDataToClient(Dictionary<string, object> data)
	{
		Dictionary<string, string> paramsString = new Dictionary<string, string>
		{
			{"access_token", data["access_token"].ToString()!},
			{"token_type", data["token_type"].ToString()!},
			{"scope", data.ContainsKey("scope") ? data["scope"].ToString()! : ""},
			{"expires_in", data["expires_in"].ToString()!},
			{"refresh_token", data["refresh_token"].ToString()!}
		};
		
		var queryParams = new FormUrlEncodedContent(paramsString);
		
		var queryUrl = "https://localhost:8081";
		await _httpClientFactory.CreateClient("LocalhostClient")
			.PostAsync(queryUrl, queryParams);
			
	}
}