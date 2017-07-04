using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DBSharp.IRIS
{
    public abstract class IRISRequest
    {
        private const int MAX_HTTP_CONNECTIONS = 8;

        private static object CounterLock = new object();
        public static ulong TotalRequestsMade { get; private set; } = 0;
        public static ulong TotalSucessfullRequests { get; private set; } = 0;

        public static SemaphoreSlim Semaphore = new SemaphoreSlim(MAX_HTTP_CONNECTIONS);
        
        /// <summary>
        /// True if last request was sucessfull
        /// </summary>
        public bool Successfull { get; protected set; }

        /// <summary>
        /// Generates the api URL for IRIS
        /// </summary>
        /// <returns>URL for IRIS request</returns>
        public virtual string GenerateURL()
        {
            return IRISConfig.BASE_URL;
        }

        /// <summary>
        /// Performs the request to DB
        /// Throws if invalid data was recived or request failed
        /// </summary>
        /// <returns>Awaitable task</returns>
        public virtual async Task DoRequestAsync()
        {
            Successfull = false;
            WebRequest request = WebRequest.Create(GenerateURL());
            ulong requestNo = 0;
            lock(CounterLock) requestNo = TotalRequestsMade++;
            var response = await request.GetResponseAsync();
            await Semaphore.WaitAsync();
            try
            {
                using (var stream = response.GetResponseStream())
                using (var sr = new StreamReader(stream))
                {
                    var xml = XElement.Parse(await sr.ReadToEndAsync());
                    Successfull = ParseResponse(xml);
                    if (Directory.Exists("./IRISLogs/"))
                        File.WriteAllText("./IRISLogs/" + requestNo + ".xml", xml.ToString());
                    if (Successfull)
                        lock(CounterLock) TotalSucessfullRequests++;
                }
            }
            finally
            {
                Semaphore.Release();
            }
        }

        protected abstract bool ParseResponse(XElement xml);
    }
}
