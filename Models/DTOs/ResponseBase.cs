namespace DotNet8.WebApi.Models.DTOs;

public class ResponseBase
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

