# Snap_to_Map_TD - Todo List

## 진행 중

- [ ] **버튼 클릭 범위 최종 적용 확인**
  - ArcherButton / KnightButton 루트에 Image 컴포넌트 추가, Color Alpha=0, Raycast Target=✅
  - IconImage → Raycast Target=❌
  - CostText → Raycast Target=❌
  - 원인: Icon.asset이 Archer-Idle.png 스프라이트시트의 100x100 프레임 전체를 사용 (실제 캐릭터는 26x21px)

## 남은 것

- [ ] **체력바 색 변경** - HP에 따라 색 변화 (초록 → 노랑 → 빨강)
- [ ] **재시작 시 웨이브 시작 버튼 사라지는 문제** - SceneManager.LoadScene 후 WaveStartButton이 표시 안 됨. WaveManager RunWaves 코루틴 재시작 흐름 확인 필요
- [ ] **Enemy 체력바 완성 확인**
  - 구조: Enemy_Prefeb > HealthBarCanvas(World Space) > Background > Fill
  - Fill 설정: Anchor=left-stretch, Pivot=(0, 0.5), Width=Background와 동일
  - HealthBar.cs: `SetSizeWithCurrentAnchors`로 너비 조절
  - 아직 정상 동작 확인 안됨

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
- [x] 버튼 클릭 범위 오프셋 원인 분석 완료
  - IconImage Raycast Target 끄기
  - 루트 Image Alpha=0으로 투명 클릭 범위 설정
