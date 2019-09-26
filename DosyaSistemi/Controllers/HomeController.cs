using DosyaSistemiDAL.Businesslayer.Entities;
using DosyaSistemiDAL.BusinessLayer.Abstract;
using DosyaSistemiDAL.BusinessLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DosyaSistemi.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeDal homeDal;
        public HomeController(IHomeDal homeDal)
        {
            this.homeDal = homeDal;
        }
        [HttpGet]
        public IActionResult Index() => View();
        [HttpPost]
        public IActionResult Index(Kullanici kullanici)
        {
            var result = homeDal.Index(kullanici);
            if (result.Data != null)
            {
                var login = (Kullanici)result.Data;
                HttpContext.Session.SetString("kullaniciId", login.KullaniciId.ToString());
                HttpContext.Session.SetString("kullaniciMaili", login.KullaniciMaili.ToString());
                HttpContext.Session.SetString("kullaniciAdi", login.KullaniciAdi.ToString());
                HttpContext.Session.SetString("kullaniciYetki", login.YetkiId.ToString());
                return RedirectToAction("AnaSayfa", "Klasor");
            }
            else
            {
                TempData["girisHatasi"] = result.Message;
                return View();
            }
        }
        [HttpGet]
        public IActionResult KayıtOl() => View();
        [HttpPost]
        public IActionResult KayıtOl(string name, string soyad, string mail, string sifre)
        {
            var response = HttpContext.Request.Form["g-recaptcha-response"];
            string secretKey = "6LeSf5sUAAAAAAVPDiZ-BKKTDKrbvjZPFKRpGDmp";
            var client = new WebClient();
            var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));
            var obj = JObject.Parse(result);
            var status = (bool)obj.SelectToken("success");
            if (status == true)
            {
                var sonuc = homeDal.KayıtOl(name, soyad, mail, sifre);
                if (sonuc.Code == 0)
                {
                    TempData["maildolu"] = sonuc.Message;
                    return View();
                }
                else
                {
                    TempData["uyekayitbasarili"] = sonuc.Message;
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["kayithata"] = "Beklenmeyen bir hata oluştu";
                return View();
            }
        }
        public IActionResult LogOut()
        {
            HttpContext.Session.Remove("kullaniciId");
            HttpContext.Session.Remove("kullaniciMaili");
            HttpContext.Session.Remove("kullaniciAdi");
            HttpContext.Session.Remove("kullaniciYetki");
            return RedirectToAction("Index", "Home");
        }
        [AllowAnonymous]
        public IActionResult ParolamıUnuttum() => View();
        [HttpPost]
        [AllowAnonymous]
        public IActionResult ParolamıUnuttum(string username)
        {
            var ipadres = HttpContext.Connection.RemoteIpAddress;
            var result = homeDal.ParolamıUnuttum(username,ipadres.ToString());
            if(result.Code == 0)
            {
                TempData["hata"] = result.Message;
            }
            else
            {
                TempData["mailbasarili"] = result.Message;
            }
            return View();
        }
        public IActionResult ResetPassword(string code,string sifre1,string sifre2)
        {
            string mail;
            var tempcode = "";
            if (TempData["mail"] == null)
            {
                mail = "";
            }
            else
            {
                tempcode = TempData["code"].ToString();
                mail = TempData["mail"].ToString();
            }
            var result = homeDal.ResetPassword(mail, tempcode, code, sifre1, sifre2);
            TempData["mail"] = result.Data;
            TempData["code"] = result.Message;
            if (result.Code == 0)
            {

                TempData["linkgecerlilik"] = result.Message;
                return RedirectToAction("Index", "Home");
            }
            else if (result.Code == 1)
            {
                TempData["sifreDegisikligi"] = result.Message;
                return View();
            }
            else if (result.Code == 2)
            {
                TempData["sifredegisikligi"] = result.Message;
                return RedirectToAction("Index", "Home");
            }
            else if (result.Code == 3)
            {
                TempData["sifreayni"] = result.Message;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["kontrolsifre"] = result.Message;
                return View();
            }
        }
        [HttpGet]
        public IActionResult KullanıcıEkle() => View();
        [HttpPost]
        public IActionResult KullanıcıEkle(long tckimlik, string adi, string soyadi, string dogumyili, string mailadresi, string sifre1, string sifre2)
        {
            var kisi = HttpContext.Session.GetString("kullaniciId");
            var result = homeDal.KullanıcıEkle(tckimlik, adi, soyadi, dogumyili, mailadresi, sifre1, sifre2,kisi);
            if (result.Code == 0)
            {
                ViewBag.MailDoluAnasayfa = result.Message;
            }
            else if (result.Code == 1)
            {
                ViewBag.UyeKaydi = result.Message;
            }
            else if (result.Code == 2)
            {
                TempData["kullanicisifrehatasi"] = result.Message;
            }
            else
            {
                TempData["tckimlikyanlis"] = result.Message;
            }
            return View();
        }
    }
}
