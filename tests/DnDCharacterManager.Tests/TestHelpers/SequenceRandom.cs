namespace DnDCharacterManager.Tests.TestHelpers;

internal sealed class SequenceRandom : Random
{
    private readonly int[] _sequence;
    private int _index;

    public SequenceRandom(params int[] sequence)
    {
        _sequence = sequence.Length > 0 ? sequence : [4, 4, 4, 4];
    }

    public override int Next(int minValue, int maxValue) =>
        _sequence[_index++ % _sequence.Length];
}
