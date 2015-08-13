using System;
using RxApiClient.Model;

namespace RxApiClient
{
    public class RatingResponse
    {
        public bool IsSuccessful { get; set; }
        public RatingModel Data { get; set; }
        public Exception Exception { get; set; }
    }
}