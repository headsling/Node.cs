using System;
using System.Net.Sockets;
namespace NodeCS.EventLoop
{
    public interface ISocketHandler
    {
        void OnRead( ILoop loop, Socket socket );
        void OnWriteable( ILoop loop, Socket socket );
        void OnError( ILoop loop, Socket socket );
    }
}

