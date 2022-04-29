using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace RickAndMortyEngine
{
    [ExcludeFromCodeCoverage]
    public class Page<T>
    {
        public int TotalCount { get; set; }
        public int NumberOfPages { get; set; }
        public string NextPageUrl { get; set; }
        public string PreviousPageUrl { get; set; }

        public List<T> Data { get; set; }
    }
}
