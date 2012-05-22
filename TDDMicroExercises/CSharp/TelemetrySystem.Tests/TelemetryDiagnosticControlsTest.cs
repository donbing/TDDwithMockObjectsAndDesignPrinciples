using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace TDDMicroExercises.TelemetrySystem.Tests
{
    [TestFixture]
    public class TelemetryDiagnosticControlsTest
    {
        private const string ExpectedTelemetryClientResponse = "foo";

        [Test]
        public void CheckTransmission_should_send_a_diagnostic_message_and_receive_a_status_message_response()
        {
            var telemetryClient = MockRepository.GenerateMock<ITelemetryClient>();
            var telemetryConnection = new TelemetryConnection(telemetryClient);

            using (telemetryClient.GetMockRepository().Ordered())
            {
                telemetryClient
                    .Stub(client => client.OnlineStatus)
                    .Return(false).Repeat.Once();

                telemetryClient
                   .Stub(client => client.OnlineStatus)
                   .Return(true);
            }

            telemetryClient
                .Stub(client => client.Receive())
                .Return(ExpectedTelemetryClientResponse);

            var telemetryDiagnosticControls = new TelemetryDiagnosticControls(telemetryClient, telemetryConnection);

            telemetryDiagnosticControls.CheckTransmission();

            telemetryClient.AssertWasCalled(client => client.Connect(Arg<string>.Is.Anything));
            telemetryClient.AssertWasCalled(client => client.Send(TelemetryClient.DiagnosticMessage));

            Assert.That(telemetryDiagnosticControls.DiagnosticInfo, Is.EqualTo(ExpectedTelemetryClientResponse));
        }
    }

    [TestFixture]
    public class ConnectionTests
    {
        [Test, ExpectedException(typeof(System.Exception), ExpectedMessage = "Unable to connect.")]
        public void CheckConnection_ThrowsAfterThreeFailedConnections()
        {
            var telemetryClient = MockRepository.GenerateMock<IConnection>();
            var telemetryConnection = new TelemetryConnection(telemetryClient);

            using (telemetryClient.GetMockRepository().Ordered())
            {
                telemetryClient.Expect(client => client.Disconnect());

                telemetryClient
                    .Stub(client => client.OnlineStatus)
                    .Return(false)
                    .Repeat.Times(3);
            }
            telemetryConnection.TryConnect(3, "any connection string");

            telemetryClient.AssertWasCalled(client => client.Connect(Arg<string>.Is.Anything), opt => opt.Repeat.Times(2));
            telemetryClient.VerifyAllExpectations();
        }

        [Test]
        public void CheckConnection_ConnectsOnThirdConnectionRetry()
        {
            var telemetryClient = MockRepository.GenerateMock<IConnection>();
            var telemetryConnection = new TelemetryConnection(telemetryClient);

            using (telemetryClient.GetMockRepository().Ordered())
            {

                telemetryClient.Expect(client => client.Disconnect());

                telemetryClient
                    .Stub(client => client.OnlineStatus)
                    .Return(false)
                    .Repeat.Times(2);

                telemetryClient
                    .Stub(client => client.OnlineStatus)
                    .Return(true);
            }
            telemetryConnection.TryConnect(3, "any connection string");
            telemetryClient.AssertWasCalled(client => client.Connect(Arg<string>.Is.Anything),opt => opt.Repeat.Times(2));
            telemetryClient.VerifyAllExpectations();
        }
    }
}
