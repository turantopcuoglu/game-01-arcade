# Scrap Magnet — UI Tasarım Spesifikasyonu

> **Hedef Platform:** Mobil (iOS / Android)
> **Referans Çözünürlük:** 1080 × 1920 (9:16 portrait)
> **UI Sistemi:** Unity UGUI (Canvas + TextMeshPro)
> **Renk Paleti:** Endüstriyel / Metal teması

---

## 1. Genel Tasarım İlkeleri

| Kural | Açıklama |
|-------|----------|
| **Safe Area** | Tüm interaktif elemanlar `Screen.safeArea` içinde kalmalı (çentikli ekranlar) |
| **Minimum Dokunma Alanı** | Butonlar en az **120×120 px** (fiziksel ~9mm) |
| **Font** | TextMeshPro — Kalın başlıklar + Normal gövde metni |
| **Animasyon** | Panel geçişleri `DOTween` veya `Animator` ile (0.3s ease) |
| **Renk Tonu** | Metal griler + turuncu/sarı vurgular (endüstriyel tema) |

### 1.1 Renk Paleti

```
Ana Arka Plan     : #1A1A2E  (Koyu lacivert)
Panel Arka Plan   : #16213E  (Koyu mavi, %90 opacity)
Birincil Vurgu    : #FF6B35  (Turuncu — butonlar, önemli metinler)
İkincil Vurgu     : #FFD700  (Altın sarı — scrap sayacı, ödüller)
Başarı Rengi      : #4CAF50  (Yeşil — kazanma, doğru gate)
Tehlike Rengi     : #E53935  (Kırmızı — kaybetme, yanlış gate)
Metin (Birincil)  : #FFFFFF  (Beyaz)
Metin (İkincil)   : #B0BEC5  (Gri — açıklamalar)
Panel Kenarlık    : #FF6B35  (Turuncu, 2px)
```

### 1.2 Tipografi

```
Başlık (H1)    : TMP - Bold, 72px, #FFFFFF, outline 3px #000000
Alt Başlık (H2): TMP - Bold, 48px, #FFD700
Gövde Metin    : TMP - Regular, 36px, #B0BEC5
Buton Metin    : TMP - Bold, 42px, #FFFFFF
Sayaç (HUD)    : TMP - Bold, 56px, #FFD700, outline 2px #000000
```

---

## 2. Canvas Yapılandırması

```
Canvas
├── Render Mode     : Screen Space - Overlay
├── UI Scale Mode   : Scale With Screen Size
├── Reference Res.  : 1080 × 1920
├── Match           : 0.5 (Width/Height dengeli)
├── Sort Order      : 10
│
├── [SafeAreaPanel]          ← RectTransform: safeArea'ya göre ayarlanır
│   ├── [BootScreen]
│   ├── [MenuPanel]
│   ├── [HUDPanel]
│   ├── [PausePanel]
│   ├── [WinPanel]
│   └── [FailPanel]
│
└── [OverlayLayer]           ← Geçiş efektleri, popup'lar (Sort: 15)
    ├── [PassingEffect]
    └── [TutorialOverlay]
```

---

## 3. Panel Detayları

---

### 3.1 Boot Screen (Yükleme Ekranı)

```
┌─────────────────────────────┐
│                             │
│                             │
│                             │
│        [OYUN LOGOSU]        │
│       SCRAP MAGNET          │
│                             │
│                             │
│      ███████░░░  70%        │
│      Loading...             │
│                             │
└─────────────────────────────┘
```

