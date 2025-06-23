namespace EmprestimosWorkerService.Interfaces;

public interface IValidacaoEmprestimo
{
    Task ValidarAsync(string contratoId);
}

