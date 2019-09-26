using DosyaSistemiDAL.Businesslayer.Entities;
using DosyaSistemiDAL.BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DosyaSistemiDAL.BusinessLayer.Abstract
{
    public interface IHomeDal
    {
        void MailTbEkle(string MailAdresi, string Guid, string ip, bool? result);
        void KullaniciTbEkle(string KullaniciAdi, string Soyadi, string KullaniciMaili, string KullaniciSifresi, string Guid, int? YetkiId, int? KimEkledi, long? tc, int? DogumYili);
        ServiceResult Index(Kullanici kullanici);
        ServiceResult KayıtOl(string name, string soyad, string mail, string sifre);
        ServiceResult ParolamıUnuttum(string username,string ipadres);
        ServiceResult ResetPassword(string mail, string tempcode, string code, string sifre1, string sifre2);
        ServiceResult KullanıcıEkle(long tckimlik, string adi, string soyadi, string dogumyili, string mailadresi, string sifre1, string sifre2,string kisi);
    }
}
