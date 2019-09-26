using System;
using System.Collections.Generic;
using System.Text;

namespace DosyaSistemiDAL.BusinessLayer.Models
{
    public class YetkiTipi
    {
        public enum Yetki
        {
            Admin,
            Uye
        }
        public string Name { get; private set; }
        public int Value { get; private set; }

        public YetkiTipi(int val, string name)
        {
            Value = val;
            Name = name;
        }
        public static YetkiTipi Admin { get; } = new YetkiTipi(1, "Admin");
        public static YetkiTipi Uye { get; } = new YetkiTipi(2, "Uye");
        public static IEnumerable<YetkiTipi> List()
        {
            return new[] { Admin, Admin };
        }

    }
}
