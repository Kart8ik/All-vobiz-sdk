package ai.vobiz.api;

import ai.vobiz.ApiClient;
import ai.vobiz.ApiException;
import com.google.gson.Gson;
import org.junit.jupiter.api.*;
import org.junit.jupiter.api.condition.EnabledIfEnvironmentVariable;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

/**
 * Vobiz Java SDK - Full Call Flow Integration Test
 */
@EnabledIfEnvironmentVariable(named = "VOBIZ_AUTH_ID", matches = ".+")
@TestMethodOrder(MethodOrderer.OrderAnnotation.class)
public class CallFlowTest {

    private static ApiClient client;
    private static String authId;
    private static String authToken;
    private static String fromNumber;
    private static String toNumber;
    private static String transferNumber;
    private static String requestUUID;

    private static final String AUDIO_URL    = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3";
    private static final String ANSWER_URL   = "https://internal-test-xml.vobiz.ai/answer";
    private static final String HANGUP_URL   = "https://internal-test-xml.vobiz.ai/hangup";
    private static final String TRANSFER_URL = "https://internal-test-xml.vobiz.ai/answer";

    @BeforeAll
    static void setup() {
        authId         = System.getenv("VOBIZ_AUTH_ID");
        authToken      = System.getenv("VOBIZ_AUTH_TOKEN");
        fromNumber     = System.getenv("VOBIZ_FROM_NUMBER");
        toNumber       = System.getenv("VOBIZ_TO_NUMBER");
        transferNumber = System.getenv("VOBIZ_TRANSFER_NUMBER");

        if (fromNumber == null || toNumber == null) {
            throw new RuntimeException("VOBIZ_FROM_NUMBER and VOBIZ_TO_NUMBER required");
        }
        client = new ApiClient();
        client.addDefaultHeader("X-Auth-ID", authId);
        client.addDefaultHeader("X-Auth-Token", authToken);
    }

    private static void sleep(int seconds) {
        try { Thread.sleep(seconds * 1000L); } catch (InterruptedException ignored) {}
    }

    private static Map<String, Object> body(Object... kvPairs) {
        Map<String, Object> map = new HashMap<>();
        for (int i = 0; i < kvPairs.length - 1; i += 2)
            map.put((String) kvPairs[i], kvPairs[i + 1]);
        return map;
    }

    @Test @Order(1)
    void step1_makeCall() throws ApiException {
        System.out.println("\n[Java] STEP 1: Making outbound call...");
        CallApi api = new CallApi(client);
        Object result = api.apiV1AccountAuthIdCallPost(
            authId, authId, authToken, "application/json",
            body("from", fromNumber, "to", toNumber,
                 "answer_url", ANSWER_URL, "answer_method", "POST",
                 "hangup_url", HANGUP_URL, "hangup_method", "POST")
        );
        // Extract request_uuid
        if (result instanceof Map) {
            requestUUID = (String) ((Map<?,?>) result).get("request_uuid");
            if (requestUUID == null) {
                Object objs = ((Map<?,?>) result).get("objects");
                if (objs instanceof List && !((List<?>) objs).isEmpty()) {
                    requestUUID = (String) ((Map<?,?>) ((List<?>) objs).get(0)).get("request_uuid");
                }
            }
        }
        Assertions.assertNotNull(requestUUID, "request_uuid must not be null");
        System.out.println("[Java] STEP 1 PASS: request_uuid = " + requestUUID);
        sleep(5);
    }

    @Test @Order(2)
    void step2_listLiveCalls() throws ApiException {
        System.out.println("[Java] STEP 2: Listing live calls...");
        new CallApi(client).apiV1AccountAuthIdCallGet(authId, authId, authToken, "application/json", "live");
        System.out.println("[Java] STEP 2 PASS");
        sleep(5);
    }

    @Test @Order(3)
    void step3_getSingleLiveCall() throws ApiException {
        System.out.println("[Java] STEP 3: Get single live call...");
        new CallApi(client).apiV1AccountAuthIdCallGet_1(authId, authId, authToken, "application/json", "live");
        System.out.println("[Java] STEP 3 PASS");
        sleep(5);
    }

    @Test @Order(4)
    void step4_speakTTS() throws ApiException {
        System.out.println("[Java] STEP 4: Speak TTS...");
        new CallApi(client).apiV1AccountAuthIdCallSpeakPost(
            authId, authId, authToken, "application/json",
            body("text", "Hello from Vobiz Java SDK.", "voice", "WOMAN", "language", "en-US", "legs", "aleg")
        );
        System.out.println("[Java] STEP 4 PASS");
        sleep(5);
    }

    @Test @Order(5)
    void step5_stopTTS() throws ApiException {
        System.out.println("[Java] STEP 5: Stop TTS...");
        new CallApi(client).apiV1AccountAuthIdCallSpeakDelete(authId, authId, authToken, "application/json");
        System.out.println("[Java] STEP 5 PASS");
    }

    @Test @Order(6)
    void step6_playAudio() throws ApiException {
        System.out.println("[Java] STEP 6: Play audio...");
        new CallApi(client).apiV1AccountAuthIdCallPlayPost(
            authId, authId, authToken, "application/json",
            body("urls", List.of(AUDIO_URL), "legs", "aleg", "loop", false, "mix", true)
        );
        System.out.println("[Java] STEP 6 PASS");
        sleep(5);
    }

    @Test @Order(7)
    void step7_stopAudio() throws ApiException {
        System.out.println("[Java] STEP 7: Stop audio...");
        new CallApi(client).apiV1AccountAuthIdCallPlayDelete(authId, authId, authToken, "application/json");
        System.out.println("[Java] STEP 7 PASS");
    }

    @Test @Order(8)
    void step8_startRecording() throws ApiException {
        System.out.println("[Java] STEP 8: Start recording...");
        new CallApi(client).apiV1AccountAuthIdCallRecordPost(
            authId, authId, authToken, "application/json",
            body("time_limit", 60, "file_format", "mp3")
        );
        System.out.println("[Java] STEP 8 PASS");
        sleep(5);
    }

    @Test @Order(9)
    void step9_dtmf() throws ApiException {
        System.out.println("[Java] STEP 9: Send DTMF...");
        new CallApi(client).apiV1AccountAuthIdCallDTMFPost(
            authId, authId, authToken, "application/json",
            body("digits", "1234", "leg", "aleg")
        );
        System.out.println("[Java] STEP 9 PASS");
    }

    @Test @Order(10)
    void step10_stopRecording() throws ApiException {
        System.out.println("[Java] STEP 10: Stop recording...");
        new CallApi(client).apiV1AccountAuthIdCallRecordDelete(authId, authId, authToken, "application/json");
        System.out.println("[Java] STEP 10 PASS");
    }

    @Test @Order(11)
    void step11_transferCall() throws ApiException {
        System.out.println("[Java] STEP 11: Transfer call...");
        String transferTo = TRANSFER_URL + (transferNumber != null ? "?to=" + transferNumber : "");
        new CallApi(client).apiV1AccountAuthIdCallPost_2(
            authId, authId, authToken, "application/json",
            body("legs", "aleg", "aleg_url", transferTo, "aleg_method", "POST")
        );
        System.out.println("[Java] STEP 11 PASS");
        sleep(5);
    }

    @Test @Order(12)
    void step12_hangUp() throws ApiException {
        System.out.println("[Java] STEP 12: Hang up...");
        new CallApi(client).apiV1AccountAuthIdCallDelete(authId, authId, authToken, "application/json");
        System.out.println("[Java] STEP 12 PASS");
        System.out.println("\n[Java] Call flow COMPLETE");
    }
}
