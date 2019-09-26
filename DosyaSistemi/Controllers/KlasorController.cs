using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DosyaSistemiDAL.BusinessLayer.Abstract;
using DosyaSistemiDAL.BusinessLayer.Concrete;
using DosyaSistemiDAL.BusinessLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DosyaSistemi.Controllers
{
    public class KlasorController : Controller
    {
        private readonly IKlasorDal klasorDal;
        public KlasorController(IKlasorDal klasorDal)
        {
            this.klasorDal = klasorDal;
        }
        public IActionResult AnaSayfa(int? id)
        {
            var kullaniciId = Convert.ToInt32(HttpContext.Session.GetString("kullaniciId"));
            ViewBag.YuklemeHata1 = TempData["YuklemeHata1"];
            ViewBag.Basarili1 = TempData["Basarili1"];
            ViewBag.IcindeOldugumKlasorId = id;
            var result = klasorDal.AnaSayfa(id, kullaniciId);
            if (result.Code == 1)
            {
                return View(result.Data);
            }
            else
            {
                return NotFound();
            }
        }
        public IActionResult Creates(string YeniKlasorAdi)
        {
            bool result;
            var session = HttpContext.Session.GetString("kullaniciId");
            var sonuc = klasorDal.Creates(YeniKlasorAdi, session);
            if (sonuc.Code == 0)
            {
                result = sonuc.Data;
                return Json(result);
            }
            else if (sonuc.Code == 1)
            {
                result = sonuc.Data;
                return Json(result);
            }
            else
            {
                result = sonuc.Data;
                TempData["Basarili1"] = sonuc.Message;
                return Json(result);
            }
        }
        public IActionResult Edits(int DosyaId, string KlasorAdi)
        {
            bool result;
            var session = Convert.ToInt32(HttpContext.Session.GetString("kullaniciId"));
            var sonuc = klasorDal.Edits(DosyaId, KlasorAdi, session);
            if (sonuc.Code == 0)
            {
                result = sonuc.Data;
                return Json(result);
            }
            else
            {
                result = sonuc.Data;
                return Json(result);
            }
        }
        public IActionResult Sil(int KlasorId)
        {
            var session = HttpContext.Session.GetString("kullaniciId");
            var sonuc = klasorDal.Sil(KlasorId, session);
            if (sonuc.Code == 1)
            {
                TempData["Basarili1"] = sonuc.Message;
                bool result = sonuc.Data;
                return Json(result);
            }
            else
            {
                return NotFound();
            }
        }
        public IActionResult KlasorAc(int? id)
        {
            ViewBag.YuklemeHata = TempData["YuklemeHata"];
            ViewBag.Basarili = TempData["Basarili"];
            ViewBag.MainId = id;
            TempData["klasordownloadicinid"] = id;
            var result = klasorDal.KlasorAc(id);
            ViewBag.MapList = result.ExternalData;
            return View(result.Data);
        }
        public IActionResult CreateINFolder(string YeniKlasorAdi, int EskiKlasorId)
        {
            var session = HttpContext.Session.GetString("kullaniciId");
            var sonuc = klasorDal.CreateInFolder(YeniKlasorAdi, EskiKlasorId, session);
            bool result;
            if (sonuc.Code == 0)
            {
                TempData["YuklemeHata"] = sonuc.Message;
                result = sonuc.Data;
                return Json(result);
            }
            else if (sonuc.Code == 1)
            {
                TempData["YuklemeHata"] = sonuc.Message;
                result = sonuc.Data;
                return Json(result);
            }
            else
            {
                TempData["Basarili"] = sonuc.Message;
                result = sonuc.Data;
                return Json(result);
            }
        }
        public IActionResult RenameINFolder(string KlasorAdi, int EskiKlasorId, int KlasorId)
        {
            var session = HttpContext.Session.GetString("kullaniciId");
            var sonuc = klasorDal.RenameINFolder(KlasorAdi, EskiKlasorId, KlasorId, session);
            bool result;
            if (sonuc.Code == 0)
            {
                result = sonuc.Data;
                return Json(result);
            }
            else
            {
                result = sonuc.Data;
                return Json(result);
            }
        }
        public IActionResult DeleteINFolder(int KlasorId,int EskiKlasorId)
        {
            bool result = false;
            var session = HttpContext.Session.GetString("kullaniciId");
            var sonuc = klasorDal.DeleteINFolder(KlasorId, EskiKlasorId,session);
            if (sonuc.Code == 1)
            {
                TempData["Basarili"] = sonuc.Message;
                result = sonuc.Data;
            }
            return Json(result);
        }
        public IActionResult DosyaINYukle() => View();
        [HttpPost]
        public IActionResult DosyaINYukle(DosyaYukleme yuklenecekDosya, int klasorId)
        {
            string referer = Request.Headers["Referer"].ToString();
            var session = HttpContext.Session.GetString("kullaniciId");
            var sonuc = klasorDal.DosyaINYukle(yuklenecekDosya, klasorId,session);
            if (sonuc.Code == 0)
            {
                TempData["YuklemeHata"] = sonuc.Message;
                return Redirect(referer);
            }
            else if (sonuc.Code == 1)
            {
                ViewBag.Message = sonuc.Message;
                return View();
            }
            else if (sonuc.Code == 2)
            {
                TempData["Basarili"] = sonuc.Message;
                return Redirect(referer);
            }
            else
            {
                TempData["YuklemeHata"] = sonuc.Message;
                return Redirect(referer);
            }
        }
        public IActionResult DosyaINDelete(int DosyaId)
        {
            bool result = false;
            var sonuc = klasorDal.DosyaINDelete(DosyaId);
            if (sonuc.Code == 1)
            {
                result = sonuc.Data;
                TempData["Basarili"] = sonuc.Message;
                return Json(result);
            }
            else
            {
                return Json(result);
            }
        }
        public IActionResult DosyaINRename(int DosyaId,string DosyaAdi,int EskiKlasorId)
        {
            bool result = false;
            var session = HttpContext.Session.GetString("kullaniciId");
            var sonuc = klasorDal.DosyaINRename(DosyaId, DosyaAdi, EskiKlasorId,session);
            if (sonuc.Code == 0)
            {
                result = sonuc.Data;
                return Json(result);
            }
            else if (sonuc.Code == 1)
            {
                result = sonuc.Data;
                return Json(result);
            }
            else
            {
                return Json(result);
            }
        }
        public FileResult KlasorDosyaDownload(string IDs)
        {
            var session = HttpContext.Session.GetString("kullaniciId");
            var result = klasorDal.KlasorDosyaDownload(IDs,session);
            if (result.Code == 1)
            {
                return File(result.Data, "application/zip", "Indirilenler.zip");
            }
            else
            {
                return null;
            }
        }
        public FileResult PaylasilanlariTopluIndirme(string IDs)
        {
            var session = HttpContext.Session.GetString("kullaniciId");
            var sonuc = klasorDal.PaylasilanlariTopluIndirme(IDs,session);
            if (sonuc.Code == 1)
            {
                return File(sonuc.Data, "application/zip", "Indirilenler.zip");
            }
            else
            {
                return null;
            }
        }
        public FileResult KlasorIciDosyaDownload(string IDs)
        {
            var session = HttpContext.Session.GetString("kullaniciId");
            var sonuc = klasorDal.KlasorIciDosyaDownload(IDs,session);
            if (sonuc.Code == 1)
            {
                return File(sonuc.Data, "application/zip", "Indirilenler.zip");
            }
            else
            {
                return null;
            }
        }
        public ActionResult KlasorDownload(int KlasorId)
        {
            var sonuc = klasorDal.KlasorDownload(KlasorId);
            if (sonuc.Code == 1)
            {
                return File(sonuc.Data, "application/zip", "Indirilenler.zip");
            }
            else
            {
                return null;
            }
        }
        public FileResult DosyaDownload(int DosyaId)
        {
            var sonuc = klasorDal.DosyaDownload(DosyaId);
            if (sonuc.Code == 1)
            {
                return File(sonuc.Data, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(sonuc.Message));
            }
            else
            {
                return null;
            }
        }
        public IActionResult FileCut(string Dosyaid)
        {
            string referer = Request.Headers["Referer"].ToString();
            var result = klasorDal.FileCut(Dosyaid);
            if (result.Code == 0)
            {
                TempData["secimyap"] = result.Message;
            }
            if (result.Code == 1)
            {
                TempData["tasinacakdosya"] = Dosyaid;
            }
            return Redirect(referer);
        }
        public IActionResult FilePaste(int DosyaId)
        {
            var session = HttpContext.Session.GetString("kullaniciId");
            string referer = Request.Headers["Referer"].ToString();
            var tempdosyaid = TempData["tasinacakdosya"].ToString();
            var result = klasorDal.FilePaste(DosyaId, tempdosyaid,session);
            if (result.Code == 0)
            {
                TempData["yapistirmaHatasi"] = result.Message;
                return Redirect(referer);
            }
            else if (result.Code == 1)
            {
                TempData["isexistcutfile"] = result.Message;
                return Redirect(referer);
            }
            else if (result.Code == 2)
            {
                TempData["isexistcutfile"] = result.Message;
                return Redirect(referer);
            }
            else if (result.Code == 3)
            {
                TempData["isexistcutfile"] = result.Message;
                return Redirect(referer);
            }
            else if (result.Code == 4)
            {
                TempData["isexistcutfile"] = result.Message;
                return Redirect(referer);
            }
            foreach (var key in TempData.Keys.ToList())
            {
                TempData.Remove(key);
            }
            return Redirect(referer);
        }
        public IActionResult Info(int id)
        {
            var session = HttpContext.Session.GetString("kullaniciId");
            var sonuc = klasorDal.Info(id,session);
            if (sonuc.Code == 1)
            {
                return Json(sonuc.Data);
            }
            else
            {
                return null;
            }
        }
        [HttpPost]
        public IActionResult YetkiDegisikligiAjax(Dictionary<String,Gift> myDictionary, int KlasorDosyaId)
        {
            var session = HttpContext.Session.GetString("kullaniciId");
            var sonuc = klasorDal.YetkiDegisikligiAjax(myDictionary, KlasorDosyaId,session);
            bool result;
            if (sonuc.Code == 0)
            {
                TempData["yetkidegisikligihata"] = sonuc.Message;
                return RedirectToAction("AnaSayfa", "Klasor");
            }
            else if (sonuc.Code == 1)
            {
                result = sonuc.Data;
                return Json(result);
            }
            else if (sonuc.Code == 3)
            {
                result = sonuc.Data;
                return Json(result);
            }
            else
            {
                result = sonuc.Data;
                return Json(result);
            }
        }
        public IActionResult LinkPaylas(string DosyaId)
        {
            var session = HttpContext.Session.GetString("kullaniciId");
            var sonuc = klasorDal.LinkPaylas(DosyaId,session);
            if (sonuc.Code == 0)
            {
                return Json(new { result = sonuc.Data });
            }
            else
            {
                return Json(new { result = sonuc.Data, link = sonuc.Message });
            }
        }
        public IActionResult ShareWithLink(string code)
        {
            var sonuc = klasorDal.ShareWithLink(code);
            if (sonuc.Code == 1)
            {
                return View(sonuc.Data);
            }
            else
            {
                TempData["sharewithlinkhatasie0"] = sonuc.Message;
                return RedirectToAction("AnaSayfa", "Klasor");
            }
        }
        public IActionResult PaylastigimLinkler()
        {
            var session = HttpContext.Session.GetString("kullaniciId");
            var result = klasorDal.PaylastigimLinkler(session);
            if (result.Code == 1)
            {
                return View(result.Data);
            }
            else
            {
                return null;
            }
        }
        public IActionResult LinkKapat2(int DosyaId, int Checked)
        {
            var result = klasorDal.LinkKapat2(DosyaId, Checked);
            if (result.Code == 1)
            {
                return Json(new { result = result.Data });
            }
            else
            {
                return Json(new { result = result.Data });
            }
        }
        public IActionResult VideoPlayer(int DosyaId)
        {
            bool result = false;
            var sonuc = klasorDal.VideoPlayer(DosyaId);
            if (sonuc.Code == 0)
            {
                result = true;
                return Json(data: new { result = result, dosyatipi = sonuc.Data, url = sonuc.Message });
            }
            else if (sonuc.Code == 1)
            {
                result = true;
                return Json(new { result = result, dosyatipi = sonuc.Data, url = sonuc.Message });
            }
            else if (sonuc.Code == 2)
            {
                result = true;
                var aa = sonuc.Message;
                string[] bol = aa.Split(',');
                var url = bol[0];
                var dosyaadi = bol[1];
                return Json(new { result = result, dosyatipi = sonuc.Data, url = url, dosyaadi = dosyaadi });
            }
            else if (sonuc.Code == 3)
            {
                result = true;
                return Json(new { result = result, dosyatipi = sonuc.Data, url = sonuc.Message });
            }
            return Json(result);
        }
    }
}