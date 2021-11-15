using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.Api.ActionConstraints
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class RequestHeaderMatchesTypeAttribute : Attribute, IActionConstraint
    {
        private readonly string _requestHeaderToMatch;
        private readonly MediaTypeCollection mediaTypes = new MediaTypeCollection();

        public RequestHeaderMatchesTypeAttribute(string requestHeaderToMatch, 
            string mediaType, params string[] otherMediaTypes)
        {
            this._requestHeaderToMatch = requestHeaderToMatch ?? 
                throw new ArgumentNullException(nameof(requestHeaderToMatch));

            if(MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            {
                mediaTypes.Add(parsedMediaType);
            }
            else
            {
                throw new ArgumentException(nameof(mediaType));
            }

            foreach (var type in otherMediaTypes)
            {
                if(MediaTypeHeaderValue.TryParse(type, out MediaTypeHeaderValue parsedOtherMediaType))
                {
                    mediaTypes.Add(parsedOtherMediaType);
                }
                else
                {
                    throw new ArgumentException(nameof(type));
                }
            }

        }

        public int Order => 0;

        public bool Accept(ActionConstraintContext context)
        {
            var requestHeader = context.RouteContext.HttpContext.Request.Headers;

            if (!requestHeader.ContainsKey(_requestHeaderToMatch))
                return false;

            var requestedMediaType = new MediaType(requestHeader[_requestHeaderToMatch]);

            foreach (var type in mediaTypes)
            {
                var parsedMediaType = new MediaType(type);
                if (requestedMediaType.Equals(parsedMediaType))
                    return true;
            }

            return false;
        }
    }
}
