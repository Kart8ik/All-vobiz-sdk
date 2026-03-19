import Foundation
import VobizSDK

// Vobiz Swift SDK - Integration Tests (Read-Only)
// Run: swift run VobizIntegrationTests
// Requires: VOBIZ_AUTH_ID and VOBIZ_AUTH_TOKEN env vars

guard let authId    = ProcessInfo.processInfo.environment["VOBIZ_AUTH_ID"],
      let authToken = ProcessInfo.processInfo.environment["VOBIZ_AUTH_TOKEN"],
      !authId.isEmpty, !authToken.isEmpty else {
    print("SKIP: VOBIZ_AUTH_ID or VOBIZ_AUTH_TOKEN not set")
    exit(0)
}

VobizSDKAPI.customHeaders = [
    "X-Auth-ID": authId,
    "X-Auth-Token": authToken
]
VobizSDKAPI.basePath = "https://api.vobiz.ai"

var passed = 0
var failed = 0
let group = DispatchGroup()

func runTest(_ name: String, _ block: @escaping (@escaping () -> Void) -> Void) {
    group.enter()
    block {
        group.leave()
    }
}

// Test 1: Get Account Details
runTest("GetAccountDetails") { done in
    AccountAPI.apiV1AuthMeGet { response, error in
        if let error = error {
            print("[Swift] FAIL: GetAccountDetails - \(error)")
            failed += 1
        } else {
            print("[Swift] PASS: GetAccountDetails")
            passed += 1
        }
        done()
    }
}

// Test 2: Get Live Calls
runTest("GetLiveCalls") { done in
    CallAPI.apiV1AccountAuthIdCallGet(authId: authId, status: "live") { response, error in
        if let error = error {
            print("[Swift] FAIL: GetLiveCalls - \(error)")
            failed += 1
        } else {
            print("[Swift] PASS: GetLiveCalls")
            passed += 1
        }
        done()
    }
}

// Test 3: List Recordings
runTest("ListRecordings") { done in
    RecordingAPI.apiV1AccountAccountIdRecordingGet(accountId: authId) { response, error in
        if let error = error {
            print("[Swift] FAIL: ListRecordings - \(error)")
            failed += 1
        } else {
            print("[Swift] PASS: ListRecordings")
            passed += 1
        }
        done()
    }
}

// Test 4: List Conferences
runTest("ListConferences") { done in
    ConferenceAPI.apiV1AccountAuthIdConferenceGet(authId: authId) { response, error in
        if let error = error {
            print("[Swift] FAIL: ListConferences - \(error)")
            failed += 1
        } else {
            print("[Swift] PASS: ListConferences")
            passed += 1
        }
        done()
    }
}

// Test 5: List Applications
runTest("ListApplications") { done in
    ApplicationAPI.apiV1AccountAuthIdApplicationGet(authId: authId) { response, error in
        if let error = error {
            print("[Swift] FAIL: ListApplications - \(error)")
            failed += 1
        } else {
            print("[Swift] PASS: ListApplications")
            passed += 1
        }
        done()
    }
}

group.wait()
print("\n[Swift] Results: \(passed) passed, \(failed) failed")
exit(failed > 0 ? 1 : 0)
