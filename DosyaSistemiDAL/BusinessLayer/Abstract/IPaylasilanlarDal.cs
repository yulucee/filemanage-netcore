using DosyaSistemiDAL.BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DosyaSistemiDAL.BusinessLayer.Abstract
{
    public interface IPaylasilanlarDal
    {
        void DosyaTB_Ekle(string DosyaYolu, int DosyaBoyutu, string DosyaTipi, string DosyaAdi, int ParentId, int KullaniciId, bool DosyaMi);
        void PaylasilanlarTB_Ekle(int DosyaId, string Adi, int PaylasilanKisi, int KimPaylasti, int Yetki, string PaylasilaninYolu, int? PaylasilanlarinParentId, bool DosyaMi);
        ServiceResult Index(int? id,string session);
        ServiceResult PaylasilanlariAc(int? id,string session);
        ServiceResult Paylastiklarim(int? id,string session);
        ServiceResult PaylastiklarimiAc(int? id,string session);
        ServiceResult Bilgi(string id,string session);
        ServiceResult BenimlePaylasilanlarBilgi(string id,string session);
        ServiceResult PaylasilanaEkle(DosyaYukleme yuklenecekDosya, int klasorId);
        ServiceResult YetkiKaldirma(int? PaylasmaId);
    }
}
