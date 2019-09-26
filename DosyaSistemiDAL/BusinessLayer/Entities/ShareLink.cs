using System;
using System.Collections.Generic;

namespace DosyaSistemiDAL.Businesslayer.Entities
{
    public partial class ShareLink
    {
        public int Id { get; set; }
        public int? DosyaId { get; set; }
        public string DosyaAdi { get; set; }
        public bool? DosyaMi { get; set; }
        public int? PaylastigimKisiler { get; set; }
        public string Guid { get; set; }
        public int? ToplamOnizleme { get; set; }
        public bool? Durumu { get; set; }
        public bool? Global { get; set; }
        public DateTime? YaratilmaZamani { get; set; }
    }
}
