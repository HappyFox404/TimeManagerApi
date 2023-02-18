namespace TimeManagerApi.Models;

public class StandartResponse<T>
{
    public string Message { get; set; } = null!;
    public T? Data { get; set; }
}

public class StandartResponseAnswer
{
    public static IResult Ok<T>(T? data, string message = "Запрос успешно обработан") => 
        Results.Json(new StandartResponse<T>() { Data = data, Message = message }, statusCode: 200);
    public static IResult Ok(string message = "Запрос успешно обработан") => 
        Results.Json(new StandartResponse<object>() { Message = message }, statusCode: 200);
    public static IResult Error<T>(T? data, string message = "Во время обработки произошла ошибка") => 
        Results.Json(new StandartResponse<T>() { Message = message, Data = data}, statusCode: 400);
    public static IResult Error(string message = "Во время обработки произошла ошибка") => 
        Results.Json(new StandartResponse<object>() {Message = message }, statusCode: 400);
}