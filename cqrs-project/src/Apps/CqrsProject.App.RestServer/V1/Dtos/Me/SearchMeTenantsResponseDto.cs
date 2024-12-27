using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CqrsProject.App.RestServer.V1.Dtos;

public record SearchMeTenantsResponseDto(
    Guid Id,
    string Name
);
