namespace EmprestimosWorkerService.Workers;

public class RelatorioDiarioWorkerTimedService(ILogger<RelatorioDiarioWorkerTimedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("[RelatorioDiarioWorkerTimedService] - Serviço iniciado.");

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
            logger.LogInformation("[RelatorioDiarioWorkerTimedService] Serviço cancelado (Stopping).");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[RelatorioDiarioWorkerTimedService] Erro inesperado no loop de execução.");
        }

        logger.LogInformation("[RelatorioDiarioWorkerTimedService] Serviço finalizado.");
    }

    private async Task ExecutarGeracaoRelatorioAsync()
    {
        try
        {
            logger.LogInformation("[RelatorioDiarioWorkerTimedService] Iniciando geração do relatório diário...");

            await Task.Run(() => GerarRelatorio());  // Simula trabalho assíncrono

            logger.LogInformation("[RelatorioDiarioWorkerTimedService] Relatório gerado com sucesso às {Hora}.", DateTime.Now);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[RelatorioDiarioWorkerTimedService] Erro durante a geração do relatório diário.");
        }
    }

    private void GerarRelatorio()
    {
        // Simulação de trabalho
        Thread.Sleep(500);
        // Aqui vai a lógica real de geração de relatório
    }
}
