namespace Soda.CrossCutting.ResultObjects;

public class Pagination<T>(IList<T>? items, int totalItems, int pageNumber, int pageSize)
{
    public IEnumerable<T>? Items { get; set; } = items;
    public int TotalItems { get; set; } = totalItems;
    public int PageNumber { get; set; } = pageNumber;
    public int PageSize { get; set; } = pageSize;
    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalItems / PageSize);
}