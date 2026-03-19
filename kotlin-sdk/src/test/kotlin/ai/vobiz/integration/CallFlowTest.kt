package ai.vobiz.integration

import ai.vobiz.apis.CallApi
import ai.vobiz.infrastructure.ApiClient

/**
 * Vobiz Kotlin SDK - Full Call Flow Integration Test
 */
fun main() {
    val authId         = System.getenv("VOBIZ_AUTH_ID")     ?: ""
    val authToken      = System.getenv("VOBIZ_AUTH_TOKEN")  ?: ""
    val fromNumber     = System.getenv("VOBIZ_FROM_NUMBER") ?: ""
    val toNumber       = System.getenv("VOBIZ_TO_NUMBER")   ?: ""
    val transferNumber = System.getenv("VOBIZ_TRANSFER_NUMBER") ?: ""

    if (authId.isEmpty() || authToken.isEmpty() || fromNumber.isEmpty() || toNumber.isEmpty()) {
        println("SKIP: VOBIZ_AUTH_ID, VOBIZ_AUTH_TOKEN, VOBIZ_FROM_NUMBER, VOBIZ_TO_NUMBER required")
        return
    }

    val AUDIO_URL    = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3"
    val ANSWER_URL   = "https://internal-test-xml.vobiz.ai/answer"
    val HANGUP_URL   = "https://internal-test-xml.vobiz.ai/hangup"
    val TRANSFER_URL = "https://internal-test-xml.vobiz.ai/answer"

    val client = ApiClient(baseUrl = "https://api.vobiz.ai").apply {
        addDefaultHeader("X-Auth-ID", authId)
        addDefaultHeader("X-Auth-Token", authToken)
    }
    val api     = CallApi(client)
    var passed  = 0
    var failed  = 0

    fun step(name: String, block: () -> Unit) {
        try { block(); println("[Kotlin] PASS: $name"); passed++ }
        catch (e: Exception) { println("[Kotlin] FAIL: $name - ${e.message}"); failed++ }
    }

    fun sleep(sec: Long) = Thread.sleep(sec * 1000)

    println("\n[Kotlin] STEP 1: Making outbound call...")
    var requestUUID = ""
    step("Make Call") {
        val result = api.apiV1AccountAuthIdCallPost(
            authId, authId, authToken, "application/json",
            mapOf("from" to fromNumber, "to" to toNumber,
                  "answer_url" to ANSWER_URL, "answer_method" to "POST",
                  "hangup_url" to HANGUP_URL, "hangup_method" to "POST")
        )
        @Suppress("UNCHECKED_CAST")
        requestUUID = (result as? Map<String, Any>)?.get("request_uuid") as? String ?: ""
        if (requestUUID.isEmpty()) throw Exception("No request_uuid in response")
        println("  -> request_uuid = $requestUUID")
    }
    sleep(5)

    step("List Live Calls") {
        api.apiV1AccountAuthIdCallGet(authId, authId, authToken, "application/json", status = "live")
    }
    sleep(5)

    step("Get Single Live Call") {
        api.apiV1AccountAuthIdCallGet_1(authId, authId, authToken, "application/json", status = "live")
    }
    sleep(5)

    step("Speak TTS") {
        api.apiV1AccountAuthIdCallSpeakPost(authId, authId, authToken, "application/json",
            mapOf("text" to "Hello from Vobiz Kotlin SDK.", "voice" to "WOMAN",
                  "language" to "en-US", "legs" to "aleg"))
    }
    sleep(5)

    step("Stop TTS") {
        api.apiV1AccountAuthIdCallSpeakDelete(authId, authId, authToken, "application/json")
    }

    step("Play Audio") {
        api.apiV1AccountAuthIdCallPlayPost(authId, authId, authToken, "application/json",
            mapOf("urls" to listOf(AUDIO_URL), "legs" to "aleg", "loop" to false, "mix" to true))
    }
    sleep(5)

    step("Stop Audio") {
        api.apiV1AccountAuthIdCallPlayDelete(authId, authId, authToken, "application/json")
    }

    step("Start Recording") {
        api.apiV1AccountAuthIdCallRecordPost(authId, authId, authToken, "application/json",
            mapOf("time_limit" to 60, "file_format" to "mp3"))
    }
    sleep(5)

    step("Send DTMF") {
        api.apiV1AccountAuthIdCallDTMFPost(authId, authId, authToken, "application/json",
            mapOf("digits" to "1234", "leg" to "aleg"))
    }

    step("Stop Recording") {
        api.apiV1AccountAuthIdCallRecordDelete(authId, authId, authToken, "application/json")
    }

    step("Transfer Call") {
        val transferTo = TRANSFER_URL + if (transferNumber.isNotEmpty()) "?to=$transferNumber" else ""
        api.apiV1AccountAuthIdCallPost_2(authId, authId, authToken, "application/json",
            mapOf("legs" to "aleg", "aleg_url" to transferTo, "aleg_method" to "POST"))
    }
    sleep(5)

    step("Hang Up Call") {
        api.apiV1AccountAuthIdCallDelete(authId, authId, authToken, "application/json")
    }

    println("\n[Kotlin] Call flow COMPLETE: $passed passed, $failed failed")
    if (failed > 0) System.exit(1)
}
