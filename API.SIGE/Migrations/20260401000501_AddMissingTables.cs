using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace API.SIGE.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add missing columns to Obra (if they don't exist)
            migrationBuilder.Sql(@"
                DO $$ BEGIN
                    ALTER TABLE ""Obra"" ADD COLUMN ""StatusObra"" integer NOT NULL DEFAULT 0;
                EXCEPTION WHEN duplicate_column THEN NULL;
                END $$;

                DO $$ BEGIN
                    ALTER TABLE ""Obra"" ADD COLUMN ""PercentualMedicao"" real NOT NULL DEFAULT 0;
                EXCEPTION WHEN duplicate_column THEN NULL;
                END $$;

                DO $$ BEGIN
                    ALTER TABLE ""Obra"" ADD COLUMN ""PercentualProducao"" real NOT NULL DEFAULT 0;
                EXCEPTION WHEN duplicate_column THEN NULL;
                END $$;

                DO $$ BEGIN
                    ALTER TABLE ""Obra"" ADD COLUMN ""PesoProduzido"" real NOT NULL DEFAULT 0;
                EXCEPTION WHEN duplicate_column THEN NULL;
                END $$;

                DO $$ BEGIN
                    ALTER TABLE ""Obra"" ADD COLUMN ""PercentualConclusao"" real NOT NULL DEFAULT 0;
                EXCEPTION WHEN duplicate_column THEN NULL;
                END $$;

                DO $$ BEGIN
                    ALTER TABLE ""Obra"" ADD COLUMN ""DataConclusao"" timestamp with time zone;
                EXCEPTION WHEN duplicate_column THEN NULL;
                END $$;

                DO $$ BEGIN
                    ALTER TABLE ""Obra"" ADD COLUMN ""Observacoes"" character varying(500);
                EXCEPTION WHEN duplicate_column THEN NULL;
                END $$;

                DO $$ BEGIN
                    ALTER TABLE ""Obra"" ADD COLUMN ""Finalizado"" boolean NOT NULL DEFAULT false;
                EXCEPTION WHEN duplicate_column THEN NULL;
                END $$;

                DO $$ BEGIN
                    ALTER TABLE ""Obra"" ADD COLUMN ""ImagemObraPath"" character varying(255);
                EXCEPTION WHEN duplicate_column THEN NULL;
                END $$;

                DO $$ BEGIN
                    ALTER TABLE ""Obra"" ADD COLUMN ""IdResponsavelVerificacao"" integer;
                EXCEPTION WHEN duplicate_column THEN NULL;
                END $$;

                DO $$ BEGIN
                    ALTER TABLE ""Obra"" ADD COLUMN ""IdResponsavelMedicao"" integer;
                EXCEPTION WHEN duplicate_column THEN NULL;
                END $$;

                DO $$ BEGIN
                    ALTER TABLE ""Obra"" ADD COLUMN ""IdResponsavelProducao"" integer;
                EXCEPTION WHEN duplicate_column THEN NULL;
                END $$;
            ");

            // Add FK constraints for Obra responsaveis (if they don't exist)
            migrationBuilder.Sql(@"
                DO $$ BEGIN
                    ALTER TABLE ""Obra"" ADD CONSTRAINT ""FK_Obra_Usuario_IdResponsavelVerificacao""
                        FOREIGN KEY (""IdResponsavelVerificacao"") REFERENCES ""Usuario"" (""IdUsuario"") ON DELETE SET NULL;
                EXCEPTION WHEN duplicate_object THEN NULL;
                END $$;

                DO $$ BEGIN
                    ALTER TABLE ""Obra"" ADD CONSTRAINT ""FK_Obra_Usuario_IdResponsavelMedicao""
                        FOREIGN KEY (""IdResponsavelMedicao"") REFERENCES ""Usuario"" (""IdUsuario"") ON DELETE SET NULL;
                EXCEPTION WHEN duplicate_object THEN NULL;
                END $$;

                DO $$ BEGIN
                    ALTER TABLE ""Obra"" ADD CONSTRAINT ""FK_Obra_Usuario_IdResponsavelProducao""
                        FOREIGN KEY (""IdResponsavelProducao"") REFERENCES ""Usuario"" (""IdUsuario"") ON DELETE SET NULL;
                EXCEPTION WHEN duplicate_object THEN NULL;
                END $$;
            ");

            // Add indexes for Obra responsaveis
            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS ""IX_Obra_IdResponsavelMedicao"" ON ""Obra"" (""IdResponsavelMedicao"");
                CREATE INDEX IF NOT EXISTS ""IX_Obra_IdResponsavelProducao"" ON ""Obra"" (""IdResponsavelProducao"");
                CREATE INDEX IF NOT EXISTS ""IX_Obra_IdResponsavelVerificacao"" ON ""Obra"" (""IdResponsavelVerificacao"");
            ");

            // Add missing columns to FamiliaCaixilho
            migrationBuilder.Sql(@"
                DO $$ BEGIN
                    ALTER TABLE ""FamiliaCaixilho"" ADD COLUMN ""StatusFamilia"" integer NOT NULL DEFAULT 0;
                EXCEPTION WHEN duplicate_column THEN NULL;
                END $$;
            ");

            // Add missing columns to Caixilho
            migrationBuilder.Sql(@"
                DO $$ BEGIN
                    ALTER TABLE ""Caixilho"" ADD COLUMN ""Liberado"" boolean NOT NULL DEFAULT false;
                EXCEPTION WHEN duplicate_column THEN NULL;
                END $$;

                DO $$ BEGIN
                    ALTER TABLE ""Caixilho"" ADD COLUMN ""DataLiberacao"" timestamp with time zone;
                EXCEPTION WHEN duplicate_column THEN NULL;
                END $$;

                DO $$ BEGIN
                    ALTER TABLE ""Caixilho"" ADD COLUMN ""DescricaoCaixilho"" text;
                EXCEPTION WHEN duplicate_column THEN NULL;
                END $$;

                DO $$ BEGIN
                    ALTER TABLE ""Caixilho"" ADD COLUMN ""StatusProducao"" integer NOT NULL DEFAULT 0;
                EXCEPTION WHEN duplicate_column THEN NULL;
                END $$;

                DO $$ BEGIN
                    ALTER TABLE ""Caixilho"" ADD COLUMN ""ObraId"" integer NOT NULL DEFAULT 0;
                EXCEPTION WHEN duplicate_column THEN NULL;
                END $$;
            ");

            // Add FK and index for Caixilho.ObraId
            migrationBuilder.Sql(@"
                DO $$ BEGIN
                    ALTER TABLE ""Caixilho"" ADD CONSTRAINT ""FK_Caixilho_Obra_ObraId""
                        FOREIGN KEY (""ObraId"") REFERENCES ""Obra"" (""IdObra"") ON DELETE CASCADE;
                EXCEPTION WHEN duplicate_object THEN NULL;
                END $$;

                CREATE INDEX IF NOT EXISTS ""IX_Caixilho_ObraId"" ON ""Caixilho"" (""ObraId"");
            ");

            // Create Cargo table
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS ""Cargo"" (
                    ""IdCargo"" integer GENERATED BY DEFAULT AS IDENTITY,
                    ""TipoCargo"" integer NOT NULL,
                    ""DescricaoCargo"" character varying(100) NOT NULL,
                    CONSTRAINT ""PK_Cargo"" PRIMARY KEY (""IdCargo"")
                );
                CREATE UNIQUE INDEX IF NOT EXISTS ""IX_Cargo_TipoCargo"" ON ""Cargo"" (""TipoCargo"");
            ");

            // Create UsuarioCargo table
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS ""UsuarioCargo"" (
                    ""IdUsuarioCargo"" integer GENERATED BY DEFAULT AS IDENTITY,
                    ""IdUsuario"" integer NOT NULL,
                    ""IdCargo"" integer NOT NULL,
                    CONSTRAINT ""PK_UsuarioCargo"" PRIMARY KEY (""IdUsuarioCargo""),
                    CONSTRAINT ""FK_UsuarioCargo_Usuario_IdUsuario"" FOREIGN KEY (""IdUsuario"") REFERENCES ""Usuario"" (""IdUsuario"") ON DELETE RESTRICT,
                    CONSTRAINT ""FK_UsuarioCargo_Cargo_IdCargo"" FOREIGN KEY (""IdCargo"") REFERENCES ""Cargo"" (""IdCargo"") ON DELETE RESTRICT
                );
                CREATE INDEX IF NOT EXISTS ""IX_UsuarioCargo_IdCargo"" ON ""UsuarioCargo"" (""IdCargo"");
                CREATE UNIQUE INDEX IF NOT EXISTS ""IX_UsuarioCargo_IdUsuario_IdCargo"" ON ""UsuarioCargo"" (""IdUsuario"", ""IdCargo"");
            ");

            // Create Notificacao table
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS ""Notificacao"" (
                    ""IdNotificacao"" integer GENERATED BY DEFAULT AS IDENTITY,
                    ""IdUsuarioDestino"" integer NOT NULL,
                    ""Titulo"" character varying(200) NOT NULL,
                    ""Mensagem"" character varying(1000) NOT NULL,
                    ""Lida"" boolean NOT NULL DEFAULT false,
                    ""DataCriacao"" timestamp with time zone NOT NULL,
                    ""TipoNotificacao"" integer NOT NULL,
                    ""IdObra"" integer,
                    CONSTRAINT ""PK_Notificacao"" PRIMARY KEY (""IdNotificacao""),
                    CONSTRAINT ""FK_Notificacao_Obra_IdObra"" FOREIGN KEY (""IdObra"") REFERENCES ""Obra"" (""IdObra"") ON DELETE SET NULL,
                    CONSTRAINT ""FK_Notificacao_Usuario_IdUsuarioDestino"" FOREIGN KEY (""IdUsuarioDestino"") REFERENCES ""Usuario"" (""IdUsuario"") ON DELETE CASCADE
                );
                CREATE INDEX IF NOT EXISTS ""IX_Notificacao_IdObra"" ON ""Notificacao"" (""IdObra"");
                CREATE INDEX IF NOT EXISTS ""IX_Notificacao_IdUsuarioDestino"" ON ""Notificacao"" (""IdUsuarioDestino"");
            ");

            // Create Medicao table
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS ""Medicao"" (
                    ""IdMedicao"" integer GENERATED BY DEFAULT AS IDENTITY,
                    ""IdFamiliaCaixilho"" integer NOT NULL,
                    ""IdResponsavel"" integer NOT NULL,
                    ""Status"" integer NOT NULL,
                    ""DataInicio"" timestamp with time zone NOT NULL,
                    ""DataEstimadaConclusao"" timestamp with time zone,
                    ""DataConclusao"" timestamp with time zone,
                    ""Descricao"" character varying(200),
                    ""Observacoes"" character varying(500),
                    CONSTRAINT ""PK_Medicao"" PRIMARY KEY (""IdMedicao""),
                    CONSTRAINT ""FK_Medicao_FamiliaCaixilho_IdFamiliaCaixilho"" FOREIGN KEY (""IdFamiliaCaixilho"") REFERENCES ""FamiliaCaixilho"" (""IdFamiliaCaixilho"") ON DELETE CASCADE,
                    CONSTRAINT ""FK_Medicao_Usuario_IdResponsavel"" FOREIGN KEY (""IdResponsavel"") REFERENCES ""Usuario"" (""IdUsuario"") ON DELETE RESTRICT
                );
                CREATE INDEX IF NOT EXISTS ""IX_Medicao_IdFamiliaCaixilho"" ON ""Medicao"" (""IdFamiliaCaixilho"");
                CREATE INDEX IF NOT EXISTS ""IX_Medicao_IdResponsavel"" ON ""Medicao"" (""IdResponsavel"");
            ");

            // Create ProducaoFamilia table
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS ""ProducaoFamilia"" (
                    ""IdProducaoFamilia"" integer GENERATED BY DEFAULT AS IDENTITY,
                    ""IdFamiliaCaixilho"" integer NOT NULL,
                    ""IdResponsavel"" integer NOT NULL,
                    ""Status"" integer NOT NULL,
                    ""DataInicio"" timestamp with time zone NOT NULL,
                    ""DataEstimadaConclusao"" timestamp with time zone,
                    ""DataConclusao"" timestamp with time zone,
                    ""Descricao"" character varying(200),
                    ""Observacoes"" character varying(500),
                    CONSTRAINT ""PK_ProducaoFamilia"" PRIMARY KEY (""IdProducaoFamilia""),
                    CONSTRAINT ""FK_ProducaoFamilia_FamiliaCaixilho_IdFamiliaCaixilho"" FOREIGN KEY (""IdFamiliaCaixilho"") REFERENCES ""FamiliaCaixilho"" (""IdFamiliaCaixilho"") ON DELETE CASCADE,
                    CONSTRAINT ""FK_ProducaoFamilia_Usuario_IdResponsavel"" FOREIGN KEY (""IdResponsavel"") REFERENCES ""Usuario"" (""IdUsuario"") ON DELETE RESTRICT
                );
                CREATE INDEX IF NOT EXISTS ""IX_ProducaoFamilia_IdFamiliaCaixilho"" ON ""ProducaoFamilia"" (""IdFamiliaCaixilho"");
                CREATE INDEX IF NOT EXISTS ""IX_ProducaoFamilia_IdResponsavel"" ON ""ProducaoFamilia"" (""IdResponsavel"");
            ");

            // Create Anexo table
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS ""Anexo"" (
                    ""IdAnexo"" integer GENERATED BY DEFAULT AS IDENTITY,
                    ""NomeArquivo"" character varying(255) NOT NULL,
                    ""CaminhoArquivo"" character varying(500) NOT NULL,
                    ""TipoArquivo"" character varying(100),
                    ""TamanhoBytes"" bigint NOT NULL,
                    ""Dados"" bytea,
                    ""DataUpload"" timestamp with time zone NOT NULL,
                    ""TipoAnexo"" integer NOT NULL,
                    ""IdMedicao"" integer,
                    ""IdProducaoFamilia"" integer,
                    ""IdUsuario"" integer NOT NULL,
                    CONSTRAINT ""PK_Anexo"" PRIMARY KEY (""IdAnexo""),
                    CONSTRAINT ""FK_Anexo_Medicao_IdMedicao"" FOREIGN KEY (""IdMedicao"") REFERENCES ""Medicao"" (""IdMedicao"") ON DELETE SET NULL,
                    CONSTRAINT ""FK_Anexo_ProducaoFamilia_IdProducaoFamilia"" FOREIGN KEY (""IdProducaoFamilia"") REFERENCES ""ProducaoFamilia"" (""IdProducaoFamilia"") ON DELETE SET NULL,
                    CONSTRAINT ""FK_Anexo_Usuario_IdUsuario"" FOREIGN KEY (""IdUsuario"") REFERENCES ""Usuario"" (""IdUsuario"") ON DELETE RESTRICT
                );
                CREATE INDEX IF NOT EXISTS ""IX_Anexo_IdMedicao"" ON ""Anexo"" (""IdMedicao"");
                CREATE INDEX IF NOT EXISTS ""IX_Anexo_IdProducaoFamilia"" ON ""Anexo"" (""IdProducaoFamilia"");
                CREATE INDEX IF NOT EXISTS ""IX_Anexo_IdUsuario"" ON ""Anexo"" (""IdUsuario"");
            ");

            // Seed Cargo data
            migrationBuilder.Sql(@"
                INSERT INTO ""Cargo"" (""IdCargo"", ""TipoCargo"", ""DescricaoCargo"")
                VALUES (1, 1, 'Gerente'), (2, 2, 'Responsável pela Verificação'), (3, 3, 'Responsável pela Medição'), (4, 4, 'Responsável pela Produção')
                ON CONFLICT (""IdCargo"") DO NOTHING;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP TABLE IF EXISTS ""Anexo"";
                DROP TABLE IF EXISTS ""Medicao"";
                DROP TABLE IF EXISTS ""ProducaoFamilia"";
                DROP TABLE IF EXISTS ""Notificacao"";
                DROP TABLE IF EXISTS ""UsuarioCargo"";
                DROP TABLE IF EXISTS ""Cargo"";
            ");
        }
    }
}
