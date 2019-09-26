using DosyaSistemiDAL.Businesslayer.Entities;
using System.Collections.Generic;

namespace DosyaSistemiDAL.BusinessLayer.Models
{
    public class LinkMultipleClass
    {
        public IEnumerable<Kullanici> kullanicilar { get; set; }
        public IEnumerable<Dosya> dosyalar { get; set; }
        public IEnumerable<Paylasilanlar> paylasilanlar { get; set; }
        public IEnumerable<ShareLink> linkler { get; set; }
    }
}
