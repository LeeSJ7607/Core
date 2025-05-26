using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public sealed class SkillTable : BaseTable<SkillTable.Row>
{
    [Serializable]
    public sealed class Row
    {
        public int Id;
        public eSkillInput SkillInputType;
        public eSkillShape SkillShapeType;
        public float SkillRange;
        public int ProjectileId;
        
        [JsonConverter(typeof(ListJsonConverter<int>))]
        public List<int> EffectIds;
    }

    protected override int GetRowKey(Row row) => row.Id;
}