using System;

public sealed class ProjectileTable : BaseTable<ProjectileTable.Row>
{
    [Serializable]
    public sealed class Row
    {
        public int Id;
        public eProjectileType ProjectileType;
        public float Speed;
        public float Acceleration;
        public float Lifetime;
        public string PrefabPath;
    }

    protected override int GetRowKey(Row row) => row.Id;
}