using System;
using UnityEngine;

public class ByteBuffer
{
    public const UInt32 DEFAULT_BUFF_SIZE = 256;

	protected byte[] __InOut_buf = new byte[128];
    protected UInt32 m_rpos = 0;
    protected UInt32 m_wpos = 0;
    protected byte[] m_buff = null;

    public ByteBuffer(UInt32 size = DEFAULT_BUFF_SIZE) { _Reserve(size); }
    public ByteBuffer(ByteBuffer r) { _Write(r.Buffer, r.m_wpos); }
    public ByteBuffer(byte[] buf) { m_buff = buf; m_wpos = (uint)buf.Length; }
    public ByteBuffer(byte[] buf, UInt32 offset, UInt32 size) { _Write(buf, offset, size); }
    protected void SetPosRead(UInt32 pos) { m_rpos = pos; }
    public byte[] Buffer { get {return m_buff;} }
    public byte[] LeftBuf
    {
        get
        {
            var ptr = new byte[m_wpos - m_rpos];
            Array.Copy(m_buff, m_rpos, ptr, 0, ptr.Length);
            return ptr;
        }
    }
    public UInt32 Size { get { return m_wpos; } }
    public void Clear(UInt32 pos = 0) { m_rpos = m_wpos = pos; }
    void _Resize(UInt32 newsize)
    {
        if (m_buff == null || newsize > m_buff.Length)
        {
            _Reserve(newsize);
        }
    }
    void _Reserve(UInt32 newsize)
    {
        if (m_buff == null)
        {
            m_buff = new byte[newsize];
        }
        else if (newsize > m_buff.Length)
        {
            newsize = (newsize > (UInt32)m_buff.Length * 2) ? newsize : (UInt32)(m_buff.Length * 2);

            byte[] tempbuff = m_buff; // copy the content to the tempbuff
            m_buff = new byte[newsize];
            Array.Copy(tempbuff, m_buff, m_wpos);
            tempbuff = null;
        }
    }
    public void _Write(byte[] src, UInt32 size) {
        _Resize(m_wpos + size);
        Array.Copy(src, 0, m_buff, m_wpos, size);
        m_wpos += size;
    }
    public void _Write(byte[] src, UInt32 offset, UInt32 size) {
        _Resize(m_wpos + size);
        Array.Copy(src, offset, m_buff, m_wpos, size);
        m_wpos += size;
    }
    public void _Read(byte[] dest, UInt32 size) {
        Array.Clear(dest, 0, dest.Length);
        if (m_rpos + size <= m_wpos) {
            Array.Copy(m_buff, m_rpos, dest, 0, size);
            m_rpos += size;
        }
    }

///////////////////////////////////////////////////////////////////////////////////////////////////////
// api
    public void WriteBool(bool value) {
        __InOut_buf[0] = value ? (byte)1 : (byte)0;
        _Write(__InOut_buf, sizeof(bool));
    }
    public void WriteInt8(sbyte value) {
        __InOut_buf[0] = (byte)value;
        _Write(__InOut_buf, sizeof(sbyte));
    }
    public void WriteUInt8(byte value) { WriteByte(value); }
    public void WriteByte(byte value)
    {
        __InOut_buf[0] = value;
        _Write(__InOut_buf, sizeof(byte));
    }
    public void WriteInt16(Int16 value) {
        NetBuffer.WriteInt16ToBuffer(value, __InOut_buf, 0);
        _Write(__InOut_buf, sizeof(Int16));
    }
    public void WriteUInt16(UInt16 value) {
        NetBuffer.WriteUInt16ToBuffer(value, __InOut_buf, 0);
        _Write(__InOut_buf, sizeof(UInt16));
    }
    public void WriteInt32(Int32 value) {
        NetBuffer.WriteInt32ToBuffer(value, __InOut_buf, 0);
        _Write(__InOut_buf, sizeof(Int32));
    }
    public void WriteUInt32(UInt32 value) {
        NetBuffer.WriteUInt32ToBuffer(value, __InOut_buf, 0);
        _Write(__InOut_buf, sizeof(UInt32));
    }
    public void WriteInt64(Int64 value) {
        NetBuffer.WriteInt64ToBuffer(value, __InOut_buf, 0);
        _Write(__InOut_buf, sizeof(Int64));
    }
    public void WriteUInt64(UInt64 value) {
        NetBuffer.WriteUInt64ToBuffer(value, __InOut_buf, 0);
        _Write(__InOut_buf, sizeof(UInt64));
    }
    public void WriteFloat(float value) {
        //Utility.WriteFloatToBuffer(value, __InOut_buf, 0);
        //_Write(__InOut_buf, sizeof(float));
        _Write(BitConverter.GetBytes(value), sizeof(float));
    }
    public void WriteDouble(double value) {
        _Write(BitConverter.GetBytes(value), sizeof(float));
    }
    public void WriteString(string value) {
        UInt16 size = (UInt16)System.Text.Encoding.UTF8.GetByteCount(value);
        WriteUInt16(size);
        if (size > 0) _Write(System.Text.Encoding.UTF8.GetBytes(value), size);
    }
    public void WriteBuf(byte[] uid)
    {
        _Write(uid, (uint)uid.Length);
    }


