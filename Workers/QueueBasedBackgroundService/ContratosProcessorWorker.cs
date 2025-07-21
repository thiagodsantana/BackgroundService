using System.Threading.Channels;

namespace EmprestimosWorkerService.Workers;

/* Queue-based Background Service
    - Utiliza Channel<T>, filas internas ou mensageria externa para desacoplar a geração de tarefas do seu processamento.
Quando usar?
    - Para processar tarefas à medida que são enfileiradas, como envio de e-mails, logs ou eventos de domínio.
 */

public class ContratosProcessorWorker(Channel<string> canal, ILogger<ContratosProcessorWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("[ContratosProcessorWorkerQueue] - Iniciando o processamento da fila de contratos...");

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
        // Inicia o processamento detalhado do contrato
        logger.LogInformation("[ContratosProcessorWorkerQueue] - Iniciando o processamento detalhado do contrato: {ContratoId}", contratoId);

        // Regras de negócio, integração com banco de dados etc.
        await Task.Delay(500, cancellationToken); // Simulação de processamento

        logger.LogInformation("[ContratosProcessorWorkerQueue] - Finalizado o processamento do contrato: {ContratoId}", contratoId);
    }
}
