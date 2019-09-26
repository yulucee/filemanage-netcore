using System;
using System.Collections.Generic;

namespace Service2.Models
{
    public partial class Mail
    {
        public string MailAdresi { get; set; }
        public string Guid { get; set; }
        public DateTime? Tarih { get; set; }
        public string Ip { get; set; }
        public bool? Result { get; set; }
        public int Id { get; set; }
    }
}
