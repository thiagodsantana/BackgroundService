namespace EmprestimosWorkerService.Workers;

public class RelatorioDiarioWorkerTimedService(ILogger<RelatorioDiarioWorkerTimedService> logger) : IHostedService, IDisposable
{
    private Timer? _timer;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("[RelatorioDiarioWorkerTimedService] - Serviço de geração de relatório diário iniciado.");

        _timer = new Timer(ExecutarGeracaoRelatorio, null, TimeSpan.Zero, TimeSpan.FromSeconds(30)); // Intervalo de exemplo

        return Task.CompletedTask;
    }

    private void ExecutarGeracaoRelatorio(object? state)
    {
        try
        {
            logger.LogInformation("[RelatorioDiarioWorkerTimedService] - Iniciando geração do relatório diário de empréstimos...");

            GerarRelatorio();

            logger.LogInformation("[RelatorioDiarioWorkerTimedService] - Relatório diário de empréstimos gerado com sucesso às {Hora}.", DateTime.Now);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[RelatorioDiarioWorkerTimedService] - Erro durante a geração do relatório diário de empréstimos.");
        }
    }

    private void GerarRelatorio()
    {
        // Aqui seria implementada a lógica real de geração de relatório (ex: consultas ao banco, exportação, etc.)
        Thread.Sleep(500); // Simulação de tempo de geração
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("[RelatorioDiarioWorkerTimedService] - Encerrando o serviço de geração de relatório diário.");

        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
