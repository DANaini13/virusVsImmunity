using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public delegate void RecvMsgCallback(byte[] src, UInt32 srcIdx, UInt32 len);
public delegate void ConnectCallback(SocketError e, string error);
public delegate void DisconnectCallback(string ip);

public class TcpLink : TcpConn
{
    string m_ip;
    ushort m_port;

    bool m_bEnableReconnect = true;
    DisconnectCallback m_disconnectCallback;
    ConnectCallback m_connectCallback;

    public bool IsConnected { get; private set; }

    public void Connect(string ip, ushort port, int localPort, ConnectCallback resultCallback)
    {
        if (m_socket != null) Close();
        ResetConn(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
        m_socket.Bind(new IPEndPoint(IPAddress.Any, localPort));
        _connect(ip, port, resultCallback);
    }
    public void Connect(string ip, ushort port, ConnectCallback resultCallback) {
        if (m_socket != null) Close();
        ResetConn(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
        _connect(ip, port, resultCallback);
    }
    public void _connect(string ip, ushort port, ConnectCallback resultCallback)
    {
        m_ip = ip; m_port = port;
        m_connectCallback = resultCallback;
        Debug.Log("connect to ip: " + ip);
        m_socket.BeginConnect(IPAddress.Parse(ip), port,
            asyncResult =>
            {
                try
                {
                    m_socket.EndConnect(asyncResult);
                    IsConnected = true;
                    m_bCanSend = true;
                    m_connectCallback(0, "");
                    BeginRecvSocket();
                }
                catch (SocketException e)
                {
                    switch (e.SocketErrorCode)
                    {
                        //case SocketError.ConnectionRefused:
                        //case SocketError.TimedOut:
                        //case SocketError.HostNotFound:
                        //    m_connectCallback(e.SocketErrorCode, "服务器正在维护中");
                        //    break;
                        default:
                            m_connectCallback(e.SocketErrorCode, e.Message);
                            break;
                    }
                }
            },
            m_socket);
    }

    // ------------------------------------------------------------
    // 异常
    override protected void OnNetLost()
    {
        UnityEngine.Debug.Log(string.Format("Client net <{0}:{1}> lost", m_ip, m_port));
        if (IsConnected) m_disconnectCallback?.Invoke(m_ip);
        Close();

        if (m_bEnableReconnect && m_socket == null)
        {
            UnityEngine.Debug.Log(string.Format("Reconnect to server {0}:{1}", this.m_ip, this.m_port));
            Connect(m_ip, m_port, m_connectCallback);
        }
    }
    // ------------------------------------------------------------
    // 辅助函数
    public void SetAutoReconnect(bool b)
    {
        m_bEnableReconnect = b;
    }
    public void SetDisconnectCallback(DisconnectCallback callback)
    {
        m_disconnectCallback = callback;
    }
}
