using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DosyaSistemiDAL.Businesslayer.Entities;
using DosyaSistemiDAL.BusinessLayer.Abstract;
using DosyaSistemiDAL.BusinessLayer.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DosyaSistemi.Controllers
{
    public class DosyaController : Controller
    {
        private readonly IDosyaDal dosyaDal;
        public DosyaController(IDosyaDal dosyaDal)
        {
            this.dosyaDal = dosyaDal;
        }
        public IActionResult Edits(int DosyaId, string DosyaAdi)
        {
            bool result;
            var kullaniciId = HttpContext.Session.GetString("kullaniciId");
            var sonuc = dosyaDal.Edits(DosyaId, DosyaAdi, kullaniciId);
            result = (sonuc.Code == 1) ? true : false;
            return Json(result);
        }
        public IActionResult Sil(int DosyaId)
        {
            bool result;
            var sonuc = dosyaDal.Sil(DosyaId);
            result = (sonuc.Code == 1) ? true : false;
            return Json(result);
        }
        [HttpPost]
        public IActionResult DosyaYukle(DosyaYukleme yuklenecekDosya)
        {
            var kullaniciId = HttpContext.Session.GetString("kullaniciId");
            var result = dosyaDal.DosyaYukle(yuklenecekDosya, kullaniciId);
            if (result.Code == 0)
            {
                TempData["YuklemeHata1"] = result.Message;
                return RedirectToAction("AnaSayfa", "Klasor");
            }
            else
            {
                TempData["Basarili1"] = result.Message;
                return RedirectToAction("AnaSayfa", "Klasor");
            }
        }
        public IActionResult IcShare(string DosyaId, string KullaniciId, string Checked)
        {
            var session = HttpContext.Session.GetString("kullaniciId");
            var sonuc = dosyaDal.IcShare(DosyaId, KullaniciId, Checked,session);
            if (sonuc.Code == 0)
            {
                TempData["dosyasharehatasi"] = sonuc.Message;
                return RedirectToAction("AnaSayfa", "Klasor");
            }
            else
            {
                TempData["dosyaicpaylasimbasarili"] = sonuc.Message;
                return RedirectToAction("AnaSayfa", "Klasor");
            }
        }
    }
}