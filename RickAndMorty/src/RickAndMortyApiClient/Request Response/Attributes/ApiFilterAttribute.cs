using System;

namespace RickAndMortyApiClient
{
    public class ApiFilterAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
