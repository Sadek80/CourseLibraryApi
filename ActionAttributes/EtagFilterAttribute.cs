using CourseLibrary.Api.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CourseLibrary.Api.ActionAttributes
{
    /// <summary>
    /// It's not a right implementation of the caching validation model
    /// as it's needed to be executed before the actual mvc middleware is executed
    /// it's just a proof of concept
    /// </summary>
    public class ETagFilterAttribute : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            context.HttpContext.Features.Set<IEtagHandlerFeature>
                (new EtagHandlerFeature(context.HttpContext.Request.Headers));

            var executed = await next();

            var result =  executed.Result as ObjectResult;
            var etag =  (result.Value).GetEtag();

            if(string.IsNullOrEmpty(etag)) return;

            if (!etag.Contains('"'))
            {
                etag = $"\"{etag}\"";
            }

            context.HttpContext.Response.Headers.Add("ETag", etag);

            if (result.StatusCode == 304)
                result.Value = null;

            return;

        }
    }
}
