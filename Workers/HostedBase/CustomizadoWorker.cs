﻿namespace EmprestimosWorkerService.Workers.HostedBase;

/*
 Implementação manual de IHostedService com IAsyncDisposable
 - Controle total sobre start, stop e liberação assíncrona de recursos.
 Quando usar?
 - Serviços que gerenciam conexões ou recursos assíncronos e precisam liberar corretamente.
*/

public class CustomizadoWorker(ILogger<CustomizadoWorker> logger) : IHostedService, IAsyncDisposable
{
    private readonly ILogger<CustomizadoWorker> _logger = logger;
    private CancellationTokenSource? _cts;
    private Task? _executando;
    private bool _disposed = false;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("[WorkerCustomizadoHosted] - Serviço customizado iniciado às {Hora}.", DateTime.Now);

        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _executando = ExecutarAsync(_cts.Token);

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("[WorkerCustomizadoHosted] - Serviço customizado sendo finalizado às {Hora}.", DateTime.Now);

        if (_cts != null)
        {
            _cts.Cancel();

            if (_executando != null)
            {
                try
                {
                    await Task.WhenAny(_executando, Task.Delay(Timeout.Infinite, cancellationToken));
                }
                catch (OperationCanceledException)
                {
                    // Cancelamento esperado
                }
            }
        }
    }

    private async Task ExecutarAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("[WorkerCustomizadoHosted] - Loop de execução iniciado.");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // Simule uma tarefa recorrente
                _logger.LogInformation("[WorkerCustomizadoHosted] - Executando tarefa em {Hora}.", DateTime.Now);

                await Task.Delay(2000, cancellationToken); // Espera 2s entre execuções
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("[WorkerCustomizadoHosted] - Execução cancelada.");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[WorkerCustomizadoHosted] - Erro durante execução da tarefa.");
            }
        }

        _logger.LogInformation("[WorkerCustomizadoHosted] - Loop de execução finalizado.");
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _logger.LogInformation("[WorkerCustomizadoHosted] - Liberando recursos assíncronos...");

        try
        {
            _cts?.Dispose();

            if (_executando != null)
                await _executando;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[WorkerCustomizadoHosted] - Erro ao liberar recursos assíncronos.");
        }

        _disposed = true;
    }
}
