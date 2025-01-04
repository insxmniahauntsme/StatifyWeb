using StatifyWeb.Interfaces;

namespace StatifyWeb.Services;

public class AccessTokenService : IAccessTokenService
{
	private readonly IHttpClientFactory _httpClientFactory;

	public AccessTokenService(IHttpClientFactory httpClientFactory)
	{
		_httpClientFactory = httpClientFactory;
	}

	public async Task SendTokenToClient(string token)
	{
		var tokenUrl = $"https://localhost:8081/?access_token={token}";
		await _httpClientFactory.CreateClient("LocalhostClient")
			.PostAsync(tokenUrl, new StringContent(token));
			
	}
}