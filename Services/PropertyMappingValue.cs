using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Services
{
    public class PropertyMappingValue
    {
        public IEnumerable<string> DestinationMappingProperties { get; private set; }
        public bool Revert { get; private set; }

        public PropertyMappingValue(IEnumerable<string> properties, bool revert = false)
        {
            DestinationMappingProperties = properties ?? throw new ArgumentNullException(nameof(properties));
            Revert = revert;
        }
    }
}
