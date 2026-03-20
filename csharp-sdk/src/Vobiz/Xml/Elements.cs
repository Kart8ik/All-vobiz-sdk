using System;
using System.Collections.Generic;

namespace Vobiz.Xml
{
    public sealed class AudioStreamElement : VobizXMLElement
    {
        public AudioStreamElement(string content, bool? bidirectional = null) : base("AudioStream", content: content)
        {
            SetAttribute("bidirectional", bidirectional);
        }
    }

    public sealed class BreakElement : VobizXMLElement
    {
        public BreakElement(string? content = null, string? strength = null, string? time = null) : base("break", content: content)
        {
            SetAttribute("strength", strength);
            SetAttribute("time", time);
        }
    }

    public sealed class ConferenceElement : VobizXMLElement
    {
        public ConferenceElement(
            string content,
            bool? muted = null,
            string? enterSound = null,
            string? exitSound = null,
            bool? startConferenceOnEnter = null,
            bool? endConferenceOnExit = null,
            bool? stayAlone = null,
            string? waitSound = null,
            int? maxMembers = null,
            bool? record = null,
            string? recordFileFormat = null,
            int? timeLimit = null,
            bool? hangupOnStar = null,
            string? action = null,
            string? method = null,
            string? callbackUrl = null,
            string? callbackMethod = null,
            string? digitsMatch = null,
            bool? floorEvent = null,
            bool? redirect = null,
            bool? relayDTMF = null) : base("Conference", content: content)
        {
            SetAttribute("muted", muted);
            SetAttribute("enterSound", enterSound);
            SetAttribute("exitSound", exitSound);
            SetAttribute("startConferenceOnEnter", startConferenceOnEnter);
            SetAttribute("endConferenceOnExit", endConferenceOnExit);
            SetAttribute("stayAlone", stayAlone);
            SetAttribute("waitSound", waitSound);
            SetAttribute("maxMembers", maxMembers);
            SetAttribute("record", record);
            SetAttribute("recordFileFormat", recordFileFormat);
            SetAttribute("timeLimit", timeLimit);
            SetAttribute("hangupOnStar", hangupOnStar);
            SetAttribute("action", action);
            SetAttribute("method", method);
            SetAttribute("callbackUrl", callbackUrl);
            SetAttribute("callbackMethod", callbackMethod);
            SetAttribute("digitsMatch", digitsMatch);
            SetAttribute("floorEvent", floorEvent);
            SetAttribute("redirect", redirect);
            SetAttribute("relayDTMF", relayDTMF);
        }
    }

    public sealed class ContElement : VobizXMLElement
    {
        public ContElement(string content) : base("cont", content: content)
        {
        }
    }

    public sealed class DialElement : VobizXMLElement
    {
        public DialElement(
            string? action = null,
            string? method = null,
            bool? hangupOnStar = null,
            int? timeLimit = null,
            int? timeout = null,
            string? callerId = null,
            string? callerName = null,
            string? confirmSound = null,
            string? confirmTimeout = null,
            string? confirmKey = null,
            string? dialMusic = null,
            string? callbackUrl = null,
            string? callbackMethod = null,
            bool? redirect = null,
            string? digitsMatch = null,
            string? digitsMatchBLeg = null,
            string? sipHeaders = null) : base("Dial", ["User", "Number"])
        {
            SetAttribute("action", action);
            SetAttribute("method", method);
            SetAttribute("hangupOnStar", hangupOnStar);
            SetAttribute("timeLimit", timeLimit);
            SetAttribute("timeout", timeout);
            SetAttribute("callerId", callerId);
            SetAttribute("callerName", callerName);
            SetAttribute("confirmSound", confirmSound);
            SetAttribute("confirmTimeout", confirmTimeout);
            SetAttribute("confirmKey", confirmKey);
            SetAttribute("dialMusic", dialMusic);
            SetAttribute("callbackUrl", callbackUrl);
            SetAttribute("callbackMethod", callbackMethod);
            SetAttribute("redirect", redirect);
            SetAttribute("digitsMatch", digitsMatch);
            SetAttribute("digitsMatchBLeg", digitsMatchBLeg);
            SetAttribute("sipHeaders", sipHeaders);
        }

