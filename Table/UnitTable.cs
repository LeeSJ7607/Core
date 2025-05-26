using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public sealed class UnitTable : BaseTable<UnitTable.Row>
{
    [Serializable]
    public sealed class Row
    {
        public int Id;
		public string PrefabName;
		public long Max_HP;
        public long Atk;
		public long Walk_Speed;
        public float Atk_Range;
		public float FOV_Radius;
		public float FOV_Angle;
        public eSeekRule SeekRuleType;

		[JsonConverter(typeof(ListJsonConverter<int>))]
		public List<int> SkillIds;

		public IReadOnlyDictionary<eSkillSlot, int> UniqueSkillIdsBySlot => _uniqueSkillIdsBySlot;
		private Dictionary<eSkillSlot, int> _uniqueSkillIdsBySlot;

		public void Initialize()
		{
			_uniqueSkillIdsBySlot = SkillSlotMapper.Map(this);
		}
	}
	
	protected override void Initialize(Row row)
	{
		row.Initialize();
	}
	
	protected override int GetRowKey(Row row) => row.Id;
}

internal static class SkillSlotMapper
{
	private static readonly eSkillSlot[] SKILL_SLOTS =
	{
		eSkillSlot.Q, 
		eSkillSlot.W, 
		eSkillSlot.E, 
		eSkillSlot.R
	};
	
	public static Dictionary<eSkillSlot, int> Map(UnitTable.Row row)
	{
		var skillMap = new Dictionary<eSkillSlot, int>(SKILL_SLOTS.Length);
		var usedSkillIds = new HashSet<int>();
		var skillIds = row.SkillIds;
		
		if (skillIds.IsNullOrEmpty())
		{
			return skillMap;
		}
		
		var count = Math.Min(SKILL_SLOTS.Length, skillIds.Count);
		for (var i = 0; i < count; i++)
		{
			var skillId = skillIds[i];
			if (skillId == 0)
			{
				continue;
			}

			if (usedSkillIds.Contains(skillId))
			{
				continue;
			}
			
			skillMap[SKILL_SLOTS[i]] = skillId;
			usedSkillIds.Add(skillId);
		}
		
		return skillMap;
	}
}