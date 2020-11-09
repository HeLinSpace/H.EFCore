using H.EF.Core.Model;
using Microsoft.AspNetCore.Mvc;
using Videos.IService;

namespace Movies.Controllers
{
    public class HomeController : Controller
    {
        private readonly IVideoManageService _videoManageService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="videoManageService"></param>
        public HomeController(IVideoManageService videoManageService)
        {
            _videoManageService = videoManageService;
        }

        public IActionResult Index()
        {
            return View(@"\Views\ClientManagement\Index.cshtml", _videoManageService.GetMovies(new PageQuery()));
        }
    }
}
