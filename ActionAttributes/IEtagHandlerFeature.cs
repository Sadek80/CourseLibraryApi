using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.Api.ActionAttributes
{
    public interface IEtagHandlerFeature
    {
        bool NonMatch(IEtaggable entity);
        bool NonMatch(string entityEtag);
    }
}
