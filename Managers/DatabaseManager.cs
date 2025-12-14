using UnityEngine;
using System.Data;
using Mono.Data.Sqlite; // 핵심 네임스페이스
using System.IO;
using System.Collections.Generic;
using System;

public class DatabaseManager : MonoBehaviour
{
    private IDbConnection dbConnection;
    private string connectionString;
    private static DatabaseManager _instance;

    // 싱글톤 패턴
    public static DatabaseManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<DatabaseManager>();
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeDB();
    }

    private void InitializeDB()
    {
        // 1. 데이터베이스 경로 및 연결 문자열 설정
        string dbName = "GameDB.db";
        string filepath = Path.Combine(Application.persistentDataPath, dbName);
        connectionString = "URI=file:" + filepath;

        Debug.Log("DB 경로: " + filepath);

        // 2. 연결 열기 (파일이 없으면 자동 생성됨)
        OpenConnection();

        // 3. 테이블 생성 (SQL 쿼리 직접 작성)
        CreateTables();

        // 4. 초기 데이터 삽입
        if (IsFirstRun())
        {
            InsertInitialData();
        }
    }

    private void OpenConnection()
    {
        try
        {
            dbConnection = new SqliteConnection(connectionString);
            dbConnection.Open();
        }
        catch (Exception e)
        {
            Debug.LogError("DB 연결 실패: " + e.Message);
        }
    }

    private void CreateTables()
    {
        // ExecuteQuery 함수를 만들어 쿼리 실행을 단순화했습니다.

        // Monster 테이블
        string queryMonsters = @"
            CREATE TABLE IF NOT EXISTS Monsters (
                monster_id INTEGER PRIMARY KEY,
                monster_name TEXT NOT NULL,
                health REAL NOT NULL,
                speed REAL NOT NULL,
                reward INTEGER NOT NULL
            );";
        ExecuteNonQuery(queryMonsters);

        // Tower 테이블
        string queryTowers = @"
            CREATE TABLE IF NOT EXISTS Towers (
                tower_id INTEGER PRIMARY KEY,
                tower_name TEXT NOT NULL,
                cost INTEGER NOT NULL,
                damage REAL NOT NULL,
                special_ability TEXT
            );";
        ExecuteNonQuery(queryTowers);

        // Stage 테이블
        string queryStages = @"
            CREATE TABLE IF NOT EXISTS Stages (
                stage_id INTEGER PRIMARY KEY,
                stage_name TEXT NOT NULL,
                map_data TEXT NOT NULL
            );";
        ExecuteNonQuery(queryStages);

        // Wave 테이블
        string queryWaves = @"
            CREATE TABLE IF NOT EXISTS Waves (
                wave_id INTEGER PRIMARY KEY,
                stage_id INTEGER,
                wave_number INTEGER NOT NULL
            );";
        ExecuteNonQuery(queryWaves);

        // WaveSpawns 테이블 (AUTOINCREMENT 적용)
        string queryWaveSpawns = @"
            CREATE TABLE IF NOT EXISTS WaveSpawns (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                wave_id INTEGER,
                monster_id INTEGER,
                spawn_count INTEGER NOT NULL
            );";
        ExecuteNonQuery(queryWaveSpawns);

        // 인덱스 생성
        ExecuteNonQuery("CREATE UNIQUE INDEX IF NOT EXISTS idx_wave_monster ON WaveSpawns(wave_id, monster_id);");

        // PlayerAccount 테이블
        string queryAccount = @"
            CREATE TABLE IF NOT EXISTS PlayerAccounts (
                player_id TEXT PRIMARY KEY,
                password TEXT NOT NULL,
                created_at TEXT NOT NULL
            );";
        ExecuteNonQuery(queryAccount);

        // PlayerProgress 테이블
        string queryProgress = @"
            CREATE TABLE IF NOT EXISTS PlayerProgress (
                player_id TEXT PRIMARY KEY,
                currency_normal INTEGER NOT NULL,
                currency_special INTEGER,
                progress_data TEXT,
                last_sync_date TEXT NOT NULL
            );";
        ExecuteNonQuery(queryProgress);

        Debug.Log("테이블 생성 완료");
    }

    private bool IsFirstRun()
    {
        // 몬스터 테이블의 갯수를 세어 판단
        IDbCommand dbCmd = dbConnection.CreateCommand();
        dbCmd.CommandText = "SELECT COUNT(*) FROM Monsters";

        // ExecuteScalar는 결과의 첫 번째 값(숫자) 하나만 가져옴
        long count = (long)dbCmd.ExecuteScalar();

        dbCmd.Dispose();
        return count == 0;
    }

    private void InsertInitialData()
    {
        Debug.Log("초기 데이터 삽입 시작... ");

        // --------------------------------------------------------
        // [1] 몬스터 정보 (Monsters)
        // 컬럼: monster_id (PK), monster_name, health, speed, reward
        // --------------------------------------------------------
        // ExecuteNonQuery("INSERT INTO Monsters (monster_id, monster_name, health, speed, reward) VALUES (1, 'Slime', 100, 2.0, 10)");
        // ExecuteNonQuery("INSERT INTO Monsters (monster_id, monster_name, health, speed, reward) VALUES (2, 'Goblin', 150, 3.0, 20)");


        // --------------------------------------------------------
        // [2] 타워 정보 (Towers)
        // 컬럼: tower_id (PK), tower_name, cost, damage, special_ability (NULL 허용)
        // --------------------------------------------------------
        // ExecuteNonQuery("INSERT INTO Towers (tower_id, tower_name, cost, damage, special_ability) VALUES (1, 'Basic Tower', 100, 10.0, NULL)");
        // ExecuteNonQuery("INSERT INTO Towers (tower_id, tower_name, cost, damage, special_ability) VALUES (2, 'Sniper Tower', 200, 50.0, 'Penetration')");


        // --------------------------------------------------------
        // [3] 스테이지 정보 (Stages)
        // 컬럼: stage_id (PK), stage_name, map_data
        // --------------------------------------------------------
        // ExecuteNonQuery("INSERT INTO Stages (stage_id, stage_name, map_data) VALUES (1, 'Stage 1', 'MapJsonStringHere...')");


        // --------------------------------------------------------
        // [4] 웨이브 정보 (Waves)
        // 컬럼: wave_id (PK), stage_id (FK), wave_number
        // --------------------------------------------------------
        // ExecuteNonQuery("INSERT INTO Waves (wave_id, stage_id, wave_number) VALUES (1, 1, 1)");


        // --------------------------------------------------------
        // [5] 웨이브 별 몬스터 스폰 정보 (WaveSpawns)
        // 컬럼: wave_id (PK, FK), monster_id (PK, FK), spawn_count
        // (주의: 코드에서 id 컬럼을 추가했다면 id 값은 생략하거나 NULL로 넣으면 자동 증가함)
        // --------------------------------------------------------
        // ExecuteNonQuery("INSERT INTO WaveSpawns (wave_id, monster_id, spawn_count) VALUES (1, 1, 5)");


        // --------------------------------------------------------
        // [6] 플레이어 계정 (PlayerAccounts) - 보통 회원가입 시 생성되므로 초기 데이터 불필요 가능성 높음
        // 컬럼: player_id (PK), password, created_at
        // --------------------------------------------------------
        // ExecuteNonQuery("INSERT INTO PlayerAccounts (player_id, password, created_at) VALUES ('testUser', '1234', '2025-10-20 12:00:00')");


        // --------------------------------------------------------
        // [7] 플레이어 진행 상황 (PlayerProgress)
        // 컬럼: player_id (PK, FK), currency_normal, currency_special, progress_data, last_sync_date
        // --------------------------------------------------------
        // ExecuteNonQuery("INSERT INTO PlayerProgress (player_id, currency_normal, currency_special, progress_data, last_sync_date) VALUES ('testUser', 0, 0, NULL, '2025-10-20 12:00:00')");


        Debug.Log("초기 데이터 삽입 완료.");
    }

    // 헬퍼 함수: 리턴값이 없는 쿼리(CREATE, INSERT, UPDATE, DELETE) 실행용
    private void ExecuteNonQuery(string query)
    {
        using (IDbCommand dbCmd = dbConnection.CreateCommand())
        {
            dbCmd.CommandText = query;
            dbCmd.ExecuteNonQuery();
        }
    }

    #region 데이터 조회 메서드 예시 (ADO.NET 방식)

    public List<Monster> GetAllMonsters()
    {
        List<Monster> monsters = new List<Monster>();

        using (IDbCommand dbCmd = dbConnection.CreateCommand())
        {
            dbCmd.CommandText = "SELECT * FROM Monsters";

            using (IDataReader reader = dbCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Monster m = new Monster();
                    // 컬럼 순서나 이름을 통해 데이터를 가져와 형변환 해야 함
                    m.monster_id = reader.GetInt32(0); // 0번째 컬럼
                    m.monster_name = reader.GetString(1); // 1번째 컬럼
                    m.health = reader.GetFloat(2);
                    m.speed = reader.GetFloat(3);
                    m.reward = reader.GetInt32(4);

                    monsters.Add(m);
                }
            }
        }
        return monsters;
    }

    // 1. 모든 타워 정보 가져오기 (상점 초기화용)
    public List<Tower> GetAllTowers()
    {
        List<Tower> list = new List<Tower>();
        using (IDbCommand dbCmd = dbConnection.CreateCommand())
        {
            dbCmd.CommandText = "SELECT * FROM Towers";
            using (IDataReader reader = dbCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new Tower
                    {
                        tower_id = reader.GetInt32(0),
                        tower_name = reader.GetString(1),
                        cost = reader.GetInt32(2),
                        damage = reader.GetFloat(3),
                        special_ability = reader.IsDBNull(4) ? null : reader.GetString(4)
                    });
                }
            }
        }
        return list;
    }

    // 2. 특정 스테이지의 웨이브 리스트 가져오기
    public List<Wave> GetWavesByStage(int stageId)
    {
        List<Wave> list = new List<Wave>();
        using (IDbCommand dbCmd = dbConnection.CreateCommand())
        {
            dbCmd.CommandText = "SELECT * FROM Waves WHERE stage_id = @sid ORDER BY wave_number ASC";
            AddParameter(dbCmd, "@sid", stageId);

            using (IDataReader reader = dbCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new Wave
                    {
                        wave_id = reader.GetInt32(0),
                        stage_id = reader.GetInt32(1),
                        wave_number = reader.GetInt32(2)
                    });
                }
            }
        }
        return list;
    }

    // 3. 특정 웨이브에 스폰될 몬스터 정보 가져오기
    public List<WaveSpawn> GetSpawnsByWave(int waveId)
    {
        List<WaveSpawn> list = new List<WaveSpawn>();
        using (IDbCommand dbCmd = dbConnection.CreateCommand())
        {
            dbCmd.CommandText = "SELECT * FROM WaveSpawns WHERE wave_id = @wid";
            AddParameter(dbCmd, "@wid", waveId);

            using (IDataReader reader = dbCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new WaveSpawn
                    {
                        id = reader.GetInt32(0),
                        wave_id = reader.GetInt32(1),
                        monster_id = reader.GetInt32(2),
                        spawn_count = reader.GetInt32(3)
                    });
                }
            }
        }
        return list;
    }

    // 4. 회원가입 (계정 생성 + 초기 진행 데이터 생성 트랜잭션)
    public bool RegisterUser(string userId, string password)
    {
        try
        {
            // ID 중복 체크는 생략되었으나, UNIQUE 제약조건 때문에 중복시 예외 발생함

            // 1) 계정 생성
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                dbCmd.CommandText = "INSERT INTO PlayerAccounts (player_id, password, created_at) VALUES (@uid, @pw, @date)";
                AddParameter(dbCmd, "@uid", userId);
                AddParameter(dbCmd, "@pw", password);
                AddParameter(dbCmd, "@date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                dbCmd.ExecuteNonQuery();
            }

            // 2) 초기 진행 데이터 생성 (기본 골드 0, 젬 0)
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                dbCmd.CommandText = "INSERT INTO PlayerProgress (player_id, currency_normal, currency_special, last_sync_date) VALUES (@uid, 0, 0, @date)";
                AddParameter(dbCmd, "@uid", userId);
                AddParameter(dbCmd, "@date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                dbCmd.ExecuteNonQuery();
            }

            Debug.Log($"유저 등록 성공: {userId}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"회원가입 실패 (ID 중복 등): {e.Message}");
            return false;
        }
    }

    // 5. 로그인 (비밀번호 검증)
    public bool LoginUser(string userId, string password)
    {
        using (IDbCommand dbCmd = dbConnection.CreateCommand())
        {
            dbCmd.CommandText = "SELECT password FROM PlayerAccounts WHERE player_id = @uid";
            AddParameter(dbCmd, "@uid", userId);

            object result = dbCmd.ExecuteScalar();

            if (result != null && result.ToString() == password)
            {
                Debug.Log("로그인 성공");
                return true;
            }
        }
        Debug.Log("로그인 실패: ID 없음 혹은 비번 불일치");
        return false;
    }

    // 6. 플레이어 진행도 가져오기 (게임 시작 시 호출)
    public PlayerProgress LoadPlayerProgress(string userId)
    {
        PlayerProgress progress = null;
        using (IDbCommand dbCmd = dbConnection.CreateCommand())
        {
            dbCmd.CommandText = "SELECT * FROM PlayerProgress WHERE player_id = @uid";
            AddParameter(dbCmd, "@uid", userId);

            using (IDataReader reader = dbCmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    progress = new PlayerProgress
                    {
                        player_id = reader.GetString(0),
                        currency_normal = reader.GetInt32(1),
                        currency_special = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                        progress_data = reader.IsDBNull(3) ? "" : reader.GetString(3),
                        last_sync_date = DateTime.Parse(reader.GetString(4))
                    };
                }
            }
        }
        return progress;
    }

    // 7. 진행도 저장 (게임 종료, 스테이지 클리어 시 호출)
    public void SavePlayerProgress(string userId, int gold, int gem, string customData)
    {
        using (IDbCommand dbCmd = dbConnection.CreateCommand())
        {
            // UPDATE 쿼리 사용
            string query = @"
                UPDATE PlayerProgress 
                SET currency_normal = @gold, 
                    currency_special = @gem, 
                    progress_data = @data, 
                    last_sync_date = @date 
                WHERE player_id = @uid";

            dbCmd.CommandText = query;
            AddParameter(dbCmd, "@gold", gold);
            AddParameter(dbCmd, "@gem", gem);
            AddParameter(dbCmd, "@data", customData);
            AddParameter(dbCmd, "@date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            AddParameter(dbCmd, "@uid", userId);

            int rows = dbCmd.ExecuteNonQuery();
            if (rows > 0) Debug.Log("저장 완료");
        }
    }

    public void AddMonster(Monster monster)
    {
        // SQL Injection 방지를 위해 파라미터 사용 권장
        using (IDbCommand dbCmd = dbConnection.CreateCommand())
        {
            string query = "INSERT INTO Monsters (monster_id, monster_name, health, speed, reward) VALUES (@id, @name, @hp, @spd, @rwd)";
            dbCmd.CommandText = query;

            // 파라미터 추가 헬퍼 로직
            AddParameter(dbCmd, "@id", monster.monster_id);
            AddParameter(dbCmd, "@name", monster.monster_name);
            AddParameter(dbCmd, "@hp", monster.health);
            AddParameter(dbCmd, "@spd", monster.speed);
            AddParameter(dbCmd, "@rwd", monster.reward);

            dbCmd.ExecuteNonQuery();
        }
    }

    // 파라미터 추가를 쉽게 하기 위한 헬퍼 함수
    private void AddParameter(IDbCommand cmd, string paramName, object value)
    {
        IDbDataParameter param = cmd.CreateParameter();
        param.ParameterName = paramName;
        param.Value = value;
        cmd.Parameters.Add(param);
    }

    #endregion

    #region 테이블 모델 (순수 데이터 클래스)
    // Mono.Data.Sqlite는 속성([Table], [PrimaryKey] 등)을 인식하지 못하므로 제거했습니다.
    // 순수하게 데이터만 담는 그릇 역할만 합니다.

    public class Monster
    {
        public int monster_id { get; set; }
        public string monster_name { get; set; }
        public float health { get; set; }
        public float speed { get; set; }
        public int reward { get; set; }
    }

    public class Tower
    {
        public int tower_id { get; set; }
        public string tower_name { get; set; }
        public int cost { get; set; }
        public float damage { get; set; }
        public string special_ability { get; set; }
    }

    public class Stage
    {
        public int stage_id { get; set; }
        public string stage_name { get; set; }
        public string map_data { get; set; }
    }

    public class Wave
    {
        public int wave_id { get; set; }
        public int stage_id { get; set; }
        public int wave_number { get; set; }
    }

    public class WaveSpawn
    {
        public int id { get; set; }
        public int wave_id { get; set; }
        public int monster_id { get; set; }
        public int spawn_count { get; set; }
    }

    public class PlayerAccount
    {
        public string player_id { get; set; }
        public string password { get; set; }
        public DateTime created_at { get; set; }
    }

    public class PlayerProgress
    {
        public string player_id { get; set; }
        public int currency_normal { get; set; }
        public int? currency_special { get; set; }
        public string progress_data { get; set; }
        public DateTime last_sync_date { get; set; }
    }
    #endregion

    void OnDestroy()
    {
        if (dbConnection != null && dbConnection.State == ConnectionState.Open)
        {
            dbConnection.Close();
            dbConnection = null;
        }
    }
}