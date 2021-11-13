using CourseLibrary.Api.Models.Core.Domain;
using CourseLibrary.Api.Models.DTOs.AuthorDtos;
using CourseLibrary.Api.Models.DTOs.CourseDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

        // Mapping properties from AuthorDto to Author 
        private Dictionary<string, PropertyMappingValue> _athorsPropertiesMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "Id", new PropertyMappingValue(new List<string>() { "Id" }) },
                { "MainCategory", new PropertyMappingValue(new List<string>() { "MainCategory" }) },
                { "Age", new PropertyMappingValue(new List<string>() { "DateOfBirth" }, true) },
                { "Name", new PropertyMappingValue(new List<string>() { "FirstName", "LastName" })}
            };

        private Dictionary<string, PropertyMappingValue> _coursesPropertiesMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "Id", new PropertyMappingValue(new List<string>() { "Id" }) },
                { "Title", new PropertyMappingValue(new List<string>() { "Title" }) },
                { "Description", new PropertyMappingValue(new List<string>() { "Description" }) }
            };

        public PropertyMappingService()
        {
            _propertyMappings.Add(new PropertyMapping<AuthorsDto, Author>(_athorsPropertiesMapping));
            _propertyMappings.Add(new PropertyMapping<CoursesDto, Course>(_coursesPropertiesMapping));
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            var mappingType = _propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();

            if (mappingType.Count() == 1)
            {
                return mappingType.First().MappingDictionary;
            }

            throw new Exception($"Cannot Find Exact mapping type for this instance" +
                $"for" + typeof(TSource) + ", " + typeof(TDestination));
        }

        public bool ValidMappingExistsFor<TSource, TDestination>(string orderBy)
        {
            if (string.IsNullOrWhiteSpace(orderBy))
                return true; 

            var mappingDictionary = GetPropertyMapping<TSource, TDestination>();

            var propertiesAfterSplit = orderBy.Split(',');

            foreach (var orderByClause in propertiesAfterSplit)
            {
                var trimmedProperty = orderByClause.Trim();

                var indexOfSpace = trimmedProperty.IndexOf(" ");
                var PropertyName = (indexOfSpace == -1 ?
                    trimmedProperty : trimmedProperty.Remove(indexOfSpace));

                if (!mappingDictionary.ContainsKey(PropertyName))
                    return false;
            }
            return true;
        }
    }
}
