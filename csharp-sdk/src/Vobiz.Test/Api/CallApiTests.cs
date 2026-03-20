using System;
using Microsoft.Extensions.DependencyInjection;
using Vobiz.Api;
using Xunit;

namespace Vobiz.Test.Api
{
    public sealed class CallApiTests : ApiTestsBase
    {
        private readonly ICallApi _instance;

        public CallApiTests() : base(Array.Empty<string>())
        {
            _instance = _host.Services.GetRequiredService<ICallApi>();
        }

        [Fact]
        public void ServiceResolves() => Assert.NotNull(_instance);

        [Theory]
        [InlineData(nameof(ICallApi.ApiV1AccountAuthIdCallDTMFPostAsync))]
        [InlineData(nameof(ICallApi.ApiV1AccountAuthIdCallDeleteAsync))]
        [InlineData(nameof(ICallApi.ApiV1AccountAuthIdCallGetAsync))]
        [InlineData(nameof(ICallApi.ApiV1AccountAuthIdCallGet_0Async))]
        [InlineData(nameof(ICallApi.ApiV1AccountAuthIdCallPlayDeleteAsync))]
        [InlineData(nameof(ICallApi.ApiV1AccountAuthIdCallPlayPostAsync))]
        [InlineData(nameof(ICallApi.ApiV1AccountAuthIdCallPostAsync))]
        [InlineData(nameof(ICallApi.ApiV1AccountAuthIdCallPost_0Async))]
        [InlineData(nameof(ICallApi.ApiV1AccountAuthIdCallRecordDeleteAsync))]
        [InlineData(nameof(ICallApi.ApiV1AccountAuthIdCallRecordPostAsync))]
        [InlineData(nameof(ICallApi.ApiV1AccountAuthIdCallSpeakDeleteAsync))]
        [InlineData(nameof(ICallApi.ApiV1AccountAuthIdCallSpeakPostAsync))]
        public void OperationMethodExists(string methodName)
        {
            Assert.Contains(typeof(ICallApi).GetMethods(), m => m.Name == methodName);
        }
    }
}