        protected override void Validate()
        {
            if (Children.Count == 0)
            {
                throw new VobizXmlException("Dial must contain at least one Number or User element");
            }
        }

        public DialElement AddNumber(string content, string? sendDigits = null, string? sendOnPreanswer = null)
        {
            Add(new NumberElement(content, sendDigits, sendOnPreanswer));
            return this;
        }

        public DialElement AddUser(string content, string? sendDigits = null, string? sendOnPreanswer = null)
        {
            Add(new UserElement(content, sendDigits, sendOnPreanswer));
            return this;
        }
    }

    public sealed class DTMFElement : VobizXMLElement
    {
        public DTMFElement(string content, bool? asyncValue = null) : base("DTMF", content: content)
        {
            SetAttribute("async", asyncValue);
        }
    }

    public sealed class EmphasisElement : VobizXMLElement
    {
        public EmphasisElement(string content, string? level = null) : base("emphasis", content: content)
        {
            SetAttribute("level", level);
        }
    }

    public sealed class GatherElement : VobizXMLElement
    {
        public GatherElement(
            string action,
            string? method = null,
            string? inputType = null,
            object? executionTimeout = null,
            object? digitEndTimeout = null,
            object? speechEndTimeout = null,
            string? finishOnKey = null,
            int? numDigits = null,
            string? speechModel = null,
            string? hints = null,
            string? language = null,
            string? interimSpeechResultsCallback = null,
            string? interimSpeechResultsCallbackMethod = null,
            bool? log = null,
            bool? redirect = null,
            bool? profanityFilter = null) : base("Gather", ["Speak", "Play"])
        {
            if (!action.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !action.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                throw new VobizXmlException("action must be a fully qualified URL");
            }

            SetAttribute("action", action);
            SetAttribute("method", method);
            SetAttribute("inputType", inputType);
            SetAttribute("executionTimeout", executionTimeout);
            SetAttribute("digitEndTimeout", digitEndTimeout);
            SetAttribute("speechEndTimeout", speechEndTimeout);
            SetAttribute("finishOnKey", finishOnKey);
            SetAttribute("numDigits", numDigits);
            SetAttribute("speechModel", speechModel);
            SetAttribute("hints", hints);
            SetAttribute("language", language);
            SetAttribute("interimSpeechResultsCallback", interimSpeechResultsCallback);
            SetAttribute("interimSpeechResultsCallbackMethod", interimSpeechResultsCallbackMethod);
            SetAttribute("log", log);
            SetAttribute("redirect", redirect);
            SetAttribute("profanityFilter", profanityFilter);
        }

        public GatherElement AddSpeak(string content, string? voice = null, string? language = null, int? loop = null)
        {
            Add(new SpeakElement(content, voice, language, loop));
            return this;
        }

        public GatherElement AddPlay(string content, int? loop = null)
        {
            Add(new PlayElement(content, loop));
            return this;
        }
    }

    public sealed class HangupElement : VobizXMLElement
    {
        public HangupElement(string? reason = null, int? schedule = null) : base("Hangup")
        {
            SetAttribute("reason", reason);
            SetAttribute("schedule", schedule);
        }
    }

    public sealed class LangElement : VobizXMLElement
    {
        public LangElement(string content, string? xmlLang = null) : base("lang", content: content)
        {
            SetAttribute("xml:lang", xmlLang);
        }
    }

    public sealed class MessageElement : VobizXMLElement
    {
        public MessageElement(string content, string? src = null, string? dst = null, string? type = null, string? callbackUrl = null, string? callbackMethod = null)
            : base("Message", content: content)
        {
            SetAttribute("src", src);
            SetAttribute("dst", dst);
            SetAttribute("type", type);
            SetAttribute("callbackUrl", callbackUrl);
            SetAttribute("callbackMethod", callbackMethod);
        }
    }

