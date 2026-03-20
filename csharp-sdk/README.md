# Vobiz C# SDK

Official C# SDK for the Vobiz voice and telephony platform.

This SDK includes:
- Generated REST API clients for Account, Calls, Conferences, Recordings, Phone Numbers, Applications, Endpoints, Trunks, Sub-Accounts, and CDR.
- A VobizXML builder module under `Vobiz.Xml` for creating call-control XML responses.
- Unit tests and integration call-flow automation.

Base URL: `https://api.vobiz.ai`

Authentication: `X-Auth-ID` and `X-Auth-Token` headers.

---

## Table of Contents

- [Requirements](#requirements)
- [Project Structure](#project-structure)
- [Installation](#installation)
- [Authentication](#authentication)
- [Quick Start](#quick-start)
- [Resource APIs](#resource-apis)
- [VobizXML](#vobizxml)
- [Environment Variables](#environment-variables)
- [Running Tests](#running-tests)
- [CI Automation](#ci-automation)
- [Build Notes](#build-notes)

---

## Requirements

- .NET SDK 8.0+
- C# 12 compatible toolchain (included in .NET 8 SDK)

---

## Project Structure

- `src/Vobiz` — main SDK library
- `src/Vobiz.Test` — unit tests (xUnit)
- `src/Vobiz.IntegrationTests` — live integration call-flow runner
- `src/Vobiz/Xml` — VobizXML builder implementation

---

## Installation

### From source (recommended for this repo)

```bash
git clone https://github.com/Kart8ik/All-vobiz-sdk.git
cd All-vobiz-sdk/csharp-sdk
dotnet restore
dotnet build --configuration Release
```

### Add as a project reference

```bash
dotnet add <your-app>.csproj reference src/Vobiz/Vobiz.csproj
```

### If published to NuGet (optional)

```bash
dotnet add package Vobiz
```

---

## Authentication

The SDK uses API key headers:
- `X-Auth-ID`
- `X-Auth-Token`

Configure tokens through DI:

```csharp
using Microsoft.Extensions.Hosting;
using Vobiz.Client;
using Vobiz.Extensions;

var authId = Environment.GetEnvironmentVariable("VOBIZ_AUTH_ID") ?? "";
var authToken = Environment.GetEnvironmentVariable("VOBIZ_AUTH_TOKEN") ?? "";

var host = Host.CreateDefaultBuilder(args)
	.ConfigureApi((context, services, options) =>
	{
		options.AddTokens(new ApiKeyToken(authId, ClientUtils.ApiKeyHeader.X_Auth_ID, prefix: ""));
		options.AddTokens(new ApiKeyToken(authToken, ClientUtils.ApiKeyHeader.X_Auth_Token, prefix: ""));
	})
	.Build();
```

> Use `prefix: ""` for Vobiz header tokens so raw values are sent.

---

## Quick Start

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vobiz.Api;
using Vobiz.Client;
using Vobiz.Extensions;

var authId = Environment.GetEnvironmentVariable("VOBIZ_AUTH_ID") ?? "";
var authToken = Environment.GetEnvironmentVariable("VOBIZ_AUTH_TOKEN") ?? "";

var host = Host.CreateDefaultBuilder(args)
	.ConfigureApi((context, services, options) =>
	{
		options.AddTokens(new ApiKeyToken(authId, ClientUtils.ApiKeyHeader.X_Auth_ID, prefix: ""));
		options.AddTokens(new ApiKeyToken(authToken, ClientUtils.ApiKeyHeader.X_Auth_Token, prefix: ""));
	})
	.Build();

var accountApi = host.Services.GetRequiredService<IAccountApi>();
var me = await accountApi.ApiV1AuthMeGetAsync(
	xAuthID: authId,
	xAuthToken: authToken,
	contentType: "application/json");

Console.WriteLine(me.StatusCode);
```

---

## Resource APIs

The following interfaces are available from DI:

- `IAccountApi`
- `IApplicationApi`
- `ICallApi`
- `ICDRApi`
- `IConferenceApi`
- `IEndpointApi`
- `IPhoneNumbersApi`
- `IRecordingApi`
- `ISubAccountApi`
- `ITrunkApi`

Optional parameters use `Option<T>` and support implicit conversion from raw values.

Example (list queued calls):

```csharp
var callApi = host.Services.GetRequiredService<ICallApi>();
var response = await callApi.ApiV1AccountAuthIdCallGetAsync(
	authId: authId,
	xAuthID: authId,
	xAuthToken: authToken,
	contentType: "application/json",
	status: "queued");

Console.WriteLine(response.StatusCode);
```

---

## VobizXML

The SDK includes a fluent XML builder under `Vobiz.Xml`.

### Build a basic response

```csharp
using Vobiz.Xml;

var response = new ResponseElement()
	.AddSpeak("Welcome to Vobiz.", voice: "WOMAN", language: "en-US")
	.AddWait(length: 2)
	.AddHangup();

var xml = response.ToString(pretty: true);
Console.WriteLine(xml);
```

### Dial with nested Number

```csharp
using Vobiz.Xml;

var dial = new DialElement(timeLimit: 30).AddNumber("+12025550123");
var response = new ResponseElement().AddDial(dial);
Console.WriteLine(response.ToString(false));
```

### Gather with nested prompts

```csharp
using Vobiz.Xml;

var gather = new GatherElement(
		action: "https://example.com/gather",
		method: "POST",
		inputType: "speech",
		executionTimeout: 15)
	.AddSpeak("Say sales or support")
	.AddPlay("https://example.com/menu.mp3");

var response = new ResponseElement().Add(gather);
Console.WriteLine(response.ToString(false));
```

Supported top-level response elements include `Conference`, `Dial`, `DTMF`, `AudioStream`, `Gather`, `Hangup`, `Message`, `Play`, `PreAnswer`, `Record`, `Redirect`, `Speak`, `Wait`, `MultiPartyCall`, and `Stream`.

---

## Environment Variables

| Variable | Required | Purpose |
|---|---|---|
| `VOBIZ_AUTH_ID` | Yes (for live API) | Account auth id |
| `VOBIZ_AUTH_TOKEN` | Yes (for live API) | Account auth token |
| `VOBIZ_FROM_NUMBER` | For integration call-flow | Outbound source number |
| `VOBIZ_TO_NUMBER` | For integration call-flow | Outbound destination number |
| `VOBIZ_TRANSFER_NUMBER` | Optional | Transfer destination |

---

## Running Tests

From `csharp-sdk` directory:

### Unit tests

```bash
dotnet test --configuration Release
```

Includes XML tests in `src/Vobiz.Test/Xml`.

### Integration call-flow runner

```bash
dotnet run --project src/Vobiz.IntegrationTests/Vobiz.IntegrationTests.csproj --configuration Release
```

If required `VOBIZ_*` variables are missing, the integration runner prints a skip message and exits cleanly.

---

## CI Automation

### Dedicated C# workflow

- `csharp-sdk/.github/workflows/csharp.yml`
- Runs on `push`, `pull_request`, and manual dispatch.
- Executes restore/build/unit tests.
- Runs integration call-flow only when required `VOBIZ_*` secrets are present.

### Monorepo workflow

- `.github/workflows/test-csharp-sdk.yml`
- `.github/workflows/test-all-sdks.yml` (C# job section)

Both are configured to avoid false failures due to missing live credentials by skipping integration call-flow when secrets are not configured.

---

## Build Notes

- SDK source is generated from OpenAPI and then extended manually (including XML support and tests).
- API test stubs under `src/Vobiz.Test/Api` are scaffolded and many are intentionally marked `Skip = "not implemented"`.
- XML feature tests are fully active and intended as a reliable functional test surface.