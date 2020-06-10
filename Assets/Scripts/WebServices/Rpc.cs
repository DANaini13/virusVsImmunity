using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

public delegate void ParseParam(NetPack buf);

public class Rpc : MonoBehaviour
{
    public static Rpc Inst => USingleton<Rpc>.Inst;

    object              m_rpcObj;
    MethodInfo[]        m_rpcLst = new MethodInfo[(int)RpcEnum.RpcEnumCnt];
    Dictionary<UInt64, ParseParam> m_responses = new Dictionary<UInt64, ParseParam>();

    //统计网络延时
    Dictionary<UInt64, long> m_rpcBeginTime = new Dictionary<UInt64, long>();
    int     m_netDelay;
    uint    m_upTraffic; //统计上行流量
    uint    m_dnTraffic; //统计下行流量
    float   m_netStatsTime;

    public void RegistRpc(object obj)
    {
        m_rpcObj = obj;
        foreach (var method in obj.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)) {
            if (method.Name.StartsWith("Rpc_")) {
                m_rpcLst[(ushort)Enum.Parse(typeof(RpcEnum), method.Name)] = method;
            }
        }
    }
    UInt32 _auto_req_idx = 0;
    NetPack m_SendBuffer = new NetPack(128);
    //public FlatBufferBuilder SendBuilder = new FlatBufferBuilder(32);
    public NetPack MakeReq(RpcEnum rid, ParseParam sendFun, ParseParam recvFun)
    {
        if (recvFun != null && m_rpcLst[(ushort)rid] != null) {
            UnityEngine.Debug.LogError(string.Format("Server and Client have the same Rpc({0}) \n", rid.ToString()));
            return null;
        }
        m_SendBuffer.Clear();
        m_SendBuffer.MsgId = (UInt16)rid;
        m_SendBuffer.ReqIdx = ++_auto_req_idx;
        sendFun(m_SendBuffer);

        m_upTraffic += m_SendBuffer.Size;

        if (recvFun != null) {
            var reqKey = m_SendBuffer.GetReqKey();
            m_responses[reqKey] = recvFun;
            m_rpcBeginTime[reqKey] = DateTime.Now.Ticks;
        }
        return m_SendBuffer;
    }

    NetPack m_BackBuffer = new NetPack(128);
    //public FlatBufferBuilder BackBuilder = new FlatBufferBuilder(32);
    void Update()
    {
        ParseParam callback;
        KeyValuePair<TcpConn, NetPack> pair;
        while (m_queue.TryDequeue(out pair))
        {
            NetPack msg = pair.Value; TcpConn conn = pair.Key;
            MethodInfo method = m_rpcLst[msg.MsgId];
            var reqKey = msg.GetReqKey();
            if (method != null) {
                m_BackBuffer.ResetHead(msg);
                method.Invoke(m_rpcObj, new object[] { msg, m_BackBuffer, conn });
                // var buf = BackBuilder.DataBuffer;
                // int len = buf.Data.Length - buf.Position;
                // if (len > 0) {
                //     m_BackBuffer._Write(buf.Data, (UInt32)buf.Position, (UInt32)len);
                //     BackBuilder.Clear();
                // }
                if (m_BackBuffer.BodySize > 0) conn.SendMsg(m_BackBuffer.Buffer, (ushort)m_BackBuffer.Size);
            }
            else if (m_responses.TryGetValue(reqKey, out callback))
            {
                int delay = (int)((DateTime.Now.Ticks - m_rpcBeginTime[reqKey])/10000); m_rpcBeginTime.Remove(reqKey);
                if (Math.Abs(delay-m_netDelay) > 20) {
                    m_netDelay = delay;
                    //GameObject.Find("HUDCanvas/DebugPanel/NetDelay").Get<Text>().text = string.Format("Net delay: {0}, rpcId({1})", delay, msg.MsgId);
                }
                callback(msg);
                m_responses.Remove(reqKey);
            }
            else
                UnityEngine.Debug.LogError(string.Format("OpCode({0}) have none Rpc or Responses\n", msg.MsgId));
        }

        //网络流量统计
        const int kTimeLong = 2;
        if ((m_netStatsTime += Time.deltaTime) > kTimeLong) {
            //GameObject.Find("HUDCanvas/DebugPanel/NetUpTraffic").Get<Text>().text = string.Format("Up traffic: {0}", m_upTraffic/kTimeLong);
            //GameObject.Find("HUDCanvas/DebugPanel/NetDnTraffic").Get<Text>().text = string.Format("Dn traffic: {0}", m_dnTraffic/kTimeLong);
            m_upTraffic = 0; m_dnTraffic = 0; m_netStatsTime = 0;
        }
    }
    public void _InsertMsg(TcpConn conn, byte[] src, UInt32 srcIdx, UInt32 len)
    {
        m_dnTraffic += len;
        var item = new KeyValuePair<TcpConn, NetPack>(conn, new NetPack(src, srcIdx, len));
        m_queue.Enqueue(item);
        //Debug.LogFormat("MsgId:{0}", recvBuf.OpCode);
    }
    public ConcurrentQueue<KeyValuePair<TcpConn, NetPack>> m_queue = new ConcurrentQueue<KeyValuePair<TcpConn, NetPack>>();
}
