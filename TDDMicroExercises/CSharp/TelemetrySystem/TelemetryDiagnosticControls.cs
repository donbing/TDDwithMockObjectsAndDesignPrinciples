
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
        private readonly ITelemetryClient connection;

        private TelemetryConnection(ITelemetryClient connection)
        {
            this.connection = connection;
        }
        
        public static void TryConnect(int retryCount, ITelemetryClient connection, string diagnosticChannelConnectionString)
        {
            var conn = new TelemetryConnection(connection);
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

        private void Connect(string connectionString)
        {
            connection.Connect(connectionString);
        }

        private void Disconnect()
        {
            connection.Disconnect();
        }

        protected bool OnlineStatus
        {
            get { return connection.OnlineStatus; }
        }
    }
}
