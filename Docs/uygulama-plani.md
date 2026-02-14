# SCRAP MAGNET TITAN — UYGULAMA PLANI (Level 1)

## 1. MİMARİ GENEL BAKIŞ

```
┌──────────────────────────────────────────────────────────────────────┐
│                         KATMANLI MİMARİ                             │
├──────────────────────────────────────────────────────────────────────┤
│                                                                      │
│  ┌──────────────────────────────────────────────────────┐           │
│  │              GameEvents (Static Event Bus)            │           │
│  │  Tüm sistemler arası iletişimi merkezi event'ler     │           │
│  │  üzerinden sağlar. Doğrudan referans ihtiyacını       │           │
│  │  ortadan kaldırır.                                    │           │
│  └────────┬────────────┬──────────────┬─────────────────┘           │
│           │            │              │                              │
│  ┌────────▼───┐  ┌─────▼─────┐  ┌────▼───────┐  ┌──────────────┐  │
│  │   CORE     │  │  PLAYER   │  │   WORLD    │  │  FEEDBACK    │  │
│  │            │  │           │  │            │  │              │  │
│  │ GameMgr   │  │ Player    │  │ Obstacle   │  │ GameCamera   │  │
│  │ LevelMgr  │  │ Vortex    │  │ Gate       │  │ FeedbackMgr  │  │
│  │ UIManager  │  │ Magnet    │  │ FinishLine │  │              │  │
│  │           │  │ ScrapItem │  │ Titan      │  │              │  │
│  │           │  │           │  │ HardObs.   │  │              │  │
│  └────────┬───┘  └─────┬─────┘  └────┬───────┘  └──────┬───────┘  │
│           │            │              │                  │          │
│  ┌────────▼────────────▼──────────────▼──────────────────▼───────┐  │
│  │                    DATA LAYER (ScriptableObjects)             │  │
│  │  LevelData: Level konfigürasyonu, prefab referansı, zorluk   │  │
│  └──────────────────────────────────────────────────────────────┘  │
│                                                                      │
│  ┌──────────────────────────────────────────────────────────────┐  │
│  │                 INFRASTRUCTURE (Değişmedi)                    │  │
│  │  InputRouter  │  AudioService  │  Haptics                    │  │
│  └──────────────────────────────────────────────────────────────┘  │
│                                                                      │
└──────────────────────────────────────────────────────────────────────┘
```

## 2. KULLANILAN DESIGN PATTERN'LER

### Observer Pattern (Event Bus)
**Nerede:** `GameEvents.cs` — static event bus.
**Neden:** Sistemlerin birbirini doğrudan tanımasını engeller. ObstacleBase
yıkıldığında `GameEvents.ObstacleDestroyed()` çağırır, FeedbackManager bunu
dinleyerek kamera shake + particle tetikler. İkisi birbirinin varlığından habersiz.

```
ObstacleBase ──fire──▶ GameEvents.ObstacleDestroyed()
                              │
FeedbackManager ◀──listen─────┘  → Shake + Particle + Sound
GameCamera      ◀──listen─────┘  → FOV Punch
```

### Interface Segregation (IDamageable)
**Nerede:** `IDamageable.cs`
**Neden:** ObstacleBase, TitanController veya gelecekte eklenen herhangi bir
hasar alan nesne bu interface'i implement eder. Hasar veren sistemler concrete
sınıfı bilmek zorunda değil.

### Data-Driven Design (ScriptableObject)
**Nerede:** `LevelData.cs`
**Neden:** Level ayarları (Titan kapasitesi, ödül çarpanları) Inspector'da
değiştirilebilir. Kod değişikliği gerektirmez. Designer-friendly.

### Component Composition (Unity MonoBehaviour)
**Nerede:** Tüm MonoBehaviour sınıfları.
**Neden:** Player objesi = PlayerController + VortexManager + MagnetSystem.
Her component tek bir sorumluluk taşır. Birleşim Inspector'da yapılır.

### Singleton (Lean)
**Nerede:** GameManagerTT, LevelManager, FeedbackManager.
**Neden:** Oyun boyunca tek instance gereken manager'lar. DontDestroyOnLoad
kullanmadan, scene-scoped singleton olarak tutuyoruz.

### Mediator Pattern (FeedbackManager)
**Nerede:** `FeedbackManager.cs`
**Neden:** Tüm juice/feedback mantığını tek bir yerde toplar. ObstacleBase'in
AudioService veya Haptics'i tanımasına gerek kalmaz. FeedbackManager aracı
olarak event'leri dinleyip uygun feedback'i tetikler.

