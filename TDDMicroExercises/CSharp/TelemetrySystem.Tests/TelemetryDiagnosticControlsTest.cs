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
            var channel = MockRepository.GenerateMock<ITelemetryChannel>();
            var connetion = MockRepository.GenerateMock<IConnection>();
            var telemetryConnection = new TelemetryConnector(connetion);

            using (channel.GetMockRepository().Ordered())
            {
                connetion
                    .Stub(client => client.OnlineStatus)
                    .Return(false).Repeat.Once();

                connetion
                   .Stub(client => client.OnlineStatus)
                   .Return(true);
            }

            channel
                .Stub(client => client.Receive())
                .Return(ExpectedTelemetryClientResponse);

            var telemetryDiagnosticControls = new TelemetryDiagnosticControls(channel, telemetryConnection);

            telemetryDiagnosticControls.CheckTransmission();

            connetion.AssertWasCalled(conn => conn.Connect(Arg<string>.Is.Anything));
            channel.AssertWasCalled(client => client.Send(TelemetryClient.DiagnosticMessage));

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
            var telemetryConnection = new TelemetryConnector(telemetryClient);

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
            var telemetryConnection = new TelemetryConnector(telemetryClient);

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
