using System;
using System.Threading.Tasks;
using Vobiz.Api;
using Vobiz.Client;

namespace Vobiz.IntegrationTests
{
    /// <summary>
    /// Vobiz C# SDK Integration Tests (Read-Only)
    /// Set VOBIZ_AUTH_ID and VOBIZ_AUTH_TOKEN environment variables before running.
    /// Run: dotnet run --project src/Vobiz.IntegrationTests/
    /// </summary>
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var authId    = Environment.GetEnvironmentVariable("VOBIZ_AUTH_ID");
            var authToken = Environment.GetEnvironmentVariable("VOBIZ_AUTH_TOKEN");

            if (string.IsNullOrEmpty(authId) || string.IsNullOrEmpty(authToken))
            {
                Console.WriteLine("SKIP: VOBIZ_AUTH_ID or VOBIZ_AUTH_TOKEN not set");
                return 0;
            }

            var hostConfig = new HostConfiguration(new System.Net.Http.HttpClient());
            hostConfig.AddApiKey("X-Auth-ID", authId);
            hostConfig.AddApiKey("X-Auth-Token", authToken);

            int passed = 0, failed = 0;

            async Task RunTest(string name, Func<Task> test)
            {
                try
                {
                    await test();
                    Console.WriteLine($"[C#] PASS: {name}");
                    passed++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[C#] FAIL: {name} - {ex.Message}");
                    failed++;
                }
            }

            var accountApi     = new AccountApi(null, null, hostConfig);
            var callApi        = new CallApi(null, null, hostConfig);
            var recordingApi   = new RecordingApi(null, null, hostConfig);
            var conferenceApi  = new ConferenceApi(null, null, hostConfig);
            var applicationApi = new ApplicationApi(null, null, hostConfig);

            await RunTest("GetAccountDetails", async () =>
            {
                var result = await accountApi.ApiV1AuthMeGetAsync();
                Console.WriteLine($"  -> Response received OK");
            });

            await RunTest("GetLiveCalls", async () =>
            {
                var result = await callApi.ApiV1AccountAuthIdCallGetAsync(authId, "live");
                Console.WriteLine($"  -> Response received OK");
            });

            await RunTest("ListRecordings", async () =>
            {
                var result = await recordingApi.ApiV1AccountAccountIdRecordingGetAsync(authId);
                Console.WriteLine($"  -> Response received OK");
            });

            await RunTest("ListConferences", async () =>
            {
                var result = await conferenceApi.ApiV1AccountAuthIdConferenceGetAsync(authId);
                Console.WriteLine($"  -> Response received OK");
            });

            await RunTest("ListApplications", async () =>
            {
                var result = await applicationApi.ApiV1AccountAuthIdApplicationGetAsync(authId);
                Console.WriteLine($"  -> Response received OK");
            });

            Console.WriteLine($"\n[C#] Results: {passed} passed, {failed} failed");
            return failed > 0 ? 1 : 0;
        }
    }
}
