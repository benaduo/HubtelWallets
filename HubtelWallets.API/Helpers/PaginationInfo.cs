namespace HubtelWallets.API.Helpers;

public class PaginationInfo<T>
{
    public IEnumerable<T> Data { get; set; }
    public Metadata Meta { get; set; }
}
