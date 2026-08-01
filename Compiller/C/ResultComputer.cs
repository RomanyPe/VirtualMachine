using Kernel.BiosSystem;

namespace Compiller.C;


public readonly struct ResultComputer
{
	public readonly Device Device;
	public readonly ResultOperation Result;

	public ResultComputer(Device device)
	{
		Device = device;
		Result = ResultOperation.Success;
	}

	public ResultComputer(ResultOperation result)
	{
		Result = result;
		Device = null!;
	}
}
