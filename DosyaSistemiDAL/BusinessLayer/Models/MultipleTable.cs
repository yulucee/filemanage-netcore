using DosyaSistemiDAL.Businesslayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DosyaSistemiDAL.BusinessLayer.Models
{
    public class MultipleTable
    {
        public IEnumerable<Dosya> dosyalar { get; set; }
        public IEnumerable<Paylasilanlar> paylasilanlar { get; set; }
        public IEnumerable<Kullanici> kullanicilar { get; set; }
    }
}
