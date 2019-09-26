using System;
using System.Collections.Generic;

namespace DosyaSistemiDAL.Businesslayer.Entities
{
    public partial class Kullanici
    {
        public Kullanici()
        {
            Dosya = new HashSet<Dosya>();
            Paylasilanlar = new HashSet<Paylasilanlar>();
        }

        public int KullaniciId { get; set; }
        public string KullaniciAdi { get; set; }
        public string KullaniciSoyadi { get; set; }
        public string KullaniciMaili { get; set; }
        public string KullaniciSifresi { get; set; }
        public string Guid { get; set; }
        public int? YetkiId { get; set; }
        public int? KimEkledi { get; set; }
        public long? Tc { get; set; }
        public int? DogumYili { get; set; }

        public virtual ICollection<Dosya> Dosya { get; set; }
        public virtual ICollection<Paylasilanlar> Paylasilanlar { get; set; }
    }
}