    public sealed class MultiPartyCallElement : VobizXMLElement
    {
        public MultiPartyCallElement(
            string content,
            string? role = null,
            int? maxDuration = null,
            int? maxParticipants = null,
            string? waitMusicUrl = null,
            string? waitMusicMethod = null,
            int? waitTime = null,
            bool? startMpcOnEnter = null,
            bool? endMpcOnExit = null,
            bool? stayAlone = null,
            bool? record = null,
            string? recordFileFormat = null,
            bool? mute = null,
            string? enterSound = null,
            string? exitSound = null,
            bool? hold = null,
            string? onExitActionUrl = null,
            string? onExitActionMethod = null,
            string? startRecordingAudio = null,
            string? stopRecordingAudio = null,
            bool? transcript = null,
            string? transcriptionUrl = null,
            bool? relayDTMFInputs = null,
            bool? coachMode = null,
            string? statusCallbackEvents = null,
            string? statusCallbackMethod = null,
            int? recordMinMemberCount = null,
            bool? recordParticipantTrack = null,
            string? recordingCallbackMethod = null,
            string? enterSoundMethod = null,
            string? exitSoundMethod = null,
            string? customerHoldMusicMethod = null,
            string? agentHoldMusicMethod = null,
            string? startRecordingAudioMethod = null,
            string? stopRecordingAudioMethod = null)
            : base("MultiPartyCall", content: content)
        {
            SetAttribute("role", role);
            SetAttribute("maxDuration", maxDuration);
            SetAttribute("maxParticipants", maxParticipants);
            SetAttribute("waitMusicUrl", waitMusicUrl);
            SetAttribute("waitMusicMethod", waitMusicMethod);
            SetAttribute("waitTime", waitTime);
            SetAttribute("startMpcOnEnter", startMpcOnEnter);
            SetAttribute("endMpcOnExit", endMpcOnExit);
            SetAttribute("stayAlone", stayAlone);
            SetAttribute("record", record);
            SetAttribute("recordFileFormat", recordFileFormat);
            SetAttribute("mute", mute);
            SetAttribute("enterSound", enterSound);
            SetAttribute("exitSound", exitSound);
            SetAttribute("hold", hold);
            SetAttribute("onExitActionUrl", onExitActionUrl);
            SetAttribute("onExitActionMethod", onExitActionMethod);
            SetAttribute("startRecordingAudio", startRecordingAudio);
            SetAttribute("stopRecordingAudio", stopRecordingAudio);
            SetAttribute("transcript", transcript);
            SetAttribute("transcriptionUrl", transcriptionUrl);
            SetAttribute("relayDTMFInputs", relayDTMFInputs);
            SetAttribute("coachMode", coachMode);
            SetAttribute("statusCallbackEvents", statusCallbackEvents);
            SetAttribute("statusCallbackMethod", statusCallbackMethod);
            SetAttribute("recordMinMemberCount", recordMinMemberCount);
            SetAttribute("recordParticipantTrack", recordParticipantTrack);
            SetAttribute("recordingCallbackMethod", recordingCallbackMethod);
            SetAttribute("enterSoundMethod", enterSoundMethod);
            SetAttribute("exitSoundMethod", exitSoundMethod);
            SetAttribute("customerHoldMusicMethod", customerHoldMusicMethod);
            SetAttribute("agentHoldMusicMethod", agentHoldMusicMethod);
            SetAttribute("startRecordingAudioMethod", startRecordingAudioMethod);
            SetAttribute("stopRecordingAudioMethod", stopRecordingAudioMethod);
        }
    }

    public sealed class NumberElement : VobizXMLElement
    {
        public NumberElement(string content, string? sendDigits = null, string? sendOnPreanswer = null) : base("Number", content: content)
        {
            SetAttribute("sendDigits", sendDigits);
            SetAttribute("sendOnPreanswer", sendOnPreanswer);
        }
    }

    public sealed class PElement : VobizXMLElement
    {
        public PElement(string content) : base("p", content: content)
        {
        }
    }

    public sealed class PhonemeElement : VobizXMLElement
    {
        public PhonemeElement(string content, string? alphabet = null, string? ph = null) : base("phoneme", content: content)
        {
            SetAttribute("alphabet", alphabet);
            SetAttribute("ph", ph);
        }
    }

    public sealed class PlayElement : VobizXMLElement
    {
        public PlayElement(string content, int? loop = null) : base("Play", content: content)
        {
            SetAttribute("loop", loop);
        }
    }

    public sealed class PreAnswerElement : VobizXMLElement
    {
        public PreAnswerElement() : base("PreAnswer")
        {
        }
    }

    public sealed class ProsodyElement : VobizXMLElement
    {
        public ProsodyElement(string content, string? volume = null, string? rate = null, string? pitch = null) : base("prosody", content: content)
        {
            SetAttribute("volume", volume);
            SetAttribute("rate", rate);
            SetAttribute("pitch", pitch);
        }
    }

