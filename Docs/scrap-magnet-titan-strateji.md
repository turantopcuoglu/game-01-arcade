# SCRAP MAGNET TITAN — HIZLI GELİŞTİRME & YAYIN STRATEJİSİ

> Bu doküman GDD'nin tamamlayıcısıdır. Hızlı geliştirme öncelikleri, eğlence
> faktörleri, level tasarım şablonları ve reklam yerleştirme stratejisini kapsar.

---

## 1. HIZLI GELİŞTİRME YOLHARITASI (MVP → Soft Launch)

### Mevcut Durum Özeti

Kodda zaten implement olan sistemler:

| Sistem | Dosya | Durum |
|--------|-------|-------|
| Game State Machine | `GameManagerTT.cs` | Tamamlandı (Boot/Menu/Gameplay/Grinding/Win/Fail) |
| Swerve Movement | `PlayerController.cs` | Tamamlandı (normalSpeed=10, grindSpeed=1) |
| Vortex Biriktirme | `VortexManager.cs` | Tamamlandı (katmanlı orbit, Sin/Cos) |
| Mıknatıs Çekimi | `MagnetSystem.cs` + `ScrapItem.cs` | Tamamlandı (Idle→Pulling→Orbiting) |
| Obstacle Grinding | `ObstacleBase.cs` | Tamamlandı (HP, grindInterval=0.1s, explosion) |
| Gate Sistemi | `GateController.cs` | Tamamlandı (Add/Subtract/Multiply/Divide) |
| Finish Line / Titan | `FinishLine.cs` | Tamamlandı (scrap launch coroutine) |
| Haptic Feedback | `Haptics.cs` | Tamamlandı (Android native) |
| Audio | `AudioService.cs` | Temel sesler mevcut |
| Input System | `InputRouter.cs` | Tamamlandı (Tap/Drag/TwoFinger) |

### Modelleme Yapmadın — Sorun Değil

Hybrid-casual oyunlarda Low-Poly primitif geometriler zaten standart. Unity'nin
built-in shape'leri (Cube, Cylinder, Sphere) + renkli unlit materyallerle MVP
tamamlanabilir.

**Modelsiz MVP Stratejisi:**

```
Karakter   → Sphere (turuncu) + küçük Cylinder (gövde)
Hurda      → Küçük Cube'lar (rastgele metal renkleri: gri, silver, koyu mavi)
Engel-Kutu → Cube (kırmızımsı)
Engel-Araba→ Stretched Cube + 4 küçük Cylinder (tekerlek)
Engel-Duvar→ Geniş, ince Cube (koyu gri)
Titan      → Capsule + Cube (kol/bacak) hierarchy → "robot iskelet" silueti
Yol        → Plane (teal/koyu yeşil)
Kapılar    → 2x Cube (direk) + 1x Cube (üst bant) → yeşil/kırmızı renk
Testere    → Cylinder (kırmızı, dönüyor) → isIndestructible=true
```

Bu şekilde **tek bir weekend'de 15 level** inşa edilebilir.

### Öncelik Sırası (Ne Yapılmalı, Hangi Sırayla)

```
PHASE 1 — PLAYABLE CORE (Mevcut Hafta)
├── [x] Core mekanikler (KOD HAZIR)
├── [ ] Primitif prefab'lar oluştur (Cube/Sphere bazlı)
├── [ ] 5 tutorial level tasarla (aşağıdaki şablona göre)
├── [ ] Titan end-screen: Progress Bar + Multiplier UI
└── [ ] Temel kamera: CinemachineVirtualCam (follow + FOV punch)

PHASE 2 — RETENTION LOOP (2. Hafta)
├── [ ] 10 level daha (toplam 15)
├── [ ] Level progression: zorlaşan HP tablosu
├── [ ] Fail → Rewarded Video → Continue mekanizması
├── [ ] Win → Titan inşaat animasyonu (scrap'ler yapışsın)
├── [ ] Coin ekonomisi (seviye sonu ödül)
└── [ ] Ana Menü: Play butonu + Level göstergesi

PHASE 3 — MONETIZATION & POLISH (3. Hafta)
├── [ ] AdMob/IronSource entegrasyonu
├── [ ] Interstitial (her 3 levelde 1)
├── [ ] Rewarded Video (continue + x3 bonus)
├── [ ] Particle efektler (toplama spark, öğütme kıvılcım, patlama)
├── [ ] UI polish (level geçiş ekranı, confetti)
└── [ ] Sound pass (toplama ting, öğütme zrrr, yıkım boom)

PHASE 4 — SOFT LAUNCH
├── [ ] Google Play Internal Testing (kapalı test)
├── [ ] 15-20 level hazır
├── [ ] Privacy Policy sayfası
├── [ ] ASO: ikon, ekran görüntüleri, açıklama
├── [ ] Release → Open Testing veya Production
└── [ ] Firebase Analytics / Retention metrikleri
```

