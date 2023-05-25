using KinoSystem.Models.Database;
using Microsoft.AspNetCore.Mvc;
using KinoSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using KinoSystem.Models.Utilities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace KinoSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly KinoDBContext _kinoDBContext;
        private readonly ILogger<HomeController> _logger;
        public HomeController(KinoDBContext db, ILogger<HomeController> logger)
        {
            _logger = logger;
            _kinoDBContext = db;

            Utililies.LoadMovies(_kinoDBContext);

        }
        [Route("/")]
        [Route("/Index")]
        [HttpGet]
        public IActionResult Index()
        {
            //_logger.LogInformation($"\nCount = {_kinoDBContext.People.ToList().Count}\n");
            return View();
        }
        [HttpGet]
        [Route("/SignIn")]
        public IActionResult SignIn() =>
             View((TempData["PersonSignIn"] != null) ? Newtonsoft.Json.JsonConvert.DeserializeObject<Person>(TempData["PersonSignIn"]!.ToString()) : null);

        [HttpPost]
        [Route("/SignIn")]
        public async Task<IActionResult> SingIn(Person person)
        {
            HttpContext.Session.Clear();
            if (ModelState.IsValid)
            {
                person.AccessRight = Models.Utilities.AccessRight.Customer;
                if (_kinoDBContext.People.Where(p => p.Login == person.Login).Count() > 0)
                {
                    HttpContext.Session.SetString("InvalidLogin", "Пользователь с такой почтой уже существует");
                    person.Login = "";
                    TempData["PersonSignIn"] = Newtonsoft.Json.JsonConvert.SerializeObject(person);
                    return RedirectToAction("SignIn");
                }
                await _kinoDBContext.People.AddAsync(person);
                await _kinoDBContext.SaveChangesAsync();
                return RedirectToAction("Index");

            }
            else
            {
                if (ModelState.Values.ToList()[1].Errors.Count > 0)
                {
                    HttpContext.Session.SetString("InvalidLogin", ModelState.Values.ToList()[1].Errors[0].ErrorMessage);
                    person.Login = "";
                }
                if (ModelState.Values.ToList()[5].Errors.Count > 0)
                {
                    HttpContext.Session.SetString("InvalidPhoneNumber", ModelState.Values.ToList()[5].Errors[0].ErrorMessage);
                    person.PhoneNumber = "";
                }
                if (ModelState.Values.ToList()[3].Errors.Count > 0)
                    HttpContext.Session.SetString("InvalidPassword", ModelState.Values.ToList()[3].Errors[0].ErrorMessage);
                TempData["PersonSignIn"] = Newtonsoft.Json.JsonConvert.SerializeObject(person);
                return RedirectToAction("SignIn");
            }


        }
        [HttpGet]
        [Route("/login")]
        public IActionResult LogIn()
            => View();
        [HttpPost]
        [Route("/login")]

        public async Task<IActionResult> LogIn(Person person)
        {
            HttpContext.Session.Clear();
            var pnL = _kinoDBContext.People.AsNoTracking().Where(p => p.Login == person.Login);
            if (pnL.Count() == 0)
            {
                HttpContext.Session.SetString("InvalidData", "Такого аккаунта не существует");
                return RedirectToAction("LogIn");
            }
            var pn = await pnL.FirstAsync();
            if (pn.Password != person.Password)
                HttpContext.Session.SetString("InvalidData", "Неверная почта или пароль");
            else
            {
                var claims = new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, pn.Login),
                        new Claim(ClaimsIdentity.DefaultRoleClaimType, pn.AccessRight.ToString())
                    };
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                return RedirectToAction("Index");
            }
            return RedirectToAction("LogIn");


        }
        [Authorize(Roles = "Administrator")]
        [Route("/test")]
        [HttpGet]
        public IActionResult Test() => StatusCode(200);

        [Authorize]
        [Route("/signout")]
        [HttpGet]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index");

        }
        [Route("/accessdenied")]
        [HttpGet]
        public IActionResult AccessDenied() => View();
        [HttpGet]
        [Route("film/{idFilm:int}")]
        public async Task<IActionResult> Film(int idFilm)
        {
            return View(model: await Utililies.GetMovieByIdAsync(_kinoDBContext, idFilm));
        }
        [HttpGet]
        [Route("film/buyticket/{idFilm:int}")]
        public IActionResult BuyTicket(int idFilm)
        {
            return Content(idFilm.ToString());
        }
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        [Route("/schedule/edit")]
        public IActionResult EditSchedule([FromQuery] DateTime date)
        {
            if (date.Year == 1)
                date = DateTime.Now;
            TempData["date"] = date.ToLongDateString();
            return View();
        }
        [HttpGet]
        [Route("/schedule/view")]
        public IActionResult ViewSchedule([FromQuery] DateTime date)
        {
            if (date.Year == 1)
                date = DateTime.Now;
            TempData["date"] = date.ToLongDateString();
            return View();
        }
    }
}
