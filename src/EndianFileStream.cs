using System.IO;
using System.Text;

namespace WindWakerItemizer
{
    internal enum EFileEndianness
    {
        Big,
        Little
    }

    internal enum EFileMode
    {
        Read,
        Write
    }

    internal class EndianFileStream : IDisposable
    {
        public string FileName { get; set; }

        private EFileEndianness mEndianness;
        private FileStream? mStream;

        public EndianFileStream(string fileName, EFileMode mode = EFileMode.Read, EFileEndianness endianness = EFileEndianness.Big)
        {
            if (mode == EFileMode.Read && !File.Exists(fileName))
            {
                throw new FileNotFoundException("Unable to open specified file", fileName);
            }

            FileName = fileName;
            mEndianness = endianness;

            switch (mode)
            {
                case EFileMode.Read:
                    mStream = File.OpenRead(fileName);
                    break;
                case EFileMode.Write:
                    mStream = File.Open(fileName, FileMode.Create, FileAccess.Write);
                    break;
                default:
                    mStream = null;
                    break;
            }

            if (mStream == null || mStream.SafeFileHandle.IsInvalid)
            {
                throw new Exception("Failed to open file for reading!");
            }
        }

        #region Position Operations
        public void Seek(long offset, SeekOrigin seekOrigin = SeekOrigin.Begin)
        {
            if (mStream is not null && mStream.CanSeek) mStream.Seek(offset, seekOrigin);
        }

        public long Tell()
        {
            return mStream is not null ? mStream.Position : -1;
        }

        public void Skip(long amt)
        {
            Seek(amt, SeekOrigin.Current);
        }
        #endregion

        #region Reading
        public byte ReadByte()
        {
            return (mStream is not null && mStream.CanRead) ? (byte)mStream.ReadByte() : (byte)0;
        }

        public ushort ReadUShort()
        {
            if (mStream is null || !mStream.CanRead) return (ushort)0;

            byte[] buf = new byte[2];

            switch(mEndianness)
            {
                case EFileEndianness.Big:
                    buf[1] = ReadByte();
                    buf[0] = ReadByte();
                    break;
                case EFileEndianness.Little:
                    buf[0] = ReadByte();
                    buf[1] = ReadByte();
                    break;
            }

            return BitConverter.ToUInt16(buf, 0);
        }

        public short ReadShort()
        {
            if (mStream is null || !mStream.CanRead) return (short)0;

            byte[] buf = new byte[2];

            switch (mEndianness)
            {
                case EFileEndianness.Big:
                    buf[1] = ReadByte();
                    buf[0] = ReadByte();
                    break;
                case EFileEndianness.Little:
                    buf[0] = ReadByte();
                    buf[1] = ReadByte();
                    break;
            }

            return BitConverter.ToInt16(buf, 0);
        }

        public uint ReadUInt()
        {
            if (mStream is null || !mStream.CanRead) return (uint)0;

            byte[] buf = new byte[4];

            switch (mEndianness)
            {
                case EFileEndianness.Big:
                    buf[3] = ReadByte();
                    buf[2] = ReadByte();
                    buf[1] = ReadByte();
                    buf[0] = ReadByte();
                    break;
                case EFileEndianness.Little:
                    buf[0] = ReadByte();
                    buf[1] = ReadByte();
                    buf[2] = ReadByte();
                    buf[3] = ReadByte();
                    break;
            }

            return BitConverter.ToUInt32(buf, 0);
        }

        public int ReadInt()
        {
            if (mStream is null || !mStream.CanRead) return (int)0;

            byte[] buf = new byte[4];

            switch (mEndianness)
            {
                case EFileEndianness.Big:
                    buf[3] = ReadByte();
                    buf[2] = ReadByte();
                    buf[1] = ReadByte();
                    buf[0] = ReadByte();
                    break;
                case EFileEndianness.Little:
                    buf[0] = ReadByte();
                    buf[1] = ReadByte();
                    buf[2] = ReadByte();
                    buf[3] = ReadByte();
                    break;
            }

            return BitConverter.ToInt32(buf, 0);
        }

        public float ReadSingle()
        {
            if (mStream is null || !mStream.CanRead) return 0.0f;

            byte[] buf = new byte[4];

            switch (mEndianness)
            {
                case EFileEndianness.Big:
                    buf[3] = ReadByte();
                    buf[2] = ReadByte();
                    buf[1] = ReadByte();
                    buf[0] = ReadByte();
                    break;
                case EFileEndianness.Little:
                    buf[0] = ReadByte();
                    buf[1] = ReadByte();
                    buf[2] = ReadByte();
                    buf[3] = ReadByte();
                    break;
            }

            return BitConverter.ToSingle(buf, 0);
        }

