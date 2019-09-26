using DosyaSistemiDAL.Businesslayer.Entities;
using DosyaSistemiDAL.BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DosyaSistemiDAL.BusinessLayer.Abstract
{
    public interface IDosyaDal
    {
        void DBSil(int DosyaId, Dosya dosya);
        void PaylasilanlarTB_Ekle(int DosyaId, string Adi, int PaylasilanKisi, int KimPaylasti, int Yetki, string PaylasilaninYolu, int? PaylasilanlarinParentId, bool? DosyaMi);
        ServiceResult DosyaYukle(DosyaYukleme yuklenecekDosya, string kullaniciId);
        ServiceResult Edits(int DosyaId, string DosyaAdi,string kullaniciId);
        ServiceResult Sil(int DosyaId);
        ServiceResult IcShare(string DosyaId, string KullaniciId, string Checked,string session);
    }
}
