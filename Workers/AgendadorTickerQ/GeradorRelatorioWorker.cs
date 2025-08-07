using System.Diagnostics;
using TickerQ.Utilities.Base;
using TickerQ.Utilities.Enums;

namespace EmprestimosWorkerService.Workers.AgendadorTickerQ;

/*
    TickerFunction: Esse atributo marca o método que será agendado para execução periódica.
    Cron expression (* * * * *): Executa a tarefa a cada minuto.
    TickerTaskPriority:
        Low: baixa prioridade, pode ser adiada caso o sistema esteja ocupado.
        Normal: prioridade padrão.
        High: ideal para tarefas críticas que não devem atrasar.
    CancellationToken: O TickerQ fornece o token automaticamente para permitir o cancelamento seguro da tarefa.
 */

public class GeradorRelatorioWorker(ILogger<GeradorRelatorioWorker> logger)
{
    // Configurado para executar a cada minuto
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
