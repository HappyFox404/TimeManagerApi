namespace TimeManagerApi.Models;

public class StandartResponse<T>
{
    public bool Status { get; set; }
    public string Message { get; set; } = null!;
    public T? Data { get; set; }
}

public class StandartResponseAnswer
{
    public static StandartResponse<T> Ok<T>(T? data, string message = "") => new() { Status = true, Data = data, Message = message };
    public static StandartResponse<object> Ok() => new() { Status = true };
    
    public static StandartResponse<T> Error<T>(T? data, string message = "") => new() { Status = false, Message = message, Data = data};
    public static StandartResponse<object> Error(string message = "") => new() { Status = false, Message = message };
}