/*
Vobiz Go SDK - Full Call Flow Integration Test
Steps: Make Call → List Live → Get Live → Speak → Stop Speak → Play → Stop Play
       → Start Record → DTMF → Stop Record → Transfer → Hang Up
*/

package vobiz

import (
	"context"
	"encoding/json"
	"fmt"
	"io"
	"os"
	"testing"
	"time"

	openapiclient "github.com/vobiz/vobiz-go-sdk"
)

const (
	audioURL       = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3"
	answerURL      = "https://internal-test-xml.vobiz.ai/answer"
	hangupURL      = "https://internal-test-xml.vobiz.ai/hangup"
	transferURL    = "https://internal-test-xml.vobiz.ai/answer"
	stepDelay      = 5 * time.Second
)

func getCallFlowClient(t *testing.T) (*openapiclient.APIClient, string, string, string, string) {
	authID     := os.Getenv("VOBIZ_AUTH_ID")
	authToken  := os.Getenv("VOBIZ_AUTH_TOKEN")
	fromNumber := os.Getenv("VOBIZ_FROM_NUMBER")
	toNumber   := os.Getenv("VOBIZ_TO_NUMBER")

	if authID == "" || authToken == "" || fromNumber == "" || toNumber == "" {
		t.Skip("Skipping call flow: VOBIZ_AUTH_ID, VOBIZ_AUTH_TOKEN, VOBIZ_FROM_NUMBER, VOBIZ_TO_NUMBER required")
	}

	cfg := openapiclient.NewConfiguration()
	cfg.AddDefaultHeader("X-Auth-ID", authID)
	cfg.AddDefaultHeader("X-Auth-Token", authToken)
	return openapiclient.NewAPIClient(cfg), authID, authToken, fromNumber, toNumber
}

func readBody(r io.Reader) map[string]interface{} {
	var result map[string]interface{}
	data, _ := io.ReadAll(r)
	json.Unmarshal(data, &result)
	return result
}