    public sbyte ReadInt8() {
        _Read(__InOut_buf, sizeof(sbyte));
        return (sbyte)__InOut_buf[0];
    }
    public bool ReadBool() {
		_Read(__InOut_buf, sizeof(bool));
        return BitConverter.ToBoolean(__InOut_buf, 0);
    }
    public byte ReadUInt8() {
		_Read(__InOut_buf, sizeof(byte));
        return __InOut_buf[0];
    }
    public byte ReadByte() {
        _Read(__InOut_buf, sizeof(byte));
        return __InOut_buf[0];
    }
    public Int16 ReadInt16() {
		_Read(__InOut_buf, sizeof(Int16));
        return BitConverter.ToInt16(__InOut_buf, 0);;
    }
    public UInt16 ReadUInt16() {
		_Read(__InOut_buf, sizeof(UInt16));
        return BitConverter.ToUInt16(__InOut_buf, 0);
    }
    public Int32 ReadInt32() {
		_Read(__InOut_buf, sizeof(Int32));
        return BitConverter.ToInt32(__InOut_buf, 0);
    }
    public UInt32 ReadUInt32() {
		_Read(__InOut_buf, sizeof(UInt32));
        return BitConverter.ToUInt32(__InOut_buf, 0);
    }
    public Int64 ReadInt64() {
		_Read(__InOut_buf, sizeof(Int64));
        return BitConverter.ToInt64(__InOut_buf, 0);
    }
    public UInt64 ReadUInt64() {
		_Read(__InOut_buf, sizeof(UInt64));
        return BitConverter.ToUInt64(__InOut_buf, 0);
    }
    public float ReadFloat() {
		_Read(__InOut_buf, sizeof(float));
        return BitConverter.ToSingle(__InOut_buf, 0);
    }
    public double ReadDouble() {
		_Read(__InOut_buf, sizeof(double));
        return BitConverter.ToDouble(__InOut_buf, 0);
    }
    public string ReadString() {
        UInt16 size = ReadUInt16();
        if (size > 0) {
            byte[] temp = new byte[size];
            _Read(temp, size);
            //value = System.Text.Encoding.ASCII.GetString(temp);
            return System.Text.Encoding.UTF8.GetString(temp);
        }
        return String.Empty;
    }

    enum AttrType : byte
    {
        T_BOOL = 0,
        T_INT8 = 1,
        T_INT16 = 2,
        T_INT32 = 3,
        T_UINT8 = 4,
        T_UINT16 = 5,
        T_UINT32 = 6,
        T_FLOAT = 7,
        T_DOUBLE = 8,
        T_STRING = 9,
        T_UUID = 10,
        T_VEC2F = 11,
        T_FACING = 12,
    }

    public object ReadByTypeId(byte typeByte)
    {
        switch((AttrType)typeByte)
        {
            case AttrType.T_BOOL: return ReadBool();
            case AttrType.T_INT8: return ReadInt8();
            case AttrType.T_INT16: return ReadInt16();
            case AttrType.T_INT32: return ReadInt32();
            case AttrType.T_UINT8: return ReadUInt8();
            case AttrType.T_UINT16: return ReadUInt16();
            case AttrType.T_UINT32: return ReadUInt32();
            case AttrType.T_FLOAT: return ReadFloat();
            case AttrType.T_DOUBLE: return ReadDouble();
            case AttrType.T_STRING: return ReadString();
            //case AttrType.T_UUID: return ReadUuid();
            case AttrType.T_VEC2F: {
                float x = ReadFloat();
                float y = ReadFloat();
                return new Vector2(x, y);
            }
            case AttrType.T_FACING: return ReadBool();
            default:
                throw new Exception(string.Format("NetPack: unkown type when reading data {0}", typeByte));
        }
    }

    //public UUID ReadUuid() {
    //    var ret = new UUID();
    //    _Read(ret._data, (uint)UUID.static_size);
    //    return ret;
    //}
    //public byte[] ReadUuid()
    //{
    //    byte[] buf = new byte[NetID.kSize];
    //    _Read(buf, (uint)NetID.kSize);
    //    return buf;
    //}

    //! Set
    public void SetPos(int pos, UInt32 v)
    {
        if (m_wpos >= pos+4) NetBuffer.WriteUInt32ToBuffer(v, m_buff, pos);
    }
    public UInt32 GetPos(int pos)
    {
        if (m_wpos >= pos + 4) return BitConverter.ToUInt32(m_buff, pos);

        return 0;
    }
}

public class NetPack : ByteBuffer
{
    public const UInt32 HEADER_SIZE = sizeof(byte) + sizeof(UInt16) + sizeof(UInt32); // packetType & Opcode & reqIdx
    const UInt32 TYPE_INDEX      = 0;
    const UInt32 OPCODE_INDEX    = 1;
    const UInt32 REQ_IDX_INDEX   = 3;

    public const byte TYPE_TCP = 0;
    public const byte TYPE_UDP = 136;
    public const byte TYPE_UNRELIABLE = 137;

    public NetPack(UInt16 size = 64)
        : base(size + HEADER_SIZE) 
    {
        Clear(HEADER_SIZE);
        Type = TYPE_TCP;
    }
    public NetPack(byte[] src, UInt32 srcIdx, UInt32 len)
        : base(src, srcIdx, len)
    {
        SetPosRead(HEADER_SIZE); //挪出头部，剩消息体
    }
    public NetPack(byte[] src) : base(src) { SetPosRead(HEADER_SIZE); }
    public byte[] Body
    {
        get
        {
            var ptr = new byte[BodySize];
            Array.Copy(m_buff, HEADER_SIZE, ptr, 0, BodySize);
            return ptr;
        }
    }
    public uint BodySize { get { return Size - HEADER_SIZE; } }
    public void Clear() { Clear(HEADER_SIZE); MsgId = 0; Type = TYPE_TCP; }
    public void ResetHead(NetPack other) {
        base.Clear();
        _Write(other.Buffer, HEADER_SIZE);
        SetPosRead(HEADER_SIZE);
    }
    public UInt64 GetReqKey() { return (UInt64)MsgId<<32 | ReqIdx; }
    public UInt16 MsgId
    {
        get { return NetBuffer.GetUint16FromBuffer(base.Buffer, (int)OPCODE_INDEX); }
        set { NetBuffer.WriteUInt16ToBuffer(value, base.Buffer, (int)OPCODE_INDEX); }
    }
    public byte Type
    {
        get { return base.Buffer[TYPE_INDEX]; }
        set { base.Buffer[TYPE_INDEX] = value; }
    }
    public UInt32 ReqIdx
    {
        get { return (UInt32)NetBuffer.GetIntFromBuffer(base.Buffer, (int)REQ_IDX_INDEX); }
        set { NetBuffer.WriteUInt32ToBuffer(value, base.Buffer, (int)REQ_IDX_INDEX); }
    }
}
