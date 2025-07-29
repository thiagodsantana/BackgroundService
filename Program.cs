using EmprestimosWorkerService;
using EmprestimosWorkerService.Interfaces;
using EmprestimosWorkerService.Services;
using EmprestimosWorkerService.Workers.AgendadorQuartz;
using EmprestimosWorkerService.Workers.BackgroundServiceBase;
using EmprestimosWorkerService.Workers.HostedBase;
using EmprestimosWorkerService.Workers.QueueBasedBackgroundService;
using EmprestimosWorkerService.Workers.ScopedService;
using EmprestimosWorkerService.Workers.TimedServiceBase;
using Quartz;
using Serilog;
using System.Threading.Channels;

#region Configura��o do Serilog
// Configura��o do Serilog antes da cria��o do host
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()

    // Log geral
    .WriteTo.Console()
    .WriteTo.File("logs/geral.log", rollingInterval: RollingInterval.Day)

    // Log espec�fico para NotificacaoContratosWorker
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(le => le.Properties["SourceContext"]?.ToString().Contains("NotificacaoContratosWorker") == true)
        .WriteTo.File("logs/NotificacaoContratosWorker.log", rollingInterval: RollingInterval.Day))

    // Log espec�fico para RelatorioDiarioWorker
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(le => le.Properties["SourceContext"]?.ToString().Contains("RelatorioDiarioWorker") == true)
        .WriteTo.File("logs/RelatorioDiarioWorker.log", rollingInterval: RollingInterval.Day))

    // Log espec�fico para ContratosProcessorWorker
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(le => le.Properties["SourceContext"]?.ToString().Contains("ContratosProcessorWorker") == true)
        .WriteTo.File("logs/ContratosProcessorWorker.log", rollingInterval: RollingInterval.Day))

    // Log espec�fico para SincronizacaoStatusContratosWorker
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(le => le.Properties["SourceContext"]?.ToString().Contains("SincronizacaoStatusContratosWorker") == true)
        .WriteTo.File("logs/SincronizacaoStatusContratosWorker.log", rollingInterval: RollingInterval.Day))

    // Log espec�fico para CustomizadoWorker
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(le => le.Properties["SourceContext"]?.ToString().Contains("WorkerCustomizadoHosted") == true)
        .WriteTo.File("logs/WorkerCustomizadoHosted.log", rollingInterval: RollingInterval.Day))

    // Log espec�fico para ValidacaoWorker
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(le => le.Properties["SourceContext"]?.ToString().Contains("ValidacaoWorker") == true)
        .WriteTo.File("logs/ValidacaoWorker.log", rollingInterval: RollingInterval.Day))

    .CreateLogger();

#endregion

var builder = Host.CreateDefaultBuilder(args)
    .UseSerilog()
    .ConfigureServices((context, services) =>
    {
        #region Worker implementado diremente com IHostedService
        // HostedBase - Servi�o customizado que implementa diretamente IHostedService, para controle total do ciclo de vida
        //services.AddHostedService<CustomizadoWorker>();
        #endregion

        #region Worker implementado com BackgroundService (Servi�o cont�nuo)
        // BackgroundService - Servi�o cont�nuo que monitora e envia notifica��es relacionadas a contratos
        //services.AddHostedService<NotificacaoContratosWorker>();
        #endregion

        #region Worker implementado com TimedServiceBase (Servi�o baseado em timer)
        // TimedService - Servi�o baseado em timer para gera��o peri�dica de relat�rios di�rios de empr�stimos
        //services.AddHostedService<RelatorioDiarioWorker>();
        #endregion

        #region Worker implementado com QueueBase (Servi�o baseado em fila)
        // QueueBasedBackgroundService - Servi�o consumidor respons�vel por processar contratos provenientes da fila (Channel)
        services.AddHostedService<ContratosProcessorWorker>();
        
        //Servi�o para entrada de comandos via Console
        services.AddHostedService<ConsoleCommandWorker>();

        //Cria��o de uma fila em mem�ria(Channel) para comunica��o ass�ncrona e desacoplada entre produtores e consumidores
        var channel = Channel.CreateUnbounded<string>();
        services.AddSingleton(channel);
        #endregion

        #region Worker implementado com ScopedService (Servi�o baseado em escopo)
        //Registro da implementa��o do servi�o de valida��o de contratos com tempo de vida Scoped(por requisi��o / escopo)
        //services.AddScoped<IValidacaoEmprestimo, ValidacaoEmprestimoService>();
        //services.AddSingleton<IValidacaoEmprestimoSingleton, ValidacaoEmprestimoSingletonService>();

        // ScopedService - Servi�o que consome o servi�o de valida��o Scoped dentro de um BackgroundService, criando escopos manuais para inje��o
        //services.AddHostedService<ValidacaoWorker>();
        #endregion

        #region Worker implementado com AgendadorQuartz (Quartz Scheduler)
        // AgendadorQuartz - Configura��o do Quartz Scheduler para agendamento avan�ado de tarefas
        //services.AddQuartz(q =>
        //{
        //    var jobKey = new JobKey("SincronizacaoStatusContratosJob");

        //    // Registro do job que executa a l�gica de sincroniza��o com sistemas externos
        //    q.AddJob<SincronizacaoStatusContratosWorker>(opt => opt.WithIdentity(jobKey));

        //    // Configura��o do trigger que dispara o job a cada 20 segundos, repetidamente
        //    q.AddTrigger(opt => opt
        //        .ForJob(jobKey)
        //        .WithSimpleSchedule(x => x.WithIntervalInSeconds(20).RepeatForever()));
        //});

        ////Hospeda o Quartz como um servi�o gerenciado, aguardando o t�rmino dos jobs no shutdown
        //services.AddQuartzHostedService(opt => opt.WaitForJobsToComplete = true);
        #endregion

    }).UseWindowsService();

await builder.Build().RunAsync();
