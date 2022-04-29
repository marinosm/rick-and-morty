using RickAndMortyApiClient;

namespace RickAndMortyEngineDefault
{
    internal class LocationInfo
    {
        internal readonly string[] Residents;

        public LocationInfo(LocationDto dto)
        {
            Residents = dto.Residents;
        }
    }
}
