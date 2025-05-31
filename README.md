# Blogic CRM – Backend (ASP.NET Core)

Tento repozitář slouží jako serverovou část aplikace **Blogic CRM** – webového systému pro správu smluv, klientů a poradců.

Frontendová část je dostupná v samostatném repozitáři: [blogic-crm-front](https://github.com/DaLukCZ/blogic-crm-front)

---

## 🛠️ Použité technologie

- ASP.NET Core Web API (.NET 9.0)
- Entity Framework Core (Code First)
- MS SQL Server Express
- Swagger (Swashbuckle) pro dokumentaci API

---

## 💻 Požadavky na prostředí

| Nástroj             | Verze               |
|---------------------|---------------------|
| .NET SDK            | 9.0                 |
| Visual Studio       | 2022 nebo novější   |
| MS SQL Server       | Express (nebo vyšší)|

---

## 🚀 Spuštění backendu

1. Repozitář je nejprve nutné naklonovat:

   ```bash
   git clone https://github.com/DaLukCZ/blogic-crm-back.git
   ```

2. Projekt by měl být otevřen v **Visual Studio 2022**.

3. Obnovit NuGet balíčky:

   ```bash
   dotnet restore
   ```

4. Databáze se vytvoří pomocí migrací spuštěním příkazu, který musí být spuštěn ve složce, kde se nachází soubor `.csproj`:

   ```bash
   dotnet ef database update
   ```

Pokud je příkaz spuštěn mimo projektovou složku, dojde k chybě.

### 5. Nastavení tajného klíče pro JWT autentizaci

Pro správné fungování autentizace je nutné nastavit tajný klíč (JWT Secret), který slouží k podepisování tokenů.

Při vývoji se doporučuje využít nástroj **User Secrets**, aby klíč nebyl uložen přímo v souboru `appsettings.json`.

---

#### 🔹 5a. Inicializace User Secrets spuštěním příkazu, který musí být spuštěn ve složce, kde se nachází soubor `.csproj`:

```bash
dotnet user-secrets init
```

Tím se vytvoří vazba mezi projektem a úložištěm tajných hodnot.

---

#### 🔹 5b. Vygenerování a nastavení tajného klíče

Pomocí OpenSSL lze vygenerovat bezpečný náhodný klíč (minimálně 32 znaků):

```bash
dotnet user-secrets set "Jwt:Key" "$(openssl rand -base64 32)"
```

✅ Tento příkaz uloží klíč jako tajný údaj do vývojového úložiště a umožní bezpečnou práci s JWT.

---

#### ❗ Co dělat, pokud `openssl` není dostupné:

- **Windows**: Nainstalujte [Win32 OpenSSL](https://slproweb.com/products/Win32OpenSSL.html)
- **Alternativa**: Nastavte klíč ručně:

```bash
dotnet user-secrets set "Jwt:Key" "Xb1Z9w2K8fMqR7vLpJt6Uc3dEa5NbVmQ"
```
6. Spuštění aplikace
Aplikace se spustí stisknutím zelené šipky „Start“ ve Visual Studiu s vybraným profilem https.

Po spuštění aplikace je databáze automaticky naplněna základními testovacími daty.

API poběží na adrese `https://localhost:7046`.
## 📚 Dokumentace API

Po spuštění API je automaticky k dispozici Swagger UI na adrese:

```
https://localhost:7046/swagger
```

Zde jsou k dispozici všechny dostupné endpointy pro práci se smlouvami, klienty a poradci.

---

## 📦 Struktura projektu

- `/Controllers` – REST API controllery
- `/Models` – datové modely entit
- `/Data` – kontext databáze a konfigurace
- `Program.cs` – hlavní vstupní bod aplikace

---

## 🔗 Související repozitář

Frontend aplikace je dostupný v samostatném repozitáři:  
➡️ [https://github.com/DaLukCZ/blogic-crm-front](https://github.com/DaLukCZ/blogic-crm-front)

---

## 🔐 JWT autentizace – vývojové nastavení

Pro správné fungování autentizace je nutné nastavit tajný klíč (JWT Secret), který se používá pro podepisování tokenů.

Při vývoji se doporučuje použít **User Secrets**, aby nebyl klíč uložen přímo v souboru `appsettings.json`.

### ✅ Postup:

1. Inicializace úložiště User Secrets (pokud ještě nebylo provedeno):

   ```bash
   dotnet user-secrets init
   ```

2. Vygenerování a nastavení klíče (např. 32+ znaků):

   ```bash
   dotnet user-secrets set "Jwt:Key" "vase-nahodna-tajna-hodnota-zde"
   ```

Pro vygenerování bezpečného klíče lze použít například:

```bash
dotnet user-secrets set "Jwt:Key" "$(openssl rand -base64 32)"
```

Pokud není OpenSSL dostupné, lze zadat ručně např. 32 náhodných znaků.

---
