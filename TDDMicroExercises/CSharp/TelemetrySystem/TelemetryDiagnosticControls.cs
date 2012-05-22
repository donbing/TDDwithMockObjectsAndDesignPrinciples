
using System;

namespace TDDMicroExercises.TelemetrySystem
{
    public class TelemetryDiagnosticControls
    {
        private const string DiagnosticChannelConnectionString = "*111#";
        
        private readonly ITelemetryClient telemetryClient;
        private string diagnosticInfo = string.Empty;
        private readonly TelemetryConnection telemetryConnection;

        public TelemetryDiagnosticControls(ITelemetryClient client)
        {
            telemetryClient = client;
            telemetryConnection = new TelemetryConnection(telemetryClient);
        }

        public string DiagnosticInfo
        {
            get { return diagnosticInfo; }
            set { diagnosticInfo = value; }
        }

        public void CheckTransmission()
        {
            diagnosticInfo = string.Empty;

            telemetryConnection.TryConnect(3, DiagnosticChannelConnectionString);

            telemetryClient.Send(TelemetryClient.DiagnosticMessage);

            diagnosticInfo = telemetryClient.Receive();
        }
    }

    public class TelemetryConnection
    {
        private readonly ITelemetryClient connection;

        public TelemetryConnection(ITelemetryClient connection)
        {
            this.connection = connection;
        }
        
        public void TryConnect(int retryCount, string diagnosticChannelConnectionString)
        {
            var conn = this;
            conn.Disconnect();

            while (conn.OnlineStatus == false && retryCount > 0)
            {
                conn.Connect(diagnosticChannelConnectionString);
                retryCount -= 1;
            }

            if (conn.OnlineStatus == false)
            {
                throw new Exception("Unable to connect.");
            }
        }

        public void Connect(string connectionString)
        {
            connection.Connect(connectionString);
        }

        public void Disconnect()
        {
            connection.Disconnect();
        }

        public bool OnlineStatus
        {
            get { return connection.OnlineStatus; }
        }
    }
}
