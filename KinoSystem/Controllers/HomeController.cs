using KinoSystem.Models.Database;
using Microsoft.AspNetCore.Mvc;
using KinoSystem.Models;

namespace KinoSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly KinoDBContext? _kinoDBContext;
        private readonly ILogger<HomeController> _logger;
        public HomeController(KinoDBContext db, ILogger<HomeController> logger)
        {
            _logger = logger;
            _kinoDBContext = db;

        }
        [Route("/")]
        [Route("/Index")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Route("/SignIn")]
        public IActionResult SignIn() =>
             View(new Person());

        [HttpPost]
        [Route("/SignIn")]
        public async Task<IActionResult> SingIn(Person person)
        {
            if (ModelState.IsValid)
            {
                person.AccessRight = Models.Utilities.AccessRight.Customer;
                await _kinoDBContext!.People.AddAsync(person);
                await _kinoDBContext!.SaveChangesAsync();
                return RedirectToAction("Index");

            }
            else
            {
                return RedirectToAction("SignIn");
            }


        }
    }
}
