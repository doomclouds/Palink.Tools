using System.Net.Sockets;
using Palink.Tools.IO;
using Palink.Tools.Logging;

namespace Palink.Tools.NModbus.Interfaces;

/// <summary>
/// Container for modbus function services.
/// </summary>
public interface IModbusFactory
{
    /// <summary>
    /// Get the service for a given function code.
    /// </summary>
    /// <param name="functionCode"></param>
    /// <returns></returns>
    IModbusFunctionService? GetFunctionService(byte functionCode);

    /// <summary>
    /// Gets all of the services.
    /// </summary>
    /// <returns></returns>
    IModbusFunctionService[] GetAllFunctionServices();

    #region Master

    /// <summary>
    /// Create an rtu master.
    /// </summary>
    /// <param name="transport"></param>
    /// <returns></returns>
    IModbusSerialMaster CreateMaster(IModbusSerialTransport transport);

    /// <summary>
    /// Create a TCP master Use Udp
    /// </summary>
    /// <param name="client"></param>
    /// <returns></returns>
    IModbusMaster CreateMaster(UdpClient client);

    /// <summary>
    /// Create a TCP master.
    /// </summary>
    /// <param name="client"></param>
    /// <returns></returns>
    IModbusMaster CreateMaster(TcpClient client);

    /// <summary>
    /// Creates an RTU transport. 
    /// </summary>
    /// <param name="streamResource"></param>
    /// <returns></returns>
    IModbusRtuTransport CreateRtuTransport(IStreamResource streamResource);

    /// <summary>
    /// Creates an Ascii Transport.
    /// </summary>
    /// <param name="streamResource"></param>
    /// <returns></returns>
    IModbusAsciiTransport CreateAsciiTransport(IStreamResource streamResource);

    #endregion

    IFreebusLogger Logger { get; }
}