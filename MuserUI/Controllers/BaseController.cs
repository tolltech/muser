using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tolltech.MuserUI.Common;

namespace Tolltech.MuserUI.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        protected Guid? SafeUserId => Request.HttpContext.User.FindFirst(x => x.Type == Constants.UserIdClaim)?.Value.SafeToGuid();

        protected async Task<T> GetFromBodyAsync<T>()
        {
            using var streamReader = new StreamReader(Request.Body);

            var strBody = await streamReader.ReadToEndAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<T>(strBody);
        }
    }
}