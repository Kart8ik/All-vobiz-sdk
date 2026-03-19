# Vobiz Ruby SDK - Integration Tests (Read-Only)
# Run: bundle exec rspec spec/integration/vobiz_integration_spec.rb
# Requires: VOBIZ_AUTH_ID and VOBIZ_AUTH_TOKEN env vars

require 'spec_helper'

VOBIZ_AUTH_ID    = ENV['VOBIZ_AUTH_ID']
VOBIZ_AUTH_TOKEN = ENV['VOBIZ_AUTH_TOKEN']

RSpec.describe 'Vobiz SDK Integration Tests', :integration do
  before(:all) do
    skip 'VOBIZ_AUTH_ID or VOBIZ_AUTH_TOKEN not set' unless VOBIZ_AUTH_ID && VOBIZ_AUTH_TOKEN

    Vobiz.configure do |config|
      config.api_key['X-Auth-ID']    = VOBIZ_AUTH_ID
      config.api_key['X-Auth-Token'] = VOBIZ_AUTH_TOKEN
    end
  end

  describe 'AccountApi' do
    it 'GET /api/v1/auth/me returns account details' do
      api    = Vobiz::AccountApi.new
      result = api.api_v1_auth_me_get
      puts "[Ruby] GetAccountDetails: OK"
      expect(result).not_to be_nil
    end
  end

  describe 'CallApi' do
    it 'GET live calls returns a response' do
      api    = Vobiz::CallApi.new
      result = api.api_v1_account_auth_id_call_get(VOBIZ_AUTH_ID, { status: 'live' })
      puts "[Ruby] GetLiveCalls: OK"
      expect(result).not_to be_nil
    end
  end

  describe 'RecordingApi' do
    it 'GET recordings returns a list' do
      api    = Vobiz::RecordingApi.new
      result = api.api_v1_account_account_id_recording_get(VOBIZ_AUTH_ID)
      puts "[Ruby] ListRecordings: OK"
      expect(result).not_to be_nil
    end
  end

  describe 'ConferenceApi' do
    it 'GET conferences returns a response' do
      api    = Vobiz::ConferenceApi.new
      result = api.api_v1_account_auth_id_conference_get(VOBIZ_AUTH_ID)
      puts "[Ruby] ListConferences: OK"
      expect(result).not_to be_nil
    end
  end

  describe 'ApplicationApi' do
    it 'GET applications returns a list' do
      api    = Vobiz::ApplicationApi.new
      result = api.api_v1_account_auth_id_application_get(VOBIZ_AUTH_ID)
      puts "[Ruby] ListApplications: OK"
      expect(result).not_to be_nil
    end
  end
end
