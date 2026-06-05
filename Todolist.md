# Snap_to_Map_TD - Todo List

## 진행 중

없음

## 남은 것

- [ ] **경로 위 타워 설치 차단** - MapRoadGenerator의 PolygonCollider2D(isTrigger)를 활용해 TowerPlacer에서 경로 위 배치 불가 처리
- [ ] **취소(휴지통) 버튼** - 타워 선택 후 휴지통 버튼 클릭 시 배치 취소, 고스트 제거
- [ ] **배치된 타워 클릭 UI** - 타워 클릭 시 강화/판매 패널 표시

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
