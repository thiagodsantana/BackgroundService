using System.Diagnostics;
using Microsoft.Extensions.Logging;
using TickerQ.Utilities.Base;
using TickerQ.Utilities.Enums;

namespace EmprestimosWorkerService.Workers.AgendadorTickerQ;

public class GeradorRelatorioWorker(ILogger<GeradorRelatorioWorker> logger)
{
    [TickerFunction(functionName: nameof(GerarRelatorio), cronExpression: "* * * * *", TickerTaskPriority.Normal)]
    public async Task GerarRelatorio(CancellationToken token)
    {
        var stopwatch = Stopwatch.StartNew();
        var startTime = DateTimeOffset.Now;

        logger.LogInformation("[{Time}] Iniciando geração de relatório...", startTime);

        try
        {
            const int totalEtapas = 5;

            for (int i = 1; i <= totalEtapas; i++)
            {
                token.ThrowIfCancellationRequested();

                // Simula uma etapa de processamento
                await Task.Delay(2000, token);

                logger.LogInformation("Etapa {EtapaAtual}/{TotalEtapas} concluída.", i, totalEtapas);
            }

            logger.LogInformation("[{Time}] Relatório diário gerado com sucesso!", DateTimeOffset.Now);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Relatório cancelado pelo sistema.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao gerar relatório.");
            throw;
        }
        finally
        {
            stopwatch.Stop();
            logger.LogInformation("Tempo total de execução: {Tempo} ms", stopwatch.ElapsedMilliseconds);
        }
    }
}
