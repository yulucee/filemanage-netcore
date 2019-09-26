using System;
using System.Collections.Generic;
using System.Text;

namespace DosyaSistemiDAL.BusinessLayer.Models
{
    public class ServiceResult
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public dynamic Data { get; set; }
        public dynamic ExternalData { get; set; }
    }
}
