using System;
using Microsoft.Extensions.DependencyInjection;
using Vobiz.Api;
using Xunit;

namespace Vobiz.Test.Api
{
    public sealed class ConferenceApiTests : ApiTestsBase
    {
        private readonly IConferenceApi _instance;

        public ConferenceApiTests() : base(Array.Empty<string>())
        {
            _instance = _host.Services.GetRequiredService<IConferenceApi>();
        }

        [Fact]
        public void ServiceResolves() => Assert.NotNull(_instance);

        [Theory]
        [InlineData(nameof(IConferenceApi.ApiV1AccountAuthIdConferenceDeleteAsync))]
        [InlineData(nameof(IConferenceApi.ApiV1AccountAuthIdConferenceDelete_0Async))]
        [InlineData(nameof(IConferenceApi.ApiV1AccountAuthIdConferenceGetAsync))]
        [InlineData(nameof(IConferenceApi.ApiV1AccountAuthIdConferenceGet_0Async))]
        [InlineData(nameof(IConferenceApi.ApiV1AccountAuthIdConferenceMemberDeafDeleteAsync))]
        [InlineData(nameof(IConferenceApi.ApiV1AccountAuthIdConferenceMemberDeafPostAsync))]
        [InlineData(nameof(IConferenceApi.ApiV1AccountAuthIdConferenceMemberDeleteAsync))]
        [InlineData(nameof(IConferenceApi.ApiV1AccountAuthIdConferenceMemberKickPostAsync))]
        [InlineData(nameof(IConferenceApi.ApiV1AccountAuthIdConferenceMemberMuteDeleteAsync))]
        [InlineData(nameof(IConferenceApi.ApiV1AccountAuthIdConferenceMemberMutePostAsync))]
        [InlineData(nameof(IConferenceApi.ApiV1AccountAuthIdConferenceMemberPlayDeleteAsync))]
        [InlineData(nameof(IConferenceApi.ApiV1AccountAuthIdConferenceMemberPlayPostAsync))]
        [InlineData(nameof(IConferenceApi.ApiV1AccountAuthIdConferenceRecordDeleteAsync))]
        [InlineData(nameof(IConferenceApi.ApiV1AccountAuthIdConferenceRecordPostAsync))]
        public void OperationMethodExists(string methodName)
        {
            Assert.Contains(typeof(IConferenceApi).GetMethods(), m => m.Name == methodName);
        }
    }
}
