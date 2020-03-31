using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebshopGraphicsCard.Models;
using WebshopGraphicsCard.Persistence;

namespace WebshopGraphicsCard.Controllers
{
    public class AuthController : Controller
    {
        PersistenceCode persistenceCode = new PersistenceCode();
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string returnUrl, Klant LoginCr)
        {
            if (ModelState.IsValid)
            {
                int IDx = persistenceCode.CheckCredentials(LoginCr);
                if (IDx!= -1)
                {

                    HttpContext.Session.SetString("user", Convert.ToString(IDx));
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, LoginCr.Gebruikersnaam)
                };
                    var userIdentity = new ClaimsIdentity(claims, "SecureLogin");
                    var userPrincipal = new ClaimsPrincipal(userIdentity);
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal,
                        new AuthenticationProperties
                        {
                            ExpiresUtc = DateTime.Today.AddDays(1),
                            IsPersistent = false,
                            AllowRefresh = false
                        });
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.fout = "Ongeldige inlog. Probeer opniew.";
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}