using System.Threading.Channels;

namespace EmprestimosWorkerService
{
    public class ConsoleCommandWorker(Channel<string> channel) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var writer = channel.Writer;

            Console.WriteLine("[ConsoleCommandWorker] Digite contratos para enviar à fila. Use 'exit' para encerrar:");

            while (!stoppingToken.IsCancellationRequested)
            {
                var input = Console.ReadLine();

                if (input?.ToLower() == "exit" || string.IsNullOrEmpty(input))
                {
                    writer.Complete(); // Fecha o canal para sinalizar fim da produção
                    Console.WriteLine("[ConsoleCommandWorker] Fila encerrada.");
                    break;
                }

                if (!string.IsNullOrWhiteSpace(input))
                {
                    await writer.WriteAsync(input, stoppingToken);
                    Console.WriteLine($"[ConsoleCommandWorker] Enviado para a fila: {input}");
                }
            }
        }
    }
}
