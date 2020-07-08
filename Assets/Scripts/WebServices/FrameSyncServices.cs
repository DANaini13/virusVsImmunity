using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class FrameSyncServices : MonoBehaviour
{
    public static FrameSyncServices Inst => USingleton<FrameSyncServices>.Inst;
    private NetLogic netLogic;
    private List<string> operationList;
    private string operationSum = "";
    // Start is called before the first frame update
    void Start()
    {
        netLogic = NetLogic.Inst;
        Rpc.Inst.RegistRpc(Tcp.Player.Inst);
        NetLogic.Inst.Init(Tcp.Player.Inst.m_connId);
        operationList = new List<string>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartFight()
    {
        netLogic.WriteToSvr(RpcEnum.Rpc_relay_lockstep_begin, (NetPack pack) => {});
    }

    public void DoInput(string operation) {
        operationList.Add(operation);
        operationSum = operationSum + operation;
        int hash = GetMD5Hash(operationSum);

        netLogic.WriteToSvr(RpcEnum.Rpc_relay_report_input, (NetPack pack) => {
            pack.WriteString(operation);
            pack.WriteInt32(hash);
        });
    }

    public void EndFight()
    {
        netLogic.WriteToSvr(RpcEnum.Rpc_relay_lockstep_end, (NetPack pack) => {});
    }

    private int GetMD5Hash(string input)
    {
        MD5 md5Hasher = MD5.Create();
        byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
        int result = 0;
        for (int i = 0; i < data.Length; i++)
        {
            result += data[i];
        }
        return result;
    }
}
