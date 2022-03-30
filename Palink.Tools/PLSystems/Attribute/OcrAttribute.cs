namespace Palink.Tools.PLSystems.Attribute;

internal class OcrAttribute : System.Attribute
{
    public string ReadName { get; set; }
    public string WriteName { get; set; }
    public bool Readable { get; set; }
    public bool Writable { get; set; }

    public OcrAttribute()
    {
        Readable = true;
        Writable = true;
    }
}