using System.Collections.Generic;

namespace Palink.Tools.System.ErrorExt;

public interface IErrorOr
{
    List<Error> Errors { get; }

    bool IsError { get; }
}