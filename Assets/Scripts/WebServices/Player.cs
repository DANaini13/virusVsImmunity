using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Tcp
{
    public class Player : UnityEngine.MonoBehaviour
    {
        public static Player Inst => USingleton<Player>.Inst;
        public uint m_connId;
        public void Rpc_svr_accept(NetPack recvBuf, NetPack backBuf, TcpConn conn) {
            m_connId = recvBuf.ReadUInt32()+10000; //防止同Mirror内部connId重复
            NetLogic.Inst.svr.CallRpc(RpcEnum.Rpc_check_identity, req => {
                req.WriteUInt32(m_connId);
            }, ack => {
                if (ack.ReadUInt16() == 1) {
                } else {
                }
            });
        }

        //---------------------------------房间逻辑----------------------------------------------
        public void Rpc_client_on_other_join_team(NetPack recvBuf, NetPack backBuf, TcpConn conn) {
        }
        public void Rpc_client_on_exit_team(NetPack recvBuf, NetPack backBuf, TcpConn conn) {
        }
        public void Rpc_client_handle_spawn_attrs(NetPack recvBuf, NetPack backBuf, TcpConn conn) {
        }
    }
}
