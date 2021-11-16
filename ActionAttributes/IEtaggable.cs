using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.Api.ActionAttributes
{
    public interface IEtaggable
    {
        string GetEtag();
    }
}
