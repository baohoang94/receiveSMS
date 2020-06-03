using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SmsDeliveredAPI.Middlewares
{
    /// <summary>
    /// 
    /// </summary>
    public class ClientSafeListMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ClientSafeListMiddleware> _logger;
        private readonly string _clientSafeList;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="logger"></param>
        /// <param name="clientSafeList"></param>
        public ClientSafeListMiddleware(
            RequestDelegate next,
            ILogger<ClientSafeListMiddleware> logger,
            string clientSafeList
            )
        {
            _next = next;
            _logger = logger;
            _clientSafeList = clientSafeList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {

            var remoteIp = context.Connection.RemoteIpAddress;

            string[] ip = _clientSafeList.Split(';');

            var bytes = remoteIp.GetAddressBytes();
            var badIp = true;
            foreach (var address in ip)
            {
                var testIp = IPAddress.Parse(address);
                if (testIp.GetAddressBytes().SequenceEqual(bytes))
                {
                    badIp = false;
                    break;
                }
            }

            if (badIp)
            {
                _logger.LogInformation($"Forbidden Request from Remote IP address: {remoteIp}");
                context.Response.StatusCode = 401;
                return;
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context).ConfigureAwait(false);
        }
    }
}
