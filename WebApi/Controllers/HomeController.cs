using Microsoft.AspNetCore.Mvc;
using System.Linq;
using H.EF.Core.Model;
using System.Collections.Generic;
using WebApi.Entity;
using H.EF.Core.Extensions;
using System;
using H.EF.Core.Repositories;
using WebApi;

namespace ZhangDianGuanJia.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitRepository _repository;

        public HomeController(IUnitRepository repository)
        {
            _repository = repository;
        }

        public ActionResult Index()
        {
            return Ok("123");
        }

        public ActionResult add([FromBody]SingleModel data)
        {
            var labels = data.key.Split(",").Distinct().ToList();
            var insert = labels.Select(s =>
            new law_label
            {
                Id = GuidExtend.NewGuid(),
                created_by = "2A63A249661D4C468EAB-69F7FA06C71B",
                updated_by = "2A63A249661D4C468EAB-69F7FA06C71B",
                created_time = DateTime.Now,
                updated_time = DateTime.Now,
                is_delete = 0,
                label = s,
                type= "FINANCIAL"
            }).ToList();

            _repository.BulkInsert<law_label>(insert);

            return Ok();
        }
    }
}
