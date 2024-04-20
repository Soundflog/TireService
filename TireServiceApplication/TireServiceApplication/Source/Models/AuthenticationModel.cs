using System.Net;
using Newtonsoft.Json;
using TireServiceApplication.Source.Data;
using TireServiceApplication.Source.Entities;

namespace TireServiceApplication.Source.Models;

public static class AuthenticationModel
{
    // Метод для авторизации. Передается логин и пароль, если сервер принял авторизацию - записать Токен
    public static async Task<string> Authentication(string login, string password)
    {
        var response = await AuthenticationData.Authentication(login, password);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            try
            {
                var content = JsonConvert.DeserializeObject<UserToken>(response.Content);
                if (content != null) Storage.SaveToken(content.access_token);
                await CheckToken();
                return string.Empty;
            }
            catch (Exception)
            {
                return "Ошибка сервера";
            }
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return "Неверный логин или пароль";
        }
        return response.Content;
    }
    
    // Метод для проверки на статус токена - если токен "живой" все в порядке, иначе очистить локальные данные
    public static async Task<bool> CheckToken()
    {
        var content = await UserData.GetUser();
        if (content == null || content.Role == null || content.Id == null)
        {
            Storage.RemoveToken();
            Storage.RemoveRole();
            Storage.RemoveId();
            return false;
        }
        Storage.SaveRole(content.Role);
        Storage.SaveId(content.Id);

        return true;
    }
}