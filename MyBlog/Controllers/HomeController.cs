using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyBlog.Data;
using MyBlog.Models;
using MyBlog.Services;
using MyBlog.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;
using Microsoft.EntityFrameworkCore;

namespace MyBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBlogEmailSender blogEmailSender;
        private readonly ApplicationDbContext context;

        public HomeController(ILogger<HomeController> logger,IBlogEmailSender blogEmailSender,ApplicationDbContext context)
        {
            _logger = logger;
            this.blogEmailSender = blogEmailSender;
            this.context = context;
        }

        public  async Task<IActionResult> Index(int? page )
        {
            var pageNumber = page ?? 1;
            var pageSize = 4;


            var posts = context.Posts.Where(p => p.ReadyStatus == Models.Enums.ReadyStatus.ProductionReady)
                .OrderByDescending(p => p.Created)
                .Include(p => p.Blog)
                .Include(p => p.Comments)
                .Include(p => p.BlogUser)
                .ToPagedListAsync(pageNumber,pageSize);
            return View(await posts);
        }

        public IActionResult Default()
        {
            return View();
        }


        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Contact(ContactMe model)
        {

            await blogEmailSender.SendContactEmailAsync(model.Email, model.Name, model.Subject, model.Message);
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
