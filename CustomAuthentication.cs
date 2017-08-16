     var ident = new ClaimsIdentity(
          new[] { 
              // adding following 2 claim just for supporting default antiforgery provider
              new Claim(ClaimTypes.NameIdentifier, username),
              new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),

              new Claim(ClaimTypes.Name,username),

              // optionally you could add roles if any
              new Claim(ClaimTypes.Role, "RoleName"),
              new Claim(ClaimTypes.Role, "AnotherRole"),

          },
          DefaultAuthenticationTypes.ApplicationCookie);

        HttpContext.GetOwinContext().Authentication.SignIn(
           new AuthenticationProperties { IsPersistent = false }, ident);

-----------------------------------------------------------------------------------------
     
        public static IIdentity GetCurrentUserIdentity()
        {
            return HttpContext.Current.User.Identity;
        }

        public static bool IsUserAuthenticated()
        {
            return HttpContext.Current.User.Identity.IsAuthenticated;
        }

        public static bool IsAdmin()
        {
            return HttpContext.Current.User.IsInRole("admin");
        }

        public static List<string> Roles()
        {
            return  ((ClaimsIdentity)HttpContext.Current.GetOwinContext().Authentication.User.Identity).Claims
                           .Where(c => c.Type == ClaimTypes.Role)
                           .Select(c => c.Value)
                           .ToList();
        }

        public static string GetCurrentUserRole()
        {
            return Roles()[0];
        }
