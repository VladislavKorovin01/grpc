using EpcDataApp.GrpcService.Entities;
using Microsoft.EntityFrameworkCore;
using Path = EpcDataApp.GrpcService.Entities.Path;

namespace EpcDataApp.GrpcService
{
    public partial class TestDbContext : DbContext
    {
        public TestDbContext()
        {
        }

        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Epc> Epcs { get; set; } = null!;
        public virtual DbSet<EpcEvent> EpcEvents { get; set; } = null!;
        public virtual DbSet<EventAdd> EventAdds { get; set; } = null!;
        public virtual DbSet<EventArrival> EventArrivals { get; set; } = null!;
        public virtual DbSet<EventDeparture> EventDepartures { get; set; } = null!;
        public virtual DbSet<EventSub> EventSubs { get; set; } = null!;
        public virtual DbSet<Park> Parks { get; set; } = null!;
        public virtual DbSet<Path> Paths { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Epc>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Epc");

                entity.Property(e => e.Number)
                    .HasMaxLength(8)
                    .IsFixedLength();
            });

            modelBuilder.Entity<EpcEvent>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("EpcEvent");
            });

            modelBuilder.Entity<EventAdd>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("EventAdd");
            });

            modelBuilder.Entity<EventArrival>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("EventArrival");

                entity.Property(e => e.TrainIndex)
                    .HasMaxLength(17)
                    .IsFixedLength();

                entity.Property(e => e.TrainNumber)
                    .HasMaxLength(4)
                    .IsFixedLength();
            });

            modelBuilder.Entity<EventDeparture>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("EventDeparture");

                entity.Property(e => e.TrainIndex)
                    .HasMaxLength(17)
                    .IsFixedLength();

                entity.Property(e => e.TrainNumber)
                    .HasMaxLength(4)
                    .IsFixedLength();
            });

            modelBuilder.Entity<EventSub>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("EventSub");
            });

            modelBuilder.Entity<Park>(entity =>
            {
                entity.ToTable("Park");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.AsuNumber)
                    .HasMaxLength(2)
                    .IsFixedLength();

                entity.Property(e => e.Name).HasMaxLength(25);
            });

            modelBuilder.Entity<Path>(entity =>
            {
                entity.ToTable("Path");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.AsuNumber)
                    .HasMaxLength(2)
                    .IsFixedLength();

                entity.HasOne(d => d.IdParkNavigation)
                    .WithMany(p => p.Paths)
                    .HasForeignKey(d => d.IdPark)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Path_Park");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
