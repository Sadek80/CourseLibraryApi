using CourseLibrary.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Helpers
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ApplySort<T> (this IQueryable<T> source, string OrderBy,
            Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (mappingDictionary == null)
                throw new ArgumentNullException(nameof(mappingDictionary));

            if (string.IsNullOrWhiteSpace(OrderBy))
                return source;

            var orderByString = string.Empty;

            var propertiesAfterSplit = OrderBy.Split(',');

            foreach (var orderByClause in propertiesAfterSplit)
            {
                var trimmedProperty = orderByClause.Trim();
                var orderByDescending = trimmedProperty.EndsWith(" desc");

                var indexOfSpace = trimmedProperty.IndexOf(" ");
                var PropertyName = (indexOfSpace == -1 ? 
                    trimmedProperty : trimmedProperty.Remove(indexOfSpace));

                if (!mappingDictionary.ContainsKey(PropertyName))
                    throw new ArgumentException($"key mapping for {PropertyName} is missing");

                var mappingValues = mappingDictionary[PropertyName];

                if (mappingValues == null)
                    throw new ArgumentNullException(nameof(mappingValues));

                foreach (var mappingValue in mappingValues.DestinationMappingProperties)
                {
                    if (mappingValues.Revert)
                        orderByDescending = !orderByDescending;

                    orderByString = orderByString +
                        (string.IsNullOrWhiteSpace(orderByString) ? string.Empty : ", ")
                        + mappingValue
                        + (orderByDescending ? " descending" : " ascending");
                }
            }

            return source.OrderBy(orderByString);
        }
    }
}
