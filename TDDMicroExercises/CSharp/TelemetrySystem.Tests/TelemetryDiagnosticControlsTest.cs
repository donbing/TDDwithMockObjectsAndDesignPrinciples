using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace TDDMicroExercises.TelemetrySystem.Tests
{
    [TestFixture]
    public class TelemetryDiagnosticControlsTest
    {
        [Test]
        public void CheckTransmission_should_send_a_diagnostic_message_and_receive_a_status_message_response()
        {
            var telemetryClient = new TelemetryClient();

            var telemetryDiagnosticControls = new TelemetryDiagnosticControls(telemetryClient);

            telemetryDiagnosticControls.CheckTransmission();

            Assert.That(telemetryDiagnosticControls.DiagnosticInfo, Is.Not.Empty);
        }

    }
}
