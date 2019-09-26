using DosyaSistemiDAL.Businesslayer.Entities;
using DosyaSistemiDAL.BusinessLayer.Abstract;
using DosyaSistemiDAL.BusinessLayer.Models;
using DosyaSistemiDAL.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DosyaSistemiDAL.BusinessLayer.Concrete
{
    public class EfPaylasilanlarDal : IPaylasilanlarDal
    {
        private readonly Filesystem_ibrahimContext db;
        public EfPaylasilanlarDal()
        {
            this.db = new Filesystem_ibrahimContext();
        }
        public ServiceResult Index(int? id,string session)
        {
            ServiceResult result = new ServiceResult();
            var mymodel = new PaylasilanlarMultipleTable();
            var kullaniciid = Convert.ToInt32(session);
            mymodel.paylasilanlar = db.Paylasilanlar.Where(m => m.PaylasilanKisi == kullaniciid && m.Durumu == null && m.PaylasilanlarinParentId == null).ToList();
            result.Data = mymodel;
            result.Code = 1;
            return result;
        }
        public ServiceResult PaylasilanlariAc(int? id,string session)
        {
            ServiceResult result = new ServiceResult();
            var mymodel = new PaylasilanlarMultipleTable();
            var kullaniciid = Convert.ToInt32(session);
            mymodel.paylasilanlar = db.Paylasilanlar.Where(m => m.PaylasilanKisi == kullaniciid && m.Durumu == null && m.PaylasilanlarinParentId == id).ToList();
            if (mymodel.paylasilanlar.Count() == 0)
            {
                result.Code = 0;
                result.Message = "Hiçbir sonuç bulunamadı";
            }
            else
            {
                result.Code = 1;
                result.Data = mymodel;
            }
            return result;
        }
        public ServiceResult Paylastiklarim(int? id,string session)
        {
            ServiceResult result = new ServiceResult();
            var mymodel = new PaylasilanlarMultipleTable();
            var kullaniciid = Convert.ToInt32(session);
            mymodel.paylasilanlar = db.Paylasilanlar.Where(m => m.KimPaylasti == kullaniciid && m.Durumu == null && m.PaylasilanlarinParentId == null).ToList();
            result.Code = 1;
            result.Data = mymodel;
            return result;
        }
        public ServiceResult PaylastiklarimiAc(int? id, string session)
        {
            ServiceResult result = new ServiceResult();
            var mymodel = new PaylasilanlarMultipleTable();
            var kullaniciid = Convert.ToInt32(session);
            mymodel.paylasilanlar = db.Paylasilanlar.Where(m => m.KimPaylasti == kullaniciid && m.Durumu == null && m.PaylasilanlarinParentId == id).ToList();
            result.Code = 1;
            result.Data = mymodel;
            return result;
        }
        public ServiceResult Bilgi(string id,string session)
        {
            ServiceResult result = new ServiceResult();
            if (String.IsNullOrWhiteSpace(id) || id == "none")
            {
                result.Code = 0;
                result.Data = "Bir dosya secimi yapılmadı!!! Lutfen bilgisini gormek istediginiz dosyayi seciniz..";
                return result;
            }
            string[] parcalar = ExtensionMethod.Parcala(id);
            var istedigimid = Convert.ToInt32(parcalar[0]);
            var kullaniciid = Convert.ToInt32(session);           
            var eris = db.Paylasilanlar.Where(m => m.DosyaId == istedigimid && m.Durumu == null && m.KimPaylasti == kullaniciid).ToList();
            var folderid = db.Paylasilanlar.Where(m => m.DosyaId == istedigimid && m.Durumu == null).Select(m => m.PaylasilanKisi).ToList();
            List<Kullanici> kullanicilar = db.Kullanici.Where(x => folderid.Contains(x.KullaniciId)).ToList();
            List<string> kullanicilistesi = new List<string>();
            var sharetime = "";
            var yetki = "";
            for (int j = 0; j < eris.Count; j++)
            {
                sharetime = eris[j].PaylasilmaTarihi.ToString();
                yetki = eris[j].Yetki.ToString();
                if (yetki == "1")
                {
                    yetki = "Önizleme";
                }
                else if (yetki == "2")
                {
                    yetki = "İndirme";
                }
                else if (yetki == "3")
                {
                    yetki = "Önizleme & İndirme";
                }
                var kullaniciismi = eris[j].Kullanici.KullaniciAdi;
                kullanicilistesi.Add("<tr><td>" + kullaniciismi + "</td>" + "<td>" + sharetime + "</td>" + "<td>" + yetki + "</td>" + "</tr>");
            }
            result.Code = 1;
            result.Data = kullanicilistesi;
            return result;
        }
        public ServiceResult BenimlePaylasilanlarBilgi(string id,string session)
        {
            ServiceResult result = new ServiceResult();
            if (String.IsNullOrWhiteSpace(id) || id == "none")
            {
                result.Code = 0;
                result.Data = "Bir dosya secimi yapılmadı!!! Lutfen bilgisini gormek istediginiz dosyayi seciniz..";
                return result;
            }
            string[] parcalar = ExtensionMethod.Parcala(id);
            var adet = parcalar.Length;
            var kullaniciid = Convert.ToInt32(session);
            var yetkiadi = "";
            List<string> kullaniciliste = new List<string>();
            for (int i = 0; i < parcalar.Length; i++)
            {
                var istedigimid = Convert.ToInt32(parcalar[i]);
                var eris = db.Paylasilanlar.Where(m => m.DosyaId == istedigimid && m.Durumu == null && m.PaylasilanKisi == kullaniciid).SingleOrDefault();
                if (eris == null)
                {
                    result.Code = 4;
                    result.Message = "Hiç bir kayıt bulunamadı";
                    return result;
                }
                var dosyaismi = eris.Adi;
                var sharetime = eris.PaylasilmaTarihi;
                var yetki = Convert.ToInt32(eris.Yetki);
                if (yetki == 1)
                {
                    yetkiadi = "Önizleme";
                }
                else if (yetki == 2)
                {
                    yetkiadi = "İndirme";
                }
                else if (yetki == 3)
                {
                    yetkiadi = "Önizleme & İndirme";
                }
                var kullaniciismi = eris.Kullanici.KullaniciAdi;
                kullaniciliste.Add("<tr><td>" + kullaniciismi + "</td>" + "<td>" + dosyaismi + "</td>" + "<td>" + sharetime + "</td>" + "<td>" + yetkiadi + "</td>" + "</tr>");
            }
            result.Code = 1;
            result.Data = kullaniciliste;
            return result;
        }
        public ServiceResult PaylasilanaEkle(DosyaYukleme yuklenecekDosya, int klasorId)
        {
            ServiceResult result = new ServiceResult();
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var item in yuklenecekDosya.Files)
                    {
                        var filename = Path.GetFileName(item.FileName);
                        var icindebulundugumklasorunid = klasorId;
                        var icindeoldugumklasorunpathi = db.Paylasilanlar.Where(x => x.DosyaId == klasorId).Select(x => x.PaylasilaninYolu).Single();
                        var path = icindeoldugumklasorunpathi + @"\" + filename;
                        var isPathExist = db.Dosya.Any(x => x.DosyaYolu == path && x.Durumu != true);
                        if (isPathExist == true)
                        {
                            result.Code = 0;
                            result.Data = "Aynı dosyadan var. Farklı bir dosya deneyiniz.";
                            return result;
                        }
                        else
                        {
                            if (item.Length < 0)
                            {
                                result.Code = 1;
                                result.Data = "Oluşturulamadı";
                                return result;
                            }
                            Paylasilanlar paylas = db.Paylasilanlar.SingleOrDefault(x => x.DosyaId == klasorId);
                            var paylasilankisi = paylas.PaylasilanKisi;
                            var kimpaylasti = paylas.KimPaylasti;
                            var yetkiid = paylas.Yetki;
                            var paylasilaninyolu = paylas.PaylasilaninYolu;
                            var parentid = paylas.PaylasilanlarinParentId;
                            var icindeoldugumklasorunpathi1 = db.Dosya.Where(x => x.DosyaId == klasorId && x.DosyaMi == null).Select(x => x.DosyaYolu).Single();
                            var path1 = icindeoldugumklasorunpathi1 + @"\" + filename;
                            DosyaTB_Ekle(path1, Convert.ToInt32(item.Length), Path.GetExtension(filename), filename, klasorId, (int)kimpaylasti, true);
                            var dosyaid = db.Dosya.Where(x => x.DosyaYolu == path1).Select(x => x.DosyaId).Single();
                            PaylasilanlarTB_Ekle(dosyaid, filename, (int)paylasilankisi, (int)kimpaylasti, (int)yetkiid, path, klasorId, true);
                            item.CopyTo(new FileStream(path, FileMode.Create));
                            item.CopyTo(new FileStream(path1, FileMode.Create));
                        }
                        transaction.Commit();
                        result.Code = 2;
                        result.Data = "Islem Basarili";
                        return result;
                    }
                }
                catch (Exception)
                {
                    result.Code = 3;
                    result.Data = "Bir hata meydana geldi. Lütfen yükleme yapmayı tekrar deneyiniz.";
                    transaction.Rollback();
                    return result;
                }
                return result;
            }
        }
        public ServiceResult YetkiKaldirma(int? PaylasmaId)
        {
            ServiceResult sonuc = new ServiceResult();
            if (PaylasmaId == null || PaylasmaId == 0)
            {
                sonuc.Code = 3;
                sonuc.Message = "Id Belirtilmemiş";
                return sonuc;
            }
            var id = PaylasmaId;
            var getir = db.Paylasilanlar.Where(x => x.ToplamPaylasmaId == PaylasmaId).SingleOrDefault();
            if (getir == null)
            {
                sonuc.Code = 4;
                sonuc.Message = "Belirtilen dosya bulunamadı";
                return sonuc;
            }
            var path = getir.PaylasilaninYolu;
            var silinecekkisi = getir.PaylasilanKisi;
            var silinecekaltklasorler = db.Paylasilanlar.Where(x => x.PaylasilaninYolu.Contains(path) && x.PaylasilanKisi == silinecekkisi).Select(x => x).ToList();
            if (silinecekaltklasorler == null)
            {
                sonuc.Code = 5;
                sonuc.Message = "Silinecek başka dosya kalmadı";
                return sonuc;
            }
            foreach (Paylasilanlar d in silinecekaltklasorler)
            {
                var kisi = d.PaylasilanKisi;
                var kisiadi = d.Kullanici.KullaniciAdi;
                var kisisoyadi = d.Kullanici.KullaniciSoyadi;
                var user = kisiadi + " " + kisisoyadi;
                d.Durumu = true;
                db.SaveChanges();
                sonuc.Code = 1;
                sonuc.Data = "<option class='fileshareoption " + silinecekkisi + "' value='" + silinecekkisi + "'>" + user + "</option>";
            }
            return sonuc;
        }
        public void DosyaTB_Ekle(string DosyaYolu, int DosyaBoyutu, string DosyaTipi, string DosyaAdi, int ParentId, int KullaniciId, bool DosyaMi)
        {
            Dosya dosya = new Dosya
            {
                DosyaBoyutu = DosyaBoyutu,
                DosyaTipi = DosyaTipi,
                DosyaAdi = DosyaAdi,
                DosyaYolu = DosyaYolu,
                KullaniciId = KullaniciId,
                OlusturulmaZamani = DateTime.Now,
                ParentId = ParentId,
                DosyaMi = DosyaMi,
                Durumu = null
            };
            db.Dosya.Add(dosya);
            db.SaveChanges();
        }
        public void PaylasilanlarTB_Ekle(int DosyaId, string Adi, int PaylasilanKisi, int KimPaylasti, int Yetki, string PaylasilaninYolu, int? PaylasilanlarinParentId, bool DosyaMi)
        {
            Paylasilanlar paylas = new Paylasilanlar
            {
                DosyaId = DosyaId,
                Adi = Adi,
                PaylasilanKisi = PaylasilanKisi,
                KimPaylasti = KimPaylasti,
                PaylasilmaTarihi = DateTime.Now,
                Yetki = Yetki,
                PaylasilaninYolu = PaylasilaninYolu,
                PaylasilanlarinParentId = PaylasilanlarinParentId,
                DosyaMi = DosyaMi
            };
            db.Paylasilanlar.Add(paylas);
            db.SaveChanges();
        }

    }
}
