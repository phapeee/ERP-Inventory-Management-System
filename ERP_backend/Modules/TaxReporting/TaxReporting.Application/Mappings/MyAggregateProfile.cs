// To use AutoMapper, add the following package to the .csproj file:
// <PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.*" />

using AutoMapper;
using PineConePro.Erp.TaxReporting.Application.Dtos;
using PineConePro.Erp.TaxReporting.Domain.Aggregates;

namespace PineConePro.Erp.TaxReporting.Application.Mappings;

internal sealed class MyAggregateProfile : Profile
{
    public MyAggregateProfile()
    {
        CreateMap<MyAggregate, MyAggregateDto>();
    }
}
