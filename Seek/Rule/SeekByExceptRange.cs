using System.Collections.Generic;

public sealed class SeekByExceptRange : ISeeker
{
    IReadOnlyList<IReadOnlyUnit> ISeeker.Seek(IEnumerable<IReadOnlyUnit> units, IReadOnlyUnit owner) => throw new System.NotImplementedException();
}