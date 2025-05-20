public enum eSkillSlot
{
    Q,
    W,
    E,
    R,
}

public enum eSkillInput
{
    Instant,
    TargetUnit,
    Directional,
    GroundPoint
}

//TODO: Instant 스킬 인풋 타입이 들어오면 강제로 None 을 설정해서 휴먼 에러 방지 코드 추가 필요.
public enum eSkillShape
{
    None,           // 판정 없음 (버프 등)
    LineProjectile, // 직선 투사체 (ex. 갈고리)
    AreaCircle,     // 원형 범위 (ex. 눈보라)
    AreaLine,       // 선형 범위 (ex. 사슬)
    Cone,           // 부채꼴 (ex. 전방 브레스)
    SelfAura,       // 자기 중심 (ex. 루시우)
    DeployZone,     // 설치형 (ex. 좀비 벽)
    Wall,           // 구조물 생성형 (ex. 망자의 벽)
    JumpDestination // 도약형 착지 지점 (ex. 무라딘 점프)
}