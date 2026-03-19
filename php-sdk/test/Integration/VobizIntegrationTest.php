<?php
/**
 * Vobiz PHP SDK - Integration Tests (Read-Only)
 * Run: php test/Integration/VobizIntegrationTest.php
 * Requires: VOBIZ_AUTH_ID and VOBIZ_AUTH_TOKEN env vars
 */

require_once __DIR__ . '/../../vendor/autoload.php';

use Vobiz\Configuration;
use Vobiz\ApiException;
use Vobiz\VobizApi\AccountApi;
use Vobiz\VobizApi\CallApi;
use Vobiz\VobizApi\RecordingApi;
use Vobiz\VobizApi\ConferenceApi;
use Vobiz\VobizApi\ApplicationApi;

$authId    = getenv('VOBIZ_AUTH_ID');
$authToken = getenv('VOBIZ_AUTH_TOKEN');

if (!$authId || !$authToken) {
    echo "SKIP: VOBIZ_AUTH_ID or VOBIZ_AUTH_TOKEN not set\n";
    exit(0);
}

$config = new Configuration();
$config->setApiKey('X-Auth-ID', $authId);
$config->setApiKey('X-Auth-Token', $authToken);

$passed = 0;
$failed = 0;

function runTest(string $name, callable $fn): void {
    global $passed, $failed;
    try {
        $fn();
        echo "[PHP] PASS: $name\n";
        $passed++;
    } catch (ApiException $e) {
        echo "[PHP] FAIL: $name - HTTP {$e->getCode()}: {$e->getMessage()}\n";
        $failed++;
    } catch (Exception $e) {
        echo "[PHP] FAIL: $name - {$e->getMessage()}\n";
        $failed++;
    }
}

// Test 1: Get Account Details
runTest('GetAccountDetails', function() use ($config) {
    $api = new AccountApi(null, $config);
    $result = $api->apiV1AuthMeGet();
    echo "  -> Response received OK\n";
});

// Test 2: Get Live Calls
runTest('GetLiveCalls', function() use ($config, $authId) {
    $api = new CallApi(null, $config);
    $result = $api->apiV1AccountAuthIdCallGet($authId, 'live');
    echo "  -> Response received OK\n";
});

// Test 3: List Recordings
runTest('ListRecordings', function() use ($config, $authId) {
    $api = new RecordingApi(null, $config);
    $result = $api->apiV1AccountAccountIdRecordingGet($authId, 20, 0);
    echo "  -> Response received OK\n";
});

// Test 4: List Conferences
runTest('ListConferences', function() use ($config, $authId) {
    $api = new ConferenceApi(null, $config);
    $result = $api->apiV1AccountAuthIdConferenceGet($authId);
    echo "  -> Response received OK\n";
});

// Test 5: List Applications
runTest('ListApplications', function() use ($config, $authId) {
    $api = new ApplicationApi(null, $config);
    $result = $api->apiV1AccountAuthIdApplicationGet($authId);
    echo "  -> Response received OK\n";
});

echo "\n[PHP] Results: $passed passed, $failed failed\n";
exit($failed > 0 ? 1 : 0);
