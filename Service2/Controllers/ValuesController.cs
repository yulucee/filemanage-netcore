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
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public ActionResult <IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }
    }
}
