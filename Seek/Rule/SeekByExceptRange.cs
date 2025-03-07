using System.Collections.Generic;

public sealed class SeekByExceptRange : ISeeker
{
    public IReadOnlyList<Unit> Seek(Unit owner) => throw new System.NotImplementedException();
}