**Hiyerarşi:**
```
BootScreen (Panel)
├── Anchor         : Stretch-Stretch (tüm ekran)
├── Color          : #1A1A2E (solid)
│
├── LogoImage
│   ├── Anchor     : Middle-Center
│   ├── Pos        : (0, 200)
│   ├── Size       : 400 × 400
│   └── Sprite     : game_logo.png
│
├── TitleText (TMP)
│   ├── Anchor     : Middle-Center
│   ├── Pos        : (0, -50)
│   ├── Font Size  : 72
│   ├── Color      : #FFD700
│   ├── Alignment  : Center
│   └── Text       : "SCRAP MAGNET"
│
├── ProgressBarBG
│   ├── Anchor     : Bottom-Center
│   ├── Pos        : (0, 300)
│   ├── Size       : 600 × 30
│   ├── Color      : #2C2C2C
│   ├── Corner     : Rounded (15px)
│   │
│   └── ProgressBarFill (Image)
│       ├── Type   : Filled (Horizontal)
│       ├── Color  : #FF6B35
│       └── Corner : Rounded (15px)
│
└── LoadingText (TMP)
    ├── Anchor     : Bottom-Center
    ├── Pos        : (0, 250)
    ├── Font Size  : 28
    ├── Color      : #B0BEC5
    └── Text       : "Loading..."
```

---

### 3.2 Menu Panel (Ana Menü)

```
┌─────────────────────────────┐
│                             │
│  ⚙ (Settings)              │
│                             │
│        SCRAP MAGNET         │
│                             │
│                             │
│       ┌─────────────┐      │
│       │  LEVEL  3   │      │
│       └─────────────┘      │
│                             │
│                             │
│     ╔═══════════════════╗   │
│     ║    ▶  OYNA        ║   │
│     ╚═══════════════════╝   │
│                             │
│   🔇    [HighScore: 250]   │
│                             │
└─────────────────────────────┘
```

**Hiyerarşi:**
```
MenuPanel (Panel)
├── Anchor         : Stretch-Stretch
├── Color          : Transparent (arka plan 3D sahne)
│
├── TopBar (Horizontal Layout)
│   ├── Anchor     : Top-Stretch
│   ├── Height     : 120
│   ├── Padding    : (40, 40, 20, 0)
│   │
│   ├── SettingsButton
│   │   ├── Anchor : Top-Left
│   │   ├── Size   : 100 × 100
│   │   ├── Sprite : icon_settings.png (dişli çark)
│   │   ├── Color  : #FFFFFF, alpha 0.7
│   │   └── OnClick: OpenSettingsPanel()
│   │
│   └── SoundToggle
│       ├── Anchor : Top-Right
│       ├── Size   : 100 × 100
│       ├── Sprite : icon_sound_on.png / icon_sound_off.png
│       └── OnClick: ToggleSound()
│
├── CenterContent (Vertical Layout)
│   ├── Anchor     : Middle-Center
│   ├── Spacing    : 40
│   │
│   ├── TitleText (TMP)
│   │   ├── Font Size  : 80
│   │   ├── Color      : #FFD700
│   │   ├── Style      : Bold, uppercase
│   │   ├── Outline    : 4px #000000
│   │   └── Text       : "SCRAP MAGNET"
│   │
│   └── LevelBadge (Image + Text)
│       ├── Size       : 300 × 100
│       ├── BG Color   : #16213E, alpha 0.8
│       ├── Border     : 2px #FF6B35, rounded 20px
│       │
│       └── LevelText (TMP)  ← [levelText referansı]
│           ├── Font Size  : 48
│           ├── Color      : #FFFFFF
│           └── Text       : "LEVEL {n}"
│
├── PlayButton
│   ├── Anchor     : Bottom-Center
│   ├── Pos        : (0, 350)
│   ├── Size       : 500 × 140
│   ├── BG Color   : #FF6B35
│   ├── Border     : Rounded 30px
│   ├── Shadow     : (0, -8), color #CC4400
│   ├── OnClick    : OnPlayButtonPressed()
│   │
│   ├── PlayIcon (Image)
│   │   ├── Size   : 50 × 50
│   │   ├── Pos    : (-80, 0)
│   │   └── Sprite : icon_play.png (▶ üçgen)
│   │
│   └── PlayText (TMP)
│       ├── Font Size  : 52
│       ├── Color      : #FFFFFF
│       ├── Style      : Bold
│       └── Text       : "OYNA"
│
│   [Buton Animasyonu]
│   - Idle    : Scale pulse 1.0 → 1.05 → 1.0 (1.5s loop)
│   - Pressed : Scale 0.9 (0.1s) → geri 1.0 (0.15s)
│
└── BottomInfo
    ├── Anchor     : Bottom-Center
    ├── Pos        : (0, 120)
    │
    └── HighScoreText (TMP)
        ├── Font Size  : 28
        ├── Color      : #B0BEC5
        └── Text       : "En Yüksek: {score}"
```

