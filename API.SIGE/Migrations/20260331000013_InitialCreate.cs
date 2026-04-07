using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.SIGE.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cargo",
                columns: table => new
                {
                    IdCargo = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TipoCargo = table.Column<int>(type: "integer", nullable: false),
                    DescricaoCargo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cargo", x => x.IdCargo);
                });

            migrationBuilder.CreateTable(
                name: "TipoUsuario",
                columns: table => new
                {
                    IdTipoUsuario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomeTipoUsuario = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoUsuario", x => x.IdTipoUsuario);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomeUsuario = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Senha = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Telefone = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    IdTipoUsuario = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.IdUsuario);
                    table.ForeignKey(
                        name: "FK_Usuario_TipoUsuario_IdTipoUsuario",
                        column: x => x.IdTipoUsuario,
                        principalTable: "TipoUsuario",
                        principalColumn: "IdTipoUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Obra",
                columns: table => new
                {
                    IdObra = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Construtora = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Nro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Logradouro = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Bairro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Cep = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: false),
                    Uf = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Cnpj = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    DataInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataTermino = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PesoFinal = table.Column<float>(type: "real", nullable: false),
                    PesoProduzido = table.Column<float>(type: "real", nullable: false),
                    PercentualConclusao = table.Column<float>(type: "real", nullable: false),
                    DataConclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Observacoes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Finalizado = table.Column<bool>(type: "boolean", nullable: false),
                    ImagemObraPath = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    StatusObra = table.Column<int>(type: "integer", nullable: false),
                    PercentualMedicao = table.Column<float>(type: "real", nullable: false),
                    PercentualProducao = table.Column<float>(type: "real", nullable: false),
                    IdUsuario = table.Column<int>(type: "integer", nullable: false),
                    IdResponsavelVerificacao = table.Column<int>(type: "integer", nullable: true),
                    IdResponsavelMedicao = table.Column<int>(type: "integer", nullable: true),
                    IdResponsavelProducao = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Obra", x => x.IdObra);
                    table.ForeignKey(
                        name: "FK_Obra_Usuario_IdResponsavelMedicao",
                        column: x => x.IdResponsavelMedicao,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Obra_Usuario_IdResponsavelProducao",
                        column: x => x.IdResponsavelProducao,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Obra_Usuario_IdResponsavelVerificacao",
                        column: x => x.IdResponsavelVerificacao,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Obra_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioCargo",
                columns: table => new
                {
                    IdUsuarioCargo = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdUsuario = table.Column<int>(type: "integer", nullable: false),
                    IdCargo = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioCargo", x => x.IdUsuarioCargo);
                    table.ForeignKey(
                        name: "FK_UsuarioCargo_Cargo_IdCargo",
                        column: x => x.IdCargo,
                        principalTable: "Cargo",
                        principalColumn: "IdCargo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuarioCargo_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FamiliaCaixilho",
                columns: table => new
                {
                    IdFamiliaCaixilho = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DescricaoFamilia = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PesoTotal = table.Column<int>(type: "integer", nullable: false),
                    IdObra = table.Column<int>(type: "integer", nullable: false),
                    StatusFamilia = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FamiliaCaixilho", x => x.IdFamiliaCaixilho);
                    table.ForeignKey(
                        name: "FK_FamiliaCaixilho_Obra_IdObra",
                        column: x => x.IdObra,
                        principalTable: "Obra",
                        principalColumn: "IdObra",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notificacao",
                columns: table => new
                {
                    IdNotificacao = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdUsuarioDestino = table.Column<int>(type: "integer", nullable: false),
                    Titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Mensagem = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Lida = table.Column<bool>(type: "boolean", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TipoNotificacao = table.Column<int>(type: "integer", nullable: false),
                    IdObra = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificacao", x => x.IdNotificacao);
                    table.ForeignKey(
                        name: "FK_Notificacao_Obra_IdObra",
                        column: x => x.IdObra,
                        principalTable: "Obra",
                        principalColumn: "IdObra",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Notificacao_Usuario_IdUsuarioDestino",
                        column: x => x.IdUsuarioDestino,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Caixilho",
                columns: table => new
                {
                    IdCaixilho = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomeCaixilho = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Largura = table.Column<int>(type: "integer", nullable: false),
                    Altura = table.Column<int>(type: "integer", nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false),
                    PesoUnitario = table.Column<float>(type: "real", nullable: false),
                    Liberado = table.Column<bool>(type: "boolean", nullable: false),
                    DataLiberacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Observacoes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DescricaoCaixilho = table.Column<string>(type: "text", nullable: true),
                    StatusProducao = table.Column<int>(type: "integer", nullable: false),
                    ObraId = table.Column<int>(type: "integer", nullable: false),
                    IdFamiliaCaixilho = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Caixilho", x => x.IdCaixilho);
                    table.ForeignKey(
                        name: "FK_Caixilho_FamiliaCaixilho_IdFamiliaCaixilho",
                        column: x => x.IdFamiliaCaixilho,
                        principalTable: "FamiliaCaixilho",
                        principalColumn: "IdFamiliaCaixilho",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Caixilho_Obra_ObraId",
                        column: x => x.ObraId,
                        principalTable: "Obra",
                        principalColumn: "IdObra",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Medicao",
                columns: table => new
                {
                    IdMedicao = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdFamiliaCaixilho = table.Column<int>(type: "integer", nullable: false),
                    IdResponsavel = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataEstimadaConclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DataConclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Descricao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Observacoes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicao", x => x.IdMedicao);
                    table.ForeignKey(
                        name: "FK_Medicao_FamiliaCaixilho_IdFamiliaCaixilho",
                        column: x => x.IdFamiliaCaixilho,
                        principalTable: "FamiliaCaixilho",
                        principalColumn: "IdFamiliaCaixilho",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Medicao_Usuario_IdResponsavel",
                        column: x => x.IdResponsavel,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProducaoFamilia",
                columns: table => new
                {
                    IdProducaoFamilia = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdFamiliaCaixilho = table.Column<int>(type: "integer", nullable: false),
                    IdResponsavel = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataEstimadaConclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DataConclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Descricao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Observacoes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProducaoFamilia", x => x.IdProducaoFamilia);
                    table.ForeignKey(
                        name: "FK_ProducaoFamilia_FamiliaCaixilho_IdFamiliaCaixilho",
                        column: x => x.IdFamiliaCaixilho,
                        principalTable: "FamiliaCaixilho",
                        principalColumn: "IdFamiliaCaixilho",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProducaoFamilia_Usuario_IdResponsavel",
                        column: x => x.IdResponsavel,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Anexo",
                columns: table => new
                {
                    IdAnexo = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomeArquivo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CaminhoArquivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    TipoArquivo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TamanhoBytes = table.Column<long>(type: "bigint", nullable: false),
                    Dados = table.Column<byte[]>(type: "bytea", nullable: true),
                    DataUpload = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TipoAnexo = table.Column<int>(type: "integer", nullable: false),
                    IdMedicao = table.Column<int>(type: "integer", nullable: true),
                    IdProducaoFamilia = table.Column<int>(type: "integer", nullable: true),
                    IdUsuario = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anexo", x => x.IdAnexo);
                    table.ForeignKey(
                        name: "FK_Anexo_Medicao_IdMedicao",
                        column: x => x.IdMedicao,
                        principalTable: "Medicao",
                        principalColumn: "IdMedicao",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Anexo_ProducaoFamilia_IdProducaoFamilia",
                        column: x => x.IdProducaoFamilia,
                        principalTable: "ProducaoFamilia",
                        principalColumn: "IdProducaoFamilia",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Anexo_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Cargo",
                columns: new[] { "IdCargo", "DescricaoCargo", "TipoCargo" },
                values: new object[,]
                {
                    { 1, "Gerente", 1 },
                    { 2, "Responsável pela Verificação", 2 },
                    { 3, "Responsável pela Medição", 3 },
                    { 4, "Responsável pela Produção", 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Anexo_IdMedicao",
                table: "Anexo",
                column: "IdMedicao");

            migrationBuilder.CreateIndex(
                name: "IX_Anexo_IdProducaoFamilia",
                table: "Anexo",
                column: "IdProducaoFamilia");

            migrationBuilder.CreateIndex(
                name: "IX_Anexo_IdUsuario",
                table: "Anexo",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Caixilho_IdFamiliaCaixilho",
                table: "Caixilho",
                column: "IdFamiliaCaixilho");

            migrationBuilder.CreateIndex(
                name: "IX_Caixilho_ObraId",
                table: "Caixilho",
                column: "ObraId");

            migrationBuilder.CreateIndex(
                name: "IX_Cargo_TipoCargo",
                table: "Cargo",
                column: "TipoCargo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FamiliaCaixilho_IdObra",
                table: "FamiliaCaixilho",
                column: "IdObra");

            migrationBuilder.CreateIndex(
                name: "IX_Medicao_IdFamiliaCaixilho",
                table: "Medicao",
                column: "IdFamiliaCaixilho");

            migrationBuilder.CreateIndex(
                name: "IX_Medicao_IdResponsavel",
                table: "Medicao",
                column: "IdResponsavel");

            migrationBuilder.CreateIndex(
                name: "IX_Notificacao_IdObra",
                table: "Notificacao",
                column: "IdObra");

            migrationBuilder.CreateIndex(
                name: "IX_Notificacao_IdUsuarioDestino",
                table: "Notificacao",
                column: "IdUsuarioDestino");

            migrationBuilder.CreateIndex(
                name: "IX_Obra_IdResponsavelMedicao",
                table: "Obra",
                column: "IdResponsavelMedicao");

            migrationBuilder.CreateIndex(
                name: "IX_Obra_IdResponsavelProducao",
                table: "Obra",
                column: "IdResponsavelProducao");

            migrationBuilder.CreateIndex(
                name: "IX_Obra_IdResponsavelVerificacao",
                table: "Obra",
                column: "IdResponsavelVerificacao");

            migrationBuilder.CreateIndex(
                name: "IX_Obra_IdUsuario",
                table: "Obra",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_ProducaoFamilia_IdFamiliaCaixilho",
                table: "ProducaoFamilia",
                column: "IdFamiliaCaixilho");

            migrationBuilder.CreateIndex(
                name: "IX_ProducaoFamilia_IdResponsavel",
                table: "ProducaoFamilia",
                column: "IdResponsavel");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_IdTipoUsuario",
                table: "Usuario",
                column: "IdTipoUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioCargo_IdCargo",
                table: "UsuarioCargo",
                column: "IdCargo");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioCargo_IdUsuario_IdCargo",
                table: "UsuarioCargo",
                columns: new[] { "IdUsuario", "IdCargo" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Anexo");

            migrationBuilder.DropTable(
                name: "Caixilho");

            migrationBuilder.DropTable(
                name: "Notificacao");

            migrationBuilder.DropTable(
                name: "UsuarioCargo");

            migrationBuilder.DropTable(
                name: "Medicao");

            migrationBuilder.DropTable(
                name: "ProducaoFamilia");

            migrationBuilder.DropTable(
                name: "Cargo");

            migrationBuilder.DropTable(
                name: "FamiliaCaixilho");

            migrationBuilder.DropTable(
                name: "Obra");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "TipoUsuario");
        }
    }
}
