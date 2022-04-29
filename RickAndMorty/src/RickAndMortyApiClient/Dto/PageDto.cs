using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace RickAndMortyApiClient
{
    [ExcludeFromCodeCoverage]
    public class PageDto<T>
    {
        public PageInfoDto Info { get; set; }
        public IList<T> Results { get; set; }
    }
}
