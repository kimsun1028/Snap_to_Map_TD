# Snap_to_Map_TD - Todo List

## 진행 중

없음

## 남은 것

### 1. 적 추가
- [ ] Orc 프리팹 + Animator 기준으로 복사 → Skeleton, Slime, Werebear, Werewolf 찍어내기
- [ ] 각 적 스탯 (HP, 속도, 골드) 조정

### 2. 아군 캐릭터 추가 (2개)
- [ ] 캐릭터 선정 후 구현 (보유 에셋에서 선택)

### 3. 최종 QA
- [ ] **애니메이션 어색함 수정** - Archer/Knight 스킬 후 평타 딜레이 없음
      → 각 프리팹 Inspector에서 `skillAnimDuration` 값 조정 (스킬 애니 끝날 때까지 평타 막기)
- [ ] **공격 쿨타임 차별화** - 타워별 attackCooldown, skillCooldown 개성 있게 분리
      → 지금은 스킬 쓰자마자 평타 나가는 게 "획일화" 원인
- [ ] **전체 밸런스 하향** - 타워 power / attackRatio / skillRatio 수치 조정
      → 각 TowerData asset에서 수치 낮추기
- [ ] 웨이브 구성 조정 (적 수, 스폰 간격 등)

## 완료된 것

- [x] Python: OpenCV 엣지 추출, BFS 경로 탐색, path_data.json 저장
- [x] MapManager.cs: JSON + 배경 이미지 로드, LineRenderer 경로 표시, 카메라 자동 조정
- [x] Enemy.cs: 경로 이동, HP, 사망/도달 이벤트, 애니메이터 연동, ApplySlow
- [x] Tower.cs: 사거리 탐색, 딜레이 데미지, 스킬, AoE, 2교대 평타, 이중공격 버그 수정
- [x] Tower.cs: 스킬 쿨타임 웨이브 중에만 감소, 웨이브 시작 시 초기화 안 함 (미시작 타워만 초기화)
- [x] WaveManager.cs: 다중 웨이브, WaveEntry delayAfterGroup 필드 추가
- [x] WaveStartButton.cs: 클릭 즉시 숨김
- [x] WaveDrawer.cs: Inspector Wave 1/2/3, Entry 1/2/3 표시
- [x] GameManager.cs: 골드/라이프 관리, onGameOver 이벤트
- [x] HUDManager.cs: Gold/Lives/Wave 텍스트
- [x] GameResultPanel.cs: 게임오버/클리어 화면 + 재시작
- [x] Archer: 애니메이터 Idle/Attack01/Skill
- [x] Knight: 애니메이터 Idle/Attack01/Attack02/Skill, 2교대 평타 + AoE
- [x] HealthBar.cs, RangeIndicator.cs, EnemyInfoPanel.cs
- [x] 버튼 클릭 범위, 경로 위 타워 설치 차단, 취소 버튼
- [x] TowerInfoPanel.cs: 강화/판매, 스킬 쿨다운 게이지, 공격속도(SPD ▲) 실시간 표시
- [x] Wizard 타워: WizardTower.cs, Fireball 투사체, Blizzard 스킬 (AoE + 슬로우)
- [x] Priest 타워: 평타(Holy Bolt) + 스킬(공격속도 버프, buffDuration 조정 가능)
