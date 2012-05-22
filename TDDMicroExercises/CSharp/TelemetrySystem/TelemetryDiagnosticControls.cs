
using System;

namespace TDDMicroExercises.TelemetrySystem
{
    public class TelemetryDiagnosticControls
    {
        private const string DiagnosticChannelConnectionString = "*111#";
        
        private readonly ITelemetryClient telemetryClient;
        private string diagnosticInfo = string.Empty;

        public TelemetryDiagnosticControls(ITelemetryClient client)
        {
            telemetryClient = client;
        }

        public string DiagnosticInfo
        {
            get { return diagnosticInfo; }
            set { diagnosticInfo = value; }
        }

        public void CheckTransmission()
        {
            diagnosticInfo = string.Empty;

            TryConnect(3, telemetryClient);

            telemetryClient.Send(TelemetryClient.DiagnosticMessage);

            diagnosticInfo = telemetryClient.Receive();
        }

        private static void TryConnect(int retryCount, ITelemetryClient connection)
        {
            connection.Disconnect();

            while (connection.OnlineStatus == false && retryCount > 0)
            {
                connection.Connect(DiagnosticChannelConnectionString);
                retryCount -= 1;
            }

            if (connection.OnlineStatus == false)
            {
                throw new Exception("Unable to connect.");
            }
        }
    }
}
