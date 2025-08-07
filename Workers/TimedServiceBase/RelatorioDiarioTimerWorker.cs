namespace EmprestimosWorkerService.Workers.TimedServiceBase;

/*
 * Worker baseado em System.Threading.Timer
 * 
 * ⚠️ Executa mesmo se a execução anterior ainda estiver rodando (risco de sobreposição)
 * ⚠️ Não é assíncrono por padrão — requer cuidado ao usar async/await
 * ⚠️ Requer controle manual de cancelamento e de ciclo de vida
 * 
 * ➕ Útil quando se quer disparos precisos mesmo que execuções se sobreponham
 */

public class RelatorioDiarioTimerWorker(ILogger<RelatorioDiarioTimerWorker> logger) : BackgroundService
{
    private Timer? _timer;
    private readonly SemaphoreSlim _semaphore = new(1, 1); // Evita sobreposição

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("[RelatorioDiarioTimerWorker] - Serviço iniciado com Timer.");

        // Executa imediatamente e depois a cada 30 segundos
        _timer = new Timer(async _ =>
        {
            // Garante que a execução não se sobreponha
            if (!_semaphore.Wait(0)) return;

            try
            {
                await ExecutarGeracaoRelatorioAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[RelatorioDiarioTimerWorker] - Erro durante execução.");
            }
            finally
            {
                _semaphore.Release();
            }

        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(30)); // delay inicial, intervalo

        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("[RelatorioDiarioTimerWorker] - Serviço sendo finalizado...");
        _timer?.Change(Timeout.Infinite, 0); // Interrompe o timer
        return base.StopAsync(cancellationToken);
    }

    private async Task ExecutarGeracaoRelatorioAsync()
    {
        try
        {
            logger.LogInformation("[RelatorioDiarioTimerWorker] - Iniciando geração do relatório...");

            await Task.Delay(2000); // Simula trabalho

            logger.LogInformation("[RelatorioDiarioTimerWorker] - Relatório gerado com sucesso às {Hora}.", DateTime.Now);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[RelatorioDiarioTimerWorker] - Falha ao gerar o relatório.");
        }
    }
}