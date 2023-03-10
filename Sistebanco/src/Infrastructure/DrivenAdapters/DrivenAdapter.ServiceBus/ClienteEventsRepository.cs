using credinet.comun.models.Credits;
using Domain.Model.Entities;
using Domain.Model.Interfaces.Gateways;
using Domain.UseCase.Common;
using DrivenAdapter.ServiceBus.Base;
using DrivenAdapter.ServiceBus.Entities;
using Helpers.ObjectsUtils;
using Microsoft.Extensions.Options;
using org.reactivecommons.api;
using System.Reflection;

namespace DrivenAdapter.ServiceBus
{
    public class ClienteEventsRepository : AsyncGatewayAdapterBase, IClienteEventsRepository
    {
        private readonly IDirectAsyncGateway<Entities.ClienteEntityDTO> _directAsyncGatewayCliente;
        private readonly IOptions<ConfiguradorAppSettings> _appSettings;

        public ClienteEventsRepository(IDirectAsyncGateway<ClienteEntityDTO> directAsyncGatewayCliente,
            IManageEventsUseCase manageEventsUseCase,
            IOptions<ConfiguradorAppSettings> appSettings)
            : base(manageEventsUseCase, appSettings)
        {
            _directAsyncGatewayCliente = directAsyncGatewayCliente;
            _appSettings = appSettings;
        }

        /// <summary>
        /// <see cref="IClienteEventsRepository.InformarCuentaCreadaEmail(Cuenta)"/>
        /// </summary>
        /// <param name="cuenta"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task InformarClienteCreadoEmail(Cliente cliente)
        {
            string commandName = "Cliente.InformarEmail";
            ClienteEntityDTO clienteCreado = MapeoCuenta(cliente);



            await HandleSendCommandAsync(_directAsyncGatewayCliente, cliente.Id,
                clienteCreado, _appSettings.Value.ColaInformacionCliente, commandName, MethodBase.GetCurrentMethod());
        }

        private static ClienteEntityDTO MapeoCuenta(Cliente cliente) => new()
        {
            Id = cliente.Id,
            Nombres = cliente.Nombres,
            Apellidos = cliente.Apellidos,
            TipoDeDocumento = cliente.TipoDeDocumento,
            DocumentoDeIdentidad = cliente.DocumentoDeIdentidad,
            CorreoElectronico = cliente.CorreoElectronico,
            FechaDeNacimiento = cliente.FechaDeNacimiento,
            FechaDeCreacion = cliente.FechaDeCreacion,
            FechaUltimaModificacion = cliente.FechaUltimaModificacion
        };
    }
}