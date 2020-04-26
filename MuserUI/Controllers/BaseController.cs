using System;
using Microsoft.AspNetCore.Mvc;
using Tolltech.MuserUI.Common;

namespace Tolltech.MuserUI.Controllers
{
    public class BaseController : Controller
    {
        protected Guid? SafeUserId => Request.HttpContext.User.FindFirst(x => x.Type == Constants.UserIdClaim)?.Value.SafeToGuid();
    }
}