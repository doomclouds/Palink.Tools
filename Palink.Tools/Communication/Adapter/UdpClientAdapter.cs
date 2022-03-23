using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using Palink.Tools.Communication.Interface;
using Palink.Tools.Communication.Utility;

namespace Palink.Tools.Communication.Adapter
{
    /// <summary>
    /// UdpClientAdapter
    /// </summary>
    public class UdpClientAdapter : IStreamResource
    {
        // strategy for cross platform r/w
        private const int MaxBufferSize = ushort.MaxValue;
        private readonly byte[] _buffer = new byte[MaxBufferSize];
        private int _bufferOffset;
        private UdpClient _udpClient;

        /// <summary>
        /// UdpClientAdapter
        /// </summary>
        /// <param name="udpClient"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UdpClientAdapter(UdpClient udpClient)
        {
            _udpClient = udpClient ?? throw new ArgumentNullException(nameof(udpClient));
        }

        /// <summary>
        /// InfiniteTimeout
        /// </summary>
        public int InfiniteTimeout => Timeout.Infinite;

        /// <summary>
        /// 读数据超时时间
        /// </summary>
        public int ReadTimeout
        {
            get => _udpClient.Client.ReceiveTimeout;
            set => _udpClient.Client.ReceiveTimeout = value;
        }

        /// <summary>
        /// 写数据超时时间
        /// </summary>
        public int WriteTimeout
        {
            get => _udpClient.Client.SendTimeout;
            set => _udpClient.Client.SendTimeout = value;
        }

        /// <summary>
        /// 清空缓存
        /// </summary>
        public void DiscardInBuffer()
        {
            // no-op
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));

            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset),
                    "Argument offset must be greater than or equal to 0.");

            if (offset > buffer.Length)
                throw new ArgumentOutOfRangeException(nameof(offset),
                    "Argument offset cannot be greater than the length of buffer.");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count),
                    "Argument count must be greater than or equal to 0.");

            if (count > buffer.Length - offset)
                throw new ArgumentOutOfRangeException(nameof(count),
                    "Argument count cannot be greater than the length of buffer minus offset.");

            if (count > 0)
                _bufferOffset =
                    _udpClient.Client.Receive(_buffer, offset, count, SocketFlags.None);


            Buffer.BlockCopy(_buffer, offset, buffer, offset, count);

            if (_bufferOffset < count)
            {
                return _bufferOffset;
            }

            return count;
        }

        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public void Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));

            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset),
                    "Argument offset must be greater than or equal to 0.");

            if (offset > buffer.Length)
                throw new ArgumentOutOfRangeException(nameof(offset),
                    "Argument offset cannot be greater than the length of buffer.");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count),
                    "Argument count must be greater than or equal to 0.");

            if (count > buffer.Length - offset)
                throw new ArgumentOutOfRangeException(nameof(count),
                    "Argument count cannot be greater than the length of buffer minus offset.");

            _udpClient.Client.Send(buffer.Skip(offset).Take(count).ToArray());
        }

        /// <summary>
        /// 释放对象
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放对象
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing) DisposableUtility.Dispose(ref _udpClient);
        }
    }
}