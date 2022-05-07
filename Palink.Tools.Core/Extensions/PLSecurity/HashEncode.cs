using System;
using System.Security.Cryptography;
using System.Text;
using Palink.Tools.Extensions.PLRandom;

namespace Palink.Tools.Extensions.PLSecurity;

public static class HashEncode
{
    public static string HashSecurity(this Random r) =>
        HashEncoding(r.StrictNext().ToString());

    public static string HashEncoding(this string security)
    {
        var code = new UnicodeEncoding();
        var message = code.GetBytes(security);
        using var arithmetic = new SHA512Managed();
        var value = arithmetic.ComputeHash(message);
        var sb = new StringBuilder();
        foreach (var o in value)
        {
            sb.Append((int)o + "O");
        }

        return sb.ToString();
    }
}