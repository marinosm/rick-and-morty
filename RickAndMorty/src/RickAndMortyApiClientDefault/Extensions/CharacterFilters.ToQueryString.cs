using RickAndMortyApiClient;
using System.Linq;
using System.Text;

namespace RickAndMortyApiClientDefault
{
    public static partial class CharacterFiltersExtensions
    {
        public static StringBuilder ToQueryStringBuilder(this CharacterFilters filters)
        {
            var builder = new StringBuilder();

            foreach (var property in filters.GetType().GetProperties())
            {
                var propertyValue = property.GetValue(filters, null);
                if (propertyValue != null)
                {
                    var attributes = property.GetCustomAttributesData();
                    var apiFilterAttribute = attributes.First(i => i.AttributeType == typeof(ApiFilterAttribute));
                    var apiFilterNameArgument = apiFilterAttribute.NamedArguments.First(i => i.MemberName == nameof(ApiFilterAttribute.Name));
                    var apiFilterName = apiFilterNameArgument.TypedValue.Value;

                    builder.Append(apiFilterName);
                    builder.Append('=');
                    builder.Append(propertyValue);
                    builder.Append('&');
                }
            }

            // remove last & if any
            if (builder.Length > 0)
            {
                builder.Remove(builder.Length - 1, 1);
            }

            return builder;
        }
    }
}
