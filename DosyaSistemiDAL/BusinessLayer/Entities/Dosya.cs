using System;
using System.Collections.Generic;

namespace DosyaSistemiDAL.Businesslayer.Entities
{
    public partial class Dosya
    {
        public Dosya()
        {
            Paylasilanlar = new HashSet<Paylasilanlar>();
        }

        public int DosyaId { get; set; }
        public string DosyaYolu { get; set; }
        public int? DosyaBoyutu { get; set; }
        public string DosyaTipi { get; set; }
        public DateTime? OlusturulmaZamani { get; set; }
        public string DosyaAdi { get; set; }
        public int? ParentId { get; set; }
        public int? KullaniciId { get; set; }
        public bool? Durumu { get; set; }
        public bool? DosyaMi { get; set; }

        public virtual Kullanici Kullanici { get; set; }
        public virtual ICollection<Paylasilanlar> Paylasilanlar { get; set; }
    }
}