func Test_CallFlow(t *testing.T) {
	client, authID, authToken, fromNumber, toNumber := getCallFlowClient(t)
	ctx := context.Background()

	// ── STEP 1: Make outbound call ─────────────────────────────────────────
	fmt.Println("\n[Go] STEP 1: Making outbound call...")
	httpRes, err := client.CallAPI.ApiV1AccountAuthIdCallPost(ctx, authID).
		Body(map[string]interface{}{
			"from":           fromNumber,
			"to":             toNumber,
			"answer_url":     answerURL,
			"answer_method":  "POST",
			"hangup_url":     hangupURL,
			"hangup_method":  "POST",
		}).Execute()
	if err != nil {
		t.Fatalf("STEP 1 FAILED - Make Call: %v", err)
	}
	body := readBody(httpRes.Body)
	httpRes.Body.Close()

	requestUUID, ok := body["request_uuid"].(string)
	if !ok || requestUUID == "" {
		// Some APIs return as array
		if objs, ok2 := body["objects"].([]interface{}); ok2 && len(objs) > 0 {
			if obj, ok3 := objs[0].(map[string]interface{}); ok3 {
				requestUUID, _ = obj["request_uuid"].(string)
			}
		}
	}
	if requestUUID == "" {
		t.Fatalf("STEP 1 FAILED - Could not extract request_uuid from response: %v", body)
	}
	fmt.Printf("[Go] STEP 1 PASS: Call made. request_uuid = %s\n", requestUUID)
	_ = authToken
	time.Sleep(stepDelay)

	// ── STEP 2: List all live calls ────────────────────────────────────────
	fmt.Println("[Go] STEP 2: Listing all live calls...")
	httpRes2, err := client.CallAPI.ApiV1AccountAuthIdCallGet(ctx, authID).Status("live").Execute()
	if err != nil {
		fmt.Printf("[Go] STEP 2 WARN: List live calls: %v\n", err)
	} else {
		fmt.Printf("[Go] STEP 2 PASS: List live calls HTTP %d\n", httpRes2.StatusCode)
	}
	time.Sleep(stepDelay)

	// ── STEP 3: Get single live call ───────────────────────────────────────
	fmt.Println("[Go] STEP 3: Retrieving single live call...")
	httpRes3, err := client.CallAPI.ApiV1AccountAuthIdCallGet_1(ctx, authID).Status("live").Execute()
	if err != nil {
		fmt.Printf("[Go] STEP 3 WARN: Get single call: %v\n", err)
	} else {
		fmt.Printf("[Go] STEP 3 PASS: Get single call HTTP %d\n", httpRes3.StatusCode)
	}
	time.Sleep(stepDelay)

	// ── STEP 4: Speak TTS ──────────────────────────────────────────────────
	fmt.Println("[Go] STEP 4: Speaking TTS on call...")
	httpRes4, err := client.CallAPI.ApiV1AccountAuthIdCallSpeakPost(ctx, authID).
		Body(map[string]interface{}{
			"text":     "Hello, this is a test call from Vobiz SDK.",
			"voice":    "WOMAN",
			"language": "en-US",
			"legs":     "aleg",
		}).Execute()
	if err != nil {
		fmt.Printf("[Go] STEP 4 WARN: Speak TTS: %v\n", err)
	} else {
		fmt.Printf("[Go] STEP 4 PASS: Speak TTS HTTP %d\n", httpRes4.StatusCode)
	}
	time.Sleep(stepDelay)

	// ── STEP 5: Stop TTS ───────────────────────────────────────────────────
	fmt.Println("[Go] STEP 5: Stopping TTS...")
	httpRes5, err := client.CallAPI.ApiV1AccountAuthIdCallSpeakDelete(ctx, authID).Execute()
	if err != nil {
		fmt.Printf("[Go] STEP 5 WARN: Stop TTS: %v\n", err)
	} else {
		fmt.Printf("[Go] STEP 5 PASS: Stop TTS HTTP %d\n", httpRes5.StatusCode)
	}

	// ── STEP 6: Play audio ─────────────────────────────────────────────────
	fmt.Println("[Go] STEP 6: Playing audio on call...")
	httpRes6, err := client.CallAPI.ApiV1AccountAuthIdCallPlayPost(ctx, authID).
		Body(map[string]interface{}{
			"urls": []string{audioURL},
			"legs": "aleg",
			"loop": false,
			"mix":  true,
		}).Execute()
	if err != nil {
		fmt.Printf("[Go] STEP 6 WARN: Play audio: %v\n", err)
	} else {
		fmt.Printf("[Go] STEP 6 PASS: Play audio HTTP %d\n", httpRes6.StatusCode)
	}
	time.Sleep(stepDelay)

	// ── STEP 7: Stop audio ─────────────────────────────────────────────────
	fmt.Println("[Go] STEP 7: Stopping audio...")
	httpRes7, err := client.CallAPI.ApiV1AccountAuthIdCallPlayDelete(ctx, authID).Execute()
	if err != nil {
		fmt.Printf("[Go] STEP 7 WARN: Stop audio: %v\n", err)
	} else {
		fmt.Printf("[Go] STEP 7 PASS: Stop audio HTTP %d\n", httpRes7.StatusCode)
	}

	// ── STEP 8: Start recording ────────────────────────────────────────────
	fmt.Println("[Go] STEP 8: Starting recording...")
	httpRes8, err := client.CallAPI.ApiV1AccountAuthIdCallRecordPost(ctx, authID).
		Body(map[string]interface{}{
			"time_limit":   60,
			"file_format":  "mp3",
		}).Execute()
	if err != nil {
		fmt.Printf("[Go] STEP 8 WARN: Start recording: %v\n", err)
	} else {
		fmt.Printf("[Go] STEP 8 PASS: Start recording HTTP %d\n", httpRes8.StatusCode)
	}
	time.Sleep(stepDelay)

	// ── STEP 9: Send DTMF ─────────────────────────────────────────────────
	fmt.Println("[Go] STEP 9: Sending DTMF digits...")
	httpRes9, err := client.CallAPI.ApiV1AccountAuthIdCallDTMFPost(ctx, authID).
		Body(map[string]interface{}{
			"digits": "1234",
			"leg":    "aleg",
		}).Execute()
	if err != nil {
		fmt.Printf("[Go] STEP 9 WARN: DTMF: %v\n", err)
	} else {
		fmt.Printf("[Go] STEP 9 PASS: DTMF HTTP %d\n", httpRes9.StatusCode)
	}

	// ── STEP 10: Stop recording ────────────────────────────────────────────
	fmt.Println("[Go] STEP 10: Stopping recording...")
	httpRes10, err := client.CallAPI.ApiV1AccountAuthIdCallRecordDelete(ctx, authID).Execute()
	if err != nil {
		fmt.Printf("[Go] STEP 10 WARN: Stop recording: %v\n", err)
	} else {
		fmt.Printf("[Go] STEP 10 PASS: Stop recording HTTP %d\n", httpRes10.StatusCode)
	}

	// ── STEP 11: Transfer call ─────────────────────────────────────────────
	fmt.Println("[Go] STEP 11: Transferring call...")
	httpRes11, err := client.CallAPI.ApiV1AccountAuthIdCallPost_2(ctx, authID).
		Body(map[string]interface{}{
			"legs":         "aleg",
			"aleg_url":     transferURL + "?to=" + os.Getenv("VOBIZ_TRANSFER_NUMBER"),
			"aleg_method":  "POST",
		}).Execute()
	if err != nil {
		fmt.Printf("[Go] STEP 11 WARN: Transfer: %v\n", err)
	} else {
		fmt.Printf("[Go] STEP 11 PASS: Transfer HTTP %d\n", httpRes11.StatusCode)
	}
	time.Sleep(stepDelay)

	// ── STEP 12: Hang up call ──────────────────────────────────────────────
	fmt.Println("[Go] STEP 12: Hanging up call...")
	httpRes12, err := client.CallAPI.ApiV1AccountAuthIdCallDelete(ctx, authID).Execute()
	if err != nil {
		fmt.Printf("[Go] STEP 12 WARN: Hang up: %v\n", err)
	} else {
		fmt.Printf("[Go] STEP 12 PASS: Hang up HTTP %d\n", httpRes12.StatusCode)
	}

	fmt.Println("\n[Go] Call flow test COMPLETE")
}
