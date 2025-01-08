using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
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
	public async Task<ActionResult> GetAccessToken([FromQuery] string code)
	{
		var clientId = _configuration["SpotifyAPI:client-id"];
		var clientSecret = _configuration["SpotifyAPI:client-secret"];
		var httpClient = _httpClientFactory.CreateClient("SpotifyClient");

		var queryParams = new FormUrlEncodedContent(new Dictionary<string, string>
		{
			{ "grant_type", "authorization_code" },
			{ "code", code },
			{ "redirect_uri", _configuration["SpotifyAPI:redirect-uri"]! },
		});
		
		var clientCredentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}"));
		
		httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", clientCredentials);
				
		var response = await httpClient.PostAsync("https://accounts.spotify.com/api/token", queryParams);
		var responseString = await response.Content.ReadAsStringAsync();

		Console.WriteLine(responseString);
		
		if (!response.IsSuccessStatusCode)
		{
			return View("Error");
		}
		
		var tokenData = JsonSerializer.Deserialize<Dictionary<string, object>>(responseString);
		if (tokenData != null)
		{
			Console.WriteLine($"Access token: {tokenData["access_token"]}");
			await _accessTokenService.SendDataToClient(tokenData);
		}

		return View("Success");
	}
}