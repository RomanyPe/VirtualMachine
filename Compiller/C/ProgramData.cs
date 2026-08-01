namespace Compiller.C;

internal readonly struct ProgramData(ulong adress, int lenght)
{
	public readonly ulong Adress = adress;
	public readonly int Lenght = lenght;
}
