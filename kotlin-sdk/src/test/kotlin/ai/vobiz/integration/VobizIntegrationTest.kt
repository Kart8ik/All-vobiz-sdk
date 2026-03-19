package ai.vobiz.integration

import ai.vobiz.apis.*
import ai.vobiz.infrastructure.ApiClient

/**
 * Vobiz Kotlin SDK - Integration Tests (Read-Only)
 * Run: ./gradlew integrationTest
 * Requires: VOBIZ_AUTH_ID and VOBIZ_AUTH_TOKEN env vars
 */
fun main() {
    val authId    = System.getenv("VOBIZ_AUTH_ID")
    val authToken = System.getenv("VOBIZ_AUTH_TOKEN")

    if (authId.isNullOrEmpty() || authToken.isNullOrEmpty()) {
        println("SKIP: VOBIZ_AUTH_ID or VOBIZ_AUTH_TOKEN not set")
        return
    }

    val client = ApiClient(
        baseUrl = "https://api.vobiz.ai",
        bearerToken = null
    ).apply {
        addDefaultHeader("X-Auth-ID", authId)
        addDefaultHeader("X-Auth-Token", authToken)
    }

    var passed = 0
    var failed = 0

    fun runTest(name: String, block: () -> Unit) {
        try {
            block()
            println("[Kotlin] PASS: $name")
            passed++
        } catch (e: Exception) {
            println("[Kotlin] FAIL: $name - ${e.message}")
            failed++
        }
    }

    runTest("GetAccountDetails") {
        val api = AccountApi(client)
        val result = api.apiV1AuthMeGet()
        println("  -> Response received OK")
    }

    runTest("GetLiveCalls") {
        val api = CallApi(client)
        val result = api.apiV1AccountAuthIdCallGet(authId, status = "live")
        println("  -> Response received OK")
    }

    runTest("ListRecordings") {
        val api = RecordingApi(client)
        val result = api.apiV1AccountAccountIdRecordingGet(authId)
        println("  -> Response received OK")
    }

    runTest("ListConferences") {
        val api = ConferenceApi(client)
        val result = api.apiV1AccountAuthIdConferenceGet(authId)
        println("  -> Response received OK")
    }

    runTest("ListApplications") {
        val api = ApplicationApi(client)
        val result = api.apiV1AccountAuthIdApplicationGet(authId)
        println("  -> Response received OK")
    }

    println("\n[Kotlin] Results: $passed passed, $failed failed")
    if (failed > 0) System.exit(1)
}
