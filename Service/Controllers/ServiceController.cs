using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DosyaSistemiDAL.Businesslayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        // GET: api/Service
        [HttpGet]
        public List<Dosya> Get()
        {
            using (Filesystem_ibrahimContext db = new Filesystem_ibrahimContext())
            {
                return db.Dosya.ToList();
            }
        }

        // GET: api/Service/5
        [HttpGet("{id}", Name = "Get")]
        public Kullanici Get(int id)
        {
            using (Filesystem_ibrahimContext db = new Filesystem_ibrahimContext())
            {
                return db.Kullanici.Single(x => x.KullaniciId == id);
            }
        }

        // POST: api/Service
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Service/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
