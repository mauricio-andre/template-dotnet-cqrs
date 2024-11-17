using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CqrsProject.CustomConsoleFormatter.Interfaces;

public interface ILoggerPropertiesService
{
    string GetAppUser();

    KeyValuePair<string, object?>[] DefaultPropertyList();

    KeyValuePair<string, object?>[] ScopeObjectStructuring(object value);
}
