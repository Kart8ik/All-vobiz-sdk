"""
Vobiz Python SDK - Full Call Flow Integration Test
Steps: Make Call → List Live → Get Live → Speak → Stop Speak → Play → Stop Play
       → Start Record → DTMF → Stop Record → Transfer → Hang Up
"""

import os
import time
import json
import pytest
import vobiz
from vobiz.api.call_api import CallApi

AUDIO_URL    = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3"
ANSWER_URL   = "https://internal-test-xml.vobiz.ai/answer"
HANGUP_URL   = "https://internal-test-xml.vobiz.ai/hangup"
TRANSFER_URL = "https://internal-test-xml.vobiz.ai/answer"
STEP_DELAY   = 5

AUTH_ID          = os.environ.get("VOBIZ_AUTH_ID", "")
AUTH_TOKEN       = os.environ.get("VOBIZ_AUTH_TOKEN", "")
FROM_NUMBER      = os.environ.get("VOBIZ_FROM_NUMBER", "")
TO_NUMBER        = os.environ.get("VOBIZ_TO_NUMBER", "")
TRANSFER_NUMBER  = os.environ.get("VOBIZ_TRANSFER_NUMBER", "")


@pytest.fixture(scope="module")
def call_api():
    if not all([AUTH_ID, AUTH_TOKEN, FROM_NUMBER, TO_NUMBER]):
        pytest.skip("VOBIZ_AUTH_ID, VOBIZ_AUTH_TOKEN, VOBIZ_FROM_NUMBER, VOBIZ_TO_NUMBER required")
    configuration = vobiz.Configuration(host="https://api.vobiz.ai")
    configuration.api_key["X-Auth-ID"]    = AUTH_ID
    configuration.api_key["X-Auth-Token"] = AUTH_TOKEN
    with vobiz.ApiClient(configuration) as client:
        yield CallApi(client)


@pytest.fixture(scope="module")
def call_uuid(call_api):
    """Make the call and return request_uuid for all subsequent steps."""
    print("\n[Python] STEP 1: Making outbound call...")
    resp = call_api.api_v1_account_auth_id_call_post(
        auth_id=AUTH_ID,
        x_auth_id=AUTH_ID,
        x_auth_token=AUTH_TOKEN,
        body={
            "from":          FROM_NUMBER,
            "to":            TO_NUMBER,
            "answer_url":    ANSWER_URL,
            "answer_method": "POST",
            "hangup_url":    HANGUP_URL,
            "hangup_method": "POST",
        }
    )
    # Extract request_uuid — response may be dict or object
    uuid = None
    if isinstance(resp, dict):
        uuid = resp.get("request_uuid")
        if not uuid and "objects" in resp:
            uuid = resp["objects"][0].get("request_uuid")
    else:
        try:
            uuid = resp.request_uuid
        except AttributeError:
            pass
    assert uuid, f"Could not extract request_uuid from response: {resp}"
    print(f"[Python] STEP 1 PASS: request_uuid = {uuid}")
    time.sleep(STEP_DELAY)
    return uuid


def test_step2_list_live_calls(call_api, call_uuid):
    print("[Python] STEP 2: Listing all live calls...")
    call_api.api_v1_account_auth_id_call_get(
        auth_id=AUTH_ID,
        x_auth_id=AUTH_ID,
        x_auth_token=AUTH_TOKEN,
        status="live"
    )
    print("[Python] STEP 2 PASS: List live calls OK")
    time.sleep(STEP_DELAY)


def test_step3_get_single_live_call(call_api, call_uuid):
    print("[Python] STEP 3: Retrieving single live call...")
    call_api.api_v1_account_auth_id_call_get_0(
        auth_id=AUTH_ID,
        x_auth_id=AUTH_ID,
        x_auth_token=AUTH_TOKEN,
        status="live"
    )
    print("[Python] STEP 3 PASS: Get single live call OK")
    time.sleep(STEP_DELAY)


