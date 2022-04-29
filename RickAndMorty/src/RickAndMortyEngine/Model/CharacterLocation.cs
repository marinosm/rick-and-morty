using System.Diagnostics.CodeAnalysis;

namespace RickAndMortyEngine
{
    [ExcludeFromCodeCoverage]
    public class CharacterLocation
    {
        /// <summary>
        /// Name to the character's location.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Link to the character's location.
        /// </summary>
        public string Url { get; set; }
    }
}
