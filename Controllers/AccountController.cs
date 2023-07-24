using DBConn_ASPNETCORE.Data;
using DBConn_ASPNETCORE.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace DBConn_ASPNETCORE.Controllers
{
    public class AccountController : Controller
    {

        private readonly Context _context;

        public AccountController(Context context)
        {
            _context = context;
        }

        // GET: AccountController
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            ClaimsPrincipal c = HttpContext.User;
            if(c.Identity != null)
            {
                if (c.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index","Home");
                }
               
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(User u)
        {
            try
            {
                using (SqlConnection conn = new(_context.Conexion))
                {
                    using(SqlCommand cmd = new("sp_validar_usuario",conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add("@UserName", System.Data.SqlDbType.VarChar).Value=u.UserName;
                        cmd.Parameters.Add("@Clave", System.Data.SqlDbType.VarChar).Value = u.Clave;
                        conn.Open();
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            if (dr["UserName"] != null && u.UserName != null) { 
                                List<Claim> c = new List<Claim>() { 
                                    new Claim(ClaimTypes.NameIdentifier, u.UserName)
                                };
                                ClaimsIdentity ci = new(c, CookieAuthenticationDefaults.AuthenticationScheme);
                                AuthenticationProperties p = new();
                                p.AllowRefresh = true;
                                p.IsPersistent = u.KeepActive;

                                if (!u.KeepActive)
                                {
                                    p.ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1);
                                }
                                else
                                {
                                    p.ExpiresUtc = DateTimeOffset.MaxValue;
                                }

                                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(ci),p);
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                ViewBag.Error = "Credenciales Incorrectas";
                            }
                        }

                        conn.Close();
                    }
                    return View();
                }
            }
            catch (System.Exception e)
            {

                ViewBag.Error = e.Message; 
                return View();
            }
        }
      
    }
}
