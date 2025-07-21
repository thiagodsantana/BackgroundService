namespace EmprestimosWorkerService.Workers.HostedBase;

/*Implementação manual de IHostedService com IAsyncDisposable
    - Controle total sobre start, stop e liberação assíncrona de recursos.
Quando usar?
    - Serviços que gerenciam conexões ou recursos assíncronos e precisam liberar corretamente.
 */

public class CustomizadoWorker(ILogger<CustomizadoWorker> logger) : IHostedService, IAsyncDisposable
{
    private readonly ILogger<CustomizadoWorker> _logger = logger;
    private bool _disposed = false;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("[WorkerCustomizadoHosted] - Serviço customizado iniciado às {Hora}.", DateTime.Now);

        // Inicialização sincrona ou async aqui (exemplo simulado)
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("[WorkerCustomizadoHosted] - Serviço customizado sendo finalizado às {Hora}.", DateTime.Now);

        // Parada sincrona ou async aqui (exemplo simulado)
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _logger.LogInformation("[WorkerCustomizadoHosted] - Liberando recursos assíncronos...");

        try
        {
            // Simula liberação assíncrona, ex: fechar conexões, liberar buffers
            await Task.Delay(500);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[WorkerCustomizadoHosted] - Erro ao liberar recursos assíncronos.");
        }

        _disposed = true;
    }
}
