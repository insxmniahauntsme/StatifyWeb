namespace StatifyWeb.Interfaces;

public interface IAccessTokenService
{
	Task SendDataToClient(Dictionary<string, object> data);
}