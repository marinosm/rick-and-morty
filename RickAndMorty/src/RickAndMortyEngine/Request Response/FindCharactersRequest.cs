using System.Diagnostics.CodeAnalysis;

namespace RickAndMortyEngine
{
    [ExcludeFromCodeCoverage]
    public class FindCharactersRequest
    {
        public string Name { get; set; }

        public string Status { get; set; }

        public string Species { get; set; }

        public string Type { get; set; }

        public string Gender { get; set; }

        public int? Page { get; set; }
    }
}
