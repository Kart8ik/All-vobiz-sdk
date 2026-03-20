using System.Xml.Linq;
using Vobiz.Xml;
using Xunit;

namespace Vobiz.Test.Xml
{
    public class ResponseXmlTests
    {
        private static string Normalize(string xml)
        {
            return XDocument.Parse(xml).ToString(SaveOptions.DisableFormatting);
        }

        [Fact]
        public void CreateResponse()
        {
            string xml = new ResponseElement().ToString(false);
            Assert.Equal("<Response />", Normalize(xml));
        }

        [Fact]
        public void AddConference()
        {
            string xml = new ResponseElement().AddConference("test").ToString(false);
            Assert.Equal("<Response><Conference>test</Conference></Response>", Normalize(xml));
        }

        [Fact]
        public void AddDialWithNumber()
        {
            DialElement dial = new DialElement(timeLimit: 2).AddNumber("+12025550123");
            string xml = new ResponseElement().AddDial(dial).ToString(false);
            Assert.Equal("<Response><Dial timeLimit=\"2\"><Number>+12025550123</Number></Dial></Response>", Normalize(xml));
        }

        [Fact]
        public void DialWithoutChildrenThrows()
        {
            ResponseElement response = new ResponseElement().AddDial(timeLimit: 2);
            Assert.Throws<VobizXmlException>(() => response.ToString(false));
        }

        [Fact]
        public void AddGather()
        {
            string xml = new ResponseElement()
                .AddGather("https://foo.example.com/gather", executionTimeout: 15, inputType: "speech")
                .ToString(false);

            Assert.Equal(
                "<Response><Gather action=\"https://foo.example.com/gather\" inputType=\"speech\" executionTimeout=\"15\" /></Response>",
                Normalize(xml));
        }

        [Fact]
        public void GatherInvalidUrlThrows()
        {
            Assert.Throws<VobizXmlException>(() => new GatherElement("foo/bar"));
        }

        [Fact]
        public void AddAudioStream()
        {
            string xml = new ResponseElement().AddAudioStream("wss://stream.example.com/ws", bidirectional: true).ToString(false);
            Assert.Equal("<Response><AudioStream bidirectional=\"true\">wss://stream.example.com/ws</AudioStream></Response>", Normalize(xml));
        }

        [Fact]
        public void AddPreAnswer()
        {
            string xml = new ResponseElement().AddPreAnswer().ToString(false);
            Assert.Equal("<Response><PreAnswer /></Response>", Normalize(xml));
        }

        [Fact]
        public void AddSpeakAndRecord()
        {
            ResponseElement response = new();
            response.Add(new SpeakElement("Please leave a message after the beep. Press the star key when done."));
            response.Add(new RecordElement(action: "http://foo.com/get_recording/", maxLength: 30, finishOnKey: "*"));
            response.Add(new SpeakElement("Recording not received."));

            string xml = response.ToString(false);
            Assert.Equal(
                "<Response><Speak>Please leave a message after the beep. Press the star key when done.</Speak><Record action=\"http://foo.com/get_recording/\" maxLength=\"30\" finishOnKey=\"*\" /><Speak>Recording not received.</Speak></Response>",
                Normalize(xml));
        }

        [Fact]
        public void AddWait()
        {
            string xml = new ResponseElement().AddWait(length: 2).ToString(false);
            Assert.Equal("<Response><Wait length=\"2\" /></Response>", Normalize(xml));
        }

        [Fact]
        public void ContinueSpeakReplacementWorks()
        {
            SpeakElement speak = new SpeakElement("hello").AddCont("world");
            string xml = new ResponseElement().Add(speak).ToString(false);
            Assert.DoesNotContain("<cont>", xml);
            Assert.DoesNotContain("</cont>", xml);
            Assert.Contains(" world ", xml);
        }

        [Fact]
        public void InvalidNestingThrows()
        {
            GatherElement gather = new("https://foo.example.com/g");
            Assert.Throws<VobizXmlException>(() => gather.Add(new DTMFElement("1")));
        }
    }
}