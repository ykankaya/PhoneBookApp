using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ReportService.Application.Tests.Mocks
{
    
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>>? _handlerFunc;

        public string? Input { get; private set; }
        public int NumberOfCalls { get; private set; }

       
        public MockHttpMessageHandler(HttpResponseMessage response)
        {
            _response = response;
        }

       
        public MockHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handlerFunc)
        {
            _handlerFunc = handlerFunc;
            _response = new HttpResponseMessage(HttpStatusCode.NotImplemented); 
        }


        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            NumberOfCalls++;
            Input = await (request.Content?.ReadAsStringAsync(cancellationToken) ?? Task.FromResult<string?>(null));

            if(_handlerFunc != null)
            {
                return await _handlerFunc(request, cancellationToken);
            }

            return _response;
        }
    }
}