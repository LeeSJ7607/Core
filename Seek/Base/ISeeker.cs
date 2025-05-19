using System.Collections.Generic;

public enum eSeekRule
{
    Self,
    Random,
    
    FOVEnemy,         // 시야에 걸린 적
    FarthestEnemy,    // 가장 먼 적
    NearestEnemy,     // 가장 가까운 적
    HPLeastEnemy,     // HP가 가장 낮은 적
    RangeEnemy,       // 원거리 적
    ExceptRange,      // 원거리 제외
    NearestIfDamaged, // 피해 입은 유닛 중, 가장 가까운 유닛
    DefenseBuilding,  // 방어 건물
    
    End,
}

public interface ISeeker
{
    IReadOnlyList<IReadOnlyUnit> Seek(IEnumerable<IReadOnlyUnit> units, IReadOnlyUnit owner);
}