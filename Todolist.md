# Snap_to_Map_TD - Todo List

## 진행 중

없음

## 남은 것

### 1. 밸런싱 및 QA
- [x] Knight 밸런스 조정 (평타 쿨, 스킬쿨, 데미지 하향)
- [x] Archer/Knight skillAnimDuration 조정 (스킬 후 평타 딜레이 어색함)
- [x] 전체 타워/적 밸런스 점검
- [x] 버그 확인 및 수정

### 2. 배포 (GitHub Releases)
- [ ] Unity Build Settings → Windows exe 빌드
- [ ] 빌드 결과물 zip 압축 → GitHub Releases 업로드
- [ ] PyInstaller로 mapmaking.py → exe 변환 검토 (게임 내 Python 실행)

## 완료된 것

- [x] Python: OpenCV 엣지 추출, BFS 경로 탐색, path_data.json 저장
- [x] Enemy.cs: 경로 이동, HP, 사망/도달 이벤트, ApplySlow, livesToLose, IsDead
- [x] Tower.cs: 이중공격 버그, 스킬쿨 웨이브 중에만 감소, AttackSpeed/IsBuffed
- [x] WaveManager.cs: Wave > Entry > EnemySpawn 구조, 병렬 스폰, 버그 수정
- [x] Wave 1~10 구성 완료
- [x] WaveStartButton.cs: 모든 적 사망 후 등장, 클릭 즉시 숨김
- [x] WaveDrawer.cs: Wave/Entry/Enemy 번호 표시
- [x] TowerInfoPanel.cs: 스킬 게이지, SPD 표시, UI 겹침 버그 수정
- [x] EnemyInfoPanel.cs: HP/Speed/Gold/Life 표시
- [x] Archer/Knight/Wizard/Priest 타워 완성
- [x] Orc/Skeleton/Slime/Werebear/Werewolf 프리팹 완성 (그림자 버전 스프라이트)
- [x] Wizard 버그 수정: Blizzard 범위 밖 소환 (IsDead 체크 + 범위 클램프)
