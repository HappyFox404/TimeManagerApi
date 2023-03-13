namespace TimeManagerApi.Models;

public class StandartResponse<T>
{
    public string Message { get; set; } = null!;
    public T? Data { get; set; }
    public int StatusCode { get; set; } = 200;
}

public class StandartResponseAnswer
{
    public static StandartResponse<T> Ok<T>(T? data, string message = "Запрос успешно обработан") => 
        new() { Data = data, Message = message, StatusCode = 200};
    public static StandartResponse<object> Ok(string message = "Запрос успешно обработан") => 
        new() { Data = null, Message = message, StatusCode = 200};
    public static StandartResponse<T> Error<T>(string message = "Во время обработки произошла ошибка", int statusCode = 400, T? data = default) =>
        new() { Message = message, StatusCode = statusCode, Data = data };
    public static StandartResponse<object> Error(string message = "Во время обработки произошла ошибка", int statusCode = 400) =>
        new() { Message = message, StatusCode = statusCode, Data = null };
}