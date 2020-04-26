using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tolltech.MuserUI.Common;

namespace Tolltech.MuserUI.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        protected Guid? SafeUserId => Request.HttpContext.User.FindFirst(x => x.Type == Constants.UserIdClaim)?.Value.SafeToGuid();
    }
}