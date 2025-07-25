namespace Soda.CrossCutting.ResultObjects;

public class Result<T>
{
    public bool Success { get; set; }
    public List<string>? Messages { get; set; }
    public T? Data { get; set; }
}