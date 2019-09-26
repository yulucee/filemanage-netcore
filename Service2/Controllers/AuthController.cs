using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DosyaSistemiDAL.Businesslayer.Entities;
using DosyaSistemiDAL.BusinessLayer.Abstract;
using DosyaSistemiDAL.BusinessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Service2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IHomeDal homeDal;
        public AuthController(IHomeDal homeDal)
        {
            this.homeDal = homeDal;
        }
        [HttpPost("login")]
        public ActionResult Login(Kullanici kullanici)
        {
            string securityKey = "this_is_our_supper_long_security_key_for_token_validation_project_2018_09_07$smesk.in";

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));

            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                    issuer: "smesk.in",
                    audience: "readers",
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: signingCredentials
                );

            var result = homeDal.Index(kullanici);
            if(result.Code == 1)
            {
                var model = (ServiceResult)result;
                model.Data = new JwtSecurityTokenHandler().WriteToken(token);
                return Json(model);
            }
            else
            {
                return Json(result);
            }
        }
        [HttpPost("listele")]
        [Authorize]
        public IActionResult Listele(Kullanici kullanici)
        {
            var result = homeDal.Index(kullanici);
            return Json(result);
        }
    }
}