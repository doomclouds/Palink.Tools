using Palink.Tools.Communication.Interface;

namespace Palink.Tools.Communication.Message
{
    /// <summary>
    /// 基本消息体
    /// </summary>
    public class BaseMessage : IMessage
    {
        /// <summary>
        /// 发送区
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// 接收区
        /// </summary>
        public byte[] Buffer { get; set; }

        /// <summary>
        /// 读取字节数
        /// </summary>
        public int ReadBytes { get; set; } = 13;

        /// <summary>
        /// 发送字节数
        /// </summary>
        public int SendBytes { get; set; } = 13;
    }
}
