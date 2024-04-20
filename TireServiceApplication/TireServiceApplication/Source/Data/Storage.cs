namespace TireServiceApplication.Source.Data;

public static class Storage
{
    /*
     * Класс, в котором можно взаимодействовать с локальными данными:
     * * Токен пользователя
     * * Id пользователя
     * * Роль пользователя
     * * Основную(базовую) ссылку для API запросов
     */
    
    private const string TokenKey = "user_token";
    private const string IdKey = "user_id";
    private const string RoleKey = "user_role";
    private const string UrlKey = "url_linq";
    
    public static void SaveToken(string token)
    {
        Preferences.Set(TokenKey, token);
    }
    public static string GetToken()
    {
        return Preferences.Get(TokenKey, string.Empty);
    }
    public static void RemoveToken()
    {
        Preferences.Remove(TokenKey);
    }

    public static void SaveRole(string role)
    {
        Preferences.Set(RoleKey, role);
    }
    public static string GetRole()
    {
        return Preferences.Get(RoleKey, string.Empty);
    }
    public static void RemoveRole()
    {
        Preferences.Remove(RoleKey);
    }
    
    public static void SaveId(string id)
    {
        Preferences.Set(IdKey, id);
    }
    public static string GetId()
    {
        return Preferences.Get(IdKey, string.Empty);
    }
    public static void RemoveId()
    {
        Preferences.Remove(IdKey);
    }
    
    public static void SaveUrl(string url)
    {
        Preferences.Set(UrlKey, url);
    }
    public static string GetUrl()
    {
        return Preferences.Get(UrlKey, "http://847222cac365.vps.myjino.ru/api/");
    }
    public static void RemoveUrl()
    {
        Preferences.Remove(UrlKey);
    }
}