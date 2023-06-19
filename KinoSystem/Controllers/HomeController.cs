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
        private readonly IConfiguration _configuration;
        public HomeController(KinoDBContext db, ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _kinoDBContext = db;
            _configuration = configuration;

        }
        [Route("/")]
        [Route("/Index")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            await Utililies.LoadMovies(_kinoDBContext);

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
                        new Claim(ClaimsIdentity.DefaultNameClaimType, pn.IdPerson.ToString()),
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
        [Authorize]
        [HttpGet]
        [Route("film/buyticket/{idSession:int}")]
        public async Task<IActionResult> BuyTicket(int idSession)
        {
            return View(await Utililies.GetSessionByIdAsync(_kinoDBContext, idSession));
        }
        [Authorize]
        [HttpPost]
        [Route("film/buyticket/{idSession:int}")]
        public async Task<IActionResult> BuyTicket(int idSession, Session s)
        {
            var takenSeats = HttpContext.Request.Cookies["seats"];
            if (string.IsNullOrEmpty(takenSeats))
            {
                return RedirectToAction("buyticket", idSession);
            }
            var session = await Utililies.GetSessionByIdAsync(_kinoDBContext, idSession);
            var seatsSplit = takenSeats.Split("|").Select(s => int.Parse(s)).ToList();
            var seats = await Utililies.GetSeatsByIdsAsync(_kinoDBContext, seatsSplit);
            var person = await Utililies.GetPersonByIdAsync(_kinoDBContext, int.Parse(HttpContext.User.FindFirst(ClaimsIdentity.DefaultNameClaimType).Value));
            foreach (var seat in seats)
                seat.Person = person;
            _kinoDBContext.Seats.UpdateRange(seats);
            await _kinoDBContext.SaveChangesAsync();
            for (int i =0; i < seats.Count; i++)
            {
                Purhaces purhace = new Purhaces();
                purhace.Session = session;
                purhace.Person = person;
                purhace.PurchaseTime = DateTime.Now;
                await _kinoDBContext.Purhaces.AddAsync(purhace);
                await _kinoDBContext.SaveChangesAsync();
            }     
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        [Route("/schedule/edit")]
        public IActionResult EditSchedule([FromQuery] DateTime date)
        {
            if (date.Year == 1)
                date = DateTime.Now;
            TempData["date"] = date.ToLongDateString();
            HttpContext.Session.Remove("SuccessAdd");

            return View();
        }
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [Route("/schedule/edit")]
        public async Task<IActionResult> EditSchedule(AddSession addsession)
        {
            if (addsession.Time.Year == 1)           
                HttpContext.Session.SetString("InvalidTime", "Вы не выбрали время");           
            if (addsession.Price == 0)         
                HttpContext.Session.SetString("InvalidPrice", "Вы не ввели цену");     
            else
            {
                Session session = new Session();
                var movie = await Utililies.GetMovieByIdAsync(_kinoDBContext, addsession.IdMovie);
                session.Film = movie;
                session.Hall = Utililies.GetHall(addsession.HallType);
                session.Price = addsession.Price;
                await _kinoDBContext.Sessions.AddAsync(session);
                await _kinoDBContext.SaveChangesAsync();
                Schedule schedule = new Schedule();
                schedule.Session = session;
                var date = DateTime.Parse(HttpContext.Session.GetString("date"));
                var start = new DateTime(date.Year, date.Month, date.Day, addsession.Time.Hour, addsession.Time.Minute, addsession.Time.Second);
                schedule.Start = start;
                var end = start.AddMinutes(double.Parse(movie.MovieLength.ToString()));
                schedule.End = end;
                HttpContext.Session.SetString("SuccessAdd", "фильм добавлен");

                await _kinoDBContext.Schedules.AddAsync(schedule);
                await _kinoDBContext.SaveChangesAsync();
            }
            return RedirectToAction("EditSchedule");
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
        [Authorize]
        [HttpGet]
        [Route("/statistics")]
        public async Task<IActionResult> Statistics() => View(await Utililies.GetLastPurchacesAsync(_kinoDBContext,100));
        [HttpGet]
        [Route("cashier/add")]
        [Authorize(Roles = "Administrator")]
        public IActionResult AddCahier() => View("AddCashireView");
        [HttpPost]
        [Route("cashier/add")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AddCashier(Person cashier)
        {
            return RedirectToAction("Index");
        }
    }
}
