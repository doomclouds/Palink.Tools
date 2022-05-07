using System;
using Palink.Tools.NModbus.Extensions.Enron;
using Palink.Tools.NModbus.Extensions.Functions;
using Palink.Tools.NModbus.Interfaces;

namespace Palink.Tools.NModbus.Extensions;

/// <summary>
/// Utility Class to support Modbus 32/64bit devices. 
/// </summary>
internal class ModbusMasterEnhanced
{
    private readonly IModbusMaster _master;
    private readonly uint _wordSize;
    private readonly Func<byte[], byte[]> _endian;
    private readonly bool _wordSwapped;


    /// <summary>
    /// Constructor with values to be used by all methods. 
    /// Default is 32bit, LittleEndian, with words wrapping.
    /// </summary>
    /// <param name="master">The Modbus master</param>
    /// <param name="wordSize">Word size used by device. 16/32/64 are valid.</param>
    /// <param name="endian">The endian encoding of the device.</param>
    /// <param name="wordSwapped">Should the ushort words mirrored then flattened to bytes.</param>
    public ModbusMasterEnhanced(IModbusMaster master, uint wordSize = 32,
        Func<byte[], byte[]>? endian = null, bool wordSwapped = false)
    {
        _master = master;
        _wordSize = wordSize;
        _endian = endian ?? Endian.LittleEndian;
        _wordSwapped = wordSwapped;
    }

    /// <summary>
    /// Reads registers and converts the result into a char array.
    /// </summary>
    /// <param name="slaveAddress">Address of device to read values from.</param>
    /// <param name="startAddress">Address to begin reading.</param>
    /// <param name="numberOfPoints">Number of chars to read.</param>
    /// <returns></returns>
    public char[]
        ReadCharHoldingRegisters(byte slaveAddress, ushort startAddress,
            ushort numberOfPoints) => RegisterFunctions.ByteValueArraysToChars(
        RegisterFunctions.ReadRegisters(slaveAddress, startAddress,
            numberOfPoints, _master, _wordSize, _endian, _wordSwapped));

    /// <summary>
    /// Reads registers and converts the result into a ushort array.
    /// </summary>
    /// <param name="slaveAddress">Address of device to read values from.</param>
    /// <param name="startAddress">Address to begin reading.</param>
    /// <param name="numberOfPoints">Number of ushorts to read.</param>
    /// <returns></returns>
    public ushort[]
        ReadUshortHoldingRegisters(byte slaveAddress, ushort startAddress,
            ushort numberOfPoints) => RegisterFunctions.ByteValueArraysToUShorts(
        RegisterFunctions.ReadRegisters(slaveAddress, startAddress,
            numberOfPoints, _master, _wordSize, _endian,
            _wordSwapped));

    /// <summary>
    /// Reads registers and converts the result into a short array.
    /// </summary>
    /// <param name="slaveAddress">Address of device to read values from.</param>
    /// <param name="startAddress">Address to begin reading.</param>
    /// <param name="numberOfPoints">Number of shots to read.</param>
    /// <returns></returns>
    public short[]
        ReadShortHoldingRegisters(byte slaveAddress, ushort startAddress,
            ushort numberOfPoints) => RegisterFunctions.ByteValueArraysToShorts(
        RegisterFunctions.ReadRegisters(slaveAddress, startAddress,
            numberOfPoints, _master, _wordSize, _endian, _wordSwapped));

    /// <summary>
    /// Reads registers and converts the result into a uint array.
    /// </summary>
    /// <param name="slaveAddress">Address of device to read values from.</param>
    /// <param name="startAddress">Address to begin reading.</param>
    /// <param name="numberOfPoints">Number of uints to read.</param>
    /// <returns></returns>
    public uint[]
        ReadUintHoldingRegisters(byte slaveAddress, ushort startAddress,
            ushort numberOfPoints) => RegisterFunctions.ByteValueArraysToUInts(
        RegisterFunctions.ReadRegisters(slaveAddress, startAddress,
            numberOfPoints, _master, _wordSize, _endian, _wordSwapped));

    /// <summary>
    /// Reads registers and converts the result into a int array.
    /// </summary>
    /// <param name="slaveAddress">Address of device to read values from.</param>
    /// <param name="startAddress">Address to begin reading.</param>
    /// <param name="numberOfPoints">Number of ints to read.</param>
    /// <returns></returns>
    public int[]
        ReadIntHoldingRegisters(byte slaveAddress, ushort startAddress,
            ushort numberOfPoints) => RegisterFunctions.ByteValueArraysToInts(
        RegisterFunctions.ReadRegisters(slaveAddress, startAddress,
            numberOfPoints, _master, _wordSize, _endian, _wordSwapped));

