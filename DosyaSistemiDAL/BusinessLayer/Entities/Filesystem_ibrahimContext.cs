using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DosyaSistemiDAL.Businesslayer.Entities
{
    public partial class Filesystem_ibrahimContext : DbContext
    {
        public Filesystem_ibrahimContext()
        {
        }

        public Filesystem_ibrahimContext(DbContextOptions<Filesystem_ibrahimContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ActionFilter> ActionFilter { get; set; }
        public virtual DbSet<Dosya> Dosya { get; set; }
        public virtual DbSet<Kullanici> Kullanici { get; set; }
        public virtual DbSet<Mail> Mail { get; set; }
        public virtual DbSet<Paylasilanlar> Paylasilanlar { get; set; }
        public virtual DbSet<ShareLink> ShareLink { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=91.229.35.248;Initial Catalog=Filesystem_ibrahim;Persist Security Info=True;User ID=ibrahim.yuluce;Password=awq6tQN9uKUVA8VY");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "3.0.0-preview5.19227.1");

            modelBuilder.Entity<ActionFilter>(entity =>
            {
                entity.HasKey(e => e.Sira);

                entity.Property(e => e.IpAdresi).HasMaxLength(50);

                entity.Property(e => e.Tarih).HasColumnType("datetime");
            });

            modelBuilder.Entity<Dosya>(entity =>
            {
                entity.Property(e => e.DosyaTipi).HasMaxLength(20);

                entity.Property(e => e.OlusturulmaZamani).HasColumnType("datetime");

                entity.HasOne(d => d.Kullanici)
                    .WithMany(p => p.Dosya)
                    .HasForeignKey(d => d.KullaniciId)
                    .HasConstraintName("FK_Dosya_Kullanici");
            });

            modelBuilder.Entity<Kullanici>(entity =>
            {
                entity.Property(e => e.KullaniciAdi).HasMaxLength(50);

                entity.Property(e => e.KullaniciSoyadi).HasMaxLength(50);

                entity.Property(e => e.Tc).HasColumnName("TC");
            });

            modelBuilder.Entity<Mail>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Ip)
                    .HasColumnName("ip")
                    .HasMaxLength(50);

                entity.Property(e => e.Tarih).HasColumnType("datetime");
            });

            modelBuilder.Entity<Paylasilanlar>(entity =>
            {
                entity.HasKey(e => e.ToplamPaylasmaId);

                entity.Property(e => e.PaylasilmaTarihi).HasColumnType("datetime");

                entity.HasOne(d => d.Dosya)
                    .WithMany(p => p.Paylasilanlar)
                    .HasForeignKey(d => d.DosyaId)
                    .HasConstraintName("FK_Paylasilanlar_Dosya");

                entity.HasOne(d => d.Kullanici)
                    .WithMany(p => p.Paylasilanlar)
                    .HasForeignKey(d => d.PaylasilanKisi)
                    .HasConstraintName("FK_Paylasilanlar_Kullanici");
            });

            modelBuilder.Entity<ShareLink>(entity =>
            {
                entity.Property(e => e.YaratilmaZamani).HasColumnType("datetime");
            });
        }
    }
}
