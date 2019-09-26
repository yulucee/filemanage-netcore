using DosyaSistemiDAL.BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DosyaSistemiDAL.BusinessLayer.Abstract
{
    public interface IKlasorDal
    {
        void ShareLinkTB_Ekle(int DosyaId, string DosyaAdi, bool? DosyaMi, int? PaylastigimKisiler, string Guid, int? ToplamOnizleme, bool? Durumu, bool? Global);
        void PaylasilanlarTB_Ekle(int DosyaId, string Adi, int PaylasilanKisi, int KimPaylasti, int Yetki, string PaylasilaninYolu, int? PaylasilanlarinParentId, bool? DosyaMi);
        void DosyaTB_Ekle(string DosyaYolu, int? DosyaBoyutu, string DosyaTipi, string DosyaAdi, int? ParentId, int KullaniciId, bool? DosyaMi);
        ServiceResult AnaSayfa(int? id,int kullaniciId);
        ServiceResult Sil(int KlasorId,string session);
        ServiceResult Edits(int DosyaId, string KlasorAdi,int session);
        ServiceResult Creates(string YeniKlasorAdi,string session);
        ServiceResult KlasorAc(int? id);
        ServiceResult CreateInFolder(string YeniKlasorAdi, int EskiKlasorId,string session);
        ServiceResult RenameINFolder(string KlasorAdi, int EskiKlasorId, int KlasorId,string session);
        ServiceResult DeleteINFolder(int KlasorId, int EskiKlasorId,string session);
        ServiceResult DosyaINYukle(DosyaYukleme yuklenecekDosya, int klasorId,string session);
        ServiceResult DosyaINDelete(int DosyaId);
        ServiceResult DosyaINRename(int DosyaId, string DosyaAdi, int EskiKlasorId,string session);
        ServiceResult KlasorDosyaDownload(string IDs,string session);
        ServiceResult PaylasilanlariTopluIndirme(string IDs,string session);
        ServiceResult KlasorIciDosyaDownload(string IDs,string session);
        ServiceResult KlasorDownload(int KlasorId);
        ServiceResult DosyaDownload(int DosyaId);
        ServiceResult VideoPlayer(int DosyaId);
        ServiceResult FileCut(string Dosyaid);
        ServiceResult FilePaste(int DosyaId, string tempdosyaid,string session);
        ServiceResult Info(int id,string session);
        ServiceResult YetkiDegisikligiAjax(Dictionary<String, Gift> myDictionary, int KlasorDosyaId,string session);
        ServiceResult LinkPaylas(string DosyaId,string session);
        ServiceResult ShareWithLink(string code);
        ServiceResult PaylastigimLinkler(string session);
        ServiceResult LinkKapat2(int DosyaId, int Checked);

    }
}
