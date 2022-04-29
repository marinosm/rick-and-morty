namespace RickAndMortyApiClient
{
    public class CharacterFilters
    {
        [ApiFilter(Name = "name")]
        public string Name { get; set; }

        [ApiFilter(Name = "status")]
        public string Status { get; set; }

        [ApiFilter(Name = "species")]
        public string Species { get; set; }

        [ApiFilter(Name = "type")]
        public string Type { get; set; }

        [ApiFilter(Name = "gender")]
        public string Gender { get; set; }

        [ApiFilter(Name = "page")]
        public int? Page { get; set; }
    }
}
