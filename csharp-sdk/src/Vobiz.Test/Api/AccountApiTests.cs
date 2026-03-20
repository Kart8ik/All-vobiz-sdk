using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Vobiz.Api;
using Xunit;

namespace Vobiz.Test.Api
{
    public sealed class AccountApiTests : ApiTestsBase
    {
        private readonly IAccountApi _instance;

        public AccountApiTests() : base(Array.Empty<string>())
        {
            _instance = _host.Services.GetRequiredService<IAccountApi>();
        }

        [Fact]
        public void ServiceResolves()
        {
            Assert.NotNull(_instance);
        }

        [Theory]
        [InlineData(nameof(IAccountApi.ApiV1AccountAccountIdBalanceINRGetAsync))]
        [InlineData(nameof(IAccountApi.ApiV1AccountAccountIdTransactionsGetAsync))]
        [InlineData(nameof(IAccountApi.ApiV1AccountAuthIdConcurrencyGetAsync))]
        [InlineData(nameof(IAccountApi.ApiV1AuthMeGetAsync))]
        public void OperationMethodExists(string methodName)
        {
            Assert.Contains(typeof(IAccountApi).GetMethods(), m => m.Name == methodName);
        }
    }
}
