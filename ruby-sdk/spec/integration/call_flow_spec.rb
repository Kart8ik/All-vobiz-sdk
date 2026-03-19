# Vobiz Ruby SDK - Full Call Flow Integration Test
# Run: bundle exec rspec spec/integration/call_flow_spec.rb --format documentation

require 'spec_helper'
require 'json'

AUTH_ID_CF         = ENV['VOBIZ_AUTH_ID']
AUTH_TOKEN_CF      = ENV['VOBIZ_AUTH_TOKEN']
FROM_NUMBER_CF     = ENV['VOBIZ_FROM_NUMBER']
TO_NUMBER_CF       = ENV['VOBIZ_TO_NUMBER']
TRANSFER_NUMBER_CF = ENV['VOBIZ_TRANSFER_NUMBER']

AUDIO_URL_CF    = 'https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3'
ANSWER_URL_CF   = 'https://internal-test-xml.vobiz.ai/answer'
HANGUP_URL_CF   = 'https://internal-test-xml.vobiz.ai/hangup'
TRANSFER_URL_CF = 'https://internal-test-xml.vobiz.ai/answer'

RSpec.describe 'Vobiz Call Flow Integration Test', :call_flow do
  before(:all) do
    skip 'Required env vars not set' unless AUTH_ID_CF && AUTH_TOKEN_CF && FROM_NUMBER_CF && TO_NUMBER_CF

    Vobiz.configure do |c|
      c.api_key['X-Auth-ID']    = AUTH_ID_CF
      c.api_key['X-Auth-Token'] = AUTH_TOKEN_CF
    end

    @api          = Vobiz::CallApi.new
    @opts         = { x_auth_id: AUTH_ID_CF, x_auth_token: AUTH_TOKEN_CF }
    @request_uuid = nil
  end

  it 'STEP 1: Makes outbound call and extracts request_uuid' do
    puts "\n[Ruby] STEP 1: Making outbound call..."
    expect {
      @api.api_v1_account_auth_id_call_post(
        AUTH_ID_CF,
        @opts.merge(body: {
          from:          FROM_NUMBER_CF,
          to:            TO_NUMBER_CF,
          answer_url:    ANSWER_URL_CF,
          answer_method: 'POST',
          hangup_url:    HANGUP_URL_CF,
          hangup_method: 'POST'
        })
      )
    }.not_to raise_error
    puts "[Ruby] STEP 1 PASS"
    sleep 5
  end

  it 'STEP 2: Lists all live calls' do
    puts "[Ruby] STEP 2: Listing live calls..."
    expect {
      @api.api_v1_account_auth_id_call_get(AUTH_ID_CF, @opts.merge(status: 'live'))
    }.not_to raise_error
    puts "[Ruby] STEP 2 PASS"
    sleep 5
  end

  it 'STEP 3: Gets single live call' do
    puts "[Ruby] STEP 3: Get single live call..."
    expect {
      @api.api_v1_account_auth_id_call_get_0(AUTH_ID_CF, @opts.merge(status: 'live'))
    }.not_to raise_error
    puts "[Ruby] STEP 3 PASS"
    sleep 5
  end

  it 'STEP 4: Speaks TTS on call' do
    puts "[Ruby] STEP 4: Speak TTS..."
    expect {
      @api.api_v1_account_auth_id_call_speak_post(AUTH_ID_CF, @opts.merge(body: {
        text: 'Hello from Vobiz Ruby SDK.', voice: 'WOMAN', language: 'en-US', legs: 'aleg'
      }))
    }.not_to raise_error
    puts "[Ruby] STEP 4 PASS"
    sleep 5
  end

  it 'STEP 5: Stops TTS' do
    puts "[Ruby] STEP 5: Stop TTS..."
    expect {
      @api.api_v1_account_auth_id_call_speak_delete(AUTH_ID_CF, @opts)
    }.not_to raise_error
    puts "[Ruby] STEP 5 PASS"
  end

  it 'STEP 6: Plays audio on call' do
    puts "[Ruby] STEP 6: Play audio..."
    expect {
      @api.api_v1_account_auth_id_call_play_post(AUTH_ID_CF, @opts.merge(body: {
        urls: [AUDIO_URL_CF], legs: 'aleg', loop: false, mix: true
      }))
    }.not_to raise_error
    puts "[Ruby] STEP 6 PASS"
    sleep 5
  end

  it 'STEP 7: Stops audio' do
    puts "[Ruby] STEP 7: Stop audio..."
    expect {
      @api.api_v1_account_auth_id_call_play_delete(AUTH_ID_CF, @opts)
    }.not_to raise_error
    puts "[Ruby] STEP 7 PASS"
  end

  it 'STEP 8: Starts recording' do
    puts "[Ruby] STEP 8: Start recording..."
    expect {
      @api.api_v1_account_auth_id_call_record_post(AUTH_ID_CF, @opts.merge(body: {
        time_limit: 60, file_format: 'mp3'
      }))
    }.not_to raise_error
    puts "[Ruby] STEP 8 PASS"
    sleep 5
  end

  it 'STEP 9: Sends DTMF digits' do
    puts "[Ruby] STEP 9: Send DTMF..."
    expect {
      @api.api_v1_account_auth_id_call_dtmf_post(AUTH_ID_CF, @opts.merge(body: {
        digits: '1234', leg: 'aleg'
      }))
    }.not_to raise_error
    puts "[Ruby] STEP 9 PASS"
  end

  it 'STEP 10: Stops recording' do
    puts "[Ruby] STEP 10: Stop recording..."
    expect {
      @api.api_v1_account_auth_id_call_record_delete(AUTH_ID_CF, @opts)
    }.not_to raise_error
    puts "[Ruby] STEP 10 PASS"
  end

  it 'STEP 11: Transfers call' do
    puts "[Ruby] STEP 11: Transfer call..."
    transfer_to = TRANSFER_URL_CF + (TRANSFER_NUMBER_CF ? "?to=#{TRANSFER_NUMBER_CF}" : '')
    expect {
      @api.api_v1_account_auth_id_call_post_0(AUTH_ID_CF, @opts.merge(body: {
        legs: 'aleg', aleg_url: transfer_to, aleg_method: 'POST'
      }))
    }.not_to raise_error
    puts "[Ruby] STEP 11 PASS"
    sleep 5
  end

  it 'STEP 12: Hangs up call' do
    puts "[Ruby] STEP 12: Hang up..."
    expect {
      @api.api_v1_account_auth_id_call_delete(AUTH_ID_CF, @opts)
    }.not_to raise_error
    puts "[Ruby] STEP 12 PASS"
    puts "\n[Ruby] Call flow COMPLETE"
  end
end
