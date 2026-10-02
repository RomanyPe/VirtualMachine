using Kernel.ControllersData;

namespace VMApplication;

public interface IPortDevice : IPortUse
{
    string Name { get; }
    DateTime CreatedAt { get; }
}

