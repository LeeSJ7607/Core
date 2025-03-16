using System.Collections.Generic;

public sealed class SeekByDefenseBuilding : ISeeker
{
    IReadOnlyList<IReadOnlyUnit> ISeeker.Seek(IEnumerable<IReadOnlyUnit> units, IReadOnlyUnit owner) => throw new System.NotImplementedException();
}