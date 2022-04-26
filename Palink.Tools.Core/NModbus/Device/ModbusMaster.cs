using System;
using System.Linq;
using System.Threading.Tasks;
using Palink.Tools.NModbus.Contracts;
using Palink.Tools.NModbus.Data;
using Palink.Tools.NModbus.Interfaces;
using Palink.Tools.NModbus.Message;

namespace Palink.Tools.NModbus.Device;

internal class ModbusMaster : ModbusDevice, IModbusMaster
{
    public ModbusMaster(IModbusTransport transport) : base(transport)
    {
    }

    /// <summary>
    ///    Reads from 1 to 2000 contiguous coils status.
    /// </summary>
    /// <param name="slaveAddress">Address of device to read values from.</param>
    /// <param name="startAddress">Address to begin reading.</param>
    /// <param name="numberOfPoints">Number of coils to read.</param>
    /// <returns>Coils status.</returns>
    public bool[] ReadCoils(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
    {
        ValidateNumberOfPoints(nameof(numberOfPoints), numberOfPoints, 2000);

        var request = new ReadCoilsInputsRequest(ModbusFunctionCodes.ReadCoils,
            slaveAddress,
            startAddress,
            numberOfPoints);

        return PerformReadDiscretes(request);
    }

    /// <summary>
    ///    Asynchronously reads from 1 to 2000 contiguous coils status.
    /// </summary>
    /// <param name="slaveAddress">Address of device to read values from.</param>
    /// <param name="startAddress">Address to begin reading.</param>
    /// <param name="numberOfPoints">Number of coils to read.</param>
    /// <returns>A task that represents the asynchronous read operation.</returns>
    public Task<bool[]> ReadCoilsAsync(byte slaveAddress, ushort startAddress,
        ushort numberOfPoints)
    {
        ValidateNumberOfPoints(nameof(numberOfPoints), numberOfPoints, 2000);

        var request = new ReadCoilsInputsRequest(ModbusFunctionCodes.ReadCoils,
            slaveAddress,
            startAddress,
            numberOfPoints);

        return PerformReadDiscretesAsync(request);
    }

    /// <summary>
    ///    Reads from 1 to 2000 contiguous discrete input status.
    /// </summary>
    /// <param name="slaveAddress">Address of device to read values from.</param>
    /// <param name="startAddress">Address to begin reading.</param>
    /// <param name="numberOfPoints">Number of discrete inputs to read.</param>
    /// <returns>Discrete inputs status.</returns>
    public bool[] ReadInputs(byte slaveAddress, ushort startAddress,
        ushort numberOfPoints)
    {
        ValidateNumberOfPoints(nameof(numberOfPoints), numberOfPoints, 2000);

        var request = new ReadCoilsInputsRequest(ModbusFunctionCodes.ReadInputs,
            slaveAddress,
            startAddress,
            numberOfPoints);

        return PerformReadDiscretes(request);
    }

    /// <summary>
    ///    Asynchronously reads from 1 to 2000 contiguous discrete input status.
    /// </summary>
    /// <param name="slaveAddress">Address of device to read values from.</param>
    /// <param name="startAddress">Address to begin reading.</param>
    /// <param name="numberOfPoints">Number of discrete inputs to read.</param>
    /// <returns>A task that represents the asynchronous read operation.</returns>
    public Task<bool[]> ReadInputsAsync(byte slaveAddress, ushort startAddress,
        ushort numberOfPoints)
    {
        ValidateNumberOfPoints(nameof(numberOfPoints), numberOfPoints, 2000);

        var request = new ReadCoilsInputsRequest(ModbusFunctionCodes.ReadCoils,
            slaveAddress,
            startAddress,
            numberOfPoints);

        return PerformReadDiscretesAsync(request);
    }

    /// <summary>
    ///    Reads contiguous block of holding registers.
    /// </summary>
    /// <param name="slaveAddress">Address of device to read values from.</param>
    /// <param name="startAddress">Address to begin reading.</param>
    /// <param name="numberOfPoints">Number of holding registers to read.</param>
    /// <returns>Holding registers status.</returns>
    public ushort[] ReadHoldingRegisters(byte slaveAddress, ushort startAddress,
        ushort numberOfPoints)
    {
        ValidateNumberOfPoints(nameof(numberOfPoints), numberOfPoints, 125);

        var request = new ReadHoldingInputRegistersRequest(
            ModbusFunctionCodes.ReadHoldingRegisters,
            slaveAddress,
            startAddress,
            numberOfPoints);

        return PerformReadRegisters(request);
    }

    /// <summary>
    ///    Asynchronously reads contiguous block of holding registers.
    /// </summary>
    /// <param name="slaveAddress">Address of device to read values from.</param>
    /// <param name="startAddress">Address to begin reading.</param>
    /// <param name="numberOfPoints">Number of holding registers to read.</param>
    /// <returns>A task that represents the asynchronous read operation.</returns>
    public Task<ushort[]> ReadHoldingRegistersAsync(byte slaveAddress,
        ushort startAddress,
        ushort numberOfPoints)
    {
        ValidateNumberOfPoints(nameof(numberOfPoints), numberOfPoints, 125);

        var request = new ReadHoldingInputRegistersRequest(
            ModbusFunctionCodes.ReadHoldingRegisters,
            slaveAddress,
            startAddress,
            numberOfPoints);

        return PerformReadRegistersAsync(request);
    }

    /// <summary>
    ///    Reads contiguous block of input registers.
    /// </summary>
    /// <param name="slaveAddress">Address of device to read values from.</param>
    /// <param name="startAddress">Address to begin reading.</param>
    /// <param name="numberOfPoints">Number of holding registers to read.</param>
    /// <returns>Input registers status.</returns>
    public ushort[] ReadInputRegisters(byte slaveAddress, ushort startAddress,
        ushort numberOfPoints)
    {
        ValidateNumberOfPoints(nameof(numberOfPoints), numberOfPoints, 125);

        var request = new ReadHoldingInputRegistersRequest(
            ModbusFunctionCodes.ReadInputRegisters,
            slaveAddress,
            startAddress,
            numberOfPoints);

        return PerformReadRegisters(request);
    }

    /// <summary>
    ///    Asynchronously reads contiguous block of input registers.
    /// </summary>
    /// <param name="slaveAddress">Address of device to read values from.</param>
    /// <param name="startAddress">Address to begin reading.</param>
    /// <param name="numberOfPoints">Number of holding registers to read.</param>
    /// <returns>A task that represents the asynchronous read operation.</returns>
    public Task<ushort[]> ReadInputRegistersAsync(byte slaveAddress, ushort startAddress,
        ushort numberOfPoints)
    {
        ValidateNumberOfPoints(nameof(numberOfPoints), numberOfPoints, 125);

        var request = new ReadHoldingInputRegistersRequest(
            ModbusFunctionCodes.ReadInputRegisters,
            slaveAddress,
            startAddress,
            numberOfPoints);

        return PerformReadRegistersAsync(request);
    }

    /// <summary>
    ///    Writes a single coil value.
    /// </summary>
    /// <param name="slaveAddress">Address of the device to write to.</param>
    /// <param name="coilAddress">Address to write value to.</param>
    /// <param name="value">Value to write.</param>
    public void WriteSingleCoil(byte slaveAddress, ushort coilAddress, bool value)
    {
        var request =
            new WriteSingleCoilRequestResponse(slaveAddress, coilAddress, value);
        Transport.UnicastMessage<WriteSingleCoilRequestResponse>(request);
    }

    /// <summary>
    ///    Asynchronously writes a single coil value.
    /// </summary>
    /// <param name="slaveAddress">Address of the device to write to.</param>
    /// <param name="coilAddress">Address to write value to.</param>
    /// <param name="value">Value to write.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    public Task WriteSingleCoilAsync(byte slaveAddress, ushort coilAddress, bool value)
    {
        var request =
            new WriteSingleCoilRequestResponse(slaveAddress, coilAddress, value);
        return PerformWriteRequestAsync<WriteSingleCoilRequestResponse>(request);
    }

    /// <summary>
    ///    Writes a single holding register.
    /// </summary>
    /// <param name="slaveAddress">Address of the device to write to.</param>
    /// <param name="registerAddress">Address to write.</param>
    /// <param name="value">Value to write.</param>
    public void WriteSingleRegister(byte slaveAddress, ushort registerAddress,
        ushort value)
    {
        var request = new WriteSingleRegisterRequestResponse(slaveAddress,
            registerAddress,
            value);

        Transport.UnicastMessage<WriteSingleRegisterRequestResponse>(request);
    }

    /// <summary>
    ///    Asynchronously writes a single holding register.
    /// </summary>
    /// <param name="slaveAddress">Address of the device to write to.</param>
    /// <param name="registerAddress">Address to write.</param>
    /// <param name="value">Value to write.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    public Task WriteSingleRegisterAsync(byte slaveAddress, ushort registerAddress,
        ushort value)
    {
        var request = new WriteSingleRegisterRequestResponse(slaveAddress,
            registerAddress,
            value);

        return PerformWriteRequestAsync<WriteSingleRegisterRequestResponse>(request);
    }

