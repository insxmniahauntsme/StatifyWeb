using Microsoft.AspNetCore.Mvc;
using StatifyWeb.Interfaces;

namespace StatifyWeb.Controllers;

public class SpotifyTokenController : Controller
{
	private readonly IConfiguration _configuration;
	private readonly IHttpClientFactory _httpClientFactory;
	private IAccessTokenService _accessTokenService;

	public SpotifyTokenController(IConfiguration configuration, IHttpClientFactory httpClientFactory, IAccessTokenService accessTokenService)
	{
		_configuration = configuration;
		_httpClientFactory = httpClientFactory;
		_accessTokenService = accessTokenService;
	}
	
	[HttpGet("/callback")]
	public async Task<ActionResult<string>> GetAccessToken([FromQuery] string code)
	{
		if (string.IsNullOrEmpty(code))
		{
			return BadRequest();
		}
		
		var tokenRequestData = new Dictionary<string, string>
		{
			{ "grant_type", "client_credentials" },
			{ "client_id", _configuration["SpotifyAPI:client-id"]! },
			{ "client_secret", _configuration["SpotifyAPI:client-secret"]! },
		};
		
		var requestContent = new FormUrlEncodedContent(tokenRequestData);
		var response = await _httpClientFactory.CreateClient("SpotifyClient")
			.PostAsync("https://accounts.spotify.com/api/token", requestContent);
		var responseString = await response.Content.ReadAsStringAsync();

		if (!response.IsSuccessStatusCode)
		{
			return View("Error");
		}
		
		var tokenData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(responseString);

		if (tokenData != null && tokenData.TryGetValue("access_token", out var accessToken))
		{
			Console.WriteLine($"Access token: {accessToken}");
			await _accessTokenService.SendTokenToClient(accessToken.ToString()!);
		}
		
		return View("Success");
	}
	
}