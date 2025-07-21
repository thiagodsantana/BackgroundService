using EmprestimosWorkerService.Interfaces;
using EmprestimosWorkerService.Services;
using EmprestimosWorkerService.Workers;
using EmprestimosWorkerService.Workers.AgendadorQuartz;
using EmprestimosWorkerService.Workers.BackgroundServiceBase;
using EmprestimosWorkerService.Workers.HostedBase;
using EmprestimosWorkerService.Workers.ScopedService;
using EmprestimosWorkerService.Workers.TimedServiceBase;
using Quartz;
using System.Threading.Channels;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // BackgroundService - Serviço contínuo que monitora e envia notificações relacionadas a contratos
        services.AddHostedService<NotificacaoContratosWorker>();

        // TimedService - Serviço baseado em timer para geração periódica de relatórios diários de empréstimos
        services.AddHostedService<RelatorioDiarioWorker>();

        // Criação de uma fila em memória (Channel) para comunicação assíncrona e desacoplada entre produtores e consumidores
        var channel = Channel.CreateUnbounded<string>();
        services.AddSingleton(channel);

        // QueueBasedBackgroundService - Serviço consumidor responsável por processar contratos provenientes da fila (Channel)
        services.AddHostedService<ContratosProcessorWorker>();

        // Simulação de produção inicial de dados na fila para testes ou inicialização
        Task.Run(async () =>
        {
            var writer = channel.Writer;

            await writer.WriteAsync("Contrato-001");
            await writer.WriteAsync("Contrato-002");
            await writer.WriteAsync("Contrato-003");

            writer.Complete();  // Indica que não haverá mais itens produzidos
        });

        // Registro da implementação do serviço de validação de contratos com tempo de vida Scoped (por requisição/escopo)
        services.AddScoped<IValidacaoEmprestimo, ValidacaoEmprestimoService>();

        // ScopedService - Serviço que consome o serviço de validação Scoped dentro de um BackgroundService, criando escopos manuais para injeção
        services.AddHostedService<ValidacaoWorkerScopedService>();

        // HostedBase - Serviço customizado que implementa diretamente IHostedService, para controle total do ciclo de vida
        services.AddHostedService<CustomizadoWorker>();

        // AgendadorQuartz - Configuração do Quartz Scheduler para agendamento avançado de tarefas
        services.AddQuartz(q =>
        {
            // Definição de chave única para o job de sincronização de status dos contratos
            var jobKey = new JobKey("SincronizacaoStatusContratosJob");

            // Registro do job que executa a lógica de sincronização com sistemas externos
            q.AddJob<SincronizacaoStatusContratosWorker>(opt => opt.WithIdentity(jobKey));

            // Configuração do trigger que dispara o job a cada 20 segundos, repetidamente
            q.AddTrigger(opt => opt
                .ForJob(jobKey)
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(20).RepeatForever()));
        });

        // Hospeda o Quartz como um serviço gerenciado, aguardando o término dos jobs no shutdown
        services.AddQuartzHostedService(opt => opt.WaitForJobsToComplete = true);
    })
    // Configura o host para rodar como serviço do Windows, se implantado nessa plataforma
    .UseWindowsService();

// Inicializa e executa o Host
await builder.Build().RunAsync();
