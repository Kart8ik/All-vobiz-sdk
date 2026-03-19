import 'dart:io';
import 'package:vobiz/api.dart';

/// Vobiz Dart SDK - Integration Tests (Read-Only)
/// Run: dart test/integration/vobiz_integration_test.dart
/// Requires: VOBIZ_AUTH_ID and VOBIZ_AUTH_TOKEN env vars
Future<void> main() async {
  final authId    = Platform.environment['VOBIZ_AUTH_ID'] ?? '';
  final authToken = Platform.environment['VOBIZ_AUTH_TOKEN'] ?? '';

  if (authId.isEmpty || authToken.isEmpty) {
    print('SKIP: VOBIZ_AUTH_ID or VOBIZ_AUTH_TOKEN not set');
    exit(0);
  }

  final client = ApiClient(basePath: 'https://api.vobiz.ai');
  client.addDefaultHeader('X-Auth-ID', authId);
  client.addDefaultHeader('X-Auth-Token', authToken);

  int passed = 0;
  int failed = 0;

  Future<void> runTest(String name, Future<void> Function() fn) async {
    try {
      await fn();
      print('[Dart] PASS: $name');
      passed++;
    } catch (e) {
      print('[Dart] FAIL: $name - $e');
      failed++;
    }
  }

  final accountApi    = AccountApi(client);
  final callApi       = CallApi(client);
  final recordingApi  = RecordingApi(client);
  final conferenceApi = ConferenceApi(client);
  final appApi        = ApplicationApi(client);

  await runTest('GetAccountDetails', () async {
    final result = await accountApi.apiV1AuthMeGet();
    print('  -> Response received OK');
  });

  await runTest('GetLiveCalls', () async {
    final result = await callApi.apiV1AccountAuthIdCallGet(authId, status: 'live');
    print('  -> Response received OK');
  });

  await runTest('ListRecordings', () async {
    final result = await recordingApi.apiV1AccountAccountIdRecordingGet(authId);
    print('  -> Response received OK');
  });

  await runTest('ListConferences', () async {
    final result = await conferenceApi.apiV1AccountAuthIdConferenceGet(authId);
    print('  -> Response received OK');
  });

  await runTest('ListApplications', () async {
    final result = await appApi.apiV1AccountAuthIdApplicationGet(authId);
    print('  -> Response received OK');
  });

  print('\n[Dart] Results: $passed passed, $failed failed');
  exit(failed > 0 ? 1 : 0);
}