---

## 3. DOSYA YAPISI (YENİ + MEVCUT)

```
Assets/_Game/Scripts/
│
├── Core/
│   ├── GameManagerTT.cs          [MEVCUT - değişiklik yok]
│   ├── UIManagerTT.cs            [MEVCUT - değişiklik yok]
│   ├── Events/
│   │   └── GameEvents.cs         [YENİ] Static event bus
│   └── Interfaces/
│       └── IDamageable.cs        [YENİ] Hasar interface'i
│
├── Data/
│   ├── LevelData.cs              [YENİ] ScriptableObject
│   └── GateOperation.cs          [YENİ] Enum (StackManager'dan çıkarıldı)
│
├── Level/
│   └── LevelManager.cs           [YENİ] Level yükleme/temizleme
│
├── Player/
│   ├── PlayerController.cs       [MEVCUT - değişiklik yok]
│   ├── MagnetSystem.cs           [MEVCUT - değişiklik yok]
│   ├── ScrapItem.cs              [MEVCUT - değişiklik yok]
│   └── StackManager.cs           [MEVCUT - REFACTOR: event fire + scrap spawn]
│       (sınıf adı: VortexManager — dosya Unity'de rename edilebilir)
│
├── World/
│   ├── ICollectable.cs           [MEVCUT - değişiklik yok]
│   ├── ObstacleBase.cs           [MEVCUT - REFACTOR: IDamageable + events]
│   ├── GateController.cs         [MEVCUT - REFACTOR: event fire]
│   ├── FinishLine.cs             [MEVCUT - değişiklik yok]
│   ├── TitanController.cs        [YENİ] Titan boss inşaat sistemi
│   └── HardObstacle.cs           [YENİ] Yıkılamaz engel (testere)
│
├── Camera/
│   └── GameCamera.cs             [YENİ] Takip + Juice efektleri
│
└── Feedback/
    └── FeedbackManager.cs        [YENİ] Merkezi feedback koordinatörü
```

**Yeni: 9 dosya | Refactor: 3 dosya | Değişmeyen: 7 dosya**

---

## 4. EVENT AKIŞ DİYAGRAMI

```
ScrapItem toplanır
  └─▶ VortexManager.AddScrap()
       └─▶ GameEvents.ScrapCollected(scrap)
            ├─▶ FeedbackManager: Haptics.CoinPickup() + "ting" ses
            └─▶ GameCamera: (opsiyonel mini pulse)

Oyuncu engele çarpar
  └─▶ ObstacleBase.StartGrinding()
       └─▶ GameEvents.GrindStarted()
            └─▶ FeedbackManager: grinding loop ses başlar

Her 0.1s grind tick
  └─▶ VortexManager.RemoveScrap() + ObstacleBase.TakeDamage()
       ├─▶ GameEvents.GrindTick()
       │    └─▶ FeedbackManager: Haptics.Impact() + kamera shake
       └─▶ GameEvents.DamageTaken(obstacle, remainingHP)

Engel yıkılır (HP=0)
  └─▶ ObstacleBase.Break()
       └─▶ GameEvents.ObstacleDestroyed(obstacle)
            ├─▶ FeedbackManager: büyük shake + FOV punch + particle
            └─▶ GameCamera: PunchFOV(8f)

Gate'den geçilir
  └─▶ GateController.OnTriggerEnter()
       └─▶ GameEvents.GateActivated(operation, value)
            └─▶ FeedbackManager: pozitif/negatif ses

Testereye çarpılır
  └─▶ HardObstacle.OnTriggerEnter()
       └─▶ GameEvents.HardObstacleHit(position)
            └─▶ FeedbackManager: Haptics.Impact() + kırmızı flash

Finish Line
  └─▶ FinishLine.OnTriggerEnter()
       └─▶ Scraps Titan'a uçar
            └─▶ TitanController.ReceiveScrap()
                 └─▶ GameEvents.TitanProgress(0.0 → 1.0)
                      └─▶ UI: Progress bar güncellenir
```

---

## 5. LEVEL 1 İÇİN GEREKEN HER ŞEY

