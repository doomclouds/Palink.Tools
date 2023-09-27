using Palink.Tools.System.ErrorExt;

namespace Palink.Tools.Core.Test.System.ErrorExt;

public class ErrorOrTest
{
    [Fact]
    public void ErrorTest()
    {
        var user = new User();
        var errors = new List<Error>
        {
            Error.Serious(),
            Error.Handleable()
        };
        ErrorOr<User> u1 = Error.Unexpected();
        ErrorOr<User> u2 = user;
        ErrorOr<User> u3 = errors;
        ErrorOr<User> u4 = errors.ToArray();
        var u5 = ErrorOr.From(new User());
        Assert.True(u1.IsError);
        Assert.True(u3.IsError);
        Assert.True(u4.IsError);
        Assert.False(u2.IsError);
        Assert.False(u5.IsError);
    }

    private class User
    {
    }
}