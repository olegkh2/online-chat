using Microsoft.AspNetCore.Mvc;
using OnlineChat.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace OnlineChat.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLogin data)
        {
            HttpClient client = new HttpClient();
            var response = await client.PostAsJsonAsync<UserLogin>("https://localhost:44331/api/login", data);

            if (!response.IsSuccessStatusCode)
                return View();

            string token = await response.Content.ReadAsStringAsync();

            return RedirectToAction("Index", "Home", new { token = token });
        }

        public IActionResult Index(string token)
        {
            ViewBag.Token = token;

            return View();
        }

        public IActionResult Chat(int id)
        {
            return View(id);
        }
    }
}
