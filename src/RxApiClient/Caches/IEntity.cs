namespace RxApiClient.Caches
{
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }
}