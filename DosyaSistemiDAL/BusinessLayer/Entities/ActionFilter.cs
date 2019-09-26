using System;
using System.Collections.Generic;

namespace DosyaSistemiDAL.Businesslayer.Entities
{
    public partial class ActionFilter
    {
        public int Sira { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string IpAdresi { get; set; }
        public string LinkNumaralari { get; set; }
        public DateTime? Tarih { get; set; }
        public int? KullaniciKim { get; set; }
        public string Bilgi { get; set; }
    }
}