---

### 3.3 HUD Panel (Oyun İçi Arayüz)

```
┌─────────────────────────────┐
│  LEVEL 3            ⏸       │
│                             │
│                             │
│                             │
│                             │
│                             │
│        (OYUN ALANI)         │
│                             │
│                             │
│                             │
│                             │
│                             │
│                             │
│         🔩 × 24             │
│                             │
│ [█████████░░░░] Titan %60   │
└─────────────────────────────┘
```

**Hiyerarşi:**
```
HUDPanel (Panel)
├── Anchor         : Stretch-Stretch
├── Raycast Target : false (dokunma 3D'ye geçsin)
│
├── TopHUD (Horizontal Layout)
│   ├── Anchor     : Top-Stretch
│   ├── Height     : 120
│   ├── Padding    : (40, 40, 30, 0)
│   │
│   ├── LevelIndicator
│   │   ├── Anchor : Top-Left
│   │   ├── Size   : 200 × 70
│   │   ├── BG     : #16213E, alpha 0.7, rounded 15px
│   │   │
│   │   └── LevelText (TMP)
│   │       ├── Font Size  : 32
│   │       ├── Color      : #FFFFFF
│   │       └── Text       : "LEVEL {n}"
│   │
│   └── PauseButton
│       ├── Anchor : Top-Right
│       ├── Size   : 80 × 80
│       ├── Sprite : icon_pause.png (⏸)
│       ├── Color  : #FFFFFF, alpha 0.7
│       └── OnClick: → GameState.Pause
│
├── ScrapCounter (Horizontal Layout Group)
│   ├── Anchor     : Bottom-Center
│   ├── Pos        : (0, 250)
│   ├── Size       : 250 × 90
│   ├── BG         : #16213E, alpha 0.8, rounded 20px
│   ├── Spacing    : 15
│   │
│   ├── ScrapIcon (Image)
│   │   ├── Size   : 60 × 60
│   │   └── Sprite : icon_scrap.png (vida/metal parçası)
│   │
│   ├── MultiplySign (TMP)
│   │   ├── Font Size  : 36
│   │   ├── Color      : #B0BEC5
│   │   └── Text       : "×"
│   │
│   └── ScrapCountText (TMP)  ← [scrapCountText referansı]
│       ├── Font Size  : 56
│       ├── Color      : #FFD700
│       ├── Style      : Bold
│       ├── Outline    : 2px #000000
│       └── Text       : "0"
│
│   [Sayaç Animasyonu]
│   - Artış : Scale punch 1.3 (0.15s) + renk flash #FFFFFF → #FFD700
│   - Azalış: Scale punch 0.8 (0.1s) + renk flash #E53935 → #FFD700
│
└── TitanProgressBar (Opsiyonel — Finishing state'te görünür)
    ├── Anchor     : Bottom-Stretch
    ├── Height     : 50
    ├── Pos        : (0, 100)
    ├── Margin     : (60, 60, 0, 0)
    ├── Visible    : Sadece GameState.Finishing sırasında
    │
    ├── ProgressBG
    │   ├── Size   : Stretch × 30
    │   ├── Color  : #2C2C2C
    │   ├── Corner : Rounded 15px
    │   │
    │   └── ProgressFill (Image - Filled)
    │       ├── Fill   : Horizontal, left-to-right
    │       ├── Color  : Gradient #FF6B35 → #FFD700
    │       └── Amount : TitanController.Progress (0-1)
    │
    └── ProgressText (TMP)
        ├── Anchor     : Center
        ├── Font Size  : 24
        ├── Color      : #FFFFFF
        └── Text       : "Titan %{progress}"
```

---

### 3.4 Pause Panel (Duraklatma Menüsü)

