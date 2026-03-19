/**
 * Vobiz Node.js SDK - Full Call Flow Integration Test
 * Steps: Make Call → List Live → Get Live → Speak → Stop Speak → Play → Stop Play
 *        → Start Record → DTMF → Stop Record → Transfer → Hang Up
 */

import { CallApi } from '../api/callApi';

const AUTH_ID         = process.env.VOBIZ_AUTH_ID         || '';
const AUTH_TOKEN      = process.env.VOBIZ_AUTH_TOKEN       || '';
const FROM_NUMBER     = process.env.VOBIZ_FROM_NUMBER      || '';
const TO_NUMBER       = process.env.VOBIZ_TO_NUMBER        || '';
const TRANSFER_NUMBER = process.env.VOBIZ_TRANSFER_NUMBER  || '';

const AUDIO_URL    = 'https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3';
const ANSWER_URL   = 'https://internal-test-xml.vobiz.ai/answer';
const HANGUP_URL   = 'https://internal-test-xml.vobiz.ai/hangup';
const TRANSFER_URL = 'https://internal-test-xml.vobiz.ai/answer';
const STEP_DELAY   = 5000;

if (!AUTH_ID || !AUTH_TOKEN || !FROM_NUMBER || !TO_NUMBER) {
    console.log('SKIP: VOBIZ_AUTH_ID, VOBIZ_AUTH_TOKEN, VOBIZ_FROM_NUMBER, VOBIZ_TO_NUMBER required');
    process.exit(0);
}

const HEADERS = { 'X-Auth-ID': AUTH_ID, 'X-Auth-Token': AUTH_TOKEN };
const sleep   = (ms: number) => new Promise(resolve => setTimeout(resolve, ms));

let passed = 0;
let failed = 0;

async function step(name: string, fn: () => Promise<void>) {
    try {
        await fn();
        console.log(`[Node] PASS: ${name}`);
        passed++;
    } catch (err: any) {
        const msg = err?.response?.statusCode
            ? `HTTP ${err.response.statusCode}`
            : err?.message || String(err);
        console.log(`[Node] FAIL: ${name} - ${msg}`);
        failed++;
    }
}

