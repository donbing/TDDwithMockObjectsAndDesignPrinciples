
using System;

namespace TDDMicroExercises.TelemetrySystem
{
    public class TelemetryDiagnosticControls
    {
        private const string DiagnosticChannelConnectionString = "*111#";
        
        private readonly ITelemetryChannel channel;
        private readonly TelemetryConnection connection;
        private string diagnosticInfo = string.Empty;

        public TelemetryDiagnosticControls(ITelemetryChannel client, TelemetryConnection connection)
        {
            channel = client;
            this.connection = connection;
        }

        public string DiagnosticInfo
        {
            get { return diagnosticInfo; }
            set { diagnosticInfo = value; }
        }

        public void CheckTransmission()
        {
            diagnosticInfo = string.Empty;

            connection.TryConnect(3, DiagnosticChannelConnectionString);

            channel.Send(TelemetryClient.DiagnosticMessage);

            diagnosticInfo = channel.Receive();
        }
    }

    public class TelemetryConnection
    {
        private readonly IConnection connection;

        public TelemetryConnection(IConnection connection)
        {
            this.connection = connection;
        }
        
        public void TryConnect(int retryCount, string diagnosticChannelConnectionString)
        {
            Disconnect();

            while (OnlineStatus == false && retryCount > 0)
            {
                Connect(diagnosticChannelConnectionString);
                retryCount -= 1;
            }

            if (OnlineStatus == false)
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
