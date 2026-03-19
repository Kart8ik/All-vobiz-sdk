/*
Vobiz API - Integration Tests (Read-Only)

Tests real API calls using VOBIZ_AUTH_ID and VOBIZ_AUTH_TOKEN env vars.
Only GET endpoints are called - no side effects.
*/

package vobiz

import (
	"context"
	"fmt"
	"os"
	"testing"

	openapiclient "github.com/vobiz/vobiz-go-sdk"
)

func getIntegrationClient(t *testing.T) *openapiclient.APIClient {
	authID := os.Getenv("VOBIZ_AUTH_ID")
	authToken := os.Getenv("VOBIZ_AUTH_TOKEN")
	if authID == "" || authToken == "" {
		t.Skip("Skipping integration test: VOBIZ_AUTH_ID or VOBIZ_AUTH_TOKEN not set")
	}

	cfg := openapiclient.NewConfiguration()
	cfg.AddDefaultHeader("X-Auth-ID", authID)
	cfg.AddDefaultHeader("X-Auth-Token", authToken)
	return openapiclient.NewAPIClient(cfg)
}

func getAuthID(t *testing.T) string {
	authID := os.Getenv("VOBIZ_AUTH_ID")
	if authID == "" {
		t.Skip("Skipping: VOBIZ_AUTH_ID not set")
	}
	return authID
}

func Test_Integration_GetAccountDetails(t *testing.T) {
	client := getIntegrationClient(t)

	resp, httpRes, err := client.AccountAPI.ApiV1AuthMeGet(context.Background()).Execute()
	if err != nil {
		t.Logf("Response body: %v", resp)
		t.Fatalf("GetAccountDetails failed: %v (HTTP %d)", err, httpRes.StatusCode)
	}
	fmt.Printf("[Go] GetAccountDetails: HTTP %d OK\n", httpRes.StatusCode)
}

func Test_Integration_GetLiveCalls(t *testing.T) {
	client := getIntegrationClient(t)
	authID := getAuthID(t)

	resp, httpRes, err := client.CallAPI.ApiV1AccountAuthIdCallGet(context.Background(), authID).Execute()
	if err != nil {
		t.Logf("Response body: %v", resp)
		t.Fatalf("GetLiveCalls failed: %v (HTTP %d)", err, httpRes.StatusCode)
	}
	fmt.Printf("[Go] GetLiveCalls: HTTP %d OK\n", httpRes.StatusCode)
}

func Test_Integration_ListRecordings(t *testing.T) {
	client := getIntegrationClient(t)
	authID := getAuthID(t)

	resp, httpRes, err := client.RecordingAPI.ApiV1AccountAccountIdRecordingGet(context.Background(), authID).Execute()
	if err != nil {
		t.Logf("Response body: %v", resp)
		t.Fatalf("ListRecordings failed: %v (HTTP %d)", err, httpRes.StatusCode)
	}
	fmt.Printf("[Go] ListRecordings: HTTP %d OK\n", httpRes.StatusCode)
}

func Test_Integration_ListConferences(t *testing.T) {
	client := getIntegrationClient(t)
	authID := getAuthID(t)

	resp, httpRes, err := client.ConferenceAPI.ApiV1AccountAuthIdConferenceGet(context.Background(), authID).Execute()
	if err != nil {
		t.Logf("Response body: %v", resp)
		t.Fatalf("ListConferences failed: %v (HTTP %d)", err, httpRes.StatusCode)
	}
	fmt.Printf("[Go] ListConferences: HTTP %d OK\n", httpRes.StatusCode)
}

func Test_Integration_ListApplications(t *testing.T) {
	client := getIntegrationClient(t)
	authID := getAuthID(t)

	resp, httpRes, err := client.ApplicationAPI.ApiV1AccountAuthIdApplicationGet(context.Background(), authID).Execute()
	if err != nil {
		t.Logf("Response body: %v", resp)
		t.Fatalf("ListApplications failed: %v (HTTP %d)", err, httpRes.StatusCode)
	}
	fmt.Printf("[Go] ListApplications: HTTP %d OK\n", httpRes.StatusCode)
}
