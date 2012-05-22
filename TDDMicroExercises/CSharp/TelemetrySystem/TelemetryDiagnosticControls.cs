
using System;

namespace TDDMicroExercises.TelemetrySystem
{
    public class TelemetryDiagnosticControls
    {
        private const string DiagnosticChannelConnectionString = "*111#";
        
        private readonly ITelemetryChannel channel;
        private readonly TelemetryConnector connection;
        private string diagnosticInfo = string.Empty;

        public TelemetryDiagnosticControls(ITelemetryChannel client, TelemetryConnector connection)
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

    public class TelemetryConnector
    {
        private readonly IConnection connection;

        public TelemetryConnector(IConnection connection)
        {
            this.connection = connection;
        }
        
        public void TryConnect(int retryCount, string diagnosticChannelConnectionString)
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
