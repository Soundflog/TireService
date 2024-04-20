using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace TireServiceApplication.Source.Data;

public static class ApiClient
{

    private static readonly HttpClient Client = new HttpClient();
    
    /*
     * Метод для отправки запроса принимает путь(url) - куда отправляется, метод запроса для заголовка и даные - тело в нужных запросах
     * Присваивается ссылка запроса, если в метод поступило тело запроса - оно конвертируется и добавляется в запрос
     * Далее идет проверка на токен, если токен сохранен в программе - используется, иначе отправляется без токена
     * В последнем пункте реализация отлова ошибки, если что-то не так с запросом - возвращается ошибка, если запрос удался - возвращается HTTP статус и тело запроса
     */
    
    /*
     * В ...Data файлах собираются данные для запроса, а после передаются в методы этого класса:
     * * Путь запроса (url)
     * * Выбор метода запроса
     * * Данные - тело для запроса
     * После запроса - методы в файлах ...Data проверяют результат на ошибки запроса
     */
    
    /*
     * В ...Model файлах происходит обработка данных и различные преобразования уже полученых данных из ...Data файлов
     */
    
    /*
     * С помощью разделения на различные классы можно легко поменять сбособ получения данных, но не менять их обработку.
     * Либо можно поменять обработку, но не менять вывод данных в UI и не изменять сбособ получения данных.
     */
    
    private static async Task<Response> Request(string url, HttpMethod method, object? data = null)
    {
        var request = new HttpRequestMessage(method, Storage.GetUrl() + url);
        if (data != null)
        {
            var json = JsonConvert.SerializeObject(data);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        var token = Storage.GetToken();
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Add("Authorization", $"Bearer {token}");
        }

        try
        {
            var httpResponse = await Client.SendAsync(request);
            var response = new Response(httpResponse.StatusCode, await httpResponse.Content.ReadAsStringAsync());
            return response;
        }
        catch (Exception)
        {
            return new Response(HttpStatusCode.NotFound,"Error 404");
        }
    }
    
    // Метод для Get запроса
    public static async Task<Response> Get(string url)
    {
        return await Request(url, HttpMethod.Get);
    }
    
    // Метод для Post запроса
    public static async Task<Response> Post(string url, object? data)
    {
        return await Request(url, HttpMethod.Post, data);
    }
    
    // Метод для Put запроса
    public static async Task<Response> Put(string url, object data)
    {
        return await Request(url, HttpMethod.Put, data);
    }
    
    // Метод для Delete запроса
    public static async Task<Response> Delete(string url)
    {
        return await Request(url, HttpMethod.Delete);
    }
}