```
┌─────────────────────────────┐
│                             │
│  ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓  │
│  ▓                       ▓  │
│  ▓     DURAKLATILDI      ▓  │
│  ▓                       ▓  │
│  ▓  ┌─────────────────┐  ▓  │
│  ▓  │  ▶  DEVAM ET    │  ▓  │
│  ▓  └─────────────────┘  ▓  │
│  ▓                       ▓  │
│  ▓  ┌─────────────────┐  ▓  │
│  ▓  │  ↺  YENİDEN     │  ▓  │
│  ▓  └─────────────────┘  ▓  │
│  ▓                       ▓  │
│  ▓  ┌─────────────────┐  ▓  │
│  ▓  │  ⚙  AYARLAR     │  ▓  │
│  ▓  └─────────────────┘  ▓  │
│  ▓                       ▓  │
│  ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓  │
│                             │
└─────────────────────────────┘
```

**Hiyerarşi:**
```
PausePanel (Panel)
├── Anchor         : Stretch-Stretch
│
├── DimOverlay (Image)
│   ├── Anchor     : Stretch-Stretch
│   ├── Color      : #000000, alpha 0.6
│   └── Raycast    : true (arkayı blokla)
│
├── PauseCard (Image)
│   ├── Anchor     : Middle-Center
│   ├── Size       : 700 × 900
│   ├── Color      : #16213E
│   ├── Border     : 3px #FF6B35, rounded 30px
│   │
│   ├── PauseTitle (TMP)
│   │   ├── Anchor     : Top-Center
│   │   ├── Pos        : (0, -60)
│   │   ├── Font Size  : 56
│   │   ├── Color      : #FFFFFF
│   │   ├── Style      : Bold
│   │   └── Text       : "DURAKLATILDI"
│   │
│   ├── Divider (Image)
│   │   ├── Anchor     : Top-Stretch
│   │   ├── Pos        : (0, -140)
│   │   ├── Size       : (stretch -80) × 3
│   │   └── Color      : #FF6B35, alpha 0.5
│   │
│   ├── ButtonsContainer (Vertical Layout)
│   │   ├── Anchor     : Middle-Center
│   │   ├── Spacing    : 30
│   │   │
│   │   ├── ResumeButton
│   │   │   ├── Size       : 500 × 110
│   │   │   ├── BG Color   : #FF6B35
│   │   │   ├── Corner     : Rounded 25px
│   │   │   ├── OnClick    : OnResumeButtonPressed()
│   │   │   │
│   │   │   ├── Icon (▶)
│   │   │   │   └── Size   : 40 × 40, Left padding 30
│   │   │   └── Text (TMP)
│   │   │       ├── Font Size  : 40
│   │   │       └── Text       : "DEVAM ET"
│   │   │
│   │   ├── RetryButton
│   │   │   ├── Size       : 500 × 110
│   │   │   ├── BG Color   : #2C3E6B
│   │   │   ├── Border     : 2px #FF6B35
│   │   │   ├── Corner     : Rounded 25px
│   │   │   ├── OnClick    : OnRetryButtonPressed()
│   │   │   │
│   │   │   ├── Icon (↺)
│   │   │   │   └── Size   : 40 × 40
│   │   │   └── Text (TMP)
│   │   │       ├── Font Size  : 40
│   │   │       └── Text       : "YENİDEN"
│   │   │
│   │   └── SettingsButton
│   │       ├── Size       : 500 × 110
│   │       ├── BG Color   : #2C3E6B
│   │       ├── Border     : 2px #FF6B35
│   │       ├── Corner     : Rounded 25px
│   │       ├── OnClick    : OpenSettingsPanel()
│   │       │
│   │       ├── Icon (⚙)
│   │       │   └── Size   : 40 × 40
│   │       └── Text (TMP)
│   │           ├── Font Size  : 40
│   │           └── Text       : "AYARLAR"
│   │
│   └── [Kart Animasyonu]
│       - Açılış : Scale 0.5 → 1.0 + Alpha 0 → 1 (0.3s, ease-out-back)
│       - Kapanış: Scale 1.0 → 0.8 + Alpha 1 → 0 (0.2s, ease-in)
```

