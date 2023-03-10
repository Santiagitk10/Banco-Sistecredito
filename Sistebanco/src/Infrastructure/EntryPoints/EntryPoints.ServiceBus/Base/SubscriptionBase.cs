using Domain.UseCase.Common;
using Helpers.ObjectsUtils;
using Microsoft.Extensions.Options;
using org.reactivecommons.api;
using org.reactivecommons.api.domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EntryPoints.ServiceBus.Base
{
    public class SubscriptionBase
    {

        private readonly IManageEventsUseCase _manageEventsUseCase;
        private readonly IOptions<ConfiguradorAppSettings> _appSettings;
        
        

        public SubscriptionBase(IManageEventsUseCase manageEventsUseCase, IOptions<ConfiguradorAppSettings> appSettings)
        {
            _manageEventsUseCase = manageEventsUseCase;
            _appSettings = appSettings;
        }

        public async Task SubscribeOnCommandAsync<T>(IDirectAsyncGateway<T> directAsyncGateway,
            string targetName, Func<Command<T>, Task> handler, MethodBase methodBase, int maxConcurrentCalls = 1,
            [CallerMemberName] string callerMemberName = null)
        {
            try
            {
                string commandName = $"{_appSettings.Value.DomainName}.{methodBase.DeclaringType.DeclaringType.Name}.{callerMemberName}";
                await _manageEventsUseCase.ConsoleLogAsync(commandName, callerMemberName, data: null);
                await directAsyncGateway.SuscripcionCommand(targetName, handler, maxConcurrentCalls);
            }catch (Exception ex)
            {
                _manageEventsUseCase.ConsoleErrorLog(ex.Message,ex);
                await LogAsync(ex, methodBase, targetName, new { targetName }, callerMemberName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="directAsyncGateway"></param>
        /// <param name="targetName"></param>
        /// <param name="subscriptionName"></param>
        /// <param name="handler"></param>
        /// <param name="methodBase"></param>
        /// <param name="maxConcurrentCalls"></param>
        /// <param name="callerMemberName"></param>
        /// <returns></returns>
        public async Task SubscribeOnEventAsync<T>(IDirectAsyncGateway<T> directAsyncGateway, 
            string targetName, string subscriptionName, Func<DomainEvent<T>, Task> handler, MethodBase methodBase, int maxConcurrentCalls = 1,
            [CallerMemberName] string callerMemberName = null)
        {
            try
            {
                string eventName = $"{_appSettings.Value.DomainName}.{methodBase.DeclaringType.DeclaringType.Name}.{callerMemberName}";
                await _manageEventsUseCase.ConsoleLogAsync(eventName, callerMemberName, data: null);
                await directAsyncGateway.SuscripcionEvent(targetName, subscriptionName, handler);
            }
            catch(Exception ex)
            {
                _manageEventsUseCase.ConsoleErrorLog(ex.Message, ex);
                await LogAsync(ex, methodBase, targetName, new { targetName, subscriptionName }, callerMemberName);
            }
        }

        public async Task HandleRequestAsync<TEntity>(
        //El handler ejecuta el caso de uso
        Func<TEntity, Task> serviceHandler,
        MethodBase methodBase, string logId,
        Command<TEntity> request,
        [CallerMemberName] string callerMemberName = null) =>

        ///Se inicia la ejecución de HandleAsync
        await HandleAsync(async () =>
        {
            ///Se valida
            _manageEventsUseCase.ConsoleInfoLog($"Se inicia procesamiento del commando {@callerMemberName} {@request}", callerMemberName, request);
            Validate(request);

            ///La ejecución entra a InvokeAsync - rediríjase a InvokeAsync
            ///A InvokeAsync le entra un handler que recibe un Tipo genérico y retorna Tipo Generico
            ///También recibe un parámetro del Tipo de entrada del handler (Esto es lo que concreta el tipo de entrada)
            ///El valor de retorno dentro de este handler es lo que tipa el valor de resultado (puede ser culaquier cosa
            ///y se define aquí) 
            return await InvokeAsync(
                //request.data es lo que proporciona la data del comando
                async (entity) =>
                {
                    //Esa entity es la request.data (ej. Command<Tienda>.data retorna la Tienda)
                    //Se la estoy pasando al handler que es la función que viene en la implementación
                    await serviceHandler(entity);
                    _manageEventsUseCase.ConsoleInfoLog($"Se finaliza procesamiento del comando {@callerMemberName} {@request}", callerMemberName, request);
                    return true;
                },
                ///Infiere el tipo e InvokeAsync
                request.data);
        },
        methodBase,
        logId,
        request,
        callerMemberName);

        public async Task HandleRequestAsync<TEntity>(
        //El handler ejecuta el caso de uso
        Func<TEntity, Task> serviceHandler,
        MethodBase methodBase, string logId,
        DomainEvent<TEntity> request,
        [CallerMemberName] string callerMemberName = null) =>

        ///Se inicia la ejecución de HandleAsync
        await HandleAsync(async () =>
        {
            ///Se valida
            _manageEventsUseCase.ConsoleInfoLog($"Se inicia procesamiento del commando {@callerMemberName} {@request}", callerMemberName, request);
            Validate(request);

            ///La ejecución entra a InvokeAsync - rediríjase a InvokeAsync
            ///A InvokeAsync le entra un handler que recibe un Tipo genérico y retorna Tipo Generico
            ///También recibe un parámetro del Tipo de entrada del handler (Esto es lo que concreta el tipo de entrada)
            ///El valor de retorno dentro de este handler es lo que tipa el valor de resultado (puede ser culaquier cosa
            ///y se define aquí) 
            return await InvokeAsync(
                //request.data es lo que proporciona la data del comando
                async (entity) =>
                {
                    //Esa entity es la request.data (ej. Command<Tienda>.data retorna la Tienda)
                    //Se la estoy pasando al handler que es la función que viene en la implementación
                    await serviceHandler(entity);
                    _manageEventsUseCase.ConsoleInfoLog($"Se finaliza procesamiento del comando {@callerMemberName} {@request}", callerMemberName, request);
                    return true;
                },
                ///Infiere el tipo e InvokeAsync
                request.data);
        },
        methodBase,
        logId,
        request,
        callerMemberName);

        #region Private
        private async Task<TResult> HandleAsync<TResult, TRequest>(Func<Task<TResult>> serviceHandler, MethodBase methodBase, string logId, TRequest request,
        [CallerMemberName] string callerMemberName = null)
        {
            try
            {
                string eventName = $"{_appSettings.Value.DomainName}.{methodBase.DeclaringType.DeclaringType.Name}.{callerMemberName}";
                await _manageEventsUseCase.ConsoleLogAsync(eventName, logId, data: null);
                return await serviceHandler();
            }
            catch (Exception ex)
            {
                _manageEventsUseCase.ConsoleErrorLog(ex.Message, ex);
                await LogAsync(ex, methodBase, logId, request, callerMemberName);
                throw;
            }
        }

        private async Task<TResult> InvokeAsync<TEntity, TResult>(Func<TEntity, Task<TResult>> handler, TEntity entity)
        {
            return await handler(entity);
        }

        private void Validate<T>(Command<T> command)
        {
            if (command == null || command.data == null)
            {
                throw new ArgumentNullException(nameof(command));
            }
        }

        private void Validate<T>(DomainEvent<T> @event)
        {
            if (@event == null || @event.data == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }
        }

        private async Task LogAsync(Exception ex, MethodBase methodBase, string logId, dynamic request, [CallerMemberName] string callerMemberName = null)
        {
            object logDetails = GetLogDetails(ex, request);
            string eventName = $"Exception.{_appSettings.Value.DomainName}.{methodBase.DeclaringType.DeclaringType.Name}.{callerMemberName}.{ex.GetType().Name}";
            await _manageEventsUseCase.ConsoleLogAsync(eventName, logId, logDetails, writeData: true);
        }


        private object GetLogDetails(Exception ex, dynamic request) =>
            new
            {
                exception = (ex != null) ? ex.ToString() : string.Empty,
                request
            };


        #endregion Private
    }
}
