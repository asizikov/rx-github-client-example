using System.Collections.Generic;

namespace RxApiClient.Model
{
    internal class RatingModelComparer : IEqualityComparer<RatingModel>
    {
        public bool Equals(RatingModel x, RatingModel y)
        {
            return x.Rating == y.Rating
                   && x.LastModified == y.LastModified
                   && x.Id == y.Id;
        }
        public int GetHashCode(RatingModel obj)
        {
            return obj.GetHashCode();
        }
    }
}