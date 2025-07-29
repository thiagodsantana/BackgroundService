namespace EmprestimosWorkerService.Interfaces;

public interface IValidacaoEmprestimoSingleton
{
    Task ValidarAsync(string contratoId);
}

