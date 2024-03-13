using HomeworkMar11.Data;
using HomeworkMar11.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace HomeworkMar11.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private string _conStr = @"Data Source=.\sqlexpress; Initial Catalog=ImageShare;Integrated Security=True;";

        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Upload(IFormFile imageFile, string password)
        {
            var fileName = $"{Guid.NewGuid()}-{imageFile.FileName}";
            var fullImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);

            using FileStream fs = new(fullImagePath, FileMode.Create);
            imageFile.CopyTo(fs);

            var repo = new ImageRepository(_conStr);
            var i = new Image
            {
                ImagePath = fileName,
                Password = password
            };
            repo.Add(i);

            Console.WriteLine(fullImagePath);

            return View(new OnceUploadedViewModel
            {
                ImageID = i.Id,
                Password = password
            });
        }
        public IActionResult ViewImage(int ID)
        {
            var repo = new ImageRepository(_conStr);

            var vm = new ViewImageViewModel
            {
                Image = repo.GetImageByID(ID)
            };
            
            if (HttpContext.Session.Get<List<int>>("imageIds") == null)
            {
                HttpContext.Session.Set("imageIds", new List<int>());
            }
            else
            {
                if (!vm.IncorrectPassword)
                {
                    repo.AddView(ID);
                }
                vm.IncorrectPassword = true;
            }
            vm.ImageIDs = HttpContext.Session.Get<List<int>>("imageIds");

            return View(vm);
        }
        [HttpPost]
        public IActionResult ViewImage(int ID, string password)
        {
            var repo = new ImageRepository(_conStr);

            var image = repo.GetImageByID(ID);

            if (password == image.Password)
            {
                var ids = HttpContext.Session.Get<List<int>>("imageIds");
                ids.Add(ID);
                HttpContext.Session.Set("imageIds", ids);
            }
            return Redirect($"/home/viewimage?id={ID}");
        }
    }
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default(T) :
                JsonSerializer.Deserialize<T>(value);
        }
    }
}