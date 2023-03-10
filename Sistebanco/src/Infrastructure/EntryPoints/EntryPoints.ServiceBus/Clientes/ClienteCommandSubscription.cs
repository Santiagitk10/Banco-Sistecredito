using Domain.Model.Request;
using Domain.UseCase.Clientes.Command;
using Domain.UseCase.Common;
using EntryPoints.ServiceBus.Base;
using Helpers.ObjectsUtils;
using Microsoft.Extensions.Options;
using org.reactivecommons.api;
using org.reactivecommons.api.domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EntryPoints.ServiceBus.Clientes
{
    public class ClienteCommandSubscription : SubscriptionBase, IClienteCommandSubscription
    {
        private readonly IOptions<ConfiguradorAppSettings> _appSettings;
        private readonly IDirectAsyncGateway<ClienteRequest> _directAsyncGateway;
        private readonly IClienteCommandUseCase _cuentaCommandUseCase;
        public ClienteCommandSubscription(
            IManageEventsUseCase manageEventsUseCase, 
            IOptions<ConfiguradorAppSettings> appSettings,
            IDirectAsyncGateway<ClienteRequest> directAsyncGateway,
            IClienteCommandUseCase cuentaCommandUseCase
            ) : base(manageEventsUseCase, appSettings)
        {
            _appSettings = appSettings;
            _directAsyncGateway = directAsyncGateway;
            _cuentaCommandUseCase = cuentaCommandUseCase;
        }

        public async Task SubscribeAsync()
        {
            await SubscribeOnCommandAsync(
                _directAsyncGateway,
                _appSettings.Value.ColaInformacionCliente,
                EmailNotificationCommand,
                MethodBase.GetCurrentMethod()!,
                maxConcurrentCalls: 1);

        }

        public async Task EmailNotificationCommand(Command<ClienteRequest> cuentaRequest) =>
            await HandleRequestAsync(async (cuenta) =>
            {
                await _cuentaCommandUseCase.EnviarNotificacionPorEmail(cuenta);
            },
             MethodBase.GetCurrentMethod()!,
             Guid.NewGuid().ToString(),
             ///Este parámetro tipa la función
             cuentaRequest
             );
    }
}
