namespace TireServiceApplication.Source.Data;

public static class AuthenticationData
{
    private const string AuthenticationUrl = "Authentication";

    // Метода отправка запроса Авторизации
    public static async Task<Response> Authentication(string login, string password)
    {
        var result = await ApiClient.Post($"{AuthenticationUrl}?login={login}&password={password}", null);
        return result;
    }
}