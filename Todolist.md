# Snap_to_Map_TD - Todo List

## 진행 중

없음

## 남은 것

- [ ] **애니메이션 스프라이트 그림자 버전으로 교체** - Archer/Knight 모든 애니메이션 스프라이트를 그림자 있는 버전으로 변경
- [ ] **적 클릭 시 적 정보 UI** - 적 클릭 시 TowerInfoPanel과 유사한 형태로 적 스탯(HP, 속도 등) 표시
- [ ] **캐릭터 설명란 추가** - 타워 선택/클릭 시 설명 텍스트 표시, 표시 방식 고민 필요
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
