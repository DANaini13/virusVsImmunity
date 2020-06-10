using System;
using System.Net;
using System.Net.Sockets;

public class TcpConn
{
    const int kNetBufferSize = 1024 * 4;
    protected NetBuffer m_recvBuf = new NetBuffer(kNetBufferSize * 2);
    protected NetBuffer m_sendBuf = new NetBuffer(kNetBufferSize);
    protected bool m_bCanSend;
    protected Socket m_socket;

    public void ResetConn(Socket fd) {
        fd.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        fd.NoDelay = true;
        m_socket = fd;
        m_bCanSend = true;
    }
    public void Close()
    {
        m_sendBuf.Clear();
        m_recvBuf.Clear();
        m_bCanSend = false;
        if (m_socket != null) {
            m_socket.Close();
            m_socket = null;
        }
    }
    public string RemoteIp { get { return m_socket.RemoteEndPoint.ToString(); } }
    public int LocalPort { get { return ((IPEndPoint)m_socket.LocalEndPoint).Port; } }

    // ------------------------------------------------------------
    public void CallRpc(RpcEnum rid, ParseParam func) {
        NetPack req = Rpc.Inst.MakeReq(rid, func, null);
        SendMsg(req.Buffer, (ushort)req.Size);
    }
    public void CallRpc(RpcEnum rid, ParseParam func, ParseParam callback) {
        NetPack req = Rpc.Inst.MakeReq(rid, func, callback);
        SendMsg(req.Buffer, (ushort)req.Size);
    }

    // ------------------------------------------------------------
    // 接收流程
    public void BeginRecvSocket()
    {
        try
        {
            m_recvBuf.ensureWritableBytes(1);
            m_socket.BeginReceive(m_recvBuf.Buffer,
                (int)m_recvBuf.WritePos,
                (int)m_recvBuf.writableBytes(),
                SocketFlags.None,
                asyncResult =>
                {
                    try
                    {
                        int recvLen = m_socket.EndReceive(asyncResult);
                        if (recvLen > 0) OnRecvDoneIO(recvLen);
                        if (recvLen >= 0) BeginRecvSocket();
                    }
                    catch (Exception e)
                    {
                        OnException(e);
                    }
                }, this);
        }
        catch (Exception e)
        {
            OnException(e);
        }
    }
    void OnRecvDoneIO(int bytes)
    {
        m_recvBuf.writerMove((UInt32)bytes);

        const UInt32 kHeadSize = sizeof(UInt16);
        while (m_recvBuf.readableBytes() >= kHeadSize)
        {
            // 【网络包：头2字节为消息体大小】
            // 【网络包长 = 消息体大小 + 头长度】
            UInt32 msgSize = m_recvBuf.ShowLength();
            UInt32 packSize = msgSize + kHeadSize;

            if (packSize > m_recvBuf.readableBytes()) break; // 【包未收完：接收字节 < 包大小】

            // 【后移2字节得：消息体指针】
            Rpc.Inst._InsertMsg(this, m_recvBuf.Buffer, m_recvBuf.ReadPos + kHeadSize, msgSize);

            m_recvBuf.readerMove(packSize);
        }
    }

    // ------------------------------------------------------------
    // 发送流程
    object _sendLock = new object();
    public void SendMsg(byte[] msg, ushort size)
    {
        lock (_sendLock)
        {
            m_sendBuf.WriteLength(size);
            m_sendBuf.Write(msg, size);

            if (m_bCanSend && m_sendBuf.readableBytes() > 0) BeginSendSocket();
        }
    }
    public void SendBuf(byte[] buf)
    {
        lock (_sendLock)
        {
            m_sendBuf.Write(buf, (uint)buf.Length);
            if (m_bCanSend && m_sendBuf.readableBytes() > 0) BeginSendSocket();
        }
    }
    void OnSendDoneIO(int bytesSend)
    {
        lock (_sendLock)
        {
            m_bCanSend = true;
            m_sendBuf.readerMove((UInt32)bytesSend);

            if (m_sendBuf.readableBytes() > 0) BeginSendSocket();
        }
    }
    void BeginSendSocket()
    {
        SocketError errorCode = SocketError.Success;
        m_bCanSend = false;
        m_socket.BeginSend(
            m_sendBuf.Buffer,
            (int)m_sendBuf.ReadPos,
            (int)m_sendBuf.readableBytes(),
            SocketFlags.None,
            out errorCode,
            asyncResult =>
            {
                try
                {
                    int n = m_socket.EndSend(asyncResult);
                    if (n > 0) OnSendDoneIO(n);
                }
                catch (Exception e)
                {
                    OnException(e);
                }
            },
            m_socket);
        if (errorCode != SocketError.Success) OnNetLost();
    }

    // ------------------------------------------------------------
    // 异常
    protected void OnException(Exception e)
    {
        UnityEngine.Debug.Log(string.Format("net exception error: {0}", e));
        if (e is SocketException)
        {
            SocketException se = e as SocketException;
            if (se.NativeErrorCode.Equals(10035) == false)
            {
                // net lost
                switch (se.SocketErrorCode)
                {
                    case SocketError.ConnectionReset:
                        UnityEngine.Debug.Log("lost connect..");
                        Close();
                        // ObjectManager.GoLogin();
                        break;
                    default:
                        OnNetLost();
                        break;
                }
            }
        }
    }
    virtual protected void OnNetLost()
    {
        Close();
    }
}