    /// <summary>
    /// Reads registers and converts the result into a float array.
    /// </summary>
    /// <param name="slaveAddress">Address of device to read values from.</param>
    /// <param name="startAddress">Address to begin reading.</param>
    /// <param name="numberOfPoints">Number of floats to read.</param>
    /// <returns></returns>
    public float[]
        ReadFloatHoldingRegisters(byte slaveAddress, ushort startAddress,
            ushort numberOfPoints) => RegisterFunctions.ByteValueArraysToFloats(
        RegisterFunctions.ReadRegisters(slaveAddress, startAddress,
            numberOfPoints, _master, _wordSize, _endian, _wordSwapped));

    /// <summary>
    ///     Write a char array to registers.
    /// </summary>
    /// <param name="slaveAddress">Address of the device to write to.</param>
    /// <param name="startAddress">Address to start writing values at.</param>
    /// <param name="data">Chars to write to device.</param>
    public void WriteCharHoldingRegisters(byte slaveAddress, ushort startAddress,
        char[] data) => RegisterFunctions.WriteRegistersFunc(slaveAddress,
        startAddress,
        RegisterFunctions.CharsToByteValueArrays(data, _wordSize),
        _master,
        _wordSize,
        _endian, _wordSwapped);

    /// <summary>
    ///     Write a ushort array to registers.
    /// </summary>
    /// <param name="slaveAddress">Address of the device to write to.</param>
    /// <param name="startAddress">Address to start writing values at.</param>
    /// <param name="data">Ushorts to write to device.</param>
    public void WriteUshortHoldingRegisters(byte slaveAddress, ushort startAddress,
        ushort[] data) => RegisterFunctions.WriteRegistersFunc(slaveAddress,
        startAddress,
        RegisterFunctions.UShortsToByteValueArrays(data, _wordSize),
        _master,
        _wordSize,
        _endian, _wordSwapped);

    /// <summary>
    ///     Write a short array to registers.
    /// </summary>
    /// <param name="slaveAddress">Address of the device to write to.</param>
    /// <param name="startAddress">Address to start writing values at.</param>
    /// <param name="data">Shorts to write to device.</param>
    public void WriteShortHoldingRegisters(byte slaveAddress, ushort startAddress,
        short[] data) => RegisterFunctions.WriteRegistersFunc(slaveAddress,
        startAddress,
        RegisterFunctions.ShortsToByteValueArrays(data, _wordSize),
        _master,
        _wordSize,
        _endian, _wordSwapped);

    /// <summary>
    ///     Write a int array to registers.
    /// </summary>
    /// <param name="slaveAddress">Address of the device to write to.</param>
    /// <param name="startAddress">Address to start writing values at.</param>
    /// <param name="data">Ints to write to device.</param>
    public void WriteIntHoldingRegisters(byte slaveAddress, ushort startAddress,
        int[] data) => RegisterFunctions.WriteRegistersFunc(slaveAddress,
        startAddress,
        RegisterFunctions.IntToByteValueArrays(data, _wordSize),
        _master,
        _wordSize,
        _endian, _wordSwapped);

    /// <summary>
    ///     Write a uint array to registers.
    /// </summary>
    /// <param name="slaveAddress">Address of the device to write to.</param>
    /// <param name="startAddress">Address to start writing values at.</param>
    /// <param name="data">Uints to write to device.</param>
    public void WriteUIntHoldingRegisters(byte slaveAddress, ushort startAddress,
        uint[] data) => RegisterFunctions.WriteRegistersFunc(slaveAddress,
        startAddress,
        RegisterFunctions.UIntToByteValueArrays(data, _wordSize),
        _master,
        _wordSize,
        _endian, _wordSwapped);

    /// <summary>
    ///     Write a float array to registers.
    /// </summary>
    /// <param name="slaveAddress">Address of the device to write to.</param>
    /// <param name="startAddress">Address to start writing values at.</param>
    /// <param name="data">Floats to write to device.</param>
    public void WriteFloatHoldingRegisters(byte slaveAddress, ushort startAddress,
        float[] data) => RegisterFunctions.WriteRegistersFunc(slaveAddress,
        startAddress,
        RegisterFunctions.FloatToByteValueArrays(data, _wordSize),
        _master,
        _wordSize,
        _endian, _wordSwapped);
}