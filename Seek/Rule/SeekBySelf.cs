using System.Collections.Generic;

public sealed class SeekBySelf : ISeeker
{
    public IReadOnlyList<Unit> Seek(Unit owner) => throw new System.NotImplementedException();
}