---

### 3.5 Win Panel (Kazanma Ekranı)

```
┌─────────────────────────────┐
│                             │
│      ✦  ✦  ✦  ✦  ✦         │
│                             │
│        TEBRIKLER!           │
│                             │
│     ┌───────────────┐       │
│     │  ⭐ ⭐ ⭐     │       │
│     │               │       │
│     │  +150 coin    │       │
│     │  × 3 bonus    │       │
│     └───────────────┘       │
│                             │
│     ╔═══════════════╗       │
│     ║  SONRAKİ LVL  ║       │
│     ╚═══════════════╝       │
│                             │
│     ┌───────────────┐       │
│     │  ↺ Tekrar     │       │
│     └───────────────┘       │
│                             │
└─────────────────────────────┘
```

**Hiyerarşi:**
```
WinPanel (Panel)
├── Anchor         : Stretch-Stretch
│
├── DimOverlay (Image)
│   ├── Color      : #000000, alpha 0.5
│
├── ParticleOverlay (Konfeti / Parıltı efekti — opsiyonel)
│   └── ParticleSystem veya animated sprite
│
├── WinCard (Image)
│   ├── Anchor     : Middle-Center
│   ├── Size       : 750 × 1100
│   ├── Color      : #16213E
│   ├── Border     : 3px #FFD700, rounded 30px
│   │
│   ├── WinTitle (TMP)
│   │   ├── Pos        : (0, -50)
│   │   ├── Font Size  : 64
│   │   ├── Color      : #FFD700
│   │   ├── Style      : Bold
│   │   └── Text       : "TEBRİKLER!"
│   │
│   ├── StarsContainer (Horizontal Layout)
│   │   ├── Anchor     : Top-Center
│   │   ├── Pos        : (0, -160)
│   │   ├── Spacing    : 20
│   │   │
│   │   ├── Star1 (Image) — 100×100, sprite: star_filled/star_empty
│   │   ├── Star2 (Image) — 100×100
│   │   └── Star3 (Image) — 100×100
│   │   │
│   │   [Yıldız Animasyonu]
│   │   - Her yıldız sırayla (0.3s aralıkla) scale 0→1.2→1.0 + rotation -15→0
│   │
│   ├── RewardsContainer
│   │   ├── Anchor     : Middle-Center
│   │   ├── Pos        : (0, -30)
│   │   ├── Size       : 550 × 200
│   │   ├── BG         : #1A1A2E, rounded 20px
│   │   │
│   │   ├── CoinReward (Horizontal Layout)
│   │   │   ├── CoinIcon — 50×50, sprite: icon_coin.png
│   │   │   └── CoinText (TMP) — "+{baseCoinReward}", 42px, #FFD700
│   │   │
│   │   └── MultiplierText (TMP)
│   │       ├── Font Size  : 36
│   │       ├── Color      : #FF6B35
│   │       └── Text       : "× {multiplier} bonus!"
│   │
│   ├── NextLevelButton
│   │   ├── Anchor     : Bottom-Center
│   │   ├── Pos        : (0, 200)
│   │   ├── Size       : 500 × 130
│   │   ├── BG Color   : #4CAF50
│   │   ├── Corner     : Rounded 30px
│   │   ├── Shadow     : (0, -6) #2E7D32
│   │   ├── OnClick    : OnNextLevelButtonPressed()
│   │   │
│   │   └── Text (TMP)
│   │       ├── Font Size  : 46
│   │       ├── Color      : #FFFFFF
│   │       └── Text       : "SONRAKİ LEVEL"
│   │
│   └── RetryButton
│       ├── Anchor     : Bottom-Center
│       ├── Pos        : (0, 80)
│       ├── Size       : 300 × 80
│       ├── BG Color   : Transparent
│       ├── OnClick    : OnRetryButtonPressed()
│       │
│       └── Text (TMP)
│           ├── Font Size  : 32
│           ├── Color      : #B0BEC5
│           ├── Decoration : Underline
│           └── Text       : "Tekrar Oyna"
```

