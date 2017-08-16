using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

    [Authorize]
    public class AccountController : Controller
    {
    
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                Session.Clear();
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = _dbModel.IsValidLogin(model);
            switch (result)
            {
                case true:
                    var role = _dbModel.GetRole(model.Email);

                    var ident = new ClaimsIdentity(
                                  new[] { 
                                      // adding following 2 claim just for supporting default antiforgery provider
                                      new Claim(ClaimTypes.NameIdentifier, model.Email),
                                      new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),
                                      new Claim(ClaimTypes.Role, role),
                                      new Claim(ClaimTypes.Name,model.Email),
                                  }, DefaultAuthenticationTypes.ApplicationCookie);

                    HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties { IsPersistent = false }, ident);

                    return RedirectToLocal(returnUrl);
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }
        
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _dbModel.RegisterAccount(model);

                if (result)
                {
                    var role = _dbModel.GetRole(model.Email);

                    //Custom Sign In
                    //Creating a new session after registration.
                    var ident = new ClaimsIdentity(
                                  new[] { 
                                      // adding following 2 claim just for supporting default antiforgery provider
                                      new Claim(ClaimTypes.NameIdentifier, model.Email),
                                      new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),
                                      new Claim(ClaimTypes.Role, role),
                                      new Claim(ClaimTypes.Name,model.Email),
                                  }, DefaultAuthenticationTypes.ApplicationCookie);

                    HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties { IsPersistent = false }, ident);

                    return RedirectToAction("Index", "Home");
                }
            }

            return View(model);
        }
    }
