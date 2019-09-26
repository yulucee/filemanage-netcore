using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace DosyaSistemiDAL.BusinessLayer.Models
{
    public class DosyaYukleme
    {
        public IList<IFormFile> Files { get; set; }
    }
}