---

### 3.6 Fail Panel (Kaybetme Ekranı)

```
┌─────────────────────────────┐
│                             │
│                             │
│                             │
│        BAŞARAMADIN!         │
│                             │
│     ┌───────────────┐       │
│     │               │       │
│     │  Toplanan: 18 │       │
│     │  Gereken:  50 │       │
│     │               │       │
│     └───────────────┘       │
│                             │
│     ╔═══════════════╗       │
│     ║  ↺ TEKRAR     ║       │
│     ╚═══════════════╝       │
│                             │
│     ┌───────────────┐       │
│     │  🏠 Ana Menü  │       │
│     └───────────────┘       │
│                             │
└─────────────────────────────┘
```

**Hiyerarşi:**
```
FailPanel (Panel)
├── Anchor         : Stretch-Stretch
│
├── DimOverlay (Image)
│   ├── Color      : #000000, alpha 0.6
│
├── FailCard (Image)
│   ├── Anchor     : Middle-Center
│   ├── Size       : 700 × 900
│   ├── Color      : #16213E
│   ├── Border     : 3px #E53935, rounded 30px
│   │
│   ├── FailTitle (TMP)
│   │   ├── Pos        : (0, -50)
│   │   ├── Font Size  : 58
│   │   ├── Color      : #E53935
│   │   ├── Style      : Bold
│   │   └── Text       : "BAŞARAMADIN!"
│   │
│   ├── StatsContainer
│   │   ├── Anchor     : Middle-Center
│   │   ├── Pos        : (0, -20)
│   │   ├── Size       : 500 × 160
│   │   ├── BG         : #1A1A2E, rounded 15px
│   │   │
│   │   ├── CollectedRow (Horizontal)
│   │   │   ├── Label (TMP) — "Toplanan:", 34px, #B0BEC5
│   │   │   └── Value (TMP) — "{count}", 34px, #FFD700
│   │   │
│   │   └── RequiredRow (Horizontal)
│   │       ├── Label (TMP) — "Gereken:", 34px, #B0BEC5
│   │       └── Value (TMP) — "{titanCapacity}", 34px, #E53935
│   │
│   ├── RetryButton
│   │   ├── Pos        : (0, 180)
│   │   ├── Size       : 500 × 120
│   │   ├── BG Color   : #FF6B35
│   │   ├── Corner     : Rounded 25px
│   │   ├── OnClick    : OnRetryButtonPressed()
│   │   │
│   │   └── Text (TMP)
│   │       ├── Font Size  : 44
│   │       ├── Color      : #FFFFFF
│   │       └── Text       : "TEKRAR DENE"
│   │
│   └── MenuButton
│       ├── Pos        : (0, 70)
│       ├── Size       : 300 × 80
│       ├── BG Color   : Transparent
│       ├── OnClick    : GoToMenu()
│       │
│       └── Text (TMP)
│           ├── Font Size  : 30
│           ├── Color      : #B0BEC5
│           ├── Decoration : Underline
│           └── Text       : "Ana Menü"
```

---

### 3.7 Settings Panel (Ayarlar — Yeni Panel Önerisi)

```
┌─────────────────────────────┐
│                             │
│  ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓  │
│  ▓    AYARLAR        ✕   ▓  │
│  ▓───────────────────────▓  │
│  ▓                       ▓  │
│  ▓  Ses       ████░░ 70% ▓  │
│  ▓                       ▓  │
│  ▓  Müzik     █████░ 90% ▓  │
│  ▓                       ▓  │
│  ▓  Titreşim  [  ON  ]   ▓  │
│  ▓                       ▓  │
│  ▓───────────────────────▓  │
│  ▓                       ▓  │
│  ▓  Gizlilik Politikası  ▓  │
│  ▓  Kullanım Şartları    ▓  │
│  ▓                       ▓  │
│  ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓  │
│                             │
└─────────────────────────────┘
```

