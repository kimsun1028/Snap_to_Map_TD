# Snap_to_Map_TD - Todo List

## 진행 중

없음

## 남은 것

### 1. Enemy 스프라이트 그림자 버전으로 교체
- [x] Skeleton, Slime, Werebear, Werewolf 각 Walk/Hurt/Death 클립 스프라이트 교체
- [x] 교체 후 애니메이션 정상 출력 확인

### 2. Wave 10까지 구성
- [ ] WaveManager Inspector에서 Wave 1~10 설정
- [ ] 적 종류/수/스폰 간격/딜레이 조정

### 3. 밸런싱 및 QA
- [ ] Knight 밸런스 조정 (평타 쿨, 스킬쿨, 데미지 하향)
- [ ] Archer/Knight skillAnimDuration 조정 (스킬 후 평타 딜레이 어색함)
- [ ] 전체 타워/적 밸런스 점검
- [ ] 버그 확인 및 수정

### 4. 배포 (GitHub Releases)
- [ ] Unity Build Settings → PC, Mac & Linux Standalone → Windows exe 빌드
- [ ] 빌드 결과물 zip으로 압축
- [ ] GitHub Releases에 업로드 → 누구나 다운받아 바로 실행 가능한지 확인

## 완료된 것

- [x] Python: OpenCV 엣지 추출, BFS 경로 탐색, path_data.json 저장
- [x] Enemy.cs: 경로 이동, HP, 사망/도달 이벤트, ApplySlow, livesToLose 추가
- [x] Tower.cs: 이중공격 버그, 스킬쿨 웨이브 중에만 감소, AttackSpeed/IsBuffed 프로퍼티
- [x] WaveManager.cs: Wave > Entry > EnemySpawn 구조, 병렬 스폰, 버그 수정
- [x] WaveStartButton.cs: 모든 적 사망 후 등장, 클릭 즉시 숨김
- [x] WaveDrawer.cs: Wave/Entry/Enemy 번호 표시
- [x] TowerInfoPanel.cs: 스킬 게이지, SPD 표시, UI 겹침 버그 수정
- [x] EnemyInfoPanel.cs: HP/Speed/Gold/Life 표시
- [x] Archer/Knight 완성
- [x] Wizard 타워: Fireball + Blizzard 스킬
- [x] Priest 타워: Holy Bolt 평타 + 공격속도 버프 스킬
- [x] Skeleton/Slime/Werebear/Werewolf 애니메이션 클립 + Controller 완성
- [x] Orc 프리팹 완성
