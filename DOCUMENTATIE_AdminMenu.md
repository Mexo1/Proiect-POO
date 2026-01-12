# Documentație AdminMenu.cs

## Descriere Generală
**AdminMenu.cs** este partea din UI (PARTEA 3) care gestionează interfața pentru administratorii ONG-ului.

---

## Structura Clasei

### Dependențe (Constructor)
```csharp
public AdminMenu(
    UserService userService,           // Serviciu pentru operații cu utilizatori
    CampaignService campaignService,   // Serviciu pentru operații cu campanii
    DonationService donationService,   // Serviciu pentru operații cu donații
    ReportService reportService,       // Serviciu pentru generare rapoarte
    ILogger logger,                    // Logger pentru înregistrare mesaje
    List<Campaign> campaigns,          // Lista globală cu toate campaniile
    List<User> users,                  // Lista globală cu toți utilizatorii
    List<Donation> donations)          // Lista globală cu toate donațiile
```

**Dependency Injection**: Toate serviciile și listele sunt primite prin constructor (injectate automat de Program.cs).

---

## Metodele Principale

### 1. `Display()` - Bucla Meniului
**Ce face:**
- Afișează meniul în buclă până când admin-ul alege să iasă
- Procesează alegerea utilizatorului prin `switch`

**Flux:**
```
┌─────────────────┐
│ Afișează Meniu  │
└────────┬────────┘
         │
    ┌────▼─────┐
    │ Citește  │
    │  Input   │
    └────┬─────┘
         │
    ┌────▼──────────────┐
    │  Switch (choice)  │
    │  1 → CreateCampaign
    │  2 → ViewAllDonors
    │  3 → GenerateReport
    │  4 → CloseCampaign
    │  5 → Ieșire
    └───────────────────┘
```

---

### 2. `DisplayAdminMenu()` - Afișare Vizuală
**Ce face:**
- Afișează chenarul vizual cu opțiunile meniului
- Folosește caractere Unicode pentru chenare frumoase (`╔`, `║`, `╚`)

**Output:**
```
╔════════════════════════════════╗
║     MENIU ADMIN                 ║
╠════════════════════════════════╣
║ 1. Crează Campanie              ║
║ 2. Vezi toți Donatorii          ║
║ 3. Generează Raport             ║
║ 4. Închide Campanie             ║
║ 5. Ieșire                       ║
╚════════════════════════════════╝
```

---

### 3. `CreateCampaign()` - Creare Campanie Nouă

**Pași:**
1. **Citește titlul** campaniei de la utilizator
2. **Citește suma țintă** și o validează (trebuie să fie număr valid)
3. **Alege categoria** (Education, Health, Environment, Social)
4. **Creează obiectul Campaign** cu:
   - `CampaignId` unic generat cu `Guid.NewGuid()`
   - Titlul introdus
   - Suma țintă
   - Categoria aleasă
5. **Adaugă în lista globală** `_campaigns`
6. **Înregistrează în log** evenimentul

**Validări:**
- `decimal.TryParse()` - verifică dacă suma e număr valid
- `title!` - operatorul `!` spune compilatorului că title nu e null

**Switch Expression:**
```csharp
var category = catChoice switch
{
    "1" => Category.Education,
    "2" => Category.Health,
    "3" => Category.Environment,
    "4" => Category.Social,
    _ => Category.Social  // Default dacă nimic nu se potrivește
};
```

---

### 4. `ViewAllDonors()` - Vizualizare Donatori

**Pași:**
1. **Găsește primul admin** din lista utilizatorilor
   - `OfType<AdminONG>()` - filtrează doar AdminONG din lista User
   - `FirstOrDefault()` - ia primul sau null
2. **Apelează serviciul** `TrackDonation()` care returnează doar Donatorii
3. **Afișează lista** cu numele și email-ul fiecărui donator

**LINQ folosit:**
- `.OfType<T>()` - filtrare după tip
- `.FirstOrDefault()` - primul element sau null

---

### 5. `GenerateReport()` - Raport General

**Statistici afișate:**
- 📊 **Total Campanii** - `_campaigns.Count`
- 💰 **Total Donații** - `_donations.Count`
- 👥 **Total Utilizatori** - `_users.Count`
- 💵 **Sumă totală** - `_campaigns.Sum(c => c.GetCurrentAmount())`

**LINQ folosit:**
```csharp
decimal totalAmount = _campaigns.Sum(c => c.GetCurrentAmount());
```
- `.Sum()` - adună toate valorile
- `c => c.GetCurrentAmount()` - lambda expression (pentru fiecare campanie, ia suma curentă)

**Formatare:**
- `:C` - formatare ca Currency (monedă)

---

### 6. `CloseCampaign()` - Închidere Campanie

**Pași:**
1. **Filtrează campaniile active**
   - `Where(c => c.IsActive)` - ia doar campaniile cu IsActive = true
   - `.ToList()` - convertește în listă
