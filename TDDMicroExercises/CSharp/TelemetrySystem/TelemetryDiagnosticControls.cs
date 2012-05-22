
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

            TelemetryConnection.TryConnect(3, telemetryClient, DiagnosticChannelConnectionString);

            telemetryClient.Send(TelemetryClient.DiagnosticMessage);

            diagnosticInfo = telemetryClient.Receive();
        }
    }

    public class TelemetryConnection
    {
        public static void TryConnect(int retryCount, ITelemetryClient connection, string diagnosticChannelConnectionString)
        {
            connection.Disconnect();

            while (connection.OnlineStatus == false && retryCount > 0)
            {
                connection.Connect(diagnosticChannelConnectionString);
                retryCount -= 1;
            }

            if (connection.OnlineStatus == false)
            {
                throw new Exception("Unable to connect.");
            }
        }
    }
}