**Hiyerarşi:**
```
SettingsPanel (Panel) — PausePanel veya MenuPanel'den açılır
├── DimOverlay — #000000, alpha 0.6
│
├── SettingsCard (Image)
│   ├── Size       : 750 × 950
│   ├── Color      : #16213E
│   ├── Border     : 3px #FF6B35, rounded 30px
│   │
│   ├── Header (Horizontal)
│   │   ├── TitleText — "AYARLAR", 48px, Bold, #FFFFFF
│   │   └── CloseButton — 70×70, "✕" ikonu, OnClick: CloseSettings()
│   │
│   ├── Divider — stretch × 3, #FF6B35 alpha 0.5
│   │
│   ├── SFXSlider
│   │   ├── Label (TMP) — "Ses Efekti", 34px, #B0BEC5
│   │   ├── Slider (Unity Slider)
│   │   │   ├── BG    : #2C2C2C
│   │   │   ├── Fill  : #FF6B35
│   │   │   ├── Handle: 50×50, #FFFFFF
│   │   │   └── Range : 0 — 1 (PlayerPrefs "SFXVolume")
│   │   └── ValueText — "{%}", 28px, #FFD700
│   │
│   ├── MusicSlider
│   │   ├── Label (TMP) — "Müzik", 34px, #B0BEC5
│   │   ├── Slider (aynı yapı)
│   │   └── PlayerPrefs key: "MusicVolume"
│   │
│   ├── HapticsToggle
│   │   ├── Label (TMP) — "Titreşim", 34px, #B0BEC5
│   │   └── Toggle (Unity Toggle)
│   │       ├── Off BG  : #2C2C2C
│   │       ├── On BG   : #FF6B35
│   │       ├── Knob    : 60×60, #FFFFFF
│   │       └── PlayerPrefs key: "HapticsEnabled"
│   │
│   ├── Divider
│   │
│   └── LinksContainer (Vertical Layout)
│       ├── PrivacyLink — "Gizlilik Politikası", 28px, #B0BEC5, underline
│       └── TermsLink — "Kullanım Şartları", 28px, #B0BEC5, underline
```

---

## 4. Ortak UI Bileşenleri (Reusable)

### 4.1 Standart Buton Stili

```
PrimaryButton (Prefab)
├── Size           : 500 × 120
├── Image (Sprite) : rounded_rect_30.png (9-slice)
├── Color          : #FF6B35
├── Shadow         : DropShadow, (0, -6), #CC4400
│
├── Icon (Image)   : Opsiyonel, 40×40, left-aligned
├── Label (TMP)    : Bold, 42px, #FFFFFF, center
│
├── Hover/Press    : Color Tint → Pressed: #CC4400, Disabled: #555555
└── Click SFX      : ui_click_001.ogg

SecondaryButton (Prefab)
├── Size           : 500 × 120
├── Image          : rounded_rect_25.png (9-slice)
├── Color          : #2C3E6B
├── Border         : 2px #FF6B35
│
├── Label (TMP)    : Bold, 40px, #FFFFFF
└── Click SFX      : ui_click_002.ogg

TextButton (Prefab)
├── Size           : 300 × 80
├── Color          : Transparent
├── Label (TMP)    : Regular, 32px, #B0BEC5, underline
└── Click SFX      : ui_select_001.ogg
```

### 4.2 Geçiş Animasyonları

```
Panel Açılış Animasyonu:
  1. DimOverlay: Alpha 0 → 0.6 (0.2s, ease-in)
  2. Card: Scale 0.7 → 1.0 (0.3s, ease-out-back)
         + Alpha 0 → 1 (0.2s)

Panel Kapanış Animasyonu:
  1. Card: Scale 1.0 → 0.8 (0.2s, ease-in)
         + Alpha 1 → 0 (0.15s)
  2. DimOverlay: Alpha 0.6 → 0 (0.2s, ease-out)

Buton Basma Animasyonu:
  - Scale: 1.0 → 0.92 → 1.0 (0.15s total)
  - Color: Normal → PressedColor → Normal

Scrap Sayaç Güncellemesi:
  - Artış: Text scale punch 1.0→1.3→1.0 (0.2s) + color flash beyaz
  - Azalış: Text scale 1.0→0.8→1.0 (0.15s) + color flash kırmızı + shake x:±5px
```

