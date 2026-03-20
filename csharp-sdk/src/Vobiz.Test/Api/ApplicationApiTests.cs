using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Vobiz.Api;
using Xunit;

namespace Vobiz.Test.Api
{
    public sealed class ApplicationApiTests : ApiTestsBase
    {
        private readonly IApplicationApi _instance;

        public ApplicationApiTests() : base(Array.Empty<string>())
        {
            _instance = _host.Services.GetRequiredService<IApplicationApi>();
        }

        [Fact]
        public void ServiceResolves() => Assert.NotNull(_instance);

        [Theory]
        [InlineData(nameof(IApplicationApi.ApiV1AccountAuthIdApplicationDeleteAsync))]
        [InlineData(nameof(IApplicationApi.ApiV1AccountAuthIdApplicationGetAsync))]
        [InlineData(nameof(IApplicationApi.ApiV1AccountAuthIdApplicationGet_0Async))]
        [InlineData(nameof(IApplicationApi.ApiV1AccountAuthIdApplicationPostAsync))]
        [InlineData(nameof(IApplicationApi.ApiV1AccountAuthIdApplicationPost_0Async))]
        [InlineData(nameof(IApplicationApi.ApiV1AccountAuthIdNumbersApplicationDeleteAsync))]
        [InlineData(nameof(IApplicationApi.ApiV1AccountAuthIdNumbersApplicationPostAsync))]
        public void OperationMethodExists(string methodName)
        {
            Assert.Contains(typeof(IApplicationApi).GetMethods(), m => m.Name == methodName);
        }
    }
}
