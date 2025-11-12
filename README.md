🛠️ Shift Defender 프로젝트 README (중간 보고서 기반)
📌 프로젝트 개요

Shift Defender는 기존의 정적인 타워 디펜스 게임의 한계를 극복하고자 **'캐릭터 직접 조작'**이라는 액션 요소를 결합한 새로운 디펜스 게임입니다. 플레이어가 직접 전장에 개입하고 캐릭터를 조종하여 타워 건설/파괴 및 전략 수행을 능동적으로 진행하는 재미를 제공합니다.



팀명: 무한반복 



주제: 디펜스 게임 


사용 도구: 유니티 게임엔진 


사용 언어: C#, SQLite 


장르 핵심: 정해진 시간 동안 몰려오는 적의 웨이브를 막아내는 "디펜스" 장르의 핵심에 충실합니다.

🚀 기존 시스템과의 차별성
| 비교 대상 게임 | 차별점 | 핵심 요소 |
| :--- | :--- | :--- |
| **기존 타워 디펜스** | 플레이어의 **관찰자 입장(정적인 한계)**을 극복. | 플레이어가 직접 **캐릭터를 조종**하여 전장에 개입하고 능동적인 전략을 수행. |
| **리로드 (Reroad)** | [cite_start]플레이어가 직접 길을 뚫는 시스템과 달리, 본 게임은 **길이 이미 정해져 있음**[cite: 24]. [cite_start]몬스터 통과나 사용자 조작으로 길과 길 사이의 이동을 수월하게 하는 등의 차이점 존재[cite: 24]. | [cite_start]능동적인 길 관리 및 지형 극복 기믹 수행[cite: 13]. |
| **킹샷 (King Shot)** | [cite_start]기지 성장이 아닌, 스테이지 클리어를 위한 전략적 수단에 캐릭터 역할 집중[cite: 31]. | [cite_start]적의 웨이브를 막아내는 **"디펜스"** 본연의 재미[cite: 30]. |
| **풍선 타워 디펜스 (Bloons TD)** | [cite_start]**캐릭터의 위치와 생존**이라는 변수를 추가하여 실시간 대응 능력을 요구[cite: 40]. | [cite_start]타워 건설/파괴를 위해 캐릭터가 **직접 해당 위치에 도달**해야 하므로, **플레이어 동선 관리** 자체가 새로운 전략 요소가 됨[cite: 43, 44, 45]. |

⚙️ 시스템 구성 및 아키텍처
1. 시스템 구성도 (Game Client & Server)

Game Client (Unity): UI Manager, Player Input Manager, Character Controller, Wave & Monster Manager, Map Manager, Tower Manager, Game Status Manager 등으로 구성.






Local DB (SQLite): 모든 오프라인 플레이를 위한 핵심 DB. 게임 규칙(스탯 등)과 플레이어의 실시간 진행 상황을 저장.




Server Services (Firebase): 선택적 백업을 위한 서버. 사용자 계정 인증과 로컬 DB 데이터의 클라우드 복원/백업 역할 수행.


2. 데이터베이스 아키텍처
| DB 종류 | 저장 데이터 | 특징 |
| :--- | :--- | :--- |
| **Local DB (SQLite)** | [cite_start]`PlayerProgress` (동적 데이터), `Stages`, `Waves`, `Monsters`, `Towers` (정적 규칙). [cite: 214, 219] | [cite_start]**핵심 DB**로 모든 오프라인 플레이를 위한 데이터와 게임 규칙을 저장합니다. [cite: 184, 185] [cite_start]`Waves`와 `Monsters`의 다대다 관계는 `WaveSpawns` 연결 테이블을 사용합니다. [cite: 221] |
| **Server DB (Firebase - Cloud Firestore)** | [cite_start]`userProgress` (플레이어 진행 상황 백업 데이터). [cite: 231] | [cite_start]BaaS(Backend as a Service)를 활용한 **NoSQL 문서 구조**를 사용합니다. [cite: 232, 233] [cite_start]사용자 계정 인증과 DB가 분리되며, 백업 창고 역할을 합니다. [cite: 186, 188, 234, 235] |


역할: 백업 창고 역할.

💻 핵심 개발 기술 (알고리즘 및 라이브러리)
1. 주요 알고리즘
| 기능 | 알고리즘 명칭 | 핵심 로직 |
| :--- | :--- | :--- |
| **타워의 공격** | 범위 내 가장 가까운 몬스터 탐색, 시간 제어, 객체 생성 및 연결 | [cite_start]`GameObject.FindGameObjectsWithTag("Monster")`로 몬스터 탐색 후 `Vector2.Distance()`로 가장 가까운 몬스터를 선택합니다. [cite: 141, 142] [cite_start]쿨다운 계산 후 총알 객체 생성 및 `SetTarget()`으로 타겟을 지정합니다. [cite: 143, 144] |
| **몬스터의 이동** | 이동 알고리즘 | [cite_start]`Update()` 내에서 `Vector2.left * speed * Time.deltaTime`로 지속 이동합니다. [cite: 146] |
| **몬스터 피격/사망** | 상태 기반 알고리즘 | [cite_start]`TakeDamage()`에서 `currentHp`를 감소시키고, 0 이하 시 `Die()` 호출 및 `Destroy(gameObject)`로 파괴합니다. [cite: 147, 148] |
| **총알의 타겟 추적** | 벡터 정규화 기반 이동 | [cite_start]`Update()` 내에서 `(target.position - transform.position).normalized`로 방향 벡터를 계산하며 타겟을 추적합니다. [cite: 151, 152] |
| **총알의 폭발 데미지** | 범위 탐색 알고리즘 | [cite_start]타겟과의 거리가 0.2f 이하 시 `Explode()`를 호출하며, `Physics2D.OverlapCircleAll()`을 사용하여 폭발 반경 내 몬스터에게 데미지를 적용합니다. [cite: 154, 155] |
| **몬스터 웨이브 생성** | 타이머 반복 호출 | [cite_start]`InvokeRepeating()`을 사용하여 1초 후부터 매 `spawnInterval` 초마다 `SpawnWave()`를 호출합니다. [cite: 158, 159, 160] |

2. 사용된 Unity API (라이브러리)

| API | 기능 |
| :--- | :--- |
| `Instantiate()` | [cite_start]프리팹 동적 생성 [cite: 164] |
| `Destroy()` | [cite_start]오브젝트 제거 [cite: 167] |
| `InvokeRepeating()` | [cite_start]주기적 함수 호출 [cite: 170] |
| `Physics2D.OverlapCircleAll()` | [cite_start]원형 범위 내 Collider (Monster) 탐색 [cite: 166] |
| `Vector2.Distance()` | [cite_start]거리 계산 [cite: 169] |
| `Time.deltaTime` | [cite_start]프레임 시간 계산 [cite: 171] |

📂 중간 개발 코드 현황
| 모듈명 | 파일명 | 기능 | 라인 수 | 개발 언어 |
| :--- | :--- | :--- | :--- | :--- |
| 타워 | `Tower.cs` | 사거리 측정, 탄환 발사 | 48 | C# |
| 몬스터 | `Monster.cs` | 공격 피격 | 38 | C# |
| 몬스터 스폰 | `MonsterSpawner.cs` | 스폰 | 24 | C# |
| 총알 | `Bullet.cs` | 몬스터 공격 | 54 | C# |

개발 언어: C# 

👥 팀원
최수환 


박가영 


하준효 


허용준
