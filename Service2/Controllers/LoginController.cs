using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DosyaSistemiDAL.Businesslayer.Entities;
using DosyaSistemiDAL.BusinessLayer.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace Service2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly IHomeDal homeDal;
        public LoginController(IHomeDal homeDal)
        {
            this.homeDal = homeDal;
        }
        [HttpPost]
        public IActionResult Giris(Kullanici kullanici)
        {
            var result = homeDal.Index(kullanici);
            return Json(result.ExternalData);
        }
    }
}