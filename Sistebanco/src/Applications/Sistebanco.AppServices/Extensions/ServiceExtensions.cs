using AutoMapper.Data;
using DrivenAdapter.ServiceBus.Entities;
using credinet.comun.api;
using Domain.Model.Interfaces.Gateways;
using Domain.Model.Interfaces.UseCases.Clientes;
using Domain.Model.Interfaces.UseCases.Cuentas;
using Domain.Model.Interfaces.UseCases.Transacciones;
using Domain.UseCase.Clientes;
using Domain.UseCase.Common;

using Domain.UseCase.Common;

using Domain.UseCase.Common;

using Domain.UseCase.Cuentas;
using Domain.UseCase.Transacciones;
using DrivenAdapters.Mongo;
using DrivenAdapters.Mongo.Entities;

using Microsoft.Extensions.DependencyInjection;
using org.reactivecommons.api;
using org.reactivecommons.api.impl;
using Sistebanco.AppServices.Automapper;

using StackExchange.Redis;

using System;
using DrivenAdapter.ServiceBus;

namespace Sistebanco.AppServices.Extensions
{
    /// <summary>
    /// Service Extensions
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Registers the cors.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="policyName">Name of the policy.</param>
        /// <returns></returns>
        public static IServiceCollection RegisterCors(this IServiceCollection services, string policyName) =>
            services.AddCors(o => o.AddPolicy(policyName, builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

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
        /// Registro del blobstorage
        /// </summary>
        /// <param name="services">Contenedor de servicios</param>
        /// <param name="connectionString">cadena de conexion del storage</param>
        /// <param name="containerName">nombre del contenedor del storage</param>
        /// <returns></returns>
        //public static IServiceCollection RegisterBlobstorage(this IServiceCollection services, string connectionString, string containerName)
        //{
        //    //Blob storage
        //    //TODO: Buscar si existe mejor implementacion de la DI
        //    services.AddSingleton<IBlobStorage>(provider => new BlobStorage(containerName, connectionString));
        //    return services;
        //}

        /// <summary>
        ///   Método para registrar Redis Cache
        /// </summary>
        /// <param name="services">services.</param>
        /// <param name="connectionString">connection string.</param>
        /// <param name="dbNumber">database number.</param>
        /// <returns></returns>
        public static IServiceCollection RegisterRedis(this IServiceCollection services, string connectionString, int dbNumber)
        {
            services.AddSingleton(s => LazyConnection(connectionString).Value.GetDatabase(dbNumber));

            ConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect(connectionString,
                opt => opt.DefaultDatabase = dbNumber);
            services.AddSingleton<IConnectionMultiplexer>(multiplexer);

            return services;
        }

        /// <summary>
        /// Método para registrar los servicios
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            #region Adaptadores

            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<ICuentaRepository, CuentaRepository>();
            services.AddScoped<ITransaccionRepository, TransaccionRepository>();

            #endregion Adaptadores

            #region UseCases

            services.AddScoped<IManageEventsUseCase, ManageEventsUseCase>();
            services.AddScoped<IClienteEventsRepository, ClienteEventsRepository>();
            services.AddScoped<ICrearClienteUseCase, CrearClienteUseCase>();
            services.AddScoped<IModificarClienteUseCase, ModificarClienteUseCase>();
            services.AddScoped<IObtenerPorDocumentoDeIdentidadUseCase, ObtenerPorDocumentoDeIdentidadUseCase>();
            services.AddScoped<IObtenerTodosLosClientesUseCase, ObtenerTodosLosClientesUseCase>();
            services.AddScoped<ICancelarCuentaUseCase, CancelarCuentaUseCase>();
            services.AddScoped<ICrearCuentaUseCase, CrearCuentaUseCase>();
            services.AddScoped<IMarcarExentaDeGravamenUseCase, MarcarExentaDeGravamenUseCase>();
            services.AddScoped<IObtenerEstadoDeCuentaUseCase, ObtenerEstadoDeCuentaUseCase>();
            services.AddScoped<IConsignarUseCase, ConsignarUseCase>();
            services.AddScoped<IRetirarUseCase, RetirarUseCase>();
            services.AddScoped<ITransferirUseCase, TransferirUseCase>();

            #endregion UseCases

            return services;
        }

        /// <summary>
        ///   Lazies the connection.
        /// </summary>
        /// <param name="connectionString">connection string.</param>
        /// <returns></returns>
        private static Lazy<ConnectionMultiplexer> LazyConnection(string connectionString) =>
            new(() =>
            {
                return ConnectionMultiplexer.Connect(connectionString);
            });

        /// <summary>
        /// RegisterAsyncGateways
        /// </summary>
        /// <param name="services"></param>
        /// <param name="serviceBusConn"></param>
        public static IServiceCollection RegisterAsyncGateways(this IServiceCollection services,
                string serviceBusConn)
        {
            services.RegisterAsyncGateway<ClienteEntityDTO>(serviceBusConn);
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