---

## 2. EĞLENCE FAKTÖRÜ (FUN / JUICE ANALİZİ)

### A. Neden Eğlenceli Olacak — Temel Psikoloji

| Psikolojik Tetikleyici | Oyundaki Karşılık |
|-------------------------|-------------------|
| **Biriktirme hazzı** (Hoarding) | Vortex büyüdükçe görsel tatmin, "dev top" etkisi |
| **Yıkma hazzı** (Destruction) | Grinding sırasında engelin parçalanması, explosion force |
| **Risk/Ödül dengesi** | Büyük engelle karşılaşmak: "Yetecek mi?" gerilimi |
| **Karar verme** | Gate seçimi (x2 mi, +20 mi?). Yanlış kapı = zorluk |
| **Büyüme hissi** (Power Fantasy) | Titan'ın dolması = "Ben bunu yaptım" duygusu |

### B. Eğlenceyi Katlayacak Juice Detayları

**1. "CRUNCH" Hissi — Grinding Anında:**
- Kamera hafif SHAKE (0.1f amplitude, her tick'te)
- Ekranın kenarlarında kırmızı vignette (scrap azaldıkça yoğunlaşır)
- Engelin üzerinde büyüyen crack texture (HP azaldıkça)
- Ses: düşük frekanslı grinding loop + metal çıtırtı katmanları

**2. "WHOOSH" Hissi — Engel Kırıldığında:**
- Slow-motion snap: 0.05s `Time.timeScale = 0.3f` → hızla 1f'e dön
- Kamera FOV punch: 60 → 70 → 60 (0.3s)
- Particle burst: metal parçacıklar + kıvılcım
- Haptic: uzun, güçlü titreşim (100ms)

**3. "TING TING TING" — Hurda Toplarken:**
- Her hurda alışta küçük scale punch (1.0→1.2→1.0, 0.1s)
- Ses pitch'i toplanan hurda sayısına göre yükselir (C4→C5 arası)
- Mini particle trail (hurda → vortex yolunda)
- Combo sayacı: Arka arkaya toplama "x5!", "x10!" popup'ı

**4. "TITAN MONTAJ" — Level Sonu:**
- Her scrap Titan'a yapışırken "tık" sesi (ascending pitch)
- Progress bar hızla dolar
- Titan gözleri yanar (son scrap yapıştığında)
- Ekran flash + confetti particle
- Multiplier göstergesi yukarı tırmanır: "x2... x3... x5!"

### C. Ek Eğlence Mekanikleri (Post-MVP)

| Mekanik | Açıklama | Zorluk |
|---------|----------|--------|
| **Magnet Boost Pickup** | Geçici olarak mıknatıs yarıçapını 3x büyüt | Kolay |
| **Speed Ramp** | Sarı rampa: 2 saniyeliğine hızı 2x | Kolay |
| **Scrap Rain** | Gökyüzünden 3 saniye hurda yağsın (jackpot hissi) | Orta |
| **Shield Gate** | Özel kapı: 5 saniyelik shield, grinding'de scrap harcamaz | Orta |
| **Golden Scrap** | Nadir altın hurda: 5 scrap değerinde + özel ses | Kolay |
| **Titan Skin Unlock** | Her 5 level'da Titan'ın görünümü değişir | Orta |

---

## 3. LEVEL TASARIMI

### A. Level Tasarım Şablonu (Blueprint)

Her level 3 bölümden (segment) oluşur:

```
[START] ──── SEGMENT 1: TOPLAMA ──── SEGMENT 2: KARAR ──── SEGMENT 3: SAVAŞ ──── [TITAN]

Segment 1 — "Toplama Fazı" (Levelin ilk %40'ı)
├── Bol hurda serpilmiş yol
├── 1-2 basit engel (2-5 HP) → grinding'e alıştırma
├── 1 pozitif kapı (gate) → oyuncuyu ödüllendir
└── Amaç: Vortex'i büyüt, güç topla

Segment 2 — "Karar Fazı" (Levelin %40-70'i)
├── Kapı çiftleri: "x2 vs -15" gibi ikili seçim
├── Orta engeller (5-15 HP) → risk hesabı
├── Dallanma yollar (swerve ile kaçınma opsiyonu)
└── Amaç: Strateji kur, riski yönet

Segment 3 — "Boss Engel" (Levelin son %30'u)
├── 1 büyük engel (20-50 HP) veya engel dizisi
├── Son bir gate opsiyonu (riskli ama karlı)
├── Hard obstacle'lar (testere) → kaçın
└── Amaç: Biriktirdiğin her şeyi harca

[TITAN] — Level Sonu
├── Finish Line trigger
├── Multiplier rampası (opsiyonel)
├── Titan inşaat animasyonu
└── Ödül hesaplama
```

### B. Zorluk Eğrisi (15 Level)

```
Level | Toplam Scrap | Engel HP'leri        | Özel Mekanik       | Testere
------|-------------|----------------------|--------------------|---------
  1   |     30      | 2, 2                 | Sadece +kapılar     | Yok
  2   |     35      | 2, 5                 | İlk gate seçimi     | Yok
  3   |     40      | 5, 5, 2              | İlk x2 kapı         | Yok
  4   |     40      | 5, 10               | İlk - kapı           | Yok
  5   |     50      | 5, 5, 10            | İlk testere          | 1 sabit
  6   |     50      | 10, 10              | Double gate seçimi   | 1 hareketli
  7   |     60      | 5, 10, 15           | İlk x3 kapı         | 2 sabit
  8   |     55      | 10, 15, 10          | /2 kapı riski        | 1 hareketli
  9   |     70      | 10, 15, 20          | Speed ramp tanıtımı  | 2 hareketli
 10   |     65      | 15, 20, 15          | Gate combo           | 2 sabit+1 hareketli
 11   |     80      | 10, 20, 25          | İlk "duvar" engel    | 2 hareketli
 12   |     75      | 20, 20, 20          | Multi-gate karar     | 3 karışık
 13   |     90      | 15, 25, 30          | Golden scrap         | 3 hareketli
 14   |     85      | 20, 30, 25          | Tüm mekanikler       | 4 karışık
 15   |    100      | 25, 30, 35          | Boss wall: 50HP      | 4 hareketli
```

### C. Level Tasarım Kuralları

1. **İlk 3 level = Tutorial.** Fail olması çok zor olsun. Oyuncuyu mekaniğe
   alıştır. Gate'ler hep pozitif.
2. **Her level minimum 1 "wow" anı içersin.** Ya büyük patlama, ya scrap rain,
   ya dev bir kapı (x3).
3. **Fail noktası kontrollü olsun.** Level 1-5'te fail olmak neredeyse imkansız.
   Level 6'dan sonra yanlış gate seçimi = fail riski başlar.
4. **Swerve ile kaçınma her zaman bir opsiyon olsun.** Engelden kaçabilir ama
   scrap toplayamazsın → trade-off.
5. **Level uzunluğu sabit: 30-45 saniye.** Hybrid-casual'da session süresi kısa
   olmalı. Mobilde dikkat süresi limitli.
6. **Hard obstacle (testere) = kaçınma testi.** Yok edilemez, sadece swerve
   ile geçilir. Vortex büyükken riskli (geniş alan = çarpma şansı artar).

### D. Level Tasarım İpuçları (Primitive Shapes ile)

Level sahnesini Unity'de kurarken:
- **Yol:** `Plane` scale(2, 1, 30) → teal material
- **Kenar duvarları:** İnce `Cube`'lar yolun iki yanında (görünmez collider)
- **Hurda placement:** Yol üzerine dağınık küçük `Cube`'lar
- **Gate çifti:** 2 kapıyı yan yana koy, aralarına ince duvar (seçim zorunlu)
- **Testere:** `Cylinder` yatay döndür, kırmızı material, `isIndestructible=true`

---

## 4. REKLAM YERLEŞTİRME STRATEJİSİ

### A. Reklam Türleri ve Tetikleyicileri

```
┌──────────────────────────────────────────────────────────────────┐
│                    REKLAM AKIŞı (AD FLOW)                        │
├──────────────────────────────────────────────────────────────────┤
│                                                                  │
│  GAMEPLAY ──→ WIN ──→ Titan Montaj ──→ Ödül Ekranı              │
│                                          │                       │
│                                   ┌──────┴──────┐               │
│                                   │ REWARDED #1  │               │
│                                   │ "x3 Ödül"   │               │
│                                   └──────┬──────┘               │
│                                          │                       │
│                                   ┌──────┴──────┐               │
│                                   │  SONUÇ      │               │
│                                   │  EKRANI     │               │
│                                   └──────┬──────┘               │
│                                          │                       │
│                              ┌───────────┴──────────┐           │
│                              │ INTERSTITIAL          │           │
│                              │ (her 3 win'de 1)      │           │
│                              └───────────┬──────────┘           │
│                                          │                       │
│                                   NEXT LEVEL                     │
│                                                                  │
│  GAMEPLAY ──→ FAIL ──→ Fail Ekranı                              │
│                          │                                       │
│                   ┌──────┴──────┐                                │
│                   │ REWARDED #2  │                                │
│                   │ "Continue   │                                │
│                   │  +50 Scrap" │                                │
│                   └──────┬──────┘                                │
│                          │                                       │
│              ┌───────────┴──────────┐                            │
│              │ INTERSTITIAL          │                            │
│              │ (her fail'de 1)       │                            │
│              └───────────┬──────────┘                            │
│                          │                                       │
│                    RETRY LEVEL                                   │
│                                                                  │
└──────────────────────────────────────────────────────────────────┘
```

### B. Reklam Kuralları

| Kural | Değer | Açıklama |
|-------|-------|----------|
| **Interstitial Sıklığı (Win)** | Her 3 level'da 1 | Oyuncuyu kaçırma. İlk 5 level'da gösterme. |
| **Interstitial Sıklığı (Fail)** | Her fail'de 1 | Fail zaten kötü his, reklam ekle ama rewarded önce gelsin |
| **İlk Interstitial** | Level 6'dan sonra | İlk 5 level reklamsız → iyi first impression |
| **Rewarded Cooldown** | 30 saniye | Arka arkaya rewarded spam'i önle |
| **Rewarded Win Multiplier** | x3 | İzle = coin ödülü 3 katına çıkar |
| **Rewarded Continue** | +50 scrap | İzle = fail noktasından devam et |
| **Banner** | KULLANMA | Hybrid-casual'da banner eCPM çok düşük, UX bozar |
| **Max Interstitial/Session** | 5 | Bir session'da 5'ten fazla interstitial gösterme |

### C. Reklam Entegrasyon Kodu Yapısı (Öneri)

```
Scripts/
└── Monetization/
    ├── AdManager.cs          → Singleton, SDK init, reklam yükleme
    ├── InterstitialHandler.cs → Gösterim kuralları, cooldown, sayaç
    └── RewardedHandler.cs     → Continue & multiplier reward callbacks
```

**Önerilen SDK:** LevelPlay (IronSource) veya AdMob Mediation.
LevelPlay tercih edilir çünkü mediation + A/B test + waterfall yönetimi tek
panelden yapılır.

### D. In-App Purchase (IAP) — MVP Sonrası

| Ürün | Fiyat | Açıklama |
|------|-------|----------|
| **No Ads** | $2.99 | Tüm interstitial'ları kaldırır. Rewarded kalır (isteğe bağlı). |
| **Starter Pack** | $0.99 | 500 coin + "Golden Magnet" skin (ilk 48 saatte teklif) |
| **VIP Scrap** | $4.99 | Kalıcı +10 başlangıç scrap her levelde |

---

## 5. YAYINLANMADAN ÖNCEKİ KRİTİK CHECKLIST

```
□ Google Play Console hesabı ($25 tek seferlik)
□ Privacy Policy sayfası (basit bir GitHub Pages yeterli)
□ App ikonu (1024x1024, eye-catching, turuncu+teal)
□ Feature graphic (1024x500)
□ En az 4 ekran görüntüsü (gameplay anları)
□ Kısa açıklama (80 karakter) + Uzun açıklama
□ Content rating questionnaire doldur
□ Data safety form (reklam SDK'ları veri toplar → beyan et)
□ Target API level ≥ 34 (Android 14+)
□ AAB format (APK değil)
□ Internal test → en az 14 gün → Open test veya Production
```

---

## 6. ÖZET: "BU HAFTA NE YAPMALIYIM?"

1. **Primitif prefab'ları oluştur** — Player (Sphere), Scrap (Cube), Obstacle
   (Cube varyantları), Gate (Cube+Text), Testere (Cylinder), Titan (Capsule
   hierarchy). Toplam: ~2 saat.

2. **5 tutorial level kur** — Yukarıdaki şablona göre. Segment 1-2-3 yapısı.
   Her level 30-45 saniye. Toplam: ~3-4 saat.

3. **Titan end-screen'i implement et** — Progress bar UI + scrap launch
   animasyonu zaten kodda var (`FinishLine.cs:31-43`). UI tarafını ekle.

4. **Kamera juice'ı ekle** — Cinemachine follow + FOV punch. Grinding'de
   kamera shake. Bu tek başına oyunu %50 daha iyi hissettirir.

5. **Test et, iterate et** — 5 level'ı oyna. "Eğlenceli mi?" sorusunu sor.
   HP değerlerini, scrap miktarlarını ayarla.

6. **Sonra** 10 level daha, reklam SDK, polish, yayınla.