def test_step4_speak_tts(call_api, call_uuid):
    print("[Python] STEP 4: Speaking TTS on call...")
    call_api.api_v1_account_auth_id_call_speak_post(
        auth_id=AUTH_ID,
        x_auth_id=AUTH_ID,
        x_auth_token=AUTH_TOKEN,
        body={
            "text":     "Hello, this is a test call from Vobiz Python SDK.",
            "voice":    "WOMAN",
            "language": "en-US",
            "legs":     "aleg",
        }
    )
    print("[Python] STEP 4 PASS: Speak TTS OK")
    time.sleep(STEP_DELAY)


def test_step5_stop_tts(call_api, call_uuid):
    print("[Python] STEP 5: Stopping TTS...")
    call_api.api_v1_account_auth_id_call_speak_delete(
        auth_id=AUTH_ID,
        x_auth_id=AUTH_ID,
        x_auth_token=AUTH_TOKEN
    )
    print("[Python] STEP 5 PASS: Stop TTS OK")


def test_step6_play_audio(call_api, call_uuid):
    print("[Python] STEP 6: Playing audio...")
    call_api.api_v1_account_auth_id_call_play_post(
        auth_id=AUTH_ID,
        x_auth_id=AUTH_ID,
        x_auth_token=AUTH_TOKEN,
        body={
            "urls": [AUDIO_URL],
            "legs": "aleg",
            "loop": False,
            "mix":  True,
        }
    )
    print("[Python] STEP 6 PASS: Play audio OK")
    time.sleep(STEP_DELAY)


def test_step7_stop_audio(call_api, call_uuid):
    print("[Python] STEP 7: Stopping audio...")
    call_api.api_v1_account_auth_id_call_play_delete(
        auth_id=AUTH_ID,
        x_auth_id=AUTH_ID,
        x_auth_token=AUTH_TOKEN
    )
    print("[Python] STEP 7 PASS: Stop audio OK")


def test_step8_start_recording(call_api, call_uuid):
    print("[Python] STEP 8: Starting recording...")
    call_api.api_v1_account_auth_id_call_record_post(
        auth_id=AUTH_ID,
        x_auth_id=AUTH_ID,
        x_auth_token=AUTH_TOKEN,
        body={"time_limit": 60, "file_format": "mp3"}
    )
    print("[Python] STEP 8 PASS: Start recording OK")
    time.sleep(STEP_DELAY)


def test_step9_dtmf(call_api, call_uuid):
    print("[Python] STEP 9: Sending DTMF...")
    call_api.api_v1_account_auth_id_call_dtmf_post(
        auth_id=AUTH_ID,
        x_auth_id=AUTH_ID,
        x_auth_token=AUTH_TOKEN,
        body={"digits": "1234", "leg": "aleg"}
    )
    print("[Python] STEP 9 PASS: DTMF OK")


def test_step10_stop_recording(call_api, call_uuid):
    print("[Python] STEP 10: Stopping recording...")
    call_api.api_v1_account_auth_id_call_record_delete(
        auth_id=AUTH_ID,
        x_auth_id=AUTH_ID,
        x_auth_token=AUTH_TOKEN
    )
    print("[Python] STEP 10 PASS: Stop recording OK")


def test_step11_transfer_call(call_api, call_uuid):
    print("[Python] STEP 11: Transferring call...")
    transfer_to = TRANSFER_URL + ("?to=" + TRANSFER_NUMBER if TRANSFER_NUMBER else "")
    call_api.api_v1_account_auth_id_call_post_0(
        auth_id=AUTH_ID,
        x_auth_id=AUTH_ID,
        x_auth_token=AUTH_TOKEN,
        body={
            "legs":        "aleg",
            "aleg_url":    transfer_to,
            "aleg_method": "POST",
        }
    )
    print("[Python] STEP 11 PASS: Transfer OK")
    time.sleep(STEP_DELAY)


def test_step12_hangup(call_api, call_uuid):
    print("[Python] STEP 12: Hanging up call...")
    call_api.api_v1_account_auth_id_call_delete(
        auth_id=AUTH_ID,
        x_auth_id=AUTH_ID,
        x_auth_token=AUTH_TOKEN
    )
    print("[Python] STEP 12 PASS: Hang up OK")
    print("\n[Python] Call flow test COMPLETE")
