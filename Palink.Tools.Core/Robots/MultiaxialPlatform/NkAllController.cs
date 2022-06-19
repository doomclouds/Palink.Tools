using System;

namespace Palink.Tools.Robots.MultiaxialPlatform;

public class NkAllController
{
    public static byte[] BuildFBFDFrame(float xAngle, float yAngle, byte speed = 50,
        byte flexibility = 10, float x = 0, float y = 0, float z = 0)
    {
        if (speed > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(speed), "速度值是0到100");
        }

        if (flexibility > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(flexibility), "柔度值是0到100");
        }

        if (Math.Abs(xAngle) > 10 || Math.Abs(yAngle) > 10)
        {
            throw new ArgumentOutOfRangeException(nameof(flexibility), "角度范围限制为-10到10");
        }

        var d = new byte[]
        {
            0xfb, 0xfd,
            0x00, 0x00, 0x00, 0x00, //X
            0x00, 0x00, 0x00, 0x00, //Y
            0x00, 0x00, 0x00, 0x00, //Z
            0x00, 0x00, 0x00, 0x00, //alpha
            0x00, 0x00, 0x00, 0x00, //beta
            0x00, 0x00, 0x00, 0x00, //gama
            100, //幅度，无效
            speed, //速度，0-100，100表示300r/min
            flexibility, //柔度，0-100，100表示加速时间为100ms
            0x00, //保留
            0x00, //保留
            0x00 //校验和
        };
        var xBytes = BitConverter.GetBytes(x);
        d[2] = xBytes[0];
        d[3] = xBytes[1];
        d[4] = xBytes[2];
        d[5] = xBytes[3];
        var yBytes = BitConverter.GetBytes(y);
        d[6] = yBytes[0];
        d[7] = yBytes[1];
        d[8] = yBytes[2];
        d[9] = yBytes[3];
        var zBytes = BitConverter.GetBytes(z);
        d[10] = zBytes[0];
        d[11] = zBytes[1];
        d[12] = zBytes[2];
        d[13] = zBytes[3];
        var angleX = BitConverter.GetBytes(xAngle);
        d[14] = angleX[0];
        d[15] = angleX[1];
        d[16] = angleX[2];
        d[17] = angleX[3];
        var angleY = BitConverter.GetBytes(yAngle);
        d[18] = angleY[0];
        d[19] = angleY[1];
        d[20] = angleY[2];
        d[21] = angleY[3];
        var total = 0;
        for (var i = 1; i < 31; i++)
        {
            total += d[i];
        }

        var bytes = BitConverter.GetBytes((short)total);
        // ReSharper disable once UseIndexFromEndExpression
        d[d.Length - 1] = bytes[0];
        return d;
    }
}