using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tolltech.Muser.Settings;
using Tolltech.MuserUI.Authentications;
using Tolltech.MuserUI.Common;
using Tolltech.MuserUI.Models;

namespace Tolltech.MuserUI.Controllers
{
    [Route("account")]
    [AllowAnonymous]
    public class AccountController : BaseController
    {
        private readonly UserContext db;
        private readonly ICryptoService cryptoService;

        public AccountController(UserContext context, ICryptoService cryptoService)
        {
            db = context;
            this.cryptoService = cryptoService;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return View((object)HttpContext.User.Identity.Name);
            }

            return RedirectToAction("Login");
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var hashPassword = cryptoService.EncryptSHA256(model.Password);
                var user = await db.Users.FirstOrDefaultAsync(u =>
                    u.Email == model.Email && u.Password == hashPassword);
                if (user != null)
                {
                    await Authenticate(model.Email, user.Id).ConfigureAwait(true);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }

            return View(model);
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email).ConfigureAwait(true);
                if (user == null)
                {
                    var hashPassword = cryptoService.EncryptSHA256(model.Password);
                    var newUser = new User {Email = model.Email, Password = hashPassword, Id = Guid.NewGuid() };
                    await db.Users.AddAsync(newUser).ConfigureAwait(true);
                    await db.SaveChangesAsync().ConfigureAwait(true);

                    await Authenticate(model.Email, newUser.Id).ConfigureAwait(true);

                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }

            return View(model);
        }

        private async Task Authenticate(string userName, Guid userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
                new Claim(Constants.UserIdClaim, userId.ToString()),
            };
            var id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id))
                .ConfigureAwait(false);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(true);
            return RedirectToAction("Login", "Account");
        }
    }
}