namespace EmprestimosWorkerService.Workers.TimedServiceBase;

/*Timed Background Service
 - Um serviço que executa em intervalos fixos usando PeriodicTimer.
Quando usar?
 - Para executar ações com base em tempo fixo — ex: verificar novos dados a cada 10 minutos.
 */

public class RelatorioDiarioWorker(ILogger<RelatorioDiarioWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogCritical("====== RelatorioDiarioWorker ======");
        logger.LogInformation("");

        logger.LogInformation("[RelatorioDiarioWorker] - Serviço iniciado.");

        // Executa imediatamente ao iniciar
        await ExecutarGeracaoRelatorioAsync();

        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(30));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await ExecutarGeracaoRelatorioAsync();
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("[RelatorioDiarioWorker] - Serviço cancelado (Stopping).");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[RelatorioDiarioWorker] - Erro inesperado no loop de execução.");
        }

        logger.LogInformation("[RelatorioDiarioWorker] - Serviço finalizado.");
    }

    private async Task ExecutarGeracaoRelatorioAsync()
    {
        try
        {
            logger.LogInformation("[RelatorioDiarioWorker] - Iniciando geração do relatório diário...");

            // Simula um trabalho assíncrono real
            await Task.Delay(500);

            // Aqui vai a lógica real de geração de relatório
            logger.LogInformation("[RelatorioDiarioWorker] - Relatório gerado com sucesso às {Hora}.", DateTime.Now);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[RelatorioDiarioWorker] - Erro durante a geração do relatório diário.");
        }
    }
}
