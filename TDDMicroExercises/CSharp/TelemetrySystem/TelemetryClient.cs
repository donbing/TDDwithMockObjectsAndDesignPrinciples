using System;

namespace TDDMicroExercises.TelemetrySystem
{
    public interface IConnection
    {
        bool OnlineStatus { get; }
        void Connect(string telemetryServerConnectionString);
        void Disconnect();
    }

    public interface ITelemetryChannel
    {
        void Send(string message);
        string Receive();
    }

    public class TelemetryConnection : IConnection
    {
        protected readonly Random ConnectionEventsSimulator = new Random(42);

        public bool OnlineStatus { get; private set; }

        public void Connect(string telemetryServerConnectionString)
        {
            if (string.IsNullOrEmpty(telemetryServerConnectionString))
            {
                throw new ArgumentNullException();
            }

            // simulate the operation on a real modem
            var success = ConnectionEventsSimulator.Next(1, 10) <= 8;

            OnlineStatus = success;
        }

        public void Disconnect()
        {
            OnlineStatus = false;
        }
    }

    public class TelemetryClient : ITelemetryChannel
    {
        public const string DiagnosticMessage = "AT#UD";

        private readonly Random randomGenerator = new Random(42);
        private string diagnosticMessageResult = string.Empty;

        public void Send(string message)
		{
			if (string.IsNullOrEmpty(message))
			{
				throw new ArgumentNullException();
			}

			if (message == DiagnosticMessage)
			{
				// simulate a status report
				diagnosticMessageResult =
					  "LAST TX rate................ 100 MBPS\r\n"
					+ "HIGHEST TX rate............. 100 MBPS\r\n"
					+ "LAST RX rate................ 100 MBPS\r\n"
					+ "HIGHEST RX rate............. 100 MBPS\r\n"
					+ "BIT RATE.................... 100000000\r\n"
					+ "WORD LEN.................... 16\r\n"
					+ "WORD/FRAME.................. 511\r\n"
					+ "BITS/FRAME.................. 8192\r\n"
					+ "MODULATION TYPE............. PCM/FM\r\n"
					+ "TX Digital Los.............. 0.75\r\n"
					+ "RX Digital Los.............. 0.10\r\n"
					+ "BEP Test.................... -5\r\n"
					+ "Local Rtrn Count............ 00\r\n"
					+ "Remote Rtrn Count........... 00";

				return;
			}
	
			// here should go the real Send operation
		}

		public string Receive()
		{
			string message;

			if (string.IsNullOrEmpty(diagnosticMessageResult) == false)
			{
				message = diagnosticMessageResult;
				diagnosticMessageResult = string.Empty;
			} 
			else
			{                
				// simulate a received message
				message = string.Empty;
				int messageLenght = randomGenerator.Next(50, 110);
				for(int i = messageLenght; i >=0; --i)
				{
					message += (char)randomGenerator.Next(40, 126);
				}
			}

			return message;
		}
	}
}