        public string ReadString(Encoding? encoding = null)
        {
            if (mStream is null || !mStream.CanRead) return "";

            List<byte> buf = new List<byte>();

            byte curByte = ReadByte();
            while (curByte != 0)
            {
                buf.Add(curByte);
                curByte = ReadByte();
            }

            return encoding is null ? Encoding.UTF8.GetString(buf.ToArray()) : encoding.GetString(buf.ToArray());
        }

        public byte[] ReadBytes(int size)
        {
            if (mStream is null || !mStream.CanRead) return new byte[1];

            byte[] buf = new byte[size];
            mStream.ReadExactly(buf, 0, size);

            return buf;
        }
        #endregion

        #region Writing
        /// <summary>
        /// Writes the given byte to the stream.
        /// </summary>
        /// <param name="val">Byte value to write to the stream.</param>
        public void WriteByte(byte val)
        {
            if (mStream is not null && mStream.CanWrite) mStream.WriteByte(val);
        }

        /// <summary>
        /// Writes the given unsigned short to the stream.
        /// </summary>
        /// <param name="val">Unsigned short value to write to the stream.</param>
        public void WriteUShort(ushort val)
        {
            if (mStream is null || !mStream.CanWrite) return;

            byte[] buf = BitConverter.GetBytes(val);
            mStream.Write(mEndianness == EFileEndianness.Big ? buf.Reverse().ToArray() : buf, 0, sizeof(ushort));
        }

        /// <summary>
        /// Writes the given signed short to the stream.
        /// </summary>
        /// <param name="val">Signed short value to write to the stream.</param>
        public void WriteShort(short val)
        {
            if (mStream is null || !mStream.CanWrite) return;

            byte[] buf = BitConverter.GetBytes(val);
            mStream.Write(mEndianness == EFileEndianness.Big ? buf.Reverse().ToArray() : buf, 0, sizeof(short));
        }

        /// <summary>
        /// Writes the given unsigned 32-bit integer to the stream.
        /// </summary>
        /// <param name="val">Unsigned 32-bit integer value to write to the stream.</param>
        public void WriteUInt(uint val)
        {
            if (mStream is null || !mStream.CanWrite) return;

            byte[] buf = BitConverter.GetBytes(val);
            mStream.Write(mEndianness == EFileEndianness.Big ? buf.Reverse().ToArray() : buf, 0, sizeof(uint));
        }

        /// <summary>
        /// Writes the given signed short to the stream.
        /// </summary>
        /// <param name="val">Signed 32-bit integer value to write to the stream.</param>
        public void WriteInt(int val)
        {
            if (mStream is null || !mStream.CanWrite) return;

            byte[] buf = BitConverter.GetBytes(val);
            mStream.Write(mEndianness == EFileEndianness.Big ? buf.Reverse().ToArray() : buf, 0, sizeof(int));
        }

        /// <summary>
        /// Writes the given single to the stream.
        /// </summary>
        /// <param name="val">Single value to write to the stream.</param>
        public void WriteFloat(float val)
        {
            if (mStream is null || !mStream.CanWrite) return;

            byte[] buf = BitConverter.GetBytes(val);
            mStream.Write(mEndianness == EFileEndianness.Big ? buf.Reverse().ToArray() : buf, 0, sizeof(float));
        }

        /// <summary>
        /// Writes the given string to the stream. Uses the given encoding to produce the bytes to write, or UTF-8
        /// if the given encoding was null.
        /// </summary>
        /// <param name="val">String to write to the stream.</param>
        /// <param name="encoding">Encoding to use to convert the given string to bytes. UTF-8 is used if no encoding is provided.</param>
        public void WriteString(string val, Encoding? encoding = null)
        {
            if (mStream is null || !mStream.CanWrite) return;

            byte[] buf = encoding is null ? Encoding.UTF8.GetBytes(val.ToCharArray()) : encoding.GetBytes(val.ToCharArray());
            if (val.Length == 0) return;

            mStream.Write(buf, 0, val.Length);
        }

        public void WriteBytes(byte[] val, int size)
        {
            if (mStream is null || !mStream.CanWrite) return;

            mStream.Write(val, 0, size);
        }

        public void Pad(int count, byte val = 0)
        {
            long nextBoundary = (Tell() + (count - 1)) & ~(count - 1);
            long delta = nextBoundary - Tell();

            for (long i = 0; i < delta; i++)
            {
                WriteByte(val);
            }
        }
        #endregion

        #region IDisposable Implementation
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing && mStream is not null)
                {
                    mStream.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
