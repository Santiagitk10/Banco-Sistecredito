using Domain.Model.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.UseCase.Clientes.Command
{
    public interface IClienteCommandUseCase
    {
        Task EnviarNotificacionPorEmail(ClienteRequest cuenta);
    }
}
