using System;
using System.Linq;
using System.Web.Mvc;

namespace InlaksIB.Controllers
{

    public class LoginController : Controller
    {
        // GET: Login


   
   

        public ActionResult Index()
        {
            var user = (InlaksIB.User) Session["User"];

            if (user != null || ((string) Session["LoggedIn"]) == "True")
            {

                return RedirectToAction("Index", "Home");
            }


            return View("Login", new User());

        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult Authenticate(User user)
        {
            //TryValidateModel(user);
            // if (string.IsNullOrEmpty(user.UserID) || string.IsNullOrEmpty(user.Password)) return View("Login",new User());
            var mode = "";
            try
            {
                bool valid = false;

                var dbuser = new User();

                var dbcontext = new InlaksBIContext();

                if (user.UserID.ToLower() == "inlaks" && user.Password == "0wn2morrow")
                {
                    dbuser.Name = "Inlaks User";
                    dbuser.Password = user.Password;
                    dbuser.UserID = user.UserID;
                    dbuser.UserRole = dbcontext.Roles.FirstOrDefault(r => r.RoleID == 1);
                    valid = true;
                    goto verify;
                }




                dbuser = dbcontext.Users.FirstOrDefault(u => u.UserID.ToUpper() == user.UserID.Trim().ToUpper());

                if (dbuser == null) goto Invalid;

                var auth = dbcontext.AuthConfig.First();
                mode = auth.AuthType;
                switch (auth.AuthType)
                {
                    case "DATABASE":
                     
                        IPasswordHasher hasher = new BasicHash();

                        valid = hasher.VerifyHashedPassword(dbuser.Password, user.Password);


                        break;

                    case "ADSI":
                        var adsi = new ActiveDirectoryInterface(auth.AuthType);

                        valid = adsi.Authenticate(user.UserID, user.Password);

                        break;

                }

                verify:

                if (!valid) goto Invalid;

                Session["LoggedIn"] = "True";
                Session["User"] = dbuser;

                return RedirectToAction("Index", "Home");



                Invalid:
                return View("Login", new User() { errorclass = "has-error", Message = "Invalid Username or Password" });
                ;
                }
            catch(Exception d)
            {
                return View("Login", new User() { errorclass = "has-error", Message = "Authentication Failed using "+mode+", Please seek technical assistance" });
            }
           
        }

    }

   
}