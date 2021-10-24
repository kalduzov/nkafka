using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Microlibs.Kafka.Config
{
    public abstract record CommonConfig
    {
        /// <summary>
        /// bootstrap.servers
        /// </summary>
        public IReadOnlyList<string> BootstrapServers { get; set; } = null!;

        /// <summary>
        /// Идентификатор клиента
        /// client.id
        /// </summary>
        public string ClientId { get; set; } = GetHostName();

        /// <summary>
        /// Таймаут обновления данных по брокерам в ms
        /// </summary>
        /// <remarks>Default - 1000ms</remarks>
        public int BrokerUpdateTimeout { get; set; } = 1000;

        private static string GetHostName()
        {
            try
            {
                return Dns.GetHostName();
            }
            catch
            {
                return $"Microlibs/{Assembly.GetCallingAssembly().GetName().Version}";
            }
        }
    }
}