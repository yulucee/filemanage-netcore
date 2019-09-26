using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DosyaSistemiDAL.BusinessLayer.Abstract;
using DosyaSistemiDAL.BusinessLayer.Concrete;
using DosyaSistemiDAL.BusinessLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DosyaSistemi.Controllers
{
    public class PaylasilanlarController : Controller
    {
        private readonly IPaylasilanlarDal paylasilanlarDal;
        public PaylasilanlarController()
        {
            paylasilanlarDal = new EfPaylasilanlarDal();
        }
        public IActionResult Index(int? id)
        {
            var session = HttpContext.Session.GetString("kullaniciId");
            var result = paylasilanlarDal.Index(id,session);
            if (result.Code == 1)
            {
                return View(result.Data);
            }
            else
            {
                return NotFound();
            }
        }
        public IActionResult PaylasilanlariAc(int? id)
        {
            var session = HttpContext.Session.GetString("kullaniciId");
            ViewBag.MainId = id;
            var result = paylasilanlarDal.PaylasilanlariAc(id,session);
            if (result.Code == 1)
            {
                return View(result.Data);
            }
            else
            {
                return NotFound();
            }
        }
        public IActionResult Paylastiklarim(int? id)
        {
            var session = HttpContext.Session.GetString("kullaniciId");
            var result = paylasilanlarDal.Paylastiklarim(id,session);
            if (result.Code == 1)
            {
                return View(result.Data);
            }
            else
            {
                return NotFound();
            }
        }
        public IActionResult PaylastiklarimiAc(int? id)
        {
            var session = HttpContext.Session.GetString("kullaniciId");
            var result = paylasilanlarDal.PaylastiklarimiAc(id,session);
            if (result.Code == 1)
            {
                return View(result.Data);
            }
            else
            {
                return NotFound();
            }
        }
        public IActionResult Bilgi(string id)
        {
            var session = HttpContext.Session.GetString("kullaniciId");
            var result = paylasilanlarDal.Bilgi(id,session);
            if (result.Code == 0)
            {
                TempData["dosyasharehatasi"] = result.Data;
                return RedirectToAction("AnaSayfa", "Klasor");
            }
            else
            {
                ViewBag.kullanicilistesi = result.Data;
                return View();
            }
        }
        public IActionResult BenimlePaylasanlarBilgi(string id)
        {
            var session = HttpContext.Session.GetString("kullaniciId");
            var result = paylasilanlarDal.BenimlePaylasilanlarBilgi(id,session);
            if (result.Code == 0)
            {
                TempData["dosyasharehatasi"] = result.Data;
                return RedirectToAction("AnaSayfa", "Klasor");
            }
            else
            {
                ViewBag.kullaniciliste = result.Data;
                return View();
            }
        }
        public IActionResult PaylasilanaEkle(DosyaYukleme yuklenecekDosya, int klasorId)
        {
            string referer = Request.Headers["Referer"].ToString();
            var result = paylasilanlarDal.PaylasilanaEkle(yuklenecekDosya, klasorId);
            if (result.Code == 0)
            {
                TempData["YuklemeHata"] = result.Data;
                return Redirect(referer);
            }
            else if (result.Code == 1)
            {
                ViewBag.Message = result.Data;
                return View();
            }
            else if (result.Code == 2)
            {
                TempData["Basarili"] = result.Data;
                return Redirect(referer);
            }
            else
            {
                TempData["YuklemeHata"] = result.Data;
                return Redirect(referer);
            }
        }
        public IActionResult YetkiKaldirma(int PaylasmaId)
        {
            var sonuc = paylasilanlarDal.YetkiKaldirma(PaylasmaId);
            if (sonuc.Code == 1)
            {
                bool result = true;
                string html = sonuc.Data;
                return Json(new { result = result, html = html });
            }
            else
            {
                return NotFound();
            }
        }
    }
}