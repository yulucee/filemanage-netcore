using DosyaSistemiDAL.Businesslayer.Entities;
using DosyaSistemiDAL.BusinessLayer.Abstract;
using DosyaSistemiDAL.BusinessLayer.Models;
using DosyaSistemiDAL.Common;
using Ionic.Zip;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DosyaSistemiDAL.BusinessLayer.Concrete
{
    public class EfKlasorDal : IKlasorDal
    {
        private readonly Filesystem_ibrahimContext db;
        private readonly IHostingEnvironment hostingEnvironment;
        public EfKlasorDal(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.db = new Filesystem_ibrahimContext();
        }
        public ServiceResult AnaSayfa(int? id,int kullaniciId)
        {
            ServiceResult result = new ServiceResult();
            List<object> mymodel = new List<object>();
            var kullaniciid1 = kullaniciId;
            mymodel.Add(db.Dosya.Where(x => x.Durumu == null && x.ParentId == null).ToList());
            mymodel.Add(db.Kullanici.ToList());
            mymodel.Add(db.Paylasilanlar.Where(m => m.KimPaylasti == kullaniciid1 && m.Durumu == null && m.PaylasilanlarinParentId == null).ToList());
            result.Code = 1;
            result.Data = mymodel;
            return result;
        }
        public ServiceResult Creates(string YeniKlasorAdi,string session)
        {
            ServiceResult result = new ServiceResult();
            if (String.IsNullOrWhiteSpace(YeniKlasorAdi))
            {
                result.Code = 3;
                result.Message = "Klasor Adi boş bırakılamaz";
                result.Data = false;
                return result;
            }
            if (YeniKlasorAdi.Contains("/") || YeniKlasorAdi.Contains(":") || YeniKlasorAdi.Contains("*") || YeniKlasorAdi.Contains("?") || YeniKlasorAdi.Contains('"') || YeniKlasorAdi.Contains("<") || YeniKlasorAdi.Contains(">") || YeniKlasorAdi.Contains("|") || YeniKlasorAdi.Contains(":"))
            {
                result.Data = false;
                result.Code = 0;
                return result;
            }
            else
            {
                var temppath = Path.Combine(hostingEnvironment.WebRootPath, "Dosyalarım",session.ToString());
                var klasornewpath = Path.Combine(temppath, YeniKlasorAdi);
                var isklasornewpath = db.Dosya.Any(x => x.DosyaYolu == klasornewpath && x.Durumu != true && x.DosyaMi == null);
                if (isklasornewpath == true)
                {
                    result.Code = 1;
                    result.Data = false;
                    return result;
                }
                else
                {
                    Directory.CreateDirectory(klasornewpath);
                    var sessionkisi = Convert.ToInt32(session);
                    DosyaTB_Ekle(klasornewpath, null, null, YeniKlasorAdi, null, sessionkisi, null);
                    result.Code = 2;
                    result.Data = true;
                    result.Message = "Başarılı bir şekilde oluşturuldu";
                    return result;
                }
            }
        }
        public ServiceResult Edits(int DosyaId, string KlasorAdi,int session)
        {
            ServiceResult result = new ServiceResult();
            Dosya klasor = db.Dosya.SingleOrDefault(x => x.DosyaId == DosyaId && x.DosyaMi == null);
            if (String.IsNullOrWhiteSpace(KlasorAdi))
            {
                result.Code = 4;
                result.Message = "Klasor Adi boş bırakılamaz";
                return result;
            }
            if (klasor == null)
            {
                result.Code = 3;
                result.Message = "Belirtilen klasor bulunamadı";
                return result;
            }
            var degisecekpath = klasor.DosyaYolu;
            var kackarekter = degisecekpath.Length;
            var removepath = db.Dosya.Where(x => x.DosyaYolu.Contains(degisecekpath) && x.Durumu == null).Select(x => x).ToList();
            db.Entry(klasor).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            var klasornewyol1 = Path.Combine(hostingEnvironment.WebRootPath, "Dosyalarım", session.ToString(), KlasorAdi);
            var ispathexist = db.Dosya.Any(x => x.DosyaYolu == klasornewyol1 && x.Durumu != true && x.DosyaMi == null);
            if (ispathexist == true)
            {
                result.Code = 0;
                result.Data = false;
                return result;
            }
            else
            {
                for (int i = 0; i < removepath.Count; i++)
                {
                    var degisenpath = removepath[i].DosyaYolu.Remove(0, kackarekter);
                    if (degisenpath == "")
                    {
                        var klasornewyol = Path.Combine(hostingEnvironment.WebRootPath, "Dosyalarım", session.ToString(), KlasorAdi);
                        klasor.DosyaAdi = KlasorAdi;
                        Directory.Move(klasor.DosyaYolu, klasornewyol);
                        klasor.DosyaYolu = klasornewyol;
                        db.SaveChanges();
                    }
                    else
                    {
                        var cikarilmishali = removepath[i].DosyaYolu = degisenpath;
                        removepath[i].DosyaYolu = klasornewyol1 + cikarilmishali;
                        db.SaveChanges();
                    }
                }
                result.Code = 1;
                result.Data = true;
                return result;
            }
        }
        public ServiceResult Sil(int KlasorId,string session)
        {
            ServiceResult result = new ServiceResult();
            Dosya klasor = db.Dosya.SingleOrDefault(x => x.DosyaId == KlasorId && x.DosyaMi == null);
            if (klasor == null)
            {
                result.Code = 3;
                result.Message = "Belirtilen klasor bulunamadı";
                return result;
            }
            var silinecekKlasor = Path.Combine(hostingEnvironment.WebRootPath, "Dosyalarım", session, klasor.DosyaAdi);
            var path = silinecekKlasor;
            var silinecekaltklasorler = db.Dosya.Where(x => x.DosyaYolu.Contains(path) && x.DosyaMi == null && x.Durumu == null).Select(x => x).ToList();
            foreach (Dosya d in silinecekaltklasorler)
            {
                d.Durumu = true;
            }
            var silinecekaltdosyalar = db.Dosya.Where(x => x.DosyaYolu.Contains(path) && x.DosyaMi == true).Select(x => x).ToList();
            foreach (Dosya d in silinecekaltdosyalar)
            {
                d.Durumu = true;
            }
            if (klasor != null)
            {
                klasor.Durumu = true;
                db.SaveChanges();
                DirectoryInfo di = new DirectoryInfo(silinecekKlasor);
                Directory.Delete(silinecekKlasor, true);
                result.Code = 1;
                result.Data = true;
                result.Message = "Başarılı bir şekilde anasayfadan silindi";
            }
            return result;
        }
        public ServiceResult KlasorAc(int? id)
        {
            ServiceResult result = new ServiceResult();
            var dosyayolu = db.Dosya.Find(id);
            if (dosyayolu == null)
            {
                result.Code = 2;
                result.Message = "Hiçbir sonuç bulunamadı";
                return result;
            }
            List<string> mapList = new List<string>
            {
                "<li><a href='/Klasor/KlasorAc/" + id + "'>" + "<span>" + dosyayolu.DosyaAdi + "</span></a></li>"
            };
            var dosya2 = db.Dosya.Find(id);
            result.ExternalData = IsParent(dosya2, mapList);
            var mymodel = new MultipleTable
            {
                dosyalar = db.Dosya.Where(m => m.ParentId == id && m.Durumu == null).ToList(),
                kullanicilar = db.Kullanici.ToList()
            };
            result.Data = mymodel;
            result.Code = 1;
            return result;
        }
        private List<string> IsParent(Dosya klasor, List<string> mapList)
        {
            if (klasor.ParentId != null)
            {
                var parent = db.Dosya.Find(klasor.ParentId);
                mapList.Add("<li><a href='/Klasor/KlasorAc/" + parent.DosyaId + "'>" + "<span>" + parent.DosyaAdi + "</span></a></li>");
                IsParent(parent, mapList);
                return mapList;
            }
            else
            {
                return mapList;
            }
        }
        public ServiceResult CreateInFolder(string YeniKlasorAdi, int EskiKlasorId,string session)
        {
            ServiceResult result = new ServiceResult();
            if (String.IsNullOrWhiteSpace(YeniKlasorAdi))
            {
                result.Code = 3;
                result.Message = "Klasor Adi boş bırakılamaz";
                result.Data = false;
                return result;
            }
            var hangiklasordeyim = EskiKlasorId;
            var icindeoldugumklasorunpathi = db.Dosya.Where(x => x.DosyaId == EskiKlasorId && x.DosyaMi == null).Select(x => x.DosyaYolu).Single();
            if (YeniKlasorAdi.Contains("/") || YeniKlasorAdi.Contains(":") || YeniKlasorAdi.Contains("*") || YeniKlasorAdi.Contains("?") || YeniKlasorAdi.Contains('"') || YeniKlasorAdi.Contains("<") || YeniKlasorAdi.Contains(">") || YeniKlasorAdi.Contains("|") || YeniKlasorAdi.Contains(":"))
            {
                result.Code = 0;
                result.Data = false;
                result.Message = "Lutfen farklı bir isim giriniz..";
                return result;
            }
            else
            {
                var yenipath = icindeoldugumklasorunpathi;
                var klasornewpath = yenipath + @"\" + YeniKlasorAdi;
                var isklasornewpath = db.Dosya.Any(x => x.DosyaYolu == klasornewpath && x.Durumu != true && x.DosyaMi == null);
                if (isklasornewpath == true)
                {
                    result.Code = 1;
                    result.Message = "Aynı isimli dosyanız var. Farklı bir dosya ismi deneyiniz.";
                    result.Data = false;
                    return result;
                }
                else
                {
                    Directory.CreateDirectory(klasornewpath);
                    var kullaniciId = Convert.ToInt32(session);
                    DosyaTB_Ekle(klasornewpath, null, null, YeniKlasorAdi, hangiklasordeyim, kullaniciId, null);
                    result.Code = 2;
                    result.Message = "Başarılı bir şekilde klasor yaratıldı";
                    result.Data = true;
                    return result;
                }
            }
        }
        public ServiceResult RenameINFolder(string KlasorAdi, int EskiKlasorId, int KlasorId,string session)
        {
            ServiceResult result = new ServiceResult();
            Dosya klasor = db.Dosya.SingleOrDefault(x => x.DosyaId == KlasorId && x.DosyaMi == null);
            if (klasor == null)
            {
                result.Code = 2;
                result.Message = "Bir sonuc bulunamadi";
                return result;
            }
            if (String.IsNullOrWhiteSpace(KlasorAdi))
            {
                result.Code = 3;
                result.Message = "Klasor Adi boş bırakılamaz";
                return result;
            }
            db.Entry(klasor).State = EntityState.Modified;
            var yeniKlasorAdi = KlasorAdi;
            var hangiklasordeyim = EskiKlasorId;
            var icindeoldugumklasorunpathi = db.Dosya.Where(x => x.DosyaId == EskiKlasorId && x.DosyaMi == null).Select(x => x.DosyaYolu).Single();
            var yenipath = icindeoldugumklasorunpathi;
            var klasornewpath = yenipath + @"\" + yeniKlasorAdi;
            var ispathexist = db.Dosya.Any(x => x.DosyaYolu == klasornewpath && x.Durumu != true && x.DosyaMi == null);
            if (ispathexist == true)
            {
                result.Code = 0;
                result.Data = false;
                return result;
            }
            else
            {
                Directory.Move(klasor.DosyaYolu, klasornewpath);
                klasor.KullaniciId = Convert.ToInt32(session);
                klasor.DosyaAdi = KlasorAdi;
                klasor.DosyaYolu = klasornewpath;
                db.SaveChanges();
                result.Code = 1;
                result.Data = true;
                return result;
            }
        }
        public ServiceResult DeleteINFolder(int KlasorId, int EskiKlasorId,string session)
        {
            ServiceResult result = new ServiceResult();
            Dosya klasor = db.Dosya.SingleOrDefault(x => x.DosyaId == KlasorId && x.DosyaMi == null);
            if (klasor == null)
            {
                result.Code = 0;
                result.Message = "Silinecek klasor bulunamadı";
                return result;
            }
            var hangiklasordeyim = EskiKlasorId;
            var icindeoldugumklasorunpathi = db.Dosya.Where(x => x.DosyaId == EskiKlasorId && x.DosyaMi == null).Select(x => x.DosyaYolu).Single();
            var yenipath = icindeoldugumklasorunpathi;
            var silinecekKlasor = yenipath + @"\" + klasor.DosyaAdi;
            var silinecekklasorunid = klasor.DosyaId;
            if (klasor != null)
            {
                klasor.Durumu = true;
                List<Dosya> sonuc2 = db.Dosya.Where(x => x.DosyaYolu.Contains(silinecekKlasor) && x.DosyaMi == null).Select(x => x).ToList();
                List<Dosya> silinecekaltdosyalar = db.Dosya.Where(x => x.DosyaYolu.Contains(silinecekKlasor)).Select(x => x).ToList();
                foreach (Dosya d in silinecekaltdosyalar)
                {
                    d.Durumu = true;
                }
                foreach (Dosya p in sonuc2)
                {
                    p.Durumu = true;
                }
                db.SaveChanges();
                DirectoryInfo di = new DirectoryInfo(silinecekKlasor);
                Directory.Delete(silinecekKlasor, true);
                result.Code = 1;
                result.Message = "Başarılı bir şekilde silindi";
                result.Data = true;
            }
            return result;
        }
        public ServiceResult DosyaINYukle(DosyaYukleme yuklenecekDosya, int klasorId,string session)
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
                        var icindeoldugumklasorunpathi = db.Dosya.Where(x => x.DosyaId == klasorId && x.DosyaMi == null).Select(x => x.DosyaYolu).Single();
                        var path = icindeoldugumklasorunpathi + @"\" + filename;
                        var isPathExist = db.Dosya.Any(x => x.DosyaYolu == path && x.Durumu != true);
                        if (isPathExist == true)
                        {
                            result.Code = 0;
                            result.Message = "Aynı dosyadan var. Farklı bir dosya deneyiniz.";
                        }
                        else
                        {
                            if (item.Length < 0)
                            {
                                result.Code = 1;
                                result.Message = "Oluşturulamadı";
                                return result;
                            }
                            using (var filestream = new FileStream(path, FileMode.Create))
                            {
                                item.CopyTo(filestream);
                            }
                            //item.CopyTo(new FileStream(path, FileMode.Create));
                            var kullaniciId = Convert.ToInt32(session);
                            DosyaTB_Ekle(path, Convert.ToInt32(item.Length), Path.GetExtension(filename), filename, klasorId, kullaniciId, true);
                            result.Code = 2;
                            result.Message = "Başarılı bir şekilde yuklendi";
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception)
                {
                    result.Code = 3;
                    result.Message = "Bir hata meydana geldi. Lütfen yükleme yapmayı tekrar deneyiniz.";
                    return result;
                }
                return result;
            }
        }
        public ServiceResult DosyaINDelete(int DosyaId)
        {
            ServiceResult result = new ServiceResult();
            Dosya dosya = db.Dosya.SingleOrDefault(x => x.DosyaId == DosyaId && x.DosyaMi == true);
            if (dosya == null)
            {
                result.Code = 0;
                result.Message = "Silinecek dosya bulunamadi";
                return result;
            }
            else
            {
                dosya.Durumu = true;
                var silinecekDosya = dosya.DosyaYolu;
                
                System.IO.File.Delete(silinecekDosya);
                db.SaveChanges();
                result.Code = 1;
                result.Data = true;
                result.Message = "Başarılı bir şekilde silindi";
                return result;
            }
        }
        public ServiceResult DosyaINRename(int DosyaId, string DosyaAdi, int EskiKlasorId, string session)
        {
            ServiceResult result = new ServiceResult();
            Dosya dosya = db.Dosya.SingleOrDefault(x => x.DosyaId == DosyaId && x.DosyaMi == true);
            if (String.IsNullOrWhiteSpace(DosyaAdi))
            {
                result.Code = 4;
                result.Message = "Dosya Adi boş bırakılamaz";
                return result;
            }
            if (dosya == null)
            {
                result.Code = 3;
                result.Message = "Dosya bulunamadi, Isleminizi kontrol ediniz";
                return result;
            }
            db.Entry(dosya).State = EntityState.Modified;
            dosya.KullaniciId = Convert.ToInt32(session);
            var dosyanındegismedenoncekiadi = dosya.DosyaAdi;
            var dosyaAdiwithExtension = DosyaAdi + dosya.DosyaTipi;
            var hangiklasordeyim = EskiKlasorId;
            var icindeoldugumklasorunpathi = db.Dosya.Where(x => x.DosyaId == EskiKlasorId && x.DosyaMi == null).Select(x => x.DosyaYolu).Single();
            var dosyaeskipath = icindeoldugumklasorunpathi + @"\" + dosyanındegismedenoncekiadi;
            var dosyanewpath = icindeoldugumklasorunpathi + @"\" + dosyaAdiwithExtension;
            var ispathExist = db.Dosya.Any(x => x.DosyaYolu == dosyanewpath && x.Durumu != true && x.DosyaMi == true);
            if (ispathExist == true)
            {
                result.Code = 0;
                result.Data = false;
                return result;
            }
            else
            {
                dosya.DosyaAdi = dosyaAdiwithExtension;
                dosya.DosyaYolu = dosyanewpath;
                System.IO.File.Move(dosyaeskipath, dosyanewpath);
                db.SaveChanges();
                result.Code = 1;
                result.Data = true;
                return result;
            }
        }
        public ServiceResult KlasorDosyaDownload(string IDs,string session)
        {
            ServiceResult result = new ServiceResult();
            if (String.IsNullOrWhiteSpace(IDs))
            {
                result.Code = 0;
                result.Message = "Dosya bulunamadı";
                return result;
            }
            if(session == null)
            {
                result.Code = 0;
                result.Message = "Çoklu indirme yapmak için giriş yapmalısınız";
                return result;
            }
            string[] parcalar = ExtensionMethod.Parcala(IDs);
            List<int> dosyaIds = Array.ConvertAll(parcalar, int.Parse).ToList();
            List<Dosya> dosya = db.Dosya.Where(x => dosyaIds.Contains(x.DosyaId) && x.DosyaMi == true).ToList();
            var klasor = (db.Dosya.Where(x => dosyaIds.Contains(x.DosyaId) && x.DosyaMi == null)).ToList();
            List<string> files = new List<string>();
            using (ZipFile zip = new ZipFile
            {
                CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression
            })
            {
                var path = Path.Combine(hostingEnvironment.WebRootPath, "Dosyalarım", session);
                for (int i = 0; i < klasor.Count; i++)
                {
                    files.AddRange(Directory.GetFiles(klasor[i].DosyaYolu, "*", SearchOption.AllDirectories).ToList());
                }
                foreach (var f in files)
                {
                    zip.AddFile(f, Path.GetDirectoryName(f).Replace(path, string.Empty));
                }
                for (int i = 0; i < dosya.Count; i++)
                {
                    var dosyaYolu = dosya[i].DosyaYolu;
                    var dosyaismi = Path.GetFileName(dosyaYolu);
                    var dosyabyte = System.IO.File.ReadAllBytes(dosyaYolu);
                    zip.AddEntry(dosyaismi, dosyabyte);
                }
                var masaustu = "C:\\Users\\deytek\\winrar\\Downloads";
                zip.Save(Path.ChangeExtension(masaustu, ".zip"));
                using (MemoryStream output = new MemoryStream())
                {
                    zip.Save(output);
                    result.Data = output.ToArray();
                    result.Code = 1;
                    return result;
                }
            }
        }
        public ServiceResult PaylasilanlariTopluIndirme(string IDs,string session)
        {
            ServiceResult result = new ServiceResult();
            if (String.IsNullOrWhiteSpace(IDs))
            {
                result.Code = 0;
                result.Message = "Dosya bulunamadı";
                return result;
            }
            string[] parcalar = ExtensionMethod.Parcala(IDs);
            List<int> dosyaIds = Array.ConvertAll(parcalar, int.Parse).ToList();
            List<Dosya> dosya = db.Dosya.Where(x => dosyaIds.Contains(x.DosyaId) && x.DosyaMi == true).ToList();
            var klasor = (db.Dosya.Where(x => dosyaIds.Contains(x.DosyaId) && x.DosyaMi == null)).ToList();
            List<string> files = new List<string>();
            using (ZipFile zip = new ZipFile
            {
                CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression
            })
            {
                var path = Path.Combine(hostingEnvironment.WebRootPath, "Dosyalarım", session, "Paylasilanlar", session);
                foreach (var item in klasor)
                {
                    files.AddRange(Directory.GetFiles(item.DosyaYolu, "*", SearchOption.AllDirectories).ToList());
                }
                foreach (var f in files)
                {
                    zip.AddFile(f, Path.GetDirectoryName(f).Replace(path, string.Empty));
                }
                for (int i = 0; i < dosya.Count; i++)
                {
                    var dosyaYolu = dosya[i].DosyaYolu;
                    var dosyaismi = Path.GetFileName(dosyaYolu);
                    var dosyabyte = System.IO.File.ReadAllBytes(dosyaYolu);
                    zip.AddEntry(dosyaismi, dosyabyte);
                }
                var masaustu = "C:\\Users\\deytek\\winrar\\Downloads";
                zip.Save(Path.ChangeExtension(masaustu, ".zip"));
                using (MemoryStream output = new MemoryStream())
                {
                    zip.Save(output);
                    result.Code = 1;
                    result.Data = output.ToArray();
                    return result;
                }
            }
        }
        public ServiceResult KlasorIciDosyaDownload(string IDs, string session)
        {
            ServiceResult result = new ServiceResult();
            if (String.IsNullOrWhiteSpace(IDs))
            {
                result.Code = 0;
                result.Message = "Dosya bulunamadı";
                return result;
            }
            string[] parcalar = ExtensionMethod.Parcala(IDs);
            var ilkid = Convert.ToInt32(parcalar[0]);
            var icindeoldugumklasorunid1 = db.Dosya.Where(x => x.DosyaId == ilkid).Select(x => x.ParentId).Single();
            var icindeoldugumklasorunpathi = db.Dosya.Where(x => x.DosyaId == icindeoldugumklasorunid1 && x.DosyaMi == null).Select(x => x.DosyaYolu).Single();
            List<int> dosyaIds = Array.ConvertAll(parcalar, int.Parse).ToList();
            List<Dosya> dosya = db.Dosya.Where(x => dosyaIds.Contains(x.DosyaId) && x.DosyaMi == true).ToList();
            if (dosya == null)
            {
                result.Code = 2;
                result.Message = "Belirtilen dosya/klasor bulunamadı";
                return result;
            }
            var klasor = (db.Dosya.Where(x => dosyaIds.Contains(x.DosyaId) && x.DosyaMi == null)).ToList();
            if (klasor == null)
            {
                result.Code = 3;
                result.Message = "Belirtilen dosya/klasor bulunamadı";
                return result;
            }
            List<string> files = new List<string>();
            using (ZipFile zip = new ZipFile
            {
                CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression
            })
            {

                var path = icindeoldugumklasorunpathi;
                foreach (var item in klasor)
                {
                    files.AddRange(Directory.GetFiles(item.DosyaYolu, "*", SearchOption.AllDirectories).ToList());
                }
                foreach (var f in files)
                {

                    zip.AddFile(f, Path.GetDirectoryName(f).Replace(path, string.Empty));
                }
                for (int i = 0; i < dosya.Count; i++)
                {
                    var dosyaYolu = dosya[i].DosyaYolu;
                    var dosyaismi = Path.GetFileName(dosyaYolu);
                    var dosyabyte = System.IO.File.ReadAllBytes(dosyaYolu);
                    zip.AddEntry(dosyaismi, dosyabyte);
                }

                var masaustu = "C:\\Users\\deytek\\winrar\\Downloads";
                zip.Save(Path.ChangeExtension(masaustu, ".zip"));

                using (MemoryStream output = new MemoryStream())
                {
                    zip.Save(output);
                    result.Code = 1;
                    result.Data = output.ToArray();
                    return result;
                }
            }
        }
        public ServiceResult KlasorDownload(int KlasorId)
        {
            ServiceResult result = new ServiceResult();
            Dosya klasor = db.Dosya.SingleOrDefault(x => x.DosyaId == KlasorId && x.DosyaMi == null);
            if (klasor == null)
            {
                result.Code = 0;
                result.Message = "Belirtilen klasor bulunamadı";
                return result;
            }
            var path = klasor.DosyaYolu;
            using (ZipFile zip = new ZipFile
            {
                CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression
            })
            {
                var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories).ToList();
                foreach (var f in files)
                {
                    zip.AddFile(f, Path.GetDirectoryName(f).Replace(path, string.Empty));
                }
                var masaustu = "C:\\Users\\deytek\\winrar\\Downloads";
                zip.Save(Path.ChangeExtension(masaustu, ".zip"));
                using (MemoryStream output = new MemoryStream())
                {
                    zip.Save(output);
                    result.Code = 1;
                    result.Data = output.ToArray();
                    return result;
                }
            }
        }
        public ServiceResult DosyaDownload(int DosyaId)
        {
            ServiceResult result = new ServiceResult();
            Dosya dosya = db.Dosya.SingleOrDefault(x => x.DosyaId == DosyaId && x.DosyaMi == true);
            if (dosya == null)
            {
                result.Code = 0;
                result.Message = "Belirtilen dosya bulunamadı";
                return result;
            }
            else
            {
                var path = dosya.DosyaYolu;
                byte[] fileBytes = GetFile(path);
                var dosyaadi = dosya.DosyaAdi;
                result.Code = 1;
                result.Data = fileBytes;
                result.Message = path;
                return result;
            }
        }
        byte[] GetFile(string s)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new System.IO.IOException(s);
            //fs.Dispose();
            return data;
        }
        public ServiceResult FileCut(string Dosyaid)
        {
            ServiceResult result = new ServiceResult();
            var tasinacakdosyaids = Dosyaid;
            if (tasinacakdosyaids == "none")
            {
                result.Code = 0;
                result.Message = "Bir secim yapınız";
                return result;
            }
            else
            {
                result.Code = 1;
                return result;
            }
        }
        public ServiceResult FilePaste(int DosyaId, string tempdosyaid,string session)
        {
            ServiceResult result = new ServiceResult();
            var icindeoldugumklasorid = DosyaId;
            var dosyaid = tempdosyaid;
            if (dosyaid == null)
            {
                result.Code = 0;
                result.Message = "Yapıştırılacak bişey bulunamadı";
                return result;
            }
            string[] parcalar = ExtensionMethod.Parcala(dosyaid);
            List<int> dosyaIds = Array.ConvertAll(parcalar, int.Parse).ToList();
            List<Dosya> dosya = db.Dosya.Where(x => dosyaIds.Contains(x.DosyaId) && x.DosyaMi == true).ToList();
            List<Dosya> klasor = db.Dosya.Where(x => dosyaIds.Contains(x.DosyaId) && x.DosyaMi == null).ToList();
            for (int m = 0; m < klasor.Count; m++)
            {
                if (icindeoldugumklasorid == 1)
                {
                    var anadizinpath = Path.Combine(hostingEnvironment.WebRootPath, "Dosyalarım", session);
                    var klasoradi = klasor[m].DosyaAdi;
                    var klasoryolu = anadizinpath + @"\" + klasoradi;
                    var isnewklasoryoluexist = db.Dosya.Any(x => x.DosyaYolu == klasoryolu && x.DosyaMi == null);
                    var tasinacakyer = db.Dosya.Where(x => x.DosyaYolu == klasoryolu && x.DosyaMi == null).Select(x => x.Durumu).FirstOrDefault();
                    if (isnewklasoryoluexist == true)
                    {
                        if (tasinacakyer == true)
                        {
                            var kesilenklasorunpathi = klasor[m].DosyaYolu;
                            var yoludegisecekaltklasorler = db.Dosya.Where(x => x.DosyaYolu.Contains(kesilenklasorunpathi) && x.DosyaMi == null).Select(x => x).ToList();
                            for (int i = 0; i < yoludegisecekaltklasorler.Count; i++)
                            {
                                if (i == 0)
                                {
                                    var klasoryoluu = yoludegisecekaltklasorler[i].DosyaYolu;
                                    var icklasoryolu = klasoryolu;
                                    var id = icindeoldugumklasorid;
                                    yoludegisecekaltklasorler[i].ParentId = id;
                                    yoludegisecekaltklasorler[i].DosyaYolu = icklasoryolu;
                                    Directory.Move(klasoryoluu, icklasoryolu);
                                    yoludegisecekaltklasorler[i].ParentId = null;
                                    db.SaveChanges();
                                }
                                else if (i != 0)
                                {
                                    var a = kesilenklasorunpathi.Length;
                                    var klasoryoluu = yoludegisecekaltklasorler[i].DosyaYolu;
                                    var son = yoludegisecekaltklasorler[i].DosyaYolu.Remove(0, a);
                                    var icklasoryolu = klasoryolu + son;
                                    yoludegisecekaltklasorler[i].DosyaYolu = icklasoryolu;
                                    db.SaveChanges();
                                }
                            }
                            var yoludegisecekaltdosya = db.Dosya.Where(x => x.DosyaYolu.Contains(kesilenklasorunpathi) && x.DosyaMi == true).Select(x => x).ToList();
                            for (int i = 0; i < yoludegisecekaltdosya.Count; i++)
                            {
                                var a = kesilenklasorunpathi.Length;
                                var son = yoludegisecekaltdosya[i].DosyaYolu.Remove(0, a);
                                var dosyayolu = yoludegisecekaltdosya[i].DosyaYolu;
                                var icdosyayolu = klasoryolu + son;
                                yoludegisecekaltdosya[i].DosyaYolu = icdosyayolu;
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            result.Code = 2;
                            result.Message = "Bu dosya zaten var";
                            return result;
                        }
                    }
                    else
                    {
                        var kesilenklasorunpathi = klasor[m].DosyaYolu;
                        var yoludegisecekaltklasorler = db.Dosya.Where(x => x.DosyaYolu.Contains(kesilenklasorunpathi) && x.DosyaMi == null).Select(x => x).ToList();
                        for (int i = 0; i < yoludegisecekaltklasorler.Count; i++)
                        {
                            if (i == 0)
                            {
                                var klasoryoluuu = yoludegisecekaltklasorler[i].DosyaYolu;
                                var icklasoryolu = klasoryolu;
                                var id = icindeoldugumklasorid;
                                yoludegisecekaltklasorler[i].ParentId = null;
                                yoludegisecekaltklasorler[i].DosyaYolu = icklasoryolu;
                                Directory.Move(klasoryoluuu, icklasoryolu);
                                db.SaveChanges();
                            }
                            else if (i != 0)
                            {
                                var a = kesilenklasorunpathi.Length;
                                var klasoryoluu = yoludegisecekaltklasorler[i].DosyaYolu;
                                var son = yoludegisecekaltklasorler[i].DosyaYolu.Remove(0, a);
                                var icklasoryolu = klasoryolu + son;
                                yoludegisecekaltklasorler[i].DosyaYolu = icklasoryolu;
                                db.SaveChanges();
                            }
                        }
                        var yoludegisecekaltdosya = db.Dosya.Where(x => x.DosyaYolu.Contains(kesilenklasorunpathi) && x.DosyaMi == true).Select(x => x).ToList();
                        for (int i = 0; i < yoludegisecekaltdosya.Count; i++)
                        {
                            var a = kesilenklasorunpathi.Length;
                            var son = yoludegisecekaltdosya[i].DosyaYolu.Remove(0, a);
                            var dosyayolu = yoludegisecekaltdosya[i].DosyaYolu;
                            var icdosyayolu = klasoryolu + son;
                            yoludegisecekaltdosya[i].DosyaYolu = icdosyayolu;
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    var path = klasor[m].DosyaYolu;
                    var icindeoldugumklasorunpathi = db.Dosya.Where(x => x.DosyaId == icindeoldugumklasorid && x.DosyaMi == null).Select(x => x.DosyaYolu).Single();
                    var klasoradi = klasor[m].DosyaAdi;
                    var newklasoryolu = icindeoldugumklasorunpathi + @"\" + klasoradi;
                    var isnewklasoryoluexist = db.Dosya.Any(x => x.DosyaYolu == newklasoryolu && x.DosyaMi == null);
                    var tasinacakyer = db.Dosya.Where(x => x.DosyaYolu == newklasoryolu && x.DosyaMi == null).Select(x => x.Durumu).FirstOrDefault();
                    if (isnewklasoryoluexist == true)
                    {
                        if (tasinacakyer == true)
                        {
                            var kesilenklasorunpathi = klasor[m].DosyaYolu;
                            var yoludegisecekaltklasorler = (from d in db.Dosya where (d.DosyaYolu.Contains(path) && d.DosyaMi == null) select d).ToList();
                            for (int i = 0; i < yoludegisecekaltklasorler.Count; i++)
                            {
                                if (i == 0)
                                {
                                    var klasoryolu = yoludegisecekaltklasorler[i].DosyaYolu;
                                    var icklasoryolu = newklasoryolu;
                                    var id = icindeoldugumklasorid;
                                    yoludegisecekaltklasorler[i].ParentId = id;
                                    yoludegisecekaltklasorler[i].DosyaYolu = icklasoryolu;
                                    Directory.Move(klasoryolu, icklasoryolu);
                                    db.SaveChanges();
                                }
                                else if (i != 0)
                                {
                                    var a = kesilenklasorunpathi.Length;
                                    var klasoryolu = yoludegisecekaltklasorler[i].DosyaYolu;
                                    var son = yoludegisecekaltklasorler[i].DosyaYolu.Remove(0, a);
                                    var icklasoryolu = newklasoryolu + son;
                                    yoludegisecekaltklasorler[i].DosyaYolu = icklasoryolu;
                                    db.SaveChanges();
                                }
                            }
                            var yoludegisecekaltdosya = (from d in db.Dosya where (d.DosyaYolu.Contains(path) && d.DosyaMi == true) select d).ToList();
                            for (int i = 0; i < yoludegisecekaltdosya.Count; i++)
                            {
                                var a = kesilenklasorunpathi.Length;
                                var son = yoludegisecekaltdosya[i].DosyaYolu.Remove(0, a);
                                var dosyayolu = yoludegisecekaltdosya[i].DosyaYolu;
                                var icdosyayolu = newklasoryolu + son;
                                yoludegisecekaltdosya[i].DosyaYolu = icdosyayolu;
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            result.Code = 3;
                            result.Message = "Bu zaten var";
                            return result;
                        }
                    }
                    else
                    {
                        var kesilenklasorunpathi = klasor[m].DosyaYolu;
                        var yoludegisecekaltklasorler = db.Dosya.Where(x => x.DosyaYolu.Contains(path) && x.DosyaMi == null).Select(x => x).ToList();
                        for (int i = 0; i < yoludegisecekaltklasorler.Count; i++)
                        {
                            if (i == 0)
                            {
                                var klasoryolu = yoludegisecekaltklasorler[i].DosyaYolu;
                                var icklasoryolu = newklasoryolu;
                                var id = icindeoldugumklasorid;
                                yoludegisecekaltklasorler[0].ParentId = id;
                                yoludegisecekaltklasorler[0].DosyaYolu = icklasoryolu;
                                Directory.Move(klasoryolu, icklasoryolu);
                                db.SaveChanges();
                            }
                            else if (i != 0)
                            {
                                var a = kesilenklasorunpathi.Length;
                                var klasoryolu = yoludegisecekaltklasorler[i].DosyaYolu;
                                var son = yoludegisecekaltklasorler[i].DosyaYolu.Remove(0, a);
                                var icklasoryolu = newklasoryolu + son;
                                yoludegisecekaltklasorler[i].DosyaYolu = icklasoryolu;
                                db.SaveChanges();
                            }
                        }
                        var yoludegisecekaltdosya = db.Dosya.Where(x => x.DosyaYolu.Contains(path) && x.DosyaMi == true).Select(x => x).ToList();
                        for (int i = 0; i < yoludegisecekaltdosya.Count; i++)
                        {
                            var a = kesilenklasorunpathi.Length;
                            var son = yoludegisecekaltdosya[i].DosyaYolu.Remove(0, a);
                            var dosyayolu = yoludegisecekaltdosya[i].DosyaYolu;
                            var icdosyayolu = newklasoryolu + son;
                            yoludegisecekaltdosya[i].DosyaYolu = icdosyayolu;
                            db.SaveChanges();
                        }
                    }
                }
            }
            for (int i = 0; i < dosya.Count; i++)
            {
                if (icindeoldugumklasorid == 1)
                {
                    var anadizinpathi = Path.Combine(hostingEnvironment.WebRootPath, "Dosyalarım", session);
                    var dosyaadii = dosya[i].DosyaAdi;
                    var newdosyayoluu = anadizinpathi + @"\" +dosyaadii;
                    var tasinacakyer = db.Dosya.Where(x => x.DosyaYolu == newdosyayoluu && x.DosyaMi == true).Select(x => x.Durumu).FirstOrDefault();
                    var isnewdosyayoluexistt = db.Dosya.Any(x => x.DosyaYolu == newdosyayoluu && x.DosyaMi == true);
                    if (isnewdosyayoluexistt == true)
                    {
                        if (tasinacakyer == true)
                        {
                            dosya[i].ParentId = null;
                            var eskiyol = dosya[i].DosyaYolu;
                            dosya[i].DosyaYolu = newdosyayoluu;
                            System.IO.File.Move(eskiyol, newdosyayoluu);
                            db.SaveChanges();
                        }
                        else
                        {
                            result.Code = 4;
                            result.Message = "Bu dosya zaten var";
                            return result;
                        }
                    }
                    else
                    {
                        var eskiyol = dosya[i].DosyaYolu;
                        dosya[i].ParentId = null;
                        dosya[i].DosyaYolu = newdosyayoluu;
                        System.IO.File.Move(eskiyol, newdosyayoluu);
                        db.SaveChanges();
                    }
                }
                else
                {
                    var icindeoldugumklasorunpathi = db.Dosya.Where(x => x.DosyaId == icindeoldugumklasorid && x.DosyaMi == null).Select(x => x.DosyaYolu).Single();
                    var dosyaadi = dosya[i].DosyaAdi;
                    var newdosyayolu = icindeoldugumklasorunpathi + @"\" + dosyaadi;
                    var isnewdosyayoluexist = db.Dosya.Any(x => x.DosyaYolu == newdosyayolu && x.DosyaMi == true);
                    var tasinacakyer = db.Dosya.Where(x => x.DosyaYolu == newdosyayolu && x.DosyaMi == true).Select(x => x.Durumu).FirstOrDefault();
                    if (isnewdosyayoluexist == true)
                    {
                        if (tasinacakyer == true)
                        {
                            dosya[i].ParentId = icindeoldugumklasorid;
                            var eskiyol = dosya[i].DosyaYolu;
                            dosya[i].DosyaYolu = newdosyayolu;
                            System.IO.File.Move(eskiyol, newdosyayolu);
                            db.SaveChanges();
                        }
                        else
                        {
                            result.Code = 4;
                            result.Message = "Bu zaten var";
                            return result;
                        }
                    }
                    else
                    {
                        var eskiyol = dosya[i].DosyaYolu;
                        dosya[i].ParentId = icindeoldugumklasorid;
                        dosya[i].DosyaYolu = newdosyayolu;
                        System.IO.File.Move(eskiyol, newdosyayolu);
                        db.SaveChanges();
                    }
                }
            }
            return result;
        }
        public ServiceResult Info(int id,string session)
        {
            ServiceResult sonuc = new ServiceResult();
            bool result = false;
            var istedigimid = id;
            var kullaniciid = Convert.ToInt32(session);
            var eris = db.Paylasilanlar.Where(m => m.DosyaId == istedigimid && m.Durumu == null && m.KimPaylasti == kullaniciid).ToList();
            if (eris == null)
            {
                sonuc.Code = 0;
                sonuc.Message = "Bir sonuç bulunamadı";
                return sonuc;
            }
            var folderid = db.Paylasilanlar.Where(m => m.DosyaId == istedigimid).Select(m => m.PaylasilanKisi).ToList();
            List<Kullanici> kullanicilar = db.Kullanici.Where(x => folderid.Contains(x.KullaniciId)).ToList();
            List<string> userlistesi = new List<string>();
            var sharetime = "";
            var yetki = "";
            List<int?> kullaniciIdArray = new List<int?>();
            for (int j = 0; j < eris.Count; j++)
            {
                var yetkihtml = "";
                sharetime = eris[j].PaylasilmaTarihi.ToString();
                yetki = eris[j].Yetki.ToString();
                var trid = eris[j].ToplamPaylasmaId;
                
                var paylasilankisi = eris[j].PaylasilanKisi;
                var kullaniciismi = eris[j].Kullanici.KullaniciAdi;
                var kullanicisoyadi = eris[j].Kullanici.KullaniciSoyadi;
                var kullanici = kullaniciismi + " " + kullanicisoyadi;
                var toplamid = eris[j].ToplamPaylasmaId;
                if (yetki == "1")
                {
                    yetki = "Önizleme";
                    yetkihtml = "<div class='content'><input type='checkbox'class='form-check-input" + toplamid + "' id='exampleCheck22'name='yetki' value='1' checked='checked'>" +
                                "<label class='form-check-label'style='margin-right:5px;'for='exampleCheck22'>Önizleme</label>" +
                                "<input type='checkbox'class='form-check-input" + toplamid + "' id='exampleCheck22'name='yetki' value='2'>" +
                                "<label class='form-check-label' for='exampleCheck22'>Indirme</label></div>";
                }
                else if (yetki == "2")
                {
                    yetki = "İndirme";
                    yetkihtml = "<div class='content'><input type='checkbox'class='form-check-input" + toplamid + "'id='exampleCheck22'name='yetki' value='1'>" +
                                "<label class='form-check-label'style='margin-right:5px;' for='exampleCheck22'>Önizleme</label>" +
                                "<input type='checkbox'class='form-check-input" + toplamid + "' id='exampleCheck22' name='yetki'value='2' checked='checked'>" +
                                "<label class='form-check-label' for='exampleCheck22'>Indirme</label></div>";
                }
                else if (yetki == "3")
                {
                    yetki = "Önizleme&İndirme";
                    yetkihtml = "<div class='content'><input type='checkbox'class='form-check-input" + toplamid + "' id='exampleCheck22'name='yetki' value='1' checked='checked'>" +
                                "<label class='form-check-label'style='margin-right:5px;' for='exampleCheck22'>Önizleme</label>" +
                                "<input type='checkbox'class='form-check-input" + toplamid + "' id='exampleCheck22' name='yetki' value='2' checked='checked'>" +
                                "<label class='form-check-label' for='exampleCheck22'>Indirme</label></div>";
                }
                userlistesi.Add("<tr id='" + trid + "'><td>" + kullanici + "</td>" + "<td>" + sharetime + "</td>" + "<td>" + yetkihtml + "</td>" + "<td class='actions'><button class='delete-row'onclick='ConfirmYetkiDelete(" + toplamid + ")'id='submit'><i class='fa fa-trash-o'></i></button></td></tr>");
                kullaniciIdArray.Add(paylasilankisi);
            }
            sonuc.Code = 1;
            result = true;
            sonuc.Data = new { result, userlistesi, kullaniciIdArray };
            return sonuc;
        }
        public ServiceResult YetkiDegisikligiAjax(Dictionary<string, Gift> myDictionary, int KlasorDosyaId,string session)
        {
            ServiceResult sonuc = new ServiceResult();
            var count = myDictionary.Count;
            foreach (var key in myDictionary.Values)
            {
                if (key == null)
                {
                    sonuc.Code = 0;
                    sonuc.Message = "Yetkilerde Degisiklik Yapilmadi..";
                    return sonuc;
                }
                else
                {
                    var yetki = 0;
                    var degisiklikyapilcakid = Convert.ToInt32(key.Id);
                    if (key.Value == null)
                    {
                        sonuc.Code = 1;
                        sonuc.Data = false;
                        return sonuc;
                    }
                    else
                    {
                        var yetkiId = key.Value.Count();
                        yetki = (yetkiId == 1) ? key.Value[0] : 3;
                    }
                    var eris = db.Paylasilanlar.Where(x => x.ToplamPaylasmaId == degisiklikyapilcakid).SingleOrDefault();
                    if (eris == null)
                    {
                        var dosyanumarasi = KlasorDosyaId;
                        var kullaniciId = Convert.ToInt32(key.Id);
                        var kullaniciyetkisi = yetki;
                        var namefind = db.Dosya.Where(x => x.DosyaId == dosyanumarasi).SingleOrDefault();
                        var dosyaismi = namefind.DosyaAdi;
                        var dosyayolu = namefind.DosyaYolu;
                        var parent = namefind.ParentId;
                        var dosyami = namefind.DosyaMi;
                        if (dosyami == true)
                        {
                            var path = Path.Combine(hostingEnvironment.WebRootPath, "Dosyalarım", kullaniciId.ToString(), "Paylasilanlar", session, dosyaismi);
                            var targetpath = Path.Combine(hostingEnvironment.WebRootPath, "Dosyalarım", kullaniciId.ToString(), "Paylasilanlar", session);
                            var ispathexist = db.Paylasilanlar.Any(x => x.PaylasilaninYolu == path && x.Durumu == null);
                            if (ispathexist != true)
                            {
                                var sessionkisi = Convert.ToInt32(session);
                                PaylasilanlarTB_Ekle(dosyanumarasi, dosyaismi, kullaniciId, sessionkisi, kullaniciyetkisi, path, parent, dosyami);
                                sonuc.Code = 2;
                                sonuc.Data = true;
                                if (!System.IO.Directory.Exists(targetpath))
                                {
                                    System.IO.Directory.CreateDirectory(targetpath);
                                }
                                System.IO.File.Copy(dosyayolu, path, true);
                            }
                            else
                            {
                                sonuc.Code = 3;
                                sonuc.Data = false;
                                return sonuc;
                            }
                        }
                        else
                        {
                            var path = Path.Combine(hostingEnvironment.WebRootPath, "Dosyalarım", kullaniciId.ToString(), "Paylasilanlar", session, dosyaismi);
                            var targetpath = Path.Combine(hostingEnvironment.WebRootPath, "Dosyalarım", kullaniciId.ToString(), "Paylasilanlar", session);
                            var ispathexist = db.Paylasilanlar.Any(x => x.PaylasilaninYolu == path && x.Durumu == null);
                            if (ispathexist != true)
                            {
                                var sessionkisi = Convert.ToInt32(session);
                                PaylasilanlarTB_Ekle(dosyanumarasi, dosyaismi, kullaniciId, sessionkisi, kullaniciyetkisi, path, parent, dosyami);
                                sonuc.Code = 4;
                                sonuc.Data = true;
                                var altklasorler = db.Dosya.Where(x => x.DosyaYolu.Contains(dosyayolu) && x.DosyaMi == null && x.Durumu == null).Select(x => x).ToList();
                                var altdosyalar = db.Dosya.Where(x => x.DosyaYolu.Contains(dosyayolu) && x.DosyaMi == true && x.Durumu == null).Select(x => x).ToList();
                                for (int x = 0; x < altklasorler.Count; x++)
                                {
                                    var parentid = altklasorler[x].ParentId;
                                    if (parentid == null)
                                    {
                                        var pathcheck = targetpath + @"\" + altklasorler[x].DosyaAdi;
                                        var ispathexist2 = db.Paylasilanlar.Any(p => p.PaylasilaninYolu == pathcheck && p.Durumu == null);
                                        if (ispathexist2 != true)
                                        {
                                            PaylasilanlarTB_Ekle(altklasorler[x].DosyaId, altklasorler[x].DosyaAdi, kullaniciId, sessionkisi, yetki, pathcheck, null, null);
                                        }
                                    }
                                    else
                                    {
                                        var klasorunparentpathi = db.Dosya.Where(d => d.DosyaId == parentid && d.DosyaMi == null && d.Durumu == null).Select(d => d.DosyaYolu).SingleOrDefault();
                                        var parentpath = klasorunparentpathi.ToString();
                                        var remove = parentpath.Remove(0, 64);
                                        var paylasmayolu = targetpath + @"\" + remove + @"\" + altklasorler[x].DosyaAdi;
                                        var ispathexist1 = db.Paylasilanlar.Any(k => k.PaylasilaninYolu == paylasmayolu && k.Durumu == null);
                                        if (ispathexist1 != true)
                                        {
                                            PaylasilanlarTB_Ekle(altklasorler[x].DosyaId, altklasorler[x].DosyaAdi, kullaniciId, sessionkisi, yetki, paylasmayolu, parentid, null);
                                        }
                                    }
                                    if (!System.IO.Directory.Exists(path))
                                    {
                                        System.IO.Directory.CreateDirectory(path);
                                    }
                                }
                                for (int x = 0; x < altdosyalar.Count; x++)
                                {
                                    var dosyayolu1 = altdosyalar[x].DosyaYolu;
                                    var altdosyaparent = altdosyalar[x].ParentId;
                                    var dosyanınparentpathi = db.Dosya.Where(s => s.DosyaId == altdosyaparent && s.DosyaMi == null && s.Durumu == null).Select(s => s.DosyaYolu).SingleOrDefault();
                                    var remove = dosyanınparentpathi.Remove(0, 64);
                                    var pathchecking = targetpath + @"\" + remove + @"\" + altdosyalar[x].DosyaAdi;
                                    var ispathexist3 = db.Paylasilanlar.Any(k => k.PaylasilaninYolu == pathchecking && k.Durumu == null);
                                    if (ispathexist3 != true)
                                    {
                                        PaylasilanlarTB_Ekle(altdosyalar[x].DosyaId, altdosyalar[x].DosyaAdi, kullaniciId, sessionkisi, yetki, pathchecking, altdosyalar[x].ParentId, true);
                                    }
                                }
                                foreach (string dirPath in Directory.GetDirectories(dosyayolu, "*", SearchOption.AllDirectories))
                                    Directory.CreateDirectory(dirPath.Replace(dosyayolu, path));
                                foreach (string newPath in Directory.GetFiles(dosyayolu, "*.*",
                                    SearchOption.AllDirectories))
                                    System.IO.File.Copy(newPath, newPath.Replace(dosyayolu, path), true);
                            }
                        }
                    }
                    else
                    {
                        eris.Yetki = yetki;
                        sonuc.Code = 5;
                        sonuc.Data = true;
                        db.SaveChanges();
                    }
                }
            }
            return sonuc;
        }
        public ServiceResult LinkPaylas(string DosyaId,string session)
        {
            ServiceResult result = new ServiceResult();
            if (String.IsNullOrWhiteSpace(DosyaId))
            {
                result.Code = 2;
                result.Message = "Klasor Adi boş bırakılamaz";
                return result;
            }
            var id = DosyaId;
            string[] parcalar = ExtensionMethod.Parcala(id);
            List<int> dosyaIds = Array.ConvertAll(parcalar, int.Parse).ToList();
            var klasor = (db.Dosya.Where(x => dosyaIds.Contains(x.DosyaId))).ToList();
            if (klasor.Count == 0)
            {
                result.Code = 0;
                result.Data = false;
                return result;

            }
            ShareLink share = new ShareLink();
            Guid v;
            v = Guid.NewGuid();
            var link = "";
            for (int i = 0; i < klasor.Count; i++)
            {
                var dosyaadi = klasor[i].DosyaAdi;
                var dosyami = klasor[i].DosyaMi;
                var dosyaid = klasor[i].DosyaId;
                if (klasor.Count > 1)
                {
                    var sessionkisi = Convert.ToInt32(session);
                    ShareLinkTB_Ekle(dosyaid, dosyaadi, dosyami, sessionkisi, v.ToString(), null, null, true);
                    link = "https://localhost:44348/Klasor/ShareWithLink?code=" + v;
                    result.Code = 1;
                    result.Data = true;
                    result.Message = link;
                }
                else
                {
                    var newguid = "";
                    var hazir = "";
                    var guidara = db.ShareLink.Where(x => x.DosyaId == dosyaid && x.Durumu == null).Select(x => x.Guid).ToList();
                    if (guidara.Count != 0)
                    {
                        for (int j = 0; j < guidara.Count; j++)
                        {
                            hazir = guidara[j];
                            var guidadet = db.ShareLink.Where(x => x.Guid == hazir && x.Durumu == null).ToList();
                            if (guidadet.Count == 1)
                            {
                                newguid = hazir;
                                break;
                            }
                        }
                        if (newguid == "")
                        {
                            var sessionkisi = Convert.ToInt32(session);
                            Guid d; d = Guid.NewGuid();
                            newguid = d.ToString();
                            ShareLinkTB_Ekle(dosyaid, dosyaadi, dosyami, sessionkisi, newguid, null, null, true);
                        }
                        link = "https://localhost:44348/Klasor/ShareWithLink?code=" + newguid;
                        result.Code = 1;
                        result.Data = true;
                        result.Message = link;
                    }
                    else
                    {
                        Guid c; c = Guid.NewGuid();
                        var sessionkisi = Convert.ToInt32(session);
                        ShareLinkTB_Ekle(dosyaid, dosyaadi, dosyami, sessionkisi, c.ToString(), null, null, true);
                        link = "https://localhost:44348/Klasor/ShareWithLink?code=" + c;
                        result.Code = 1;
                        result.Data = true;
                        result.Message = link;
                    }
                }
            }
            return result;
        }
        public ServiceResult ShareWithLink(string code)
        {
            ServiceResult result = new ServiceResult();
            ShareLink share = new ShareLink();
            if (String.IsNullOrWhiteSpace(code))
            {
                result.Code = 3;
                result.Message = "Hatalı link";
                return result;
            }
            var guid = code;
            var eris = db.ShareLink.Where(x => x.Guid == guid && x.Durumu == null).ToList();
            var mymodel = new LinkMultipleClass();
            if (eris != null)
            {
                for (int i = 0; i < eris.Count; i++)
                {
                    var toplam = eris[i].ToplamOnizleme;
                    if (toplam == null)
                    {
                        var yenitoplam = 1;
                        eris[i].ToplamOnizleme = yenitoplam;
                        db.SaveChanges();
                    }
                    else
                    {
                        toplam += 1;
                        eris[i].ToplamOnizleme = toplam;
                        db.SaveChanges();
                    }
                }
                mymodel.linkler = db.ShareLink.Where(x => x.Guid == guid && x.Durumu == null).ToList();
                result.Code = 1;
                result.Data = mymodel;
                return result;
            }
            else
            {
                result.Code = 0;
                result.Message = "Linkin gecerliligi dolmuştur.";
                return result;
            }
        }
        public ServiceResult PaylastigimLinkler(string session)
        {
            ServiceResult result = new ServiceResult();
            var mymodel = new LinkMultipleClass();
            var kullaniId = Convert.ToInt32(session);
            mymodel.linkler = db.ShareLink.Where(x => x.PaylastigimKisiler == kullaniId && x.Durumu == null).ToList();
            result.Data = mymodel;
            result.Code = 1;
            return result;
        }
        public ServiceResult LinkKapat2(int DosyaId, int Checked)
        {
            ServiceResult sonuc = new ServiceResult();
            var checkedvalue = Checked;
            ShareLink share = new ShareLink();
            var eris = db.ShareLink.Where(x => x.Id == DosyaId && x.Durumu == null).SingleOrDefault();
            if (eris == null)
            {
                sonuc.Code = 0;
                sonuc.Message = "Bulunamadı";
                return sonuc;
            }
            var dosyaadi = eris.DosyaAdi;
            var dosyami = eris.DosyaMi;
            var durumu = eris.Durumu;
            if (checkedvalue == 1 && durumu == true)
            {
                Guid v;
                v = Guid.NewGuid();
                ShareLinkTB_Ekle(DosyaId, dosyaadi, dosyami, null, v.ToString(), null, null, true);
                sonuc.Code = 1;
                sonuc.Data = true;
                return sonuc;
            }
            else
            {
                eris.Durumu = true;
                db.SaveChanges();
            }
            sonuc.Data = true;
            return sonuc;
        }
        public ServiceResult VideoPlayer(int DosyaId)
        {
            ServiceResult result = new ServiceResult();
            Dosya dosya = db.Dosya.SingleOrDefault(x => x.DosyaId == DosyaId && x.DosyaMi == true);
            if (dosya == null)
            {
                result.Code = 4;
                result.Message = "Dosya bulunamadı..";
                return result;
            }
            var dosyatipi = dosya.DosyaTipi;
            var dosyaadi = dosya.DosyaAdi;
            var path = dosya.DosyaYolu;
            if (dosyatipi == ".mp4")
            {
                var virtualpath = path.Remove(0, 63);
                var source = virtualpath.Replace(@"\", @"/");
                var src = "/" + source;
                var url = src;
                result.Code = 0;
                result.Data = dosyatipi;
                result.Message = url;
                return result;
            }
            else if (dosyatipi == ".png" || dosyatipi == ".jpeg" || dosyatipi == ".jpg")
            {
                var virtualpath = path.Remove(0, 63);
                var source = virtualpath.Replace(@"\", @"/");
                var src = "/" + source;
                var url = src;
                result.Code = 1;
                result.Data = dosyatipi;
                result.Message = url;
                return result;
            }
            else if (dosyatipi == ".pdf")
            {
                var virtualpath = path.Remove(0, 63);
                var source = virtualpath.Replace(@"\", @"/");
                var src = "/" + source;
                var url = src;
                result.Code = 2;
                result.Message = url + "," + dosyaadi;
                result.Data = dosyatipi;
                return result;

            }
            else if (dosyatipi == ".txt")
            {
                var virtualpath = path.Remove(0, 63);
                var source = virtualpath.Replace(@"\", @"/");
                var src = "/" + source;
                var url = src;
                result.Code = 3;
                result.Data = dosyatipi;
                result.Message = url;
                return result;
            }
            return result;
        }
        public void ShareLinkTB_Ekle(int DosyaId, string DosyaAdi, bool? DosyaMi, int? PaylastigimKisiler, string Guid, int? ToplamOnizleme, bool? Durumu, bool? Global)
        {
            ShareLink share = new ShareLink
            {
                Guid = Guid,
                PaylastigimKisiler = PaylastigimKisiler,
                DosyaId = DosyaId,
                DosyaAdi = DosyaAdi,
                DosyaMi = DosyaMi,
                Global = Global,
                Durumu = Durumu,
                ToplamOnizleme = ToplamOnizleme,
                YaratilmaZamani = DateTime.Now
            };
            db.ShareLink.Add(share);
            db.SaveChanges();
        }
        public void DosyaTB_Ekle(string DosyaYolu, int? DosyaBoyutu, string DosyaTipi, string DosyaAdi, int? ParentId, int KullaniciId, bool? DosyaMi)
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
        public void PaylasilanlarTB_Ekle(int DosyaId, string Adi, int PaylasilanKisi, int KimPaylasti, int Yetki, string PaylasilaninYolu, int? PaylasilanlarinParentId, bool? DosyaMi)
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
