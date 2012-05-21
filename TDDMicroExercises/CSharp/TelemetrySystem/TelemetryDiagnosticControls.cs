
using System;

namespace TDDMicroExercises.TelemetrySystem
{
    public class TelemetryDiagnosticControls
    {
        private const string DiagnosticChannelConnectionString = "*111#";
        
        private readonly TelemetryClient telemetryClient;
        private string diagnosticInfo = string.Empty;

        public TelemetryDiagnosticControls(TelemetryClient client)
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

            telemetryClient.Disconnect();

            int retryLeft = 3;
            while (telemetryClient.OnlineStatus == false && retryLeft > 0)
            {
                telemetryClient.Connect(DiagnosticChannelConnectionString);
                retryLeft -= 1;
            }
             
            if(telemetryClient.OnlineStatus == false)
            {
                throw new Exception("Unable to connect.");
            }

            telemetryClient.Send(TelemetryClient.DiagnosticMessage);
            diagnosticInfo = telemetryClient.Receive();
        }
    }
}