---

## 5. Sprite / İkon Listesi (Oluşturulması Gerekenler)

| Dosya Adı | Boyut | Açıklama |
|-----------|-------|----------|
| `icon_scrap.png` | 128×128 | Metal vida/parça ikonu (HUD sayaç) |
| `icon_play.png` | 64×64 | ▶ üçgen play butonu |
| `icon_pause.png` | 64×64 | ⏸ pause ikonu |
| `icon_settings.png` | 64×64 | ⚙ dişli çark |
| `icon_sound_on.png` | 64×64 | 🔊 ses açık |
| `icon_sound_off.png` | 64×64 | 🔇 ses kapalı |
| `icon_retry.png` | 64×64 | ↺ yeniden dene |
| `icon_home.png` | 64×64 | 🏠 ana menü |
| `icon_coin.png` | 64×64 | Altın coin (ödül) |
| `star_filled.png` | 128×128 | Dolu yıldız (kazanma) |
| `star_empty.png` | 128×128 | Boş yıldız |
| `rounded_rect_30.png` | 100×100 | 9-slice rounded rect, 30px radius |
| `rounded_rect_25.png` | 100×100 | 9-slice rounded rect, 25px radius |
| `game_logo.png` | 512×512 | Oyun logosu |

> **Not:** 9-slice sprite'lar için Unity'de **Sprite Editor → Border** ayarlarını yapın.

---

## 6. Unity Hiyerarşi Özeti (Canvas.prefab)

```
Canvas (Screen Space - Overlay, Scale With Screen Size 1080×1920)
│
├── SafeAreaPanel (script: SafeAreaFitter.cs)
│   │
│   ├── BootScreen         [GameState.Boot]
│   ├── MenuPanel          [GameState.Menu]
│   ├── HUDPanel           [GameState.Gameplay/Grinding/Finishing]
│   ├── PausePanel         [GameState.Pause]
│   ├── WinPanel           [GameState.Win]
│   ├── FailPanel          [GameState.Fail]
│   └── SettingsPanel      [Pause/Menu'den açılır]
│
├── PassingEffect          [Sahne geçiş animasyonu]
└── TutorialOverlay        [İlk oyun tutorial'ı — opsiyonel]
```

---

## 7. Implementasyon Notları

### UIManagerTT.cs Güncelleme Önerileri:
1. **Settings Panel** desteği ekle (`settingsPanel` referansı)
2. **Titan Progress Bar** için `GameEvents.TitanProgressChanged` dinle
3. **Scrap sayaç animasyonu** için DOTween punch/scale ekle
4. **Panel geçiş animasyonları** için `CanvasGroup` + alpha animasyonu ekle
5. **Safe Area** desteği için `SafeAreaFitter` bileşeni yaz

### Performans:
- Panel'leri `SetActive(false)` ile gizle (mevcut yaklaşım doğru)
- `CanvasGroup.alpha = 0` + `blocksRaycasts = false` ile animasyonlu gizleme
- HUD'daki `Raycast Target` kapalı olsun (gereksiz dokunma yakalama)
- TMP font atlas'ı tek seferde oluştur (Runtime'da font atlas rebuild'den kaçın)

---

## 8. Dosya Organizasyonu

```
Assets/_Game/
├── Art/
│   └── UI/
│       ├── Icons/          ← Tüm ikonlar buraya
│       ├── Sprites/        ← 9-slice ve genel sprite'lar
│       ├── Fonts/          ← TMP font asset'leri
│       └── Materials/      ← UI shader/material (blur vb.)
│
├── Prefabs/
│   ├── Canvas.prefab       ← Ana UI prefab (güncelle)
│   └── UI/
│       ├── PrimaryButton.prefab
│       ├── SecondaryButton.prefab
│       └── TextButton.prefab
│
└── Scripts/
    └── Core/
        ├── UIManagerTT.cs  ← Güncelle (settings, animations)
        └── SafeAreaFitter.cs ← Yeni script
```
