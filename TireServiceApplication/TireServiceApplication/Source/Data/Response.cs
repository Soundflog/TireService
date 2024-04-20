using System.Net;

namespace TireServiceApplication.Source.Data;

public class Response
{
    /*
     * Класс, который хранит ответ сервера после запроса.
     * Поля:
     * * Тело - возвращенный результат запроса
     * * Статус запроса сервера
     */
    
    public HttpStatusCode StatusCode { get; }
    public string Content { get; }
    
    public Response(HttpStatusCode statusCode, string body)
    {
        StatusCode = statusCode;
        Content = body;
    }
}