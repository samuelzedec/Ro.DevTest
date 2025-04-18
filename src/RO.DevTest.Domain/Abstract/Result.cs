using System.Text.Json.Serialization;

namespace RO.DevTest.Domain.Abstract;

public class Result<T>(T? data, int statusCode = 200, params string[] messages)
{
    [JsonIgnore] public int StatusCode { get; private set; } = statusCode;
    public T? Data { get; set; } = data;
    public string[] Message { get; set; } = messages;

    public static Result<T> Success(T? data, int statusCode = 200, params string[] messages) 
        => new(data, statusCode, messages);
    
    public static Result<T> Failure(int statusCode = 400, params string[] messages) 
        => new(default, statusCode, messages);
}