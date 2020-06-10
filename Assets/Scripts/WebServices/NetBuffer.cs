using System;

public class NetBuffer
{
    private const int DEFAULT_BUFF_SIZE = 1024;

    private UInt32 m_rpos = 0;
    private UInt32 m_wpos = 0;
    private byte[] m_buff = null;

    public NetBuffer(UInt32 size = DEFAULT_BUFF_SIZE) { _Reserve(size); }
    public byte[] Buffer { get {return m_buff;} }
    public UInt32 ReadPos { get { return m_rpos; } }
    public UInt32 WritePos { get { return m_wpos; } }
    public void Clear() { m_rpos = m_wpos = 0; }
    public UInt32 readableBytes() { return m_wpos - m_rpos; } //buff中还有多少数据
    public UInt32 writableBytes() { return (uint)m_buff.Length - m_wpos; } //剩余空间
	public UInt32 prependBytes() { return m_rpos; }
    public void readerMove(UInt32 len)
    {
        if (len < readableBytes())
            m_rpos += len;
        else
            m_rpos = m_wpos = 0; // 全部读完，回退至起点
    }
    public void writerMove(UInt32 len)
    {
        ensureWritableBytes(len);
        m_wpos += len;
    }
    public void ensureWritableBytes(UInt32 len) //确保剩余空间足够，一定能写入
    {
        if (writableBytes() < len) makeSpace(len);
    }
    public void makeSpace(UInt32 len)
	{
		if (writableBytes() + prependBytes() < len)
		{
            _Resize(m_wpos + len);
		}
		else
		{
			// move readable data to the front, make space inside buffer
			UInt32 oldReadable = readableBytes();
            Array.Copy(m_buff, m_rpos, m_buff, 0, oldReadable);
			m_wpos = oldReadable;
			m_rpos = 0;
		}
	}
    public void Write(byte[] src, UInt32 size)
    {
        ensureWritableBytes(size);
        Array.Copy(src, 0, m_buff, m_wpos, size);
        m_wpos += size;
    }
    public void Read(byte[] dest, UInt32 size)
    {
        if (readableBytes() >= size)
        {
            Array.Copy(m_buff, m_rpos, dest, 0, size);
            readerMove(size);
        }
    }
    public void WriteLength(UInt16 len)
    {
        ensureWritableBytes(sizeof(UInt16));
        WriteUInt16ToBuffer(len, m_buff, (int)m_wpos);
        m_wpos += sizeof(UInt16);
    }
    public UInt16 ReadLength()
    {
        UInt16 len = 0;
        if (readableBytes() >= sizeof(UInt16))
        {
            len = GetUint16FromBuffer(m_buff, (int)m_rpos);
            readerMove(sizeof(UInt16));
        }
        return len;
    }
    public UInt16 ShowLength()
    {
        return GetUint16FromBuffer(m_buff, (int)m_rpos);
    }
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

    public static void WriteUInt32ToBuffer(uint value, byte[] buffer, int index)
    {
        buffer[index] = (byte)value;
        buffer[index + 1] = (byte)(value >> 8);
        buffer[index + 2] = (byte)(value >> 16);
        buffer[index + 3] = (byte)(value >> 24);
    }
    public static void WriteInt32ToBuffer(int value, byte[] buffer, int index)
    {
        buffer[index] = (byte)value;
        buffer[index + 1] = (byte)(value >> 8);
        buffer[index + 2] = (byte)(value >> 16);
        buffer[index + 3] = (byte)(value >> 24);
    }
    public static void WriteInt64ToBuffer(Int64 value, byte[] buffer, int index)
    {
        buffer[index] = (byte)value;
        buffer[index + 1] = (byte)(value >> 8);
        buffer[index + 2] = (byte)(value >> 16);
        buffer[index + 3] = (byte)(value >> 24);
        buffer[index + 4] = (byte)(value >> 32);
        buffer[index + 5] = (byte)(value >> 40);
        buffer[index + 6] = (byte)(value >> 48);
        buffer[index + 7] = (byte)(value >> 56);
    }
    public static void WriteUInt64ToBuffer(UInt64 value, byte[] buffer, int index)
    {
        buffer[index] = (byte)value;
        buffer[index + 1] = (byte)(value >> 8);
        buffer[index + 2] = (byte)(value >> 16);
        buffer[index + 3] = (byte)(value >> 24);
        buffer[index + 4] = (byte)(value >> 32);
        buffer[index + 5] = (byte)(value >> 40);
        buffer[index + 6] = (byte)(value >> 48);
        buffer[index + 7] = (byte)(value >> 56);
    }
    public static void WriteInt16ToBuffer(short value, byte[] buffer, int index)
    {
        buffer[index] = (byte)value;
        buffer[index + 1] = (byte)(value >> 8);
    }
    public static void WriteUInt16ToBuffer(ushort value, byte[] buffer, int index)
    {
        buffer[index] = (byte)value;
        buffer[index + 1] = (byte)(value >> 8);
    }
    public static UInt16 GetUint16FromBuffer(byte[] buffer, int beginLen)
    {
        return (UInt16)((int)buffer[beginLen]| (int)buffer[beginLen + 1] << 8);
    }
    public static int GetIntFromBuffer(byte[] buffer, int beginLen)
    {
        return (int)buffer[beginLen]
                | (int)buffer[beginLen + 1] << 8
                | (int)buffer[beginLen + 2] << 16
                | (int)buffer[beginLen + 3] << 24;
    }
}