    public sealed class RecordElement : VobizXMLElement
    {
        public RecordElement(
            string action,
            string? method = null,
            string? fileFormat = null,
            bool? redirect = null,
            int? timeout = null,
            int? maxLength = null,
            bool? playBeep = null,
            string? finishOnKey = null,
            bool? recordSession = null,
            bool? startOnDialAnswer = null,
            string? transcriptionType = null,
            string? transcriptionUrl = null,
            string? transcriptionReportType = null,
            string? transcriptionMethod = null,
            string? callbackUrl = null,
            string? callbackMethod = null)
            : base("Record")
        {
            SetAttribute("action", action);
            SetAttribute("method", method);
            SetAttribute("fileFormat", fileFormat);
            SetAttribute("redirect", redirect);
            SetAttribute("timeout", timeout);
            SetAttribute("maxLength", maxLength);
            SetAttribute("playBeep", playBeep);
            SetAttribute("finishOnKey", finishOnKey);
            SetAttribute("recordSession", recordSession);
            SetAttribute("startOnDialAnswer", startOnDialAnswer);
            SetAttribute("transcriptionType", transcriptionType);
            SetAttribute("transcriptionUrl", transcriptionUrl);
            SetAttribute("transcriptionReportType", transcriptionReportType);
            SetAttribute("transcriptionMethod", transcriptionMethod);
            SetAttribute("callbackUrl", callbackUrl);
            SetAttribute("callbackMethod", callbackMethod);
        }
    }

    public sealed class RedirectElement : VobizXMLElement
    {
        public RedirectElement(string content, string? method = null) : base("Redirect", content: content)
        {
            SetAttribute("method", method);
        }
    }

    public sealed class SayAsElement : VobizXMLElement
    {
        public SayAsElement(string content, string? interpretAs = null, string? format = null) : base("say-as", content: content)
        {
            SetAttribute("interpret-as", interpretAs);
            SetAttribute("format", format);
        }
    }

    public sealed class SElement : VobizXMLElement
    {
        public SElement(string content) : base("s", content: content)
        {
        }
    }

    public sealed class SpeakElement : VobizXMLElement
    {
        public SpeakElement(string content, string? voice = null, string? language = null, int? loop = null)
            : base("Speak", ["break", "cont", "emphasis", "lang", "p", "phoneme", "prosody", "s", "say-as", "sub", "w"], content)
        {
            SetAttribute("voice", voice);
            SetAttribute("language", language);
            SetAttribute("loop", loop);
        }

        public SpeakElement AddBreak(string? content = null, string? strength = null, string? time = null)
        {
            Add(new BreakElement(content, strength, time));
            return this;
        }

        public SpeakElement AddCont(string content)
        {
            Add(new ContElement(content));
            return this;
        }

        public SpeakElement AddEmphasis(string content, string? level = null)
        {
            Add(new EmphasisElement(content, level));
            return this;
        }

        public SpeakElement AddLang(string content, string? xmlLang = null)
        {
            Add(new LangElement(content, xmlLang));
            return this;
        }

        public SpeakElement AddP(string content)
        {
            Add(new PElement(content));
            return this;
        }

        public SpeakElement AddPhoneme(string content, string? alphabet = null, string? ph = null)
        {
            Add(new PhonemeElement(content, alphabet, ph));
            return this;
        }

        public SpeakElement AddProsody(string content, string? volume = null, string? rate = null, string? pitch = null)
        {
            Add(new ProsodyElement(content, volume, rate, pitch));
            return this;
        }

        public SpeakElement AddS(string content)
        {
            Add(new SElement(content));
            return this;
        }

        public SpeakElement AddSayAs(string content, string? interpretAs = null, string? format = null)
        {
            Add(new SayAsElement(content, interpretAs, format));
            return this;
        }

        public SpeakElement AddSub(string content, string? alias = null)
        {
            Add(new SubElement(content, alias));
            return this;
        }

        public SpeakElement AddW(string content, string? role = null)
        {
            Add(new WElement(content, role));
            return this;
        }
    }