    /// <summary>
    ///    Writes a block of 1 to 123 contiguous registers.
    /// </summary>
    /// <param name="slaveAddress">Address of the device to write to.</param>
    /// <param name="startAddress">Address to begin writing values.</param>
    /// <param name="data">Values to write.</param>
    public void WriteMultipleRegisters(byte slaveAddress, ushort startAddress,
        ushort[] data)
    {
        ValidateData(nameof(data), data, 123);

        var request = new WriteMultipleRegistersRequest(slaveAddress,
            startAddress,
            new RegisterCollection(data));

        Transport.UnicastMessage<WriteMultipleRegistersResponse>(request);
    }

    /// <summary>
    ///    Asynchronously writes a block of 1 to 123 contiguous registers.
    /// </summary>
    /// <param name="slaveAddress">Address of the device to write to.</param>
    /// <param name="startAddress">Address to begin writing values.</param>
    /// <param name="data">Values to write.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    public Task WriteMultipleRegistersAsync(byte slaveAddress, ushort startAddress,
        ushort[] data)
    {
        ValidateData(nameof(data), data, 123);

        var request = new WriteMultipleRegistersRequest(slaveAddress,
            startAddress,
            new RegisterCollection(data));

        return PerformWriteRequestAsync<WriteMultipleRegistersResponse>(request);
    }

    /// <summary>
    ///    Writes a sequence of coils.
    /// </summary>
    /// <param name="slaveAddress">Address of the device to write to.</param>
    /// <param name="startAddress">Address to begin writing values.</param>
    /// <param name="data">Values to write.</param>
    public void WriteMultipleCoils(byte slaveAddress, ushort startAddress, bool[] data)
    {
        ValidateData(nameof(data), data, 1968);

        var request = new WriteMultipleCoilsRequest(slaveAddress,
            startAddress,
            new DiscreteCollection(data));

        Transport.UnicastMessage<WriteMultipleCoilsResponse>(request);
    }

    /// <summary>
    ///    Asynchronously writes a sequence of coils.
    /// </summary>
    /// <param name="slaveAddress">Address of the device to write to.</param>
    /// <param name="startAddress">Address to begin writing values.</param>
    /// <param name="data">Values to write.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    public Task WriteMultipleCoilsAsync(byte slaveAddress, ushort startAddress,
        bool[] data)
    {
        ValidateData(nameof(data), data, 1968);

        var request = new WriteMultipleCoilsRequest(slaveAddress,
            startAddress,
            new DiscreteCollection(data));

        return PerformWriteRequestAsync<WriteMultipleCoilsResponse>(request);
    }

    /// <summary>
    /// Write a file record to the device.
    /// </summary>
    /// <param name="slaveAddress">Address of device to write values to</param>
    /// <param name="fileNumber">The Extended Memory file number</param>
    /// <param name="startingAddress">The starting register address within the file</param>
    /// <param name="data">The data to be written</param>
    public void WriteFileRecord(byte slaveAddress, ushort fileNumber,
        ushort startingAddress,
        byte[] data)
    {
        ValidateMaxData(nameof(data), data, 244);
        var request = new WriteFileRecordRequest(slaveAddress,
            new FileRecordCollection(fileNumber, startingAddress, data));

        Transport.UnicastMessage<WriteFileRecordResponse>(request);
    }

    /// <summary>
    ///    Executes the custom message.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="request">The request.</param>
    public TResponse ExecuteCustomMessage<TResponse>(IModbusMessage request)
        where TResponse : IModbusMessage, new()
    {
        return Transport.UnicastMessage<TResponse>(request);
    }

    #region Share

    private static void ValidateNumberOfPoints(string argumentName, ushort numberOfPoints,
        ushort maxNumberOfPoints)
    {
        if (numberOfPoints >= 1 && numberOfPoints <= maxNumberOfPoints)
        {
            return;
        }

        var msg =
            $"Argument {argumentName} must be between 1 and {maxNumberOfPoints} inclusive.";
        throw new ArgumentException(msg);
    }

    private bool[] PerformReadDiscretes(ReadCoilsInputsRequest request)
    {
        var response =
            Transport.UnicastMessage<ReadCoilsInputsResponse>(request);
        if (response.Data == null)
            throw new ArgumentNullException(nameof(response.Data), "Data dose not null");
        if (!request.NumberOfPoints.HasValue)
            throw new ArgumentNullException(nameof(request.NumberOfPoints),
                "NumberOfPoints dose not null");
        return response.Data.Take(request.NumberOfPoints.Value).ToArray();
    }

    private Task<bool[]> PerformReadDiscretesAsync(ReadCoilsInputsRequest request)
    {
        return Task.Run(() => PerformReadDiscretes(request));
    }

    private ushort[] PerformReadRegisters(ReadHoldingInputRegistersRequest request)
    {
        var response =
            Transport.UnicastMessage<ReadHoldingInputRegistersResponse>(request);

        if (response.Data == null)
            throw new ArgumentNullException(nameof(response.Data), "Data dose not null");
        if (!request.NumberOfPoints.HasValue)
            throw new ArgumentNullException(nameof(request.NumberOfPoints),
                "NumberOfPoints dose not null");
        return response.Data.Take(request.NumberOfPoints.Value).ToArray();
    }

    private Task<ushort[]> PerformReadRegistersAsync(
        ReadHoldingInputRegistersRequest request)
    {
        return Task.Run(() => PerformReadRegisters(request));
    }

    private Task PerformWriteRequestAsync<T>(IModbusMessage request)
        where T : IModbusMessage, new()
    {
        return Task.Run(() => Transport.UnicastMessage<T>(request));
    }

    private static void ValidateData<T>(string argumentName, T[] data, int maxDataLength)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        if (data.Length == 0 || data.Length > maxDataLength)
        {
            var msg =
                $"The length of argument {argumentName} must be between 1 and {maxDataLength} inclusive.";
            throw new ArgumentException(msg);
        }
    }

    private static void ValidateMaxData<T>(string argumentName, T[] data,
        int maxDataLength)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        if (data.Length > maxDataLength)
        {
            var msg =
                $"The length of argument {argumentName} must not be greater than {maxDataLength}.";
            throw new ArgumentException(msg);
        }
    }

    #endregion
}