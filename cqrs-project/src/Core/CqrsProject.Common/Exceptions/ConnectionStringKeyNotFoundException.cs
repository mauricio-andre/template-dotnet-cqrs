using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Common.Exceptions;

public class ConnectionStringKeyNotFoundException : BusinessException
{
    public ConnectionStringKeyNotFoundException(IStringLocalizer localizer, string tenantId, string keyName)
        : base(localizer["message:validation:ConnectionStringKeyNotFound", tenantId, keyName])
    {
    }
}
