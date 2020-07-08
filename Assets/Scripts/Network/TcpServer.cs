using System;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class TcpServer : MonoBehaviour
{
    public static TcpServer Inst => USingleton<TcpServer>.Inst;

    public TcpListener listener;
    Thread listenerThread;

    public bool Active => listenerThread != null && listenerThread.IsAlive;

    public bool Listen(int port)
    {
        if (Active) return false;
        listenerThread = new Thread(() => { _listen(port); });
        listenerThread.IsBackground = true;
        listenerThread.Priority = System.Threading.ThreadPriority.BelowNormal;
        listenerThread.Start();
        return true;
    }
    public void Stop()
    {
        if (!Active) return;
        listener?.Stop();
        listenerThread?.Interrupt();
        listenerThread = null;
    }
    void _listen(int port)
    {
        try
        {
            listener = TcpListener.Create(port);
            listener.Server.NoDelay = true;
            listener.Server.SendTimeout = 5000;
            listener.Start();
            while (true) {
                TcpClient client = listener.AcceptTcpClient();
                client.NoDelay = true;
                client.SendTimeout = 5000;
                var conn = new TcpConn();
                conn.ResetConn(client.Client);
                conn.BeginRecvSocket();
            }
        }
        catch (ThreadAbortException exception)
        {
            Debug.Log("Server thread aborted. That's okay. " + exception);
        }
        catch (SocketException exception)
        {
            Debug.Log("Server Thread stopped. That's okay. " + exception);
        }
        catch (Exception exception)
        {
            Debug.LogError("Server Exception: " + exception);
        }
    }
}