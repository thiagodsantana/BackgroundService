using System.Threading.Channels;

namespace EmprestimosWorkerService.Workers;

public class ContratosProcessorWorkerQueue(Channel<string> canal, ILogger<ContratosProcessorWorkerQueue> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("[ContratosProcessorWorkerQueue] - Iniciando o processamento da fila de contratos...");
     
        _ = Task.Run(async () =>
        {
            await canal.Writer.WriteAsync("001");
            await canal.Writer.WriteAsync("002");
            canal.Writer.Complete(); 
        }, stoppingToken);
        try
        {
            await foreach (var contrato in canal.Reader.ReadAllAsync(stoppingToken))
            {
                if (string.IsNullOrWhiteSpace(contrato))
                {
                    logger.LogWarning("[ContratosProcessorWorkerQueue] - Contrato vazio ou nulo recebido na fila. Ignorando.");
                    continue;
                }

                logger.LogInformation("[ContratosProcessorWorkerQueue] - Processando contrato com ID: {ContratoId}", contrato);

                // Simula o processamento do contrato
                await ProcessarContratoAsync(contrato, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("[ContratosProcessorWorkerQueue] - Cancelamento solicitado. Encerrando o processamento da fila de contratos.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[ContratosProcessorWorkerQueue] - Erro inesperado durante o processamento da fila de contratos.");
        }
        finally
        {
            logger.LogInformation("[ContratosProcessorWorkerQueue] - Processamento da fila de contratos finalizado.");
        }
    }

    private async Task ProcessarContratoAsync(string contratoId, CancellationToken cancellationToken)
    {
        logger.LogInformation("[ContratosProcessorWorkerQueue] - Iniciando o processamento detalhado do contrato: {ContratoId}", contratoId);

        // Regras de negócio, integração com banco de dados etc.
        await Task.Delay(500, cancellationToken); // Simulação de processamento

        logger.LogInformation("[ContratosProcessorWorkerQueue] - Finalizado o processamento do contrato: {ContratoId}", contratoId);
    }
}
