using System.Text.Json.Serialization;

namespace RO.DevTest.Domain.Abstract;

public class Result<T>(T? data, bool isSuccess,int statusCode = 200,params string[] messages)
{
    [JsonIgnore] public int StatusCode { get; private set; } = statusCode;
    [JsonIgnore] public bool IsSuccess { get; private set; } = isSuccess;
    public T? Data { get; set; } = data;
    public string[] Message { get; set; } = messages;

    public static Result<T> Success(T? data, int statusCode = 200, params string[] messages) 
        => new(data, true, statusCode, messages);
    
    public static Result<T> Failure(int statusCode = 400, params string[] messages) 
        => new(default, false, statusCode, messages);
}