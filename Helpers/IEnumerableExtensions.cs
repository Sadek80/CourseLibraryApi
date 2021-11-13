using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Helpers
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<ExpandoObject> ShapeData<TSource>(this IEnumerable<TSource> source, string fields)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            
            // ExpandoObjectList that will be returned
            var expandoObjectList = new List<ExpandoObject>();

            // PropertyInfoList that will hold all the properties wanted to be returned
            var propertyInfoList = new List<PropertyInfo>();

            // if no shaped data ordered, get all the properties info of TSource
            // and add them on the propertyInfoList
            if (string.IsNullOrWhiteSpace(fields))
            {
                var propertyInfos = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                propertyInfoList.AddRange(propertyInfos);
            }

            // if shaped data ordered, get the properties names then get their propertyInfo
            // and adds them on propertyInfoList
            else
            {
                var splittedProperties = fields.Split(',');

                foreach (var splittedProperty in splittedProperties)
                {
                    var propertyName = splittedProperty.Trim();

                    var propertyInfo = typeof(TSource).GetProperty(propertyName,
                        BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo == null)
                        throw new Exception($"Propert {propertyName} doesn't exist on {typeof(TSource)}");

                    propertyInfoList.Add(propertyInfo);
                }
            }

            // foreach object on the source, get the property ordered values
            // and add them on an ExpandoObject 
            // then add this ExpandoObject to expandoObjectList
            foreach (var sourceObject in source)
            {
                var dataShapedObject = new ExpandoObject();

                foreach (var propertyInfo in propertyInfoList)
                {
                    var propertyValue = propertyInfo.GetValue(sourceObject);

                    ((IDictionary<string, object>)dataShapedObject)
                        .Add(propertyInfo.Name, propertyValue);
                }

                expandoObjectList.Add(dataShapedObject);
            }

            return expandoObjectList;
        }
    }
}
