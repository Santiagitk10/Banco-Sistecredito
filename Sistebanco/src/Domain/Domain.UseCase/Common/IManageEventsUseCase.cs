using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Domain.UseCase.Common
{
    /// <summary>
    /// IManageEventsUseCase
    /// </summary>
    public interface IManageEventsUseCase
    {
        /// <summary>
        /// Console error log
        /// </summary>
        void ConsoleProcessLog(string eventName, string id, dynamic? data, bool writeData = false, [CallerMemberName] string? callerMemberName = null);

        /// <summary>
        /// Console log
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="writeData"></param>
        /// <returns></returns>
        Task ConsoleLogAsync(string eventName, string id, dynamic data, bool writeData = false);

        /// <summary>
        /// Console error log
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        void ConsoleErrorLog(string message, Exception exception);

        /// <summary>
        /// ConsoleTraceLog
        /// </summary>
        /// <param name="message"></param>
        void ConsoleTraceLog(string message);

        /// <summary>
        /// ConsoleDebugLog
        /// </summary>
        /// <param name="message"></param>
        void ConsoleDebugLog(string message);

        /// <summary>
        /// Console information log
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        void ConsoleInfoLog(string message, params object[] args);
    }
}