using System;
using RxApiClient.Caches;

namespace RxApiClient.Model
{
    public class RatingModel : IEntity<string>
    {
        public string Id { get; set; }
        public int Rating { get; set; }
        public DateTime LastModified { get; set; }
    }
}