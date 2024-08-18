using Microsoft.EntityFrameworkCore;

namespace HubtelWallets.API.Helpers;

public class PagedList<T> : List<T>
{
    public Metadata Meta { get; set; }

    public PagedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        Meta = new Metadata
        {
            CurrentPage = pageNumber,
            TotalPages = (int)Math.Ceiling(count / (double)pageSize),
            PageSize = pageSize,
            TotalCount = count
        };

        AddRange(items);
    }

    public static async Task<PagedList<T>> ToPageableAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}
