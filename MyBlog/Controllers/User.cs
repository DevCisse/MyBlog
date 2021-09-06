using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Controllers
{

    [Route("users")]
    public class User : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
