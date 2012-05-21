using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace TDDMicroExercises.TelemetrySystem.Tests
{
    [TestFixture]
    public class TelemetryDiagnosticControlsTest
    {
        [Test]
        public void CheckTransmission_should_send_a_diagnostic_message_and_receive_a_status_message_response()
        {
            var telemetryClient = MockRepository.GenerateMock<ITelemetryClient>();

            telemetryClient.Stub(client => client.OnlineStatus).Return(true);
            telemetryClient.Stub(client => client.Receive()).Return("foo");
            
            var telemetryDiagnosticControls = new TelemetryDiagnosticControls(telemetryClient);

            telemetryDiagnosticControls.CheckTransmission();
            
            Assert.That(telemetryDiagnosticControls.DiagnosticInfo, Is.EqualTo("foo"));
        }

    }
}
