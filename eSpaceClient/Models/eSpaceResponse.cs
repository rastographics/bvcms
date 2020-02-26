using System.Collections.Generic;

namespace eSpace.Models
{
    internal class eSpaceResponse<T>
    {
        public bool IsSuccessStatusCode { get; set; }

        public string Message { get; set; }

        public List<T> Data { get; set; } = default(List<T>);
    }
}