async function main() {
    const api = new CallApi();

    // STEP 1: Make outbound call
    console.log('\n[Node] STEP 1: Making outbound call...');
    let requestUUID = '';
    await step('Make Call', async () => {
        const res = await api.apiV1AccountAuthIdCallPost(
            AUTH_ID, AUTH_ID, AUTH_TOKEN, 'application/json',
            {
                from:           FROM_NUMBER,
                to:             TO_NUMBER,
                answer_url:     ANSWER_URL,
                answer_method:  'POST',
                hangup_url:     HANGUP_URL,
                hangup_method:  'POST',
            },
            { headers: HEADERS }
        );
        const body = res.body as any;
        requestUUID = body?.request_uuid || body?.objects?.[0]?.request_uuid || '';
        if (!requestUUID) throw new Error(`No request_uuid in response: ${JSON.stringify(body)}`);
        console.log(`  -> request_uuid = ${requestUUID}`);
    });

    if (!requestUUID) {
        console.log('[Node] Cannot continue without request_uuid');
        process.exit(1);
    }
    await sleep(STEP_DELAY);

    // STEP 2: List all live calls
    console.log('[Node] STEP 2: Listing all live calls...');
    await step('List Live Calls', async () => {
        const res = await api.apiV1AccountAuthIdCallGet(
            AUTH_ID, AUTH_ID, AUTH_TOKEN, 'application/json', 'live',
            { headers: HEADERS }
        );
        console.log(`  -> HTTP ${res.response.statusCode}`);
    });
    await sleep(STEP_DELAY);

    // STEP 3: Get single live call
    console.log('[Node] STEP 3: Retrieving single live call...');
    await step('Get Single Live Call', async () => {
        const res = await api.apiV1AccountAuthIdCallGet_1(
            AUTH_ID, AUTH_ID, AUTH_TOKEN, 'application/json', 'live',
            { headers: HEADERS }
        );
        console.log(`  -> HTTP ${res.response.statusCode}`);
    });
    await sleep(STEP_DELAY);

    // STEP 4: Speak TTS
    console.log('[Node] STEP 4: Speaking TTS...');
    await step('Speak TTS', async () => {
        const res = await api.apiV1AccountAuthIdCallSpeakPost(
            AUTH_ID, AUTH_ID, AUTH_TOKEN, 'application/json',
            { text: 'Hello from Vobiz Node SDK.', voice: 'WOMAN', language: 'en-US', legs: 'aleg' },
            { headers: HEADERS }
        );
        console.log(`  -> HTTP ${res.response.statusCode}`);
    });
    await sleep(STEP_DELAY);

    // STEP 5: Stop TTS
    console.log('[Node] STEP 5: Stopping TTS...');
    await step('Stop TTS', async () => {
        const res = await api.apiV1AccountAuthIdCallSpeakDelete(
            AUTH_ID, AUTH_ID, AUTH_TOKEN, 'application/json',
            { headers: HEADERS }
        );
        console.log(`  -> HTTP ${res.response.statusCode}`);
    });

    // STEP 6: Play audio
    console.log('[Node] STEP 6: Playing audio...');
    await step('Play Audio', async () => {
        const res = await api.apiV1AccountAuthIdCallPlayPost(
            AUTH_ID, AUTH_ID, AUTH_TOKEN, 'application/json',
            { urls: [AUDIO_URL], legs: 'aleg', loop: false, mix: true },
            { headers: HEADERS }
        );
        console.log(`  -> HTTP ${res.response.statusCode}`);
    });
    await sleep(STEP_DELAY);

    // STEP 7: Stop audio
    console.log('[Node] STEP 7: Stopping audio...');
    await step('Stop Audio', async () => {
        const res = await api.apiV1AccountAuthIdCallPlayDelete(
            AUTH_ID, AUTH_ID, AUTH_TOKEN, 'application/json',
            { headers: HEADERS }
        );
        console.log(`  -> HTTP ${res.response.statusCode}`);
    });

    // STEP 8: Start recording
    console.log('[Node] STEP 8: Starting recording...');
    await step('Start Recording', async () => {
        const res = await api.apiV1AccountAuthIdCallRecordPost(
            AUTH_ID, AUTH_ID, AUTH_TOKEN, 'application/json',
            { time_limit: 60, file_format: 'mp3' },
            { headers: HEADERS }
        );
        console.log(`  -> HTTP ${res.response.statusCode}`);
    });
    await sleep(STEP_DELAY);

    // STEP 9: Send DTMF
    console.log('[Node] STEP 9: Sending DTMF...');
    await step('Send DTMF', async () => {
        const res = await api.apiV1AccountAuthIdCallDTMFPost(
            AUTH_ID, AUTH_ID, AUTH_TOKEN, 'application/json',
            { digits: '1234', leg: 'aleg' },
            { headers: HEADERS }
        );
        console.log(`  -> HTTP ${res.response.statusCode}`);
    });

    // STEP 10: Stop recording
    console.log('[Node] STEP 10: Stopping recording...');
    await step('Stop Recording', async () => {
        const res = await api.apiV1AccountAuthIdCallRecordDelete(
            AUTH_ID, AUTH_ID, AUTH_TOKEN, 'application/json',
            { headers: HEADERS }
        );
        console.log(`  -> HTTP ${res.response.statusCode}`);
    });

    // STEP 11: Transfer call
    console.log('[Node] STEP 11: Transferring call...');
    await step('Transfer Call', async () => {
        const transferTo = TRANSFER_URL + (TRANSFER_NUMBER ? `?to=${TRANSFER_NUMBER}` : '');
        const res = await api.apiV1AccountAuthIdCallPost_2(
            AUTH_ID, AUTH_ID, AUTH_TOKEN, 'application/json',
            { legs: 'aleg', aleg_url: transferTo, aleg_method: 'POST' },
            { headers: HEADERS }
        );
        console.log(`  -> HTTP ${res.response.statusCode}`);
    });
    await sleep(STEP_DELAY);

    // STEP 12: Hang up
    console.log('[Node] STEP 12: Hanging up call...');
    await step('Hang Up Call', async () => {
        const res = await api.apiV1AccountAuthIdCallDelete(
            AUTH_ID, AUTH_ID, AUTH_TOKEN, 'application/json',
            { headers: HEADERS }
        );
        console.log(`  -> HTTP ${res.response.statusCode}`);
    });

    console.log(`\n[Node] Call flow COMPLETE: ${passed} passed, ${failed} failed`);
    process.exit(failed > 0 ? 1 : 0);
}

main();
