using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.Api.ActionAttributes
{
    public class EtagHandlerFeature : IEtagHandlerFeature
    {
        private readonly IHeaderDictionary _header;

        public EtagHandlerFeature(IHeaderDictionary header)
        {
            this._header = header;
        }
        public bool NonMatch(IEtaggable entity)
        {
            if (!_header.Keys.Contains("If-Non-Match"))
                return true;

            var etagsFromHeader = _header["If-Non-Match"].ToString();

            var entityEtag = entity.GetEtag();
            if (string.IsNullOrEmpty(entityEtag))
                return true;

            if (!entityEtag.Contains('"'))
                entityEtag = $"\"{entityEtag}\"";

            return !etagsFromHeader.Contains(entityEtag);
        }

        public bool NonMatch(string entityEtag)
        {
            if (!_header.Keys.Contains("If-Non-Match"))
                return true;

            var etagsFromHeader = _header["If-Non-Match"].ToString();

            if (string.IsNullOrEmpty(entityEtag))
                return true;

            if (!entityEtag.Contains('"'))
                entityEtag = $"\"{entityEtag}\"";

            return !etagsFromHeader.Contains(entityEtag);
        }
    }
}
