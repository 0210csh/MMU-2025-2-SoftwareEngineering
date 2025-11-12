🛠️ Shift Defender 프로젝트 README (중간 보고서 기반)
📌 프로젝트 개요

Shift Defender는 기존의 정적인 타워 디펜스 게임의 한계를 극복하고자 **'캐릭터 직접 조작'**이라는 액션 요소를 결합한 새로운 디펜스 게임입니다. 플레이어가 직접 전장에 개입하고 캐릭터를 조종하여 타워 건설/파괴 및 전략 수행을 능동적으로 진행하는 재미를 제공합니다.



팀명: 무한반복 



주제: 디펜스 게임 


사용 도구: 유니티 게임엔진 


사용 언어: C#, SQLite 


장르 핵심: 정해진 시간 동안 몰려오는 적의 웨이브를 막아내는 "디펜스" 장르의 핵심에 충실합니다.

🚀 기존 시스템과의 차별성
비교 대상 게임,차별점,핵심 요소
기존 타워 디펜스,플레이어의 **관찰자 입장(정적인 한계)**을 극복.,플레이어가 직접 캐릭터를 조종하여 전장에 개입하고 능동적인 전략을 수행.
리로드 (Reroad),"플레이어가 직접 길을 뚫는 시스템과 달리, 본 게임은 길이 이미 정해져 있음. 몬스터 통과나 사용자 조작으로 길과 길 사이의 이동을 수월하게 하는 등의 차이점 존재.",능동적인 길 관리 및 지형 극복 기믹 수행.
킹샷 (King Shot),"기지 성장이 아닌, 스테이지 클리어를 위한 전략적 수단에 캐릭터 역할 집중.","적의 웨이브를 막아내는 ""디펜스"" 본연의 재미."
풍선 타워 디펜스 (Bloons TD),캐릭터의 위치와 생존이라는 변수를 추가하여 실시간 대응 능력을 요구.,"타워 건설/파괴를 위해 캐릭터가 직접 해당 위치에 도달해야 하므로, 플레이어 동선 관리 자체가 새로운 전략 요소가 됨."

⚙️ 시스템 구성 및 아키텍처
1. 시스템 구성도 (Game Client & Server)

Game Client (Unity): UI Manager, Player Input Manager, Character Controller, Wave & Monster Manager, Map Manager, Tower Manager, Game Status Manager 등으로 구성.






Local DB (SQLite): 모든 오프라인 플레이를 위한 핵심 DB. 게임 규칙(스탯 등)과 플레이어의 실시간 진행 상황을 저장.




Server Services (Firebase): 선택적 백업을 위한 서버. 사용자 계정 인증과 로컬 DB 데이터의 클라우드 복원/백업 역할 수행.


2. 데이터베이스 아키텍처
Local DB (SQLite):


동적 데이터: 플레이어 세이브 파일인 PlayerProgress 테이블.


정적 규칙: Stages, Waves, Monsters, Towers 테이블에 게임의 모든 정적 규칙 저장.


관계: Waves와 Monsters의 다대다(N:M) 관계는 WaveSpawns 연결 테이블을 사용.

Server DB (Firebase - Cloud Firestore):


구조: BaaS(Backend as a Service)를 활용한 NoSQL 문서 구조.


분리: 인증(Firebase Authentication)과 DB(Cloud Firestore)를 분리.


역할: 백업 창고 역할.

💻 핵심 개발 기술 (알고리즘 및 라이브러리)
1. 주요 알고리즘

기능,알고리즘 명칭,핵심 로직
타워의 공격,"범위 내 가장 가까운 몬스터 탐색 , 시간 제어 , 객체 생성 및 연결 ","GameObject.FindGameObjectsWithTag(""Monster"")로 몬스터 탐색 후 Vector2.Distance()로 가장 가까운 몬스터 선택. 쿨다운 계산 후 총알 객체 생성 및 SetTarget()으로 타겟 지정."
몬스터의 이동,이동 알고리즘 ,Update() 내에서 Vector2.left * speed * Time.deltaTime로 지속 이동.
몬스터 피격/사망,상태 기반 알고리즘 ,"TakeDamage()에서 currentHp 감소, 0 이하 시 Die() 호출 및 Destroy(gameObject)로 파괴."
총알의 타겟 추적,벡터 정규화 기반 이동 ,(target.position - transform.position).normalized로 방향 벡터 계산.
총알의 폭발 데미지,범위 탐색 알고리즘 ,타겟과의 거리가 0.2f 이하 시 Explode() 호출. Physics2D.OverlapCircleAll()로 폭발 반경 내 몬스터에게 데미지 적용.
몬스터 웨이브 생성,타이머 반복 호출 ,InvokeRepeating()을 사용하여 일정 간격(spawnInterval)으로 SpawnWave() 호출.

2. 사용된 Unity API (라이브러리)

Instantiate(): 프리팹 동적 생성.


Destroy(): 오브젝트 제거.


InvokeRepeating(): 주기적 함수 호출 (웨이브 생성).


Physics2D.OverlapCircleAll(): 원형 범위 내 Collider(Monster) 탐색 (폭발 데미지).


Vector2.Distance(): 거리 계산 (타워의 목표 탐색, 총알의 충돌 판단).


Time.deltaTime: 프레임 시간 계산 (이동, 쿨다운).

📂 중간 개발 코드 현황
모듈명,파일명,기능,라인 수
타워,Tower.cs,"사거리 측정, 탄환 발사",48 
몬스터,Monster.cs,"공격 피격, 이동, 사망",38 
몬스터 스폰,MonsterSpawner.cs,웨이브 및 몬스터 스폰,24 
총알,Bullet.cs,"타겟 추적, 몬스터 공격",54 

개발 언어: C# 

👥 팀원
최수환 


박가영 


하준효 


허용준
