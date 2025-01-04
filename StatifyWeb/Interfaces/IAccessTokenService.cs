namespace StatifyWeb.Interfaces;

public interface IAccessTokenService
{
	Task SendTokenToClient(string token);
}