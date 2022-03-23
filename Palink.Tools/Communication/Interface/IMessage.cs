namespace Palink.Tools.Communication.Interface
{
    /// <summary>
    /// 信息存储类
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// 发送区
        /// </summary>
        byte[] Data { get; set; }

        /// <summary>
        /// 接收区
        /// </summary>
        byte[] Buffer { get; set; }

        /// <summary>
        /// 读取字节数
        /// </summary>
        int ReadBytes { get; set; }

        /// <summary>
        /// 发送字节数
        /// </summary>
        int SendBytes { get; set; }
    }
}