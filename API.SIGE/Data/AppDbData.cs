using API.SIGE.Models;
using Microsoft.EntityFrameworkCore;

namespace API.SIGE.Data;

public class AppDbData : DbContext
{
    public AppDbData(DbContextOptions<AppDbData> options)
        : base(options)
    {
    }

    public DbSet<Caixilho> Caixilhos => Set<Caixilho>();
    public DbSet<Obra> Obras => Set<Obra>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<TipoUsuario> TiposUsuario => Set<TipoUsuario>();
    public DbSet<FamiliaCaixilho> FamiliaCaixilhos => Set<FamiliaCaixilho>();
    public DbSet<Cargo> Cargos => Set<Cargo>();
    public DbSet<UsuarioCargo> UsuarioCargos => Set<UsuarioCargo>();
    public DbSet<Medicao> Medicoes => Set<Medicao>();
    public DbSet<ProducaoFamilia> ProducoesFamilia => Set<ProducaoFamilia>();
    public DbSet<Anexo> Anexos => Set<Anexo>();
    public DbSet<Notificacao> Notificacoes => Set<Notificacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Usuario → TipoUsuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasOne(u => u.TipoUsuario)
                .WithMany()
                .HasForeignKey("IdTipoUsuario")
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Cargo — unique index em TipoCargo + seed
        modelBuilder.Entity<Cargo>(entity =>
        {
            entity.HasIndex(c => c.TipoCargo).IsUnique();

            entity.HasData(
                new Cargo { IdCargo = 1, TipoCargo = TipoCargo.Gerente, DescricaoCargo = "Gerente" },
                new Cargo { IdCargo = 2, TipoCargo = TipoCargo.ResponsavelVerificacao, DescricaoCargo = "Responsável pela Verificação" },
                new Cargo { IdCargo = 3, TipoCargo = TipoCargo.ResponsavelMedicao, DescricaoCargo = "Responsável pela Medição" },
                new Cargo { IdCargo = 4, TipoCargo = TipoCargo.ResponsavelProducao, DescricaoCargo = "Responsável pela Produção" }
            );
        });

        // UsuarioCargo — unique composto (IdUsuario, IdCargo)
        modelBuilder.Entity<UsuarioCargo>(entity =>
        {
            entity.HasIndex(uc => new { uc.IdUsuario, uc.IdCargo }).IsUnique();

            entity.HasOne(uc => uc.Usuario)
                .WithMany(u => u.UsuarioCargos)
                .HasForeignKey(uc => uc.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(uc => uc.Cargo)
                .WithMany(c => c.UsuarioCargos)
                .HasForeignKey(uc => uc.IdCargo)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Obra → Usuarios responsáveis (3 FKs opcionais)
        modelBuilder.Entity<Obra>(entity =>
        {
            entity.HasOne(o => o.ResponsavelVerificacao)
                .WithMany()
                .HasForeignKey(o => o.IdResponsavelVerificacao)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(o => o.ResponsavelMedicao)
                .WithMany()
                .HasForeignKey(o => o.IdResponsavelMedicao)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(o => o.ResponsavelProducao)
                .WithMany()
                .HasForeignKey(o => o.IdResponsavelProducao)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // FamiliaCaixilho → Obra
        modelBuilder.Entity<FamiliaCaixilho>(entity =>
        {
            entity.HasOne(f => f.Obra)
                .WithMany()
                .HasForeignKey(f => f.IdObra)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Medicao → FamiliaCaixilho + Usuario
        modelBuilder.Entity<Medicao>(entity =>
        {
            entity.HasOne(m => m.FamiliaCaixilho)
                .WithMany()
                .HasForeignKey(m => m.IdFamiliaCaixilho)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(m => m.Responsavel)
                .WithMany()
                .HasForeignKey(m => m.IdResponsavel)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ProducaoFamilia → FamiliaCaixilho + Usuario
        modelBuilder.Entity<ProducaoFamilia>(entity =>
        {
            entity.HasOne(p => p.FamiliaCaixilho)
                .WithMany()
                .HasForeignKey(p => p.IdFamiliaCaixilho)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(p => p.Responsavel)
                .WithMany()
                .HasForeignKey(p => p.IdResponsavel)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Anexo → Medicao / ProducaoFamilia / Usuario
        modelBuilder.Entity<Anexo>(entity =>
        {
            entity.HasOne(a => a.Medicao)
                .WithMany()
                .HasForeignKey(a => a.IdMedicao)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(a => a.ProducaoFamilia)
                .WithMany()
                .HasForeignKey(a => a.IdProducaoFamilia)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(a => a.Usuario)
                .WithMany()
                .HasForeignKey(a => a.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Notificacao → Usuario + Obra
        modelBuilder.Entity<Notificacao>(entity =>
        {
            entity.HasOne(n => n.UsuarioDestino)
                .WithMany()
                .HasForeignKey(n => n.IdUsuarioDestino)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(n => n.Obra)
                .WithMany()
                .HasForeignKey(n => n.IdObra)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
