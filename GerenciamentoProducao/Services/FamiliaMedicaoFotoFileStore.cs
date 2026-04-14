using System.Text.Json;
using GerenciamentoProducao.Interfaces;
using GerenciamentoProducao.Models;

namespace GerenciamentoProducao.Services;

/// <summary>
/// Persistência local de fotos de medição (JSON + base64). A API externa pode ser alinhada depois.
/// </summary>
public class FamiliaMedicaoFotoFileStore : IFamiliaMedicaoFotoStore
{
    private readonly string _directory;
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = false };

    public FamiliaMedicaoFotoFileStore(IWebHostEnvironment env)
    {
        _directory = Path.Combine(env.ContentRootPath, "App_Data", "MedicaoFamilia");
        Directory.CreateDirectory(_directory);
    }

    private static string FilePath(string dir, int id) =>
        Path.Combine(dir, $"familia-{id}.json");

    public Task<FamiliaMedicaoFotoState?> GetAsync(int idFamiliaCaixilho, CancellationToken cancellationToken = default)
    {
        var path = FilePath(_directory, idFamiliaCaixilho);
        if (!File.Exists(path))
            return Task.FromResult<FamiliaMedicaoFotoState?>(null);

        var json = File.ReadAllText(path);
        var state = JsonSerializer.Deserialize<FamiliaMedicaoFotoState>(json, JsonOptions);
        return Task.FromResult(state);
    }

    public Task SaveAsync(FamiliaMedicaoFotoState state, CancellationToken cancellationToken = default)
    {
        var path = FilePath(_directory, state.IdFamiliaCaixilho);
        var json = JsonSerializer.Serialize(state, JsonOptions);
        File.WriteAllText(path, json);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int idFamiliaCaixilho, CancellationToken cancellationToken = default)
    {
        var path = FilePath(_directory, idFamiliaCaixilho);
        if (File.Exists(path))
            File.Delete(path);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<int>> ListFamiliasComFotoPendenteAsync(CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(_directory))
            return Task.FromResult<IReadOnlyList<int>>(Array.Empty<int>());

        var ids = new List<int>();
        const string prefix = "familia-";
        foreach (var path in Directory.EnumerateFiles(_directory, "familia-*.json"))
        {
            var name = Path.GetFileNameWithoutExtension(path);
            if (name.Length > prefix.Length
                && name.StartsWith(prefix, StringComparison.Ordinal)
                && int.TryParse(name.AsSpan(prefix.Length), out var id))
            {
                // Filtrar fotos já aprovadas
                var json = File.ReadAllText(path);
                var state = JsonSerializer.Deserialize<FamiliaMedicaoFotoState>(json, JsonOptions);
                if (state != null && !state.Aprovada)
                    ids.Add(id);
            }
        }

        return Task.FromResult<IReadOnlyList<int>>(ids);
    }

    public Task<IReadOnlyList<int>> ListFamiliasComFotoAsync(CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(_directory))
            return Task.FromResult<IReadOnlyList<int>>(Array.Empty<int>());

        var ids = new List<int>();
        const string prefix = "familia-";
        foreach (var path in Directory.EnumerateFiles(_directory, "familia-*.json"))
        {
            var name = Path.GetFileNameWithoutExtension(path);
            if (name.Length > prefix.Length
                && name.StartsWith(prefix, StringComparison.Ordinal)
                && int.TryParse(name.AsSpan(prefix.Length), out var id))
            {
                ids.Add(id);
            }
        }

        return Task.FromResult<IReadOnlyList<int>>(ids);
    }
}
