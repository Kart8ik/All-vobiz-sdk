<?php
/**
 * Vobiz PHP SDK - Full Call Flow Integration Test
 * Steps: Make Call → List Live → Get Live → Speak → Stop Speak → Play → Stop Play
 *        → Start Record → DTMF → Stop Record → Transfer → Hang Up
 * Run: php test/Integration/CallFlowTest.php
 */

require_once __DIR__ . '/../../vendor/autoload.php';

use Vobiz\Configuration;
use Vobiz\ApiException;
use Vobiz\VobizApi\CallApi;

$authId         = getenv('VOBIZ_AUTH_ID');
$authToken      = getenv('VOBIZ_AUTH_TOKEN');
$fromNumber     = getenv('VOBIZ_FROM_NUMBER');
$toNumber       = getenv('VOBIZ_TO_NUMBER');
$transferNumber = getenv('VOBIZ_TRANSFER_NUMBER');

if (!$authId || !$authToken || !$fromNumber || !$toNumber) {
    echo "SKIP: VOBIZ_AUTH_ID, VOBIZ_AUTH_TOKEN, VOBIZ_FROM_NUMBER, VOBIZ_TO_NUMBER required\n";
    exit(0);
}

const AUDIO_URL    = 'https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3';
const ANSWER_URL   = 'https://internal-test-xml.vobiz.ai/answer';
const HANGUP_URL   = 'https://internal-test-xml.vobiz.ai/hangup';
const TRANSFER_URL = 'https://internal-test-xml.vobiz.ai/answer';

$config = new Configuration();
$config->setApiKey('X-Auth-ID', $authId);
$config->setApiKey('X-Auth-Token', $authToken);

$api     = new CallApi(null, $config);
$passed  = 0;
$failed  = 0;

function step(string $name, callable $fn): void {
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

// STEP 1: Make outbound call
echo "\n[PHP] STEP 1: Making outbound call...\n";
$requestUUID = null;
step('Make Call', function() use ($api, $authId, $authToken, $fromNumber, $toNumber, &$requestUUID) {
    $result = $api->apiV1AccountAuthIdCallPost(
        $authId, $authId, $authToken, 'application/json',
        [
            'from'           => $fromNumber,
            'to'             => $toNumber,
            'answer_url'     => ANSWER_URL,
            'answer_method'  => 'POST',
            'hangup_url'     => HANGUP_URL,
            'hangup_method'  => 'POST',
        ]
    );
    $data = is_array($result) ? $result : json_decode(json_encode($result), true);
    $requestUUID = $data['request_uuid'] ?? ($data['objects'][0]['request_uuid'] ?? null);
    if (!$requestUUID) throw new Exception("No request_uuid in response: " . json_encode($data));
    echo "  -> request_uuid = $requestUUID\n";
});

if (!$requestUUID) { echo "[PHP] Cannot continue without request_uuid\n"; exit(1); }
sleep(5);

// STEP 2: List all live calls
echo "[PHP] STEP 2: Listing all live calls...\n";
step('List Live Calls', function() use ($api, $authId, $authToken) {
    $api->apiV1AccountAuthIdCallGet($authId, $authId, $authToken, 'application/json', 'live');
    echo "  -> OK\n";
});
sleep(5);

// STEP 3: Get single live call
echo "[PHP] STEP 3: Getting single live call...\n";
step('Get Single Live Call', function() use ($api, $authId, $authToken) {
    $api->apiV1AccountAuthIdCallGet_0($authId, $authId, $authToken, 'application/json', 'live');
    echo "  -> OK\n";
});
sleep(5);

// STEP 4: Speak TTS
echo "[PHP] STEP 4: Speaking TTS...\n";
step('Speak TTS', function() use ($api, $authId, $authToken) {
    $api->apiV1AccountAuthIdCallSpeakPost($authId, $authId, $authToken, 'application/json', [
        'text' => 'Hello from Vobiz PHP SDK.', 'voice' => 'WOMAN', 'language' => 'en-US', 'legs' => 'aleg'
    ]);
    echo "  -> OK\n";
});
sleep(5);

// STEP 5: Stop TTS
echo "[PHP] STEP 5: Stopping TTS...\n";
step('Stop TTS', function() use ($api, $authId, $authToken) {
    $api->apiV1AccountAuthIdCallSpeakDelete($authId, $authId, $authToken, 'application/json');
    echo "  -> OK\n";
});

// STEP 6: Play audio
echo "[PHP] STEP 6: Playing audio...\n";
step('Play Audio', function() use ($api, $authId, $authToken) {
    $api->apiV1AccountAuthIdCallPlayPost($authId, $authId, $authToken, 'application/json', [
        'urls' => [AUDIO_URL], 'legs' => 'aleg', 'loop' => false, 'mix' => true
    ]);
    echo "  -> OK\n";
});
sleep(5);

// STEP 7: Stop audio
echo "[PHP] STEP 7: Stopping audio...\n";
step('Stop Audio', function() use ($api, $authId, $authToken) {
    $api->apiV1AccountAuthIdCallPlayDelete($authId, $authId, $authToken, 'application/json');
    echo "  -> OK\n";
});

// STEP 8: Start recording
echo "[PHP] STEP 8: Starting recording...\n";
step('Start Recording', function() use ($api, $authId, $authToken) {
    $api->apiV1AccountAuthIdCallRecordPost($authId, $authId, $authToken, 'application/json', [
        'time_limit' => 60, 'file_format' => 'mp3'
    ]);
    echo "  -> OK\n";
});
sleep(5);

// STEP 9: Send DTMF
echo "[PHP] STEP 9: Sending DTMF...\n";
step('Send DTMF', function() use ($api, $authId, $authToken) {
    $api->apiV1AccountAuthIdCallDtmfPost($authId, $authId, $authToken, 'application/json', [
        'digits' => '1234', 'leg' => 'aleg'
    ]);
    echo "  -> OK\n";
});

// STEP 10: Stop recording
echo "[PHP] STEP 10: Stopping recording...\n";
step('Stop Recording', function() use ($api, $authId, $authToken) {
    $api->apiV1AccountAuthIdCallRecordDelete($authId, $authId, $authToken, 'application/json');
    echo "  -> OK\n";
});

// STEP 11: Transfer call
echo "[PHP] STEP 11: Transferring call...\n";
step('Transfer Call', function() use ($api, $authId, $authToken, $transferNumber) {
    $transferTo = TRANSFER_URL . ($transferNumber ? "?to=$transferNumber" : '');
    $api->apiV1AccountAuthIdCallPost_0($authId, $authId, $authToken, 'application/json', [
        'legs' => 'aleg', 'aleg_url' => $transferTo, 'aleg_method' => 'POST'
    ]);
    echo "  -> OK\n";
});
sleep(5);

// STEP 12: Hang up
echo "[PHP] STEP 12: Hanging up call...\n";
step('Hang Up Call', function() use ($api, $authId, $authToken) {
    $api->apiV1AccountAuthIdCallDelete($authId, $authId, $authToken, 'application/json');
    echo "  -> OK\n";
});

echo "\n[PHP] Call flow COMPLETE: $passed passed, $failed failed\n";
exit($failed > 0 ? 1 : 0);
