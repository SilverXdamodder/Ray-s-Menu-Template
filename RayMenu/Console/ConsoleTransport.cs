using System;

namespace RayMenu.Console
{
    public interface IConsoleTransport
    {
        bool Connected { get; }
        void Connect();
        void Disconnect();
        void Send(string kind, string payload);
        event Action<string, string> Received;
    }

    public class LocalTransport : IConsoleTransport
    {
        public bool Connected { get; private set; }

        public event Action<string, string> Received;

        public void Connect() => Connected = true;

        public void Disconnect() => Connected = false;

        public void Send(string kind, string payload)
        {
            var handler = Received;
            if (handler != null) handler(kind, payload);
        }
    }
}
