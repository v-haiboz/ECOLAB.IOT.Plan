using Newtonsoft.Json;

namespace ECOLAB.IOT.Plan.Entity
{
    public class TData
    {
        /// <summary>
        /// 1 successful, 2 failed.
        /// </summary>
        public int Tag { get; set; } = 0;

        public string? Message { get; set; }

        public string? Description { get; set; }
    }

    public class TData<T> : TData
    {
        public T? Data { get; set;}
    }

    public class TDataPage<T> : TData<T>
    {
        public int? Total { get; set; }

        public int? PageIndex { get; set; }

        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? PageSize { get; set; }
    }
}
