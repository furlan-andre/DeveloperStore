using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;

namespace Ambev.DeveloperEvaluation.Unit.Common;

public static class AutoMapperTestHelper
{
    public static MapperConfiguration CreateConfiguration(Action<IMapperConfigurationExpression> configure)
    {
        return new MapperConfiguration(configure, NullLoggerFactory.Instance);
    }
}
