using System;
using Microsoft.Extensions.DependencyInjection;
using Vobiz.Api;
using Xunit;

namespace Vobiz.Test.Api
{
    public sealed class TrunkApiTests : ApiTestsBase
    {
        private readonly ITrunkApi _instance;

        public TrunkApiTests() : base(Array.Empty<string>())
        {
            _instance = _host.Services.GetRequiredService<ITrunkApi>();
        }

        [Fact]
        public void ServiceResolves() => Assert.NotNull(_instance);

        [Theory]
        [InlineData(nameof(ITrunkApi.ApiV1AccountAccountIdCredentialsCredentialIdDeleteAsync))]
        [InlineData(nameof(ITrunkApi.ApiV1AccountAccountIdCredentialsCredentialIdPutAsync))]
        [InlineData(nameof(ITrunkApi.ApiV1AccountAccountIdCredentialsPostAsync))]
        [InlineData(nameof(ITrunkApi.ApiV1AccountAccountIdIpAclIpAclIdDeleteAsync))]
        [InlineData(nameof(ITrunkApi.ApiV1AccountAccountIdIpAclIpAclIdPutAsync))]
        [InlineData(nameof(ITrunkApi.ApiV1AccountAccountIdIpAclPostAsync))]
        [InlineData(nameof(ITrunkApi.ApiV1AccountAccountIdNumbersAssignDeleteAsync))]
        [InlineData(nameof(ITrunkApi.ApiV1AccountAccountIdNumbersAssignPostAsync))]
        [InlineData(nameof(ITrunkApi.ApiV1AccountAccountIdOriginationUrisPostAsync))]
        [InlineData(nameof(ITrunkApi.ApiV1AccountAccountIdOriginationUrisUriIdDeleteAsync))]
        [InlineData(nameof(ITrunkApi.ApiV1AccountAccountIdOriginationUrisUriIdPutAsync))]
        [InlineData(nameof(ITrunkApi.ApiV1AccountAccountIdTrunksCredentialsGetAsync))]
        [InlineData(nameof(ITrunkApi.ApiV1AccountAccountIdTrunksGetAsync))]
        [InlineData(nameof(ITrunkApi.ApiV1AccountAccountIdTrunksIpAclGetAsync))]
        [InlineData(nameof(ITrunkApi.ApiV1AccountAccountIdTrunksOriginationUrisGetAsync))]
        [InlineData(nameof(ITrunkApi.ApiV1AccountAccountIdTrunksPostAsync))]
        [InlineData(nameof(ITrunkApi.ApiV1AccountAccountIdTrunksTrunkIdDeleteAsync))]
        [InlineData(nameof(ITrunkApi.ApiV1AccountAccountIdTrunksTrunkIdGetAsync))]
        [InlineData(nameof(ITrunkApi.ApiV1AccountAccountIdTrunksTrunkIdPutAsync))]
        public void OperationMethodExists(string methodName)
        {
            Assert.Contains(typeof(ITrunkApi).GetMethods(), m => m.Name == methodName);
        }
    }
}
