namespace RxApiClient.Caches
{
    public interface ICache<in TKey, TValue> where TValue : IEntity<TKey>
    {
        bool HasCached(TKey key);
        TValue GetCachedItem(TKey key);
        void Put(TValue updatedRating);
    }
}