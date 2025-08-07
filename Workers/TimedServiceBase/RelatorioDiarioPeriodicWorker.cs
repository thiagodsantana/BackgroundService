namespace EmprestimosWorkerService.Workers.TimedServiceBase;

/*
 * Worker baseado em PeriodicTimer (recomendado em .NET 6+)
 * 
 * ✅ Assíncrono nativamente
 * ✅ Garante espaçamento fixo entre execuções
 * ✅ Espera o término da execução anterior antes de iniciar a próxima
 * ✅ Suporte direto a CancellationToken
 * ✅ Mais seguro contra concorrência e sobreposição
 */

public class RelatorioDiarioPeriodicWorker(ILogger<RelatorioDiarioPeriodicWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("[RelatorioDiarioPeriodicWorker] - Serviço iniciado com PeriodicTimer.");

        // Executa imediatamente ao iniciar
        await ExecutarGeracaoRelatorioAsync();

        // Define o intervalo fixo de execução
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
            logger.LogInformation("[RelatorioDiarioPeriodicWorker] - Serviço cancelado.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[RelatorioDiarioPeriodicWorker] - Erro inesperado durante execução.");
        }

        logger.LogInformation("[RelatorioDiarioPeriodicWorker] - Serviço finalizado.");
    }

    private async Task ExecutarGeracaoRelatorioAsync()
    {
        try
        {
            logger.LogInformation("[RelatorioDiarioPeriodicWorker] - Iniciando geração do relatório...");

            // Simula uma tarefa demorada
            await Task.Delay(2000);

            logger.LogInformation("[RelatorioDiarioPeriodicWorker] - Relatório gerado com sucesso às {Hora}.", DateTime.Now);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[RelatorioDiarioPeriodicWorker] - Falha ao gerar o relatório.");
        }
    }
}
