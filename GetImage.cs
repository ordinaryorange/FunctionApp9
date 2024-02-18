using System.Net;
using System.Runtime.ConstrainedExecution;
using CefSharp;
using CefSharp.OffScreen;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace functionapp9
{
    public class GetImage
    {
        private readonly ILogger _logger;

        public GetImage(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GetImage>();
        }

        [Function("GetImage")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            var sett = new CefSettings()
            {
                //LogSeverity = LogSeverity.Verbose
            };
            if (!Cef.IsInitialized)
            {
                Cef.Initialize(sett);
                _logger.LogInformation("Cef initialized");
            }
            using (var browser = new ChromiumWebBrowser("https://www.google.com"))
            {
                _logger.LogInformation("Browser started");
                var initialLoadResponse = await browser.WaitForInitialLoadAsync();
                await Task.Delay(1000);
                var bitmapAsByteArray = await browser.CaptureScreenshotAsync();
                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "image/bmp");
                response.Body = new MemoryStream(bitmapAsByteArray);
                return response;
            }
        }
    }
}
