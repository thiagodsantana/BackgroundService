namespace EmprestimosWorkerService.Workers;

public class WorkerCustomizadoHosted(ILogger<WorkerCustomizadoHosted> logger) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("[WorkerCustomizadoHosted] - Serviço customizado iniciado às {Hora}.", DateTime.Now);

        try
        {
            // Lógica de inicialização (ex: abrir conexões, carregar cache, etc.)
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[WorkerCustomizadoHosted] - Erro durante a inicialização do serviço customizado.");
            throw;
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("[WorkerCustomizadoHosted] - Serviço customizado sendo finalizado às {Hora}.", DateTime.Now);

        try
        {
            // Lógica de liberação de recursos (ex: fechar conexões, limpar buffers, etc.)
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[WorkerCustomizadoHosted] - Erro ao tentar finalizar o serviço customizado.");
        }

        return Task.CompletedTask;
    }
}