    public sealed class StreamElement : VobizXMLElement
    {
        public StreamElement(string content, bool? bidirectional = null) : base("Stream", content: content)
        {
            SetAttribute("bidirectional", bidirectional);
        }
    }

    public sealed class SubElement : VobizXMLElement
    {
        public SubElement(string content, string? alias = null) : base("sub", content: content)
        {
            SetAttribute("alias", alias);
        }
    }

    public sealed class UserElement : VobizXMLElement
    {
        public UserElement(string content, string? sendDigits = null, string? sendOnPreanswer = null) : base("User", content: content)
        {
            SetAttribute("sendDigits", sendDigits);
            SetAttribute("sendOnPreanswer", sendOnPreanswer);
        }
    }

    public sealed class WaitElement : VobizXMLElement
    {
        public WaitElement(int? length = null, bool? silence = null, bool? beep = null) : base("Wait")
        {
            SetAttribute("length", length);
            SetAttribute("silence", silence);
            SetAttribute("beep", beep);
        }
    }

    public sealed class WElement : VobizXMLElement
    {
        public WElement(string content, string? role = null) : base("w", content: content)
        {
            SetAttribute("role", role);
        }
    }

    public sealed class ResponseElement : VobizXMLElement
    {
        public ResponseElement() : base("Response", ["Conference", "Dial", "DTMF", "AudioStream", "Gather", "Hangup", "Message", "Play", "PreAnswer", "Record", "Redirect", "Speak", "Wait", "MultiPartyCall", "Stream"])
        {
        }

        public ResponseElement AddConference(string content, bool? muted = null, string? enterSound = null, string? exitSound = null)
        {
            Add(new ConferenceElement(content, muted: muted, enterSound: enterSound, exitSound: exitSound));
            return this;
        }

        public ResponseElement AddDial(DialElement dial)
        {
            Add(dial);
            return this;
        }

        public ResponseElement AddDial(int? timeLimit = null)
        {
            Add(new DialElement(timeLimit: timeLimit));
            return this;
        }

        public ResponseElement AddDtmf(string content, bool? asyncValue = null)
        {
            Add(new DTMFElement(content, asyncValue));
            return this;
        }

        public ResponseElement AddAudioStream(string content, bool? bidirectional = null)
        {
            Add(new AudioStreamElement(content, bidirectional));
            return this;
        }

        public ResponseElement AddGather(string action, string? method = null, string? inputType = null, int? executionTimeout = null)
        {
            Add(new GatherElement(action, method: method, inputType: inputType, executionTimeout: executionTimeout));
            return this;
        }

        public ResponseElement AddHangup(string? reason = null, int? schedule = null)
        {
            Add(new HangupElement(reason, schedule));
            return this;
        }

        public ResponseElement AddMessage(string content, string? src = null, string? dst = null, string? type = null)
        {
            Add(new MessageElement(content, src, dst, type));
            return this;
        }

        public ResponseElement AddPlay(string content, int? loop = null)
        {
            Add(new PlayElement(content, loop));
            return this;
        }

        public ResponseElement AddPreAnswer()
        {
            Add(new PreAnswerElement());
            return this;
        }

        public ResponseElement AddRecord(string action, int? maxLength = null, string? finishOnKey = null)
        {
            Add(new RecordElement(action, maxLength: maxLength, finishOnKey: finishOnKey));
            return this;
        }

        public ResponseElement AddRedirect(string content, string? method = null)
        {
            Add(new RedirectElement(content, method));
            return this;
        }

        public ResponseElement AddSpeak(string content, string? voice = null, string? language = null, int? loop = null)
        {
            Add(new SpeakElement(content, voice, language, loop));
            return this;
        }

        public ResponseElement AddWait(int? length = null, bool? silence = null, bool? beep = null)
        {
            Add(new WaitElement(length, silence, beep));
            return this;
        }

        public ResponseElement AddMultiPartyCall(string content, string? role = null, int? maxDuration = null, int? maxParticipants = null)
        {
            Add(new MultiPartyCallElement(content, role: role, maxDuration: maxDuration, maxParticipants: maxParticipants));
            return this;
        }

        public ResponseElement AddStream(string content, bool? bidirectional = null)
        {
            Add(new StreamElement(content, bidirectional));
            return this;
        }
    }

    public static class VobizXml
    {
        public static ResponseElement Response() => new();
    }
}