Shift Defencer (Shift-Defencer)
목포해양대학교 2025년 2학기 소프트웨어공학 - 팀 무한반복
1. 🚀 프로젝트 개요 (Concept)
"전략의 한계, 당신의 컨트롤로 돌파하라!"
Shift Defencer는 전통적인 타워 디펜스 장르에 캐릭터 직접 조작이라는 액션 요소를 결합한 새로운 형태의 디펜스 게임입니다.
기존의 정적인 타워 디펜스 게임에서 플레이어가 관찰자 입장에 머무르는 한계를 넘어, 플레이어가 직접 전장에 개입하고 능동적으로 전략을 수행하는 재미를 제공하는 것을 목표로 합니다.
2. ✨ 주요 기능 (Features)
캐릭터 직접 조작: 플레이어는 맵 상의 캐릭터를 직접 조종하며 게임을 진행합니다.
현장 건설 및 파괴: 타워를 건설하거나 파괴하기 위해서는 반드시 캐릭터가 해당 위치에 접근해야 하므로, 플레이어의 동선 관리가 새로운 전략적 요소가 됩니다.
동적 스테이지: 몬스터가 새로운 길을 뚫거나, 플레이어가 '다리'와 같은 구조물을 건설하여 지형에 상호작용할 수 있습니다.
듀얼 데이터베이스: 오프라인 플레이를 위해 로컬(SQLite)에 데이터를 저장하며, 사용자가 원할 때만 서버(Firebase)에 데이터를 동기화(백업/복원)할 수 있습니다.
3. 🏗️ 시스템 아키텍처 (Architecture)
본 프로젝트는 Unity로 제작된 **'게임 클라이언트(Game Client)'**와 데이터 동기화를 위한 **'서버 서비스(Server Services)'**로 명확하게 분리됩니다.
3.1. 게임 클라이언트 (Game Client - Unity)
클라이언트는 각 기능을 전담하는 매니저(Manager) 클래스들이 유기적으로 상호작용하는 모듈형 아키텍처를 따릅니다.
@startuml
!theme materia-outline
skinparam linetype ortho
skinparam componentStyle uml2

package "Game Client (Unity)" {
    node "UI Manager" {
        component [Player Input (UI)] as UI_Input
        component [UI Viewer] as UI_Viewer
    }
    
    component [Player Input Manager] as PIM
    component [Character Controller] as CC
    component [Tower Manager] as TM
    component [Map Manager] as MM
    component [Wave & Monster Manager] as WMM
    component [Game Status Manager] as GSM
    database "Local DB (SQLite)" as LocalDB
}

component "Server Services (Firebase)" as Server

UI_Input --> PIM
PIM --> CC
GSM --> UI_Viewer

CC <--> MM
CC --> TM
CC <--> GSM
WMM --> MM
WMM --> GSM
GSM <--> LocalDB

GSM --> Server : 데이터 백업/복원
CC --> Server : 로그인/계정 인증
@enduml


3.2. 데이터베이스 (Database Design)
게임은 '오프라인 구동'과 '선택적 백업'을 모두 지원하기 위해 두 개의 DB를 사용합니다.
A. 로컬 DB (SQLite) - ERD
오프라인 플레이를 위한 핵심 DB입니다. 게임의 모든 규칙(정적)과 플레이어의 세이브 파일(동적)을 저장합니다.
@startuml
!theme materia-outline
title "Shift Defender - Local DB (SQLite) ERD"
left to right direction
hide empty members

entity "Towers (타워 정보)" as Towers {
  * <b>tower_id</b> (PK)
  --
  * tower_name
  * cost
  * damage
  * special_ability
}

entity "Monsters (몬스터 정보)" as Monsters {
  * <b>monster_id</b> (PK)
  --
  * monster_name
  * health
  * speed
  * reward
}

entity "Stages (스테이지 정보)" as Stages {
  * <b>stage_id</b> (PK)
  --
  * stage_name
  * map_data
}

entity "Waves (웨이브 정보)" as Waves {
  * <b>wave_id</b> (PK)
  --
  * stage_id (FK)
  * wave_number
}

entity "WaveSpawns (웨이브 몬스터 구성)" as WaveSpawns {
  * <b>wave_id</b> (PK, FK)
  * <b>monster_id</b> (PK, FK)
  --
  * spawn_count
}

entity "PlayerProgress (플레이어 진행 상황)" as PP {
  * <b>player_id</b> (PK)
  --
  * currency_normal
  * currency_special
  * progress_data
  * last_sync_date
}

Stages ||--|{ Waves : "구성된다"
Waves ||--|{ WaveSpawns : "포함한다"
WaveSpawns }|--o| Monsters : "참조한다"
@enduml


B. 서버 서비스 (Firebase) - 계층 구조
BaaS(Backend as a Service)인 Firebase를 사용하며, 인증과 DB 백업을 담당합니다.
@startmindmap
title "Shift Defender - Server (Firebase) Data Structure"

* Firebase Server Services
  ** Firebase Authentication
    *** (User Accounts)
      **** Provides: User ID (e.g., "a5T...kXy9")
  
  ** Cloud Firestore (Database)
    *** "userProgress" (Collection)
      **** [Document ID: "a5T...kXy9"] (User ID와 동일)
        ***** currency_normal: 1500
        ***** currency_special: 20
        ***** progress_data: "{...}" (JSON String)
        ***** last_sync_date: "..."
      **** [Document ID: "b8R...zPw4"]
        ***** ...
@endmindmap


4. 🛠️ 기술 스택 (Tech Stack)
Game Engine: Unity (C#)
Local Database: SQLite (ORM으로 sqlite-net-pcl 라이브러리 활용)
Server & Auth: Firebase (Authentication, Cloud Firestore)
Version Control: Git & GitHub
5. 👨‍💻 팀 무한반복 (Team)
이름
역할
최수환
팀장, 프로젝트 총괄, 기획
박가영
UI/UX 디자인, 리소스 제작
하준효
게임플레이 로직 (타워, 몬스터, 총알) 구현
허용준
핵심 시스템 (DB, 매니저 구조) 설계 및 구현

