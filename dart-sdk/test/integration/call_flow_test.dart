import 'dart:io';
import 'dart:convert';
import 'package:vobiz/api.dart';

/// Vobiz Dart SDK - Full Call Flow Integration Test
Future<void> main() async {
  final authId         = Platform.environment['VOBIZ_AUTH_ID']         ?? '';
  final authToken      = Platform.environment['VOBIZ_AUTH_TOKEN']       ?? '';
  final fromNumber     = Platform.environment['VOBIZ_FROM_NUMBER']      ?? '';
  final toNumber       = Platform.environment['VOBIZ_TO_NUMBER']        ?? '';
  final transferNumber = Platform.environment['VOBIZ_TRANSFER_NUMBER']  ?? '';

  if (authId.isEmpty || authToken.isEmpty || fromNumber.isEmpty || toNumber.isEmpty) {
    print('SKIP: VOBIZ_AUTH_ID, VOBIZ_AUTH_TOKEN, VOBIZ_FROM_NUMBER, VOBIZ_TO_NUMBER required');
    exit(0);
  }

  const audioUrl    = 'https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3';
  const answerUrl   = 'https://internal-test-xml.vobiz.ai/answer';
  const hangupUrl   = 'https://internal-test-xml.vobiz.ai/hangup';
  const transferUrl = 'https://internal-test-xml.vobiz.ai/answer';

  final client = ApiClient(basePath: 'https://api.vobiz.ai');
  client.addDefaultHeader('X-Auth-ID', authId);
  client.addDefaultHeader('X-Auth-Token', authToken);

  final api    = CallApi(client);
  int passed   = 0;
  int failed   = 0;

  Future<void> step(String name, Future<void> Function() fn) async {
    try {
      await fn();
      print('[Dart] PASS: $name');
      passed++;
    } catch (e) {
      print('[Dart] FAIL: $name - $e');
      failed++;
    }
  }

  Future<void> sleep(int sec) => Future.delayed(Duration(seconds: sec));

  // STEP 1: Make outbound call
  print('\n[Dart] STEP 1: Making outbound call...');
  String requestUUID = '';
  await step('Make Call', () async {
    final result = await api.apiV1AccountAuthIdCallPost(
      authId,
      xAuthId: authId,
      xAuthToken: authToken,
      contentType: 'application/json',
      body: {
        'from': fromNumber, 'to': toNumber,
        'answer_url': answerUrl, 'answer_method': 'POST',
        'hangup_url': hangupUrl, 'hangup_method': 'POST',
      },
    );
    final data = result as Map<String, dynamic>? ?? {};
    requestUUID = data['request_uuid'] as String? ??
        ((data['objects'] as List?)?.first as Map<String,dynamic>?)?['request_uuid'] as String? ?? '';
    if (requestUUID.isEmpty) throw Exception('No request_uuid in response: $data');
    print('  -> request_uuid = $requestUUID');
  });
  if (requestUUID.isEmpty) { print('[Dart] Cannot continue'); exit(1); }
  await sleep(5);

  await step('List Live Calls', () async {
    await api.apiV1AccountAuthIdCallGet(authId, xAuthId: authId, xAuthToken: authToken, status: 'live');
  });
  await sleep(5);

  await step('Get Single Live Call', () async {
    await api.apiV1AccountAuthIdCallGet0(authId, xAuthId: authId, xAuthToken: authToken, status: 'live');
  });
  await sleep(5);

  await step('Speak TTS', () async {
    await api.apiV1AccountAuthIdCallSpeakPost(
      authId, xAuthId: authId, xAuthToken: authToken,
      body: {'text': 'Hello from Vobiz Dart SDK.', 'voice': 'WOMAN', 'language': 'en-US', 'legs': 'aleg'},
    );
  });
  await sleep(5);

  await step('Stop TTS', () async {
    await api.apiV1AccountAuthIdCallSpeakDelete(authId, xAuthId: authId, xAuthToken: authToken);
  });

  await step('Play Audio', () async {
    await api.apiV1AccountAuthIdCallPlayPost(
      authId, xAuthId: authId, xAuthToken: authToken,
      body: {'urls': [audioUrl], 'legs': 'aleg', 'loop': false, 'mix': true},
    );
  });
  await sleep(5);

  await step('Stop Audio', () async {
    await api.apiV1AccountAuthIdCallPlayDelete(authId, xAuthId: authId, xAuthToken: authToken);
  });

  await step('Start Recording', () async {
    await api.apiV1AccountAuthIdCallRecordPost(
      authId, xAuthId: authId, xAuthToken: authToken,
      body: {'time_limit': 60, 'file_format': 'mp3'},
    );
  });
  await sleep(5);

  await step('Send DTMF', () async {
    await api.apiV1AccountAuthIdCallDTMFPost(
      authId, xAuthId: authId, xAuthToken: authToken,
      body: {'digits': '1234', 'leg': 'aleg'},
    );
  });

  await step('Stop Recording', () async {
    await api.apiV1AccountAuthIdCallRecordDelete(authId, xAuthId: authId, xAuthToken: authToken);
  });

  await step('Transfer Call', () async {
    final transferTo = transferUrl + (transferNumber.isNotEmpty ? '?to=$transferNumber' : '');
    await api.apiV1AccountAuthIdCallPost0(
      authId, xAuthId: authId, xAuthToken: authToken,
      body: {'legs': 'aleg', 'aleg_url': transferTo, 'aleg_method': 'POST'},
    );
  });
  await sleep(5);

  await step('Hang Up Call', () async {
    await api.apiV1AccountAuthIdCallDelete(authId, xAuthId: authId, xAuthToken: authToken);
  });

  print('\n[Dart] Call flow COMPLETE: $passed passed, $failed failed');
  exit(failed > 0 ? 1 : 0);
}