2. **Afișează lista numerotată** cu `for` loop
3. **Citește alegerea** utilizatorului
4. **Validează input-ul** cu `int.TryParse()` ȘI verifică intervalul
5. **Închide campania** apelând `campaign.Close()`

**Indexare:**
```csharp
for (int i = 0; i < activeCampaigns.Count; i++)
{
    Console.WriteLine($"{i + 1}. {activeCampaigns[i].Title}");
    // i + 1 pentru că afișăm de la 1, dar indexul e de la 0
}

// La alegere:
activeCampaigns[choice - 1].Close();
// choice - 1 pentru că utilizatorul alege de la 1, dar indexul e de la 0
```

**Validare complexă:**
```csharp
if (int.TryParse(Console.ReadLine(), out int choice) && 
    choice > 0 && 
    choice <= activeCampaigns.Count)
```
- `TryParse()` - încearcă să convertească în int, returnează true/false
- `out int choice` - salvează rezultatul în variabila choice
- `&&` - operator logic AND (toate condițiile trebuie îndeplinite)

---

## Gestionarea Erorilor

**Toate metodele folosesc try-catch:**
```csharp
try
{
    // Cod principal
}
catch (Exception ex)
{
    Console.WriteLine($"⚠ Eroare: {ex.Message}");
}
```

**Beneficii:**
- Aplicația nu se oprește la erori
- Utilizatorul vede mesaj clar despre eroare
- Aplicația continuă să ruleze

---

## Concepte C# Folosite

### 1. **Readonly Fields**
```csharp
private readonly UserService _userService;
```
- Poate fi setat doar în constructor
- Nu poate fi modificat după

### 2. **Nullable Reference Types**
```csharp
string? title = Console.ReadLine();
```
- `?` = poate fi null
- `!` = "sunt sigur că nu e null"

### 3. **String Interpolation**
```csharp
Console.WriteLine($"Total: {count}");
```
- `$` înainte de string permite inserarea variabilelor cu `{}`

### 4. **Switch Expression (C# 8.0+)**
```csharp
var result = input switch
{
    "1" => Value1,
    "2" => Value2,
    _ => Default
};
```

### 5. **LINQ (Language Integrated Query)**
```csharp
_campaigns.Where(c => c.IsActive).ToList()
_campaigns.Sum(c => c.GetCurrentAmount())
_users.OfType<AdminONG>().FirstOrDefault()
```

---

## Flux de Date

```
Program.cs
    │
    ├─ Creează listele globale (campaigns, users, donations)
    │
    ├─ Injectează în ConsoleMenu
    │
    └─ ConsoleMenu transmite la AdminMenu
            │
            ├─ AdminMenu modifică listele direct
            │   (Add, Remove, etc.)
            │
            └─ Modificările sunt vizibile peste tot
                (aceleași liste sunt partajate)
```

---

## Diferența dintre PARTEA 2 și PARTEA 3

### PARTEA 2 (Servicii)
- **CampaignService, UserService, etc.**
- Conțin **logica de business**
- Validări, calcule, reguli

### PARTEA 3 (UI - AdminMenu)
- **ConsoleMenu, AdminMenu, DonatorMenu**
- Conțin **interfața utilizator**
- Citire input, afișare meniuri
- **Apelează** serviciile din Partea 2

**Exemplu:**
```csharp
// PARTEA 3 (AdminMenu) - UI
var donors = _userService.TrackDonation(adminOng, _users);
                  ↑
                  └─── Apelează PARTEA 2 (UserService)
```

---

## Vocabular Tehnic

| Termen | Explicație |
|--------|------------|
| **Constructor** | Metodă specială care inițializează obiectul |
| **Dependency Injection** | Primirea dependențelor prin constructor |
| **LINQ** | Language Integrated Query - interogări pe colecții |
| **Lambda** | Funcție anonimă: `c => c.IsActive` |
| **Nullable** | Tip care poate fi null (`string?`) |
| **Switch Expression** | Versiune modernă a switch pentru returnare valori |
| **Try-Catch** | Bloc pentru prinderea erorilor |
| **ReadOnly** | Câmp care nu poate fi modificat după constructor |

---

## Întrebări Frecvente

**Q: De ce folosim `_` înaintea variabilelor?**
A: Convenție C# pentru câmpuri private (`_logger`, `_campaigns`)

**Q: Ce înseamnă `out` în `TryParse(input, out int result)`?**
A: Parametrul `out` permite metodei să returneze o valoare prin parametru

**Q: De ce `i + 1` când afișăm lista?**
A: Pentru că indexarea începe de la 0, dar vrem să afișăm de la 1 pentru utilizator

**Q: Ce face `?.` ?**
A: Null-conditional operator - apelează metoda doar dacă obiectul nu e null

**Q: De ce toate metodele sunt `private`?**
A: Sunt folosite doar în interiorul clasei, nu trebuie accesate din exterior

---

**Autor:** GitHub Copilot  
**Dată:** 11 Ianuarie 2026  
**Versiune:** 1.0
