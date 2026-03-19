package ai.vobiz.api;

import ai.vobiz.ApiClient;
import ai.vobiz.ApiException;
import org.junit.jupiter.api.BeforeAll;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.condition.EnabledIfEnvironmentVariable;

import static org.junit.jupiter.api.Assertions.*;

/**
 * Vobiz Java SDK - Integration Tests (Read-Only)
 * Requires VOBIZ_AUTH_ID and VOBIZ_AUTH_TOKEN environment variables.
 */
@EnabledIfEnvironmentVariable(named = "VOBIZ_AUTH_ID", matches = ".+")
public class VobizIntegrationTest {

    private static ApiClient client;
    private static String authId;

    @BeforeAll
    static void setup() {
        authId = System.getenv("VOBIZ_AUTH_ID");
        String authToken = System.getenv("VOBIZ_AUTH_TOKEN");

        client = new ApiClient();
        client.addDefaultHeader("X-Auth-ID", authId);
        client.addDefaultHeader("X-Auth-Token", authToken);
    }

    @Test
    void testGetAccountDetails() throws ApiException {
        AccountApi api = new AccountApi(client);
        Object result = api.apiV1AuthMeGet();
        System.out.println("[Java] GetAccountDetails: OK");
        assertNotNull(result);
    }

    @Test
    void testGetLiveCalls() throws ApiException {
        CallApi api = new CallApi(client);
        Object result = api.apiV1AccountAuthIdCallGet(authId, "live");
        System.out.println("[Java] GetLiveCalls: OK");
        assertNotNull(result);
    }

    @Test
    void testListRecordings() throws ApiException {
        RecordingApi api = new RecordingApi(client);
        Object result = api.apiV1AccountAccountIdRecordingGet(authId, 20, 0, null, null);
        System.out.println("[Java] ListRecordings: OK");
        assertNotNull(result);
    }

    @Test
    void testListConferences() throws ApiException {
        ConferenceApi api = new ConferenceApi(client);
        Object result = api.apiV1AccountAuthIdConferenceGet(authId);
        System.out.println("[Java] ListConferences: OK");
        assertNotNull(result);
    }

    @Test
    void testListApplications() throws ApiException {
        ApplicationApi api = new ApplicationApi(client);
        Object result = api.apiV1AccountAuthIdApplicationGet(authId, 20, 0);
        System.out.println("[Java] ListApplications: OK");
        assertNotNull(result);
    }
}
