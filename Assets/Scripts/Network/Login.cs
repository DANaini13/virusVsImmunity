using UnityEngine;

public class NetLogic : MonoBehaviour
{
    public static NetLogic Inst => USingleton<NetLogic>.Inst;

    public TcpLink svr = new TcpLink();

    readonly string SvrRelayIp = "52.82.37.128";
    //readonly string SvrRelayIp = "192.168.1.111";

    public void Init(uint connId)
    {
        svr.Connect(SvrRelayIp, 9999, (err, str) => {
            if (err == 0) {
                var firstMsg = new byte[4]; 
                NetBuffer.WriteUInt32ToBuffer(connId, firstMsg, 0);
                svr.SendBuf(firstMsg); //首包，上报connId
            } else {
                //Debug.LogError(str);
            }
        });
    }
    public void CallRpc(int pid, RpcEnum rid, ParseParam func) {
        svr.CallRpc(RpcEnum.Rpc_relay_to_other, buf => {
            buf.WriteInt32(pid);
            buf.WriteUInt16((ushort)rid);
            func(buf);
        });
    }

    public void WriteToSvr(RpcEnum pid, ParseParam writeFunc) {
        svr.CallRpc(pid, writeFunc);
    }
}
