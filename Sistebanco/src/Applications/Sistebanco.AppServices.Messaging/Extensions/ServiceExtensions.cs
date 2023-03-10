using AutoMapper.Data;
using Domain.Model.Request;
using Domain.UseCase.Clientes.Command;
using Domain.UseCase.Common;
using DrivenAdapters.Mongo;
using EntryPoints.ServiceBus.Clientes;
using org.reactivecommons.api;
using org.reactivecommons.api.impl;
using Sistebanco.AppServices.Automapper;

namespace Tiendas.AppServices.Messaging.Extensions
{
    /// <summary>
    /// Service Extensions
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Método para registrar AutoMapper
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static IServiceCollection RegisterAutoMapper(this IServiceCollection services) =>
            services.AddAutoMapper(cfg =>
            {
                cfg.AddDataReaderMapping();
            }, typeof(ConfigurationProfile));

        /// <summary>
        /// Método para registrar Mongo
        /// </summary>
        /// <param name="services">services.</param>
        /// <param name="connectionString">connection string.</param>
        /// <param name="db">database.</param>
        /// <returns></returns>
        public static IServiceCollection RegisterMongo(this IServiceCollection services, string connectionString, string db) =>
                                    services.AddSingleton<IContext>(provider => new Context(connectionString, db));

        /// <summary>
        /// Método para registrar los servicios
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            #region UseCases

            services.AddTransient<IClienteCommandUseCase, ClienteCommandUseCase>();
            services.AddTransient<IManageEventsUseCase, ManageEventsUseCase>();

            #endregion UseCases

            return services;
        }

        /// <summary>
        /// RegisterSubscriptions
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection RegisterSubscriptions(this IServiceCollection services)
        {
            services.AddTransient<IClienteCommandSubscription, ClienteCommandSubscription>();

            return services;
        }

        /// <summary>
        /// RegisterAsyncGateways
        /// </summary>
        /// <param name="services"></param>
        /// <param name="serviceBusConn"></param>
        public static IServiceCollection RegisterAsyncGateways(this IServiceCollection services,
                string serviceBusConn)
        {
            services.RegisterAsyncGateway<ClienteRequest>(serviceBusConn);
            return services;
        }

        /// <summary>
        /// Register Gateway
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="services"></param>
        /// <param name="serviceBusConn"></param>
        private static void RegisterAsyncGateway<TEntity>(this IServiceCollection services, string serviceBusConn) =>
                services.AddSingleton<IDirectAsyncGateway<TEntity>>(new DirectAsyncGatewayServiceBus<TEntity>(serviceBusConn));
    }
}