using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Helpers
{
    public static class ObjectExtensions
    {
        public static ExpandoObject ShapeData<TSource>(this TSource source, string fields)
        {
            if(source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var expandoObject = new ExpandoObject();

            if (string.IsNullOrWhiteSpace(fields))
            {
                var propertiesInfos = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (var propertyInfo in propertiesInfos)
                {
                    var propertyValue = propertyInfo.GetValue(source);

                    ((IDictionary<string, object>)expandoObject)
                        .Add(propertyInfo.Name, propertyValue);
                }
            }
            else
            {
                var splittedProperties = fields.Split(',');

                foreach (var property in splittedProperties)
                {
                    var propertyName = property.Trim();

                    var propertyInfo = typeof(TSource).GetProperty(propertyName,
                        BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo == null)
                    {
                        throw new Exception($"Property {propertyName} wasn't found " +
                            $"on {typeof(TSource)}");
                    }

                    var propertyValue = propertyInfo.GetValue(source);


                    ((IDictionary<string, object>)expandoObject)
                        .Add(propertyInfo.Name, propertyValue);
                }
            }

            return expandoObject;
            
        }
    }
}