### Kodda Hazır Olan Sistemler
- [x] PlayerController (swerve + ileri hareket)
- [x] VortexManager (yörünge sistemi)
- [x] MagnetSystem (mıknatıs trigger)
- [x] ScrapItem (toplama + yörünge + fırlatma)
- [x] ObstacleBase (grinding + HP + patlama)
- [x] GateController (matematik kapılar)
- [x] FinishLine (bitiş trigger + scrap launch)
- [x] InputRouter (dokunmatik input)
- [x] AudioService (temel sesler)
- [x] Haptics (titreşim)
- [x] UIManagerTT (panel yönetimi)
- [x] GameManagerTT (state machine)

### Bu PR ile Eklenen Sistemler
- [ ] GameEvents (event bus — tüm sistemleri bağlar)
- [ ] IDamageable (interface — decoupling)
- [ ] LevelData (ScriptableObject — level config)
- [ ] LevelManager (level yükleme)
- [ ] GameCamera (kamera takip + juice)
- [ ] FeedbackManager (merkezi feedback)
- [ ] TitanController (level sonu boss)
- [ ] HardObstacle (testere engeli — Level 1'de yok ama kod hazır)

### Level 1 Konfigürasyonu
```
Level 1: "İlk Adım"
├── Toplam scrap: 30 adet (yol üzerine serpilmiş)
├── Engeller: 2 adet basit kutu (2 HP, 2 HP)
├── Kapılar: 1 adet pozitif (+10)
├── Testere: Yok
├── Titan kapasitesi: 30
├── Süre: ~30 saniye
└── Fail riski: Çok düşük (tutorial)
```

---

## 6. UNITY İÇİ UYGULAMA ADIMLARI

### Adım 1: Klasör Yapısını Oluştur
Unity Project penceresinde:
```
Assets/_Game/Scripts/ altında şu klasörleri oluştur:
  Core/Events/
  Core/Interfaces/
  Data/
  Level/
  Camera/
  Feedback/
```
Script dosyaları zaten doğru klasörlere yazıldı.

### Adım 2: ScriptableObject Asset Oluştur
1. Project penceresinde sağ tık → Create → ScrapMagnet → Level Data
2. İsim: "Level_01"
3. Inspector'da ayarla:
   - Level Index: 1
   - Display Name: "İlk Adım"
   - Level Prefab: (Adım 5'te oluşturulacak)
   - Titan Capacity: 30
   - Base Coin Reward: 100

### Adım 3: Sahne Hierarchy'sini Kur
```
Hierarchy:
├── --- MANAGERS ---
│   ├── GameManager        → GameManagerTT component
│   ├── LevelManager       → LevelManager component
│   │                        (levels array'e Level_01 SO'yu sürükle)
│   ├── FeedbackManager    → FeedbackManager component
│   └── AudioService       → AudioService component (DontDestroyOnLoad)
│
├── --- CAMERA ---
│   └── Main Camera        → GameCamera component
│                            (target = Player transform)
│
├── --- UI ---
│   └── Canvas             → UIManagerTT component (mevcut)
│
├── --- PLAYER ---
│   └── Player             → (mevcut prefab)
│       ├── PlayerController
│       ├── VortexManager (StackManager)
│       ├── MagnetSystem (child: MagnetTrigger)
│       └── MeshRenderer (Sphere, turuncu material)
│
├── --- WORLD ---
│   └── LevelParent        → Boş GameObject
│       └── (Level prefab buraya instantiate edilir)
│
└── --- INPUT ---
    └── InputRouter         → InputRouter component (DontDestroyOnLoad)
```

### Adım 4: Player Prefab Yapısı
```
Player (GameObject)
├── Components:
│   ├── PlayerController
│   ├── Rigidbody (isKinematic: false, useGravity: true)
│   └── CapsuleCollider (veya SphereCollider)
│
├── VortexPivot (Child GameObject)
│   └── VortexManager component
│
├── MagnetTrigger (Child GameObject)
│   ├── SphereCollider (isTrigger: true, radius: 5)
│   └── MagnetSystem component
│
└── Visual (Child GameObject)
    ├── Sphere mesh (scale 0.8)
    └── Material: Turuncu Unlit
```

### Adım 5: Level 1 Prefab Oluştur
1. Boş GameObject oluştur, adı: "Level_01_Prefab"
2. İçine ekle:

```
Level_01_Prefab
├── Road
│   └── Plane (scale: 2, 1, 50)
│       Material: Teal Unlit
│
├── Walls (görünmez sınır)
│   ├── Wall_L: Cube (pos: -3.5, 1, 25) (scale: 0.2, 2, 100)
│   └── Wall_R: Cube (pos:  3.5, 1, 25) (scale: 0.2, 2, 100)
│
├── Scraps (30 adet)
│   ├── Scrap_01: Scrap prefab (pos: -1, 0.3, 5)
│   ├── Scrap_02: Scrap prefab (pos:  0, 0.3, 6)
│   ├── Scrap_03: Scrap prefab (pos:  1, 0.3, 7)
│   │   ... (yol boyunca dağıtılmış, z: 5-80 arası)
│   └── Scrap_30: Scrap prefab (pos: 0, 0.3, 78)
│
├── Obstacles
│   ├── Obstacle_01: Cube (pos: 0, 0.5, 30)
│   │   └── ObstacleBase (maxHP: 2)
│   └── Obstacle_02: Cube (pos: 1, 0.5, 55)
│       └── ObstacleBase (maxHP: 2)
│
├── Gates
│   └── GatePair_01 (pos: 0, 0, 45)
│       ├── Gate_Left:  GateController (Add, +10)  pos: -1.5
│       └── Gate_Right: GateController (Add, +5)   pos:  1.5
│
├── FinishLine
│   └── FinishLine (pos: 0, 0, 90)
│       ├── FinishLine component
│       └── BoxCollider (isTrigger: true)
│
└── Titan
    └── TitanBoss (pos: 0, 0, 100)
        ├── TitanController component (totalCapacity: 30)
        ├── SphereCollider (isTrigger: true, radius: 3)
        └── BodyParts (child objeler):
            ├── Legs:   2x Cube (bacak)
            ├── Torso:  Capsule (gövde)
            ├── Arms:   2x Cube (kol)
            └── Head:   Sphere (kafa)
            (Hepsi başlangıçta SetActive(false))
```

3. Bu prefab'ı Level_01 ScriptableObject'in "Level Prefab" alanına sürükle.

### Adım 6: Scrap Prefab Yapısı
```
Scrap (Prefab)
├── Components:
│   ├── ScrapItem
│   ├── BoxCollider (size: 0.3)
│   └── Rigidbody (isKinematic: true)
│
└── Visual
    ├── Cube mesh (scale: 0.3, 0.3, 0.3)
    └── Material: Gri metalik Unlit
```
VortexManager'ın Inspector'ında "Scrap Prefab" alanına bu prefab'ı sürükle.

### Adım 7: Test Et
1. Play'e bas
2. Menu'den Play butonuna tıkla
3. Parmağını sürükleyerek (veya mouse) swerve yap
4. Scrap'leri topla, vortex'in büyüdüğünü gör
5. Engele çarp, grinding'i gör
6. Gate'den geç, scrap sayısının arttığını gör
7. Finish line'a ulaş, Titan'ın inşa edildiğini gör

---

## 7. BAĞIMLILIK MATRİSİ

| Sınıf | Bildiği Sınıflar | İletişim Yöntemi |
|-------|-------------------|------------------|
| **PlayerController** | GameManagerTT, InputRouter | Event subscription |
| **VortexManager** | ScrapItem, GameManagerTT, GameEvents | Direct + Events |
| **MagnetSystem** | ICollectable, GameManagerTT | Interface |
| **ObstacleBase** | PlayerController, VortexManager, GameEvents, IDamageable | Interface + Events |
| **GateController** | VortexManager, GameEvents | Direct + Events |
| **FinishLine** | PlayerController, VortexManager, GameManagerTT | Direct |
| **TitanController** | ScrapItem, GameEvents | Events |
| **HardObstacle** | VortexManager, GameManagerTT, GameEvents | Events |
| **FeedbackManager** | GameCamera, GameEvents, AudioService, Haptics | Events |
| **GameCamera** | Transform (target) | Follow |
| **LevelManager** | LevelData, GameManagerTT | Data-driven |
| **GameManagerTT** | — (merkez, kimseyi bilmez) | Events (fire only) |

---

## 8. İLERİ ADIMLAR (Level 1 Sonrası)

1. **Level 2-5:** LevelData SO'larını kopyala, zorluk parametrelerini artır
2. **Particle efektler:** FeedbackManager'a ParticleSystem referansları ekle
3. **Rewarded Video:** AdManager sınıfı oluştur, Fail ekranına "Continue" butonu
4. **Sound pass:** AudioService'e grinding loop + toplama ting sesleri ekle
5. **Titan skin'leri:** Her 5 level'da farklı Titan modeli
