using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Palink.Tools.IO;
using Palink.Tools.Logging;
using Palink.Tools.NModbus.Device;
using Palink.Tools.NModbus.Device.MessageHandlers;
using Palink.Tools.NModbus.Extensions;
using Palink.Tools.NModbus.Interfaces;
using Palink.Tools.NModbus.IO;

namespace Palink.Tools.NModbus;

public class ModbusFactory : IModbusFactory
{
    /// <summary>
    /// The "built-in" message handlers.
    /// </summary>
    private static readonly IModbusFunctionService[] BuiltInFunctionServices =
    {
        new ReadCoilsService(),
        new ReadInputsService(),
        new ReadHoldingRegistersService(),
        new ReadInputRegistersService(),
        new WriteSingleCoilService(),
        new WriteSingleRegisterService(),
        new WriteMultipleRegistersService(),
        new WriteMultipleCoilsService(),
        new WriteFileRecordService(),
        new DiagnosticsService(),
    };

    private readonly IDictionary<byte, IModbusFunctionService> _functionServices;

    public ModbusFactory()
    {
        _functionServices =
            BuiltInFunctionServices.ToDictionary(s => s.FunctionCode, s => s);
        Logger = new DebugFreebusLogger();
    }

    /// <summary>
    /// Create a factory which optionally uses the built in function services and allows custom services to be added.
    /// </summary>
    /// <param name="functionServices">User provided function services.</param>
    /// <param name="includeBuiltIn">If true, the built in function services are included. Otherwise, all function services will come from the functionService parameter.</param>
    /// <param name="logger">Logger</param>
    public ModbusFactory(IEnumerable<IModbusFunctionService>? functionServices = null,
        bool includeBuiltIn = true,
        IFreebusLogger? logger = null)
    {
        Logger = logger ?? NullFreebusLogger.Instance;

        //Determine if we're including the built in services
        if (includeBuiltIn)
        {
            //Make a dictionary out of the built in services
            _functionServices = BuiltInFunctionServices
                .ToDictionary(s => s.FunctionCode, s => s);
        }
        else
        {
            //Create an empty dictionary
            _functionServices = new Dictionary<byte, IModbusFunctionService>();
        }

        if (functionServices != null)
        {
            //Add and replace the provided function services as necessary.
            foreach (var service in functionServices)
            {
                //This will add or replace the service.
                _functionServices[service.FunctionCode] = service;
            }
        }
    }

    /// <summary>
    /// Get the service for a given function code.
    /// </summary>
    /// <param name="functionCode"></param>
    /// <returns></returns>
    public IModbusFunctionService? GetFunctionService(byte functionCode)
    {
        return _functionServices.GetValueOrDefault(functionCode);
    }

    /// <summary>
    /// Gets all of the services.
    /// </summary>
    /// <returns></returns>
    public IModbusFunctionService[] GetAllFunctionServices()
    {
        return _functionServices
            .Values
            .ToArray();
    }

    /// <summary>
    /// Create an rtu master.
    /// </summary>
    /// <param name="transport"></param>
    /// <returns></returns>
    public IModbusSerialMaster CreateMaster(IModbusSerialTransport transport)
    {
        return new ModbusSerialMaster(transport);
    }

    /// <summary>
    /// Creates an RTU transport. 
    /// </summary>
    /// <param name="streamResource"></param>
    /// <returns></returns>
    public IModbusRtuTransport CreateRtuTransport(IStreamResource streamResource)
    {
        return new ModbusRtuTransport(streamResource, this, Logger);
    }

    public IModbusAsciiTransport CreateAsciiTransport(IStreamResource streamResource)
    {
        return new ModbusAsciiTransport(streamResource, this, Logger);
    }

    public IModbusMaster CreateMaster(Socket client)
    {
        var adapter = new SocketAdapter(client);

        var transport = new ModbusRtuTransport(adapter, this, Logger);

        return new ModbusSerialMaster(transport);
    }

    public IModbusMaster CreateMaster(UdpClient client)
    {
        var adapter = new UdpClientAdapter(client);

        var transport = new ModbusIpTransport(adapter, this, Logger);

        return new ModbusIpMaster(transport);
    }

    public IModbusMaster CreateMaster(TcpClient client)
    {
        var adapter = new TcpClientAdapter(client);

        var transport = new ModbusIpTransport(adapter, this, Logger);

        return new ModbusIpMaster(transport);
    }

    public IFreebusLogger Logger { get; }
}