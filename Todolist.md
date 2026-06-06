# Snap_to_Map_TD - Todo List

## 진행 중

없음

## 남은 것

- [x] **적 클릭 시 적 정보 UI** - EnemyInfoPanel.cs, Enemy에 OnMouseDown + 프로퍼티 추가, TowerInfoPanel과 같은 위치에 배치
- [x] **캐릭터 설명 툴팁** - 타워 버튼 호버 시 TowerInfoPanel에 설명+스킬 표시, 배치된 타워 클릭 시 숨김
- [ ] **스킬 쿨다운 UI 변경** - SkillCooldownText → 원형 다이얼 UI로 교체 (에셋 보유)
- [ ] **스킬 쿨다운 시작 시점 수정** - 타워 설치 즉시 카운트 → 웨이브 시작 시 카운트 시작
- [ ] **캐릭터 및 적 추가** - 새 타워 종류 및 새 적 종류 추가

## 완료된 것

- [x] Python: OpenCV 엣지 추출, BFS 경로 탐색, path_data.json 저장
- [x] MapManager.cs: JSON + 배경 이미지 로드, LineRenderer 경로 표시, 카메라 자동 조정
- [x] Enemy.cs: 경로 이동, HP, 사망/도달 이벤트, 애니메이터 연동
- [x] Tower.cs: 사거리 탐색, 딜레이 데미지, 스킬, AoE, 2교대 평타
- [x] WaveManager.cs: 다중 웨이브, 웨이브 시작 버튼 대기
- [x] GameManager.cs: 골드/라이프 관리, onGameOver 이벤트
- [x] HUDManager.cs: Gold/Lives/Wave 텍스트
- [x] GameResultPanel.cs: 게임오버/클리어 화면 + 재시작
- [x] WaveStartButton.cs: 웨이브 시작 버튼
- [x] Archer: 애니메이터 Idle/Attack01/Skill, CanTransitionToSelf 수정
- [x] Knight: 애니메이터 Idle/Attack01/Attack02/Skill, 2교대 평타 + AoE
- [x] HealthBar.cs 작성 완료
- [x] 버튼 클릭 범위 최종 적용 완료
- [x] 체력바 색 빨강 고정
- [x] 재시작 시 웨이브 시작 버튼 문제 해결
- [x] Enemy 체력바 정상 동작 확인
- [x] 경로 위 타워 설치 차단 - MapRoadGenerator PolygonCollider2D + OverlapCircleAll
- [x] 취소(휴지통) 버튼 - TowerPlacer cancelButton, Layout Element Ignore Layout
- [x] 배치된 타워 클릭 UI - TowerInfoPanel(강화/판매), RangeIndicator(채워진 원)
- [x] Tower power 스탯 추가 - 모든 데미지를 power × ratio로 계산
- [x] Tower 공격 버그 수정 - 타이머를 적 감지 시에만 감소, 첫 진입 즉시 공격
- [x] 스킬 타겟 리다이렉트 - 발동 시 고정 타겟 대신 딜레이 후 재탐색
- [x] 애니메이션 스프라이트 그림자 버전으로 교체 - PNG 재배치 + .meta GUID 복원으로 anim 연결 유지
