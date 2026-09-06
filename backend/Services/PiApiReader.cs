using System.Text.Json;
using BizDataRouter.Models;
using BizDataRouter.Settings;
using BizDataRouter.Status;

namespace BizDataRouter.Services;

public sealed class PiApiReader
{
    private readonly HttpClient _httpClient;
    private readonly AppSettings _settings;
    private readonly PipelineStatus _status;

    public PiApiReader(HttpClient httpClient, AppSettings settings, PipelineStatus status)
    {
        _httpClient = httpClient;
        _settings = settings;
        _status = status;
    }

    public async Task<PiBatch> ReadAsync(CancellationToken cancellationToken)
    {
        try
        {
            using HttpResponseMessage response = await _httpClient.GetAsync(
                _settings.PiApi.Url,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync(cancellationToken);
            PiBatch? piBatch = JsonSerializer.Deserialize<PiBatch>(json);

            if (piBatch is null)
                throw new InvalidDataException("La réponse JSON est vide ou invalide.");

            _status.LastCollectionAt = DateTime.UtcNow;
            _status.PiApiConnected = true;
            return piBatch;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            _status.PiApiConnected = false;
            throw;
        }
    }
}
