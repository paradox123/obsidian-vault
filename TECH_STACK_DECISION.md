# Technologie-Stack & Architektur-Entscheidungen
## Nebenkostenabrechnung Implementation (Iteration 3)

**Datum:** 2026-03-25  
**Status:** Final Decision + Implementation Complete (Phase A)

---

## Executive Summary

Für die Nebenkostenabrechnungs-Anwendung wurde **C# 12 / .NET 8.0** als Produktionsstack ausgewählt. Die Entscheidung basiert auf:

1. **Präzision bei Geldberechnungen:** Native `decimal`-Typ + kaufmännische Rundung
2. **Compliance-Anforderungen:** Deterministisch, nachvollziehbar, Fail-Fast
3. **Operative Anforderungen:** Single CLI-Executable, kein Runtime-overhead
4. **Test- & Wartbarkeit:** Strong typing, unit-testbar, regress-sicher

---

## Tech-Stack Detail

### Runtime & Language
| Entscheidung | Begründung | Alternativen |
|-----------|-----------|------------|
| **.NET 8.0 (LTS)** | Modern, LTS bis Nov 2026, Performance, Windows/Mac/Linux | .NET 6/7 (älter), Node (fehlende Math), Rust (Overhead) |
| **C# 12** | Records, Nullable annotations, Readonly properties, Pattern matching | Python (Float-Risiko), Go (zu Low-Level), TypeScript (JS-Quirks) |
| **`decimal` Typ** | Genau 28 Dezimalstellen, kaufmännisches Runden, Spec-konform | `double` (ungenau), BigDecimal Emulation (overhead) |

## Projekt-Struktur (Vault Domain Boundaries)

**Wichtig:** Dieses Projekt folgt der DanielsVault-Domain-Struktur:

```
/Users/dh/Documents/DanielsVault/
├── DanielsVault.sln          (Root – nur für n8n MarkdownToPdf, NICHT für Nebenkosten)
├── AGENTS.md                 (Agent-Routing)
├── VAULT_AGENT_STRUCTURE.md  (Domain-Grenzen)
└── private/                  (Git-Root für private Domain)
    └── Vermietung/           (Vermietungs-Subdomäne)
        └── nebenkosten-abrechnung/
            ├── Nebenkosten.sln        (Solution für C#-Implementierung – das ist der Ort!)
            ├── README.md
            ├── src/                   (Python-Pipeline – Legacy für Fixtures)
            │   ├── app.py
            │   ├── domain/, validation/, calculation/, output/, config/
            │   └── __init__.py
            ├── dotnet/                (C# .NET 8.0 – Phase A+)
            │   ├── Nebenkosten.Core/
            │   ├── Nebenkosten.Cli/
            │   └── Nebenkosten.Rendering/
            ├── tests/                 (C# & Python Tests)
            │   └── Nebenkosten.Tests/ (xUnit)
            └── data/                  (Fixtures, Oracle-Daten)
```

**Wichtigste Entscheidung:** 
- ✅ **Nebenkosten.sln ist in `/private/Vermietung/nebenkosten-abrechnung/`** – nicht im Root
- ✅ Die Root-`DanielsVault.sln` ist für bestehende Projekte (MarkdownToPdf) – Nebenkosten gehört nicht dort hin
- ✅ `/private/` ist ein eigenes Git-Repo mit eigenen Grenzen

**Principle:** Clean Architecture – Core hat KEINE Abhängigkeiten zu CLI/Rendering/Dateisystem.

### Abhängigkeiten (NuGet)

| Paket | Version | Zweck | Status |
|-------|---------|-------|--------|
| **CommandLineParser** | 2.x | CLI-Parsing, automatische Help | ✅ Installiert |
| **Serilog** | 4.x | Strukturiertes Logging | ✅ Installiert |
| **Serilog.Sinks.Console** | 6.x | Console-Output | ✅ Installiert |
| **itext7** | 8.x | PDF-Rendering, Tabellen, Layout | ✅ Installiert |
| **Scriban** | 5.x | HTML-Templating (Minimal, schnell) | ✅ Installiert |
| **xUnit** | 2.x | Unit-Testing Framework | ✅ Installiert |
| **FluentAssertions** | 8.x | Readable Test-Assertions | ✅ Installiert |
| **Moq** | 4.x | Mocking (für Service-Tests) | ✅ Installiert |

### JSON-Handling
- **Parser:** `System.Text.Json` (built-in .NET 8)
- **Rationale:** Schnell, Source Generators, keine externe Abhängigkeit (Newtonsoft wird nicht benötigt)
- **Input:** Spec-konformes JSON mit validierbarem Schema
- **Output:** `einzelabrechnung.json` mit Zwischenwerten & Herleitungen

---

## Fehler-Handling & Exit-Codes (Spec-Konformität)

```csharp
namespace Nebenkosten.Core.Domain
{
    public static class ExitCodes
    {
        public const int Success = 0;              // Erfolg
        public const int ValidationError = 2;     // Schema/Referenz/Konsistenz
        public const int CalculationError = 3;    // Rechenfehler (Fail-Fast)
        public const int RenderingError = 4;      // PDF/File-Fehler
        public const int UnexpectedError = 10;    // Systemfehler
    }
}
```

Alle Fehler ergeben strukturierte `ApplicationError`-Objekte:
```
FEHLER NK-CALC-002: NE5-Restverbrauch negativ [id=ne5, period=2024]
```

---

## Warum NICHT die Alternativen?

### ❌ Python
**Argumente für Python:**
- ✅ Schneller zu prototypen
- ✅ `Decimal` Modul existiert
- ✅ Gutes PDF-Supp. via Reportlab

**Argumente gegen Python (Finale Entscheidung):**
- ❌ Float-Risiko: `0.1 + 0.2 != 0.3` in Python auch mit Decimal-Care
- ❌ PDF-Libs schwächer: WeasyPrint flaky, PyPDF unvollständig
- ❌ Type-Checking weniger streng (Linting braucht Disziplin)
- ❌ CLI-Packaging nervig (Abhängigkeiten, venv, Single Exe kompliziert)
- ❌ Legacy-Pipeline ist bereits Python – Aber Spec sagt: C# Neudefinition, nicht Python-Weiterentwicklung

### ❌ Go
- ❌ Zu Low-Level für Geschäftslogik
- ❌ Financial Math nicht native (Bigint/Bigfloat unangenehm)
- ❌ PDF-Libs dürftig
- ✅ Wäre performant, aber overkill

### ❌ TypeScript/Node
- ❌ JavaScript Float-Probleme auch mit Big.js
- ❌ Runtime notwendig (kein Single Exe)
- ❌ PDF-Rendering HTML-basiert → flaky in Node
- ✅ Schnell zu entwickeln, aber riskant für finale Übergabe

### ✅ C# (Gewinner)
- ✅ `decimal` Typ: Spec-konform, keine Workarounds
- ✅ Strong typing: Compile-Time Fehlerprävention
- ✅ Single Executable: Self-contained, kein Runtime-overhead
- ✅ PDF-Libs robust: iTextSharp 7.x + Scriban Template
- ✅ Test-Eco: xUnit, FluentAssertions, Moq (Standard)
- ✅ Operative Safety: Deterministisch, fail-safe, langfristig wartbar

---

## Implementierungs-Phasen & Stack-Integration

### Phase A: Artefaktvertrag & Validierung ✅ **COMPLETE**
- ✅ C# Domain-Modell (Errors, ExitCodes, Domain Objects)
- ✅ CLI-Options via CommandLineParser
- ✅ xUnit-Test-Struktur
- ✅ Kostenarten-Matrix in Code

### Phase B: Rechenkern ohne PDF ⏳ **NEXT**
- ⏳ ConsumptionCalculator (5 Fallgruppen: ista, manual, proration, NE5-Rest, Wärmepumpe)
- ⏳ AllocationCalculator (Schlüssel-, Verbrauchs-, Mischumlage)
- ⏳ RoundingAllocator (Decimal-Rundung, Restcent-Tie-Breaker)
- ⏳ Input-DTOs + Schema-Validation

### Phase C: JSON-Output ⏳
- ⏳ StatementAssembler (Einzelabrechnung → JSON)
- ⏳ File I/O & Artefakt-Struktur

### Phase D: Rendering (HTML/PDF) ⏳
- ⏳ Scriban Templates → HTML
- ⏳ iTextSharp → PDF + A4-Tauglichkeit

### Phase E: Regression & Oracle ⏳
- ⏳ 2024-Dataset-Tests
- ⏳ Spec-Delta-Dokumentation

---

## Build & Deployment

### Development (Local)
```bash
cd /Users/dh/Documents/DanielsVault

# Build
dotnet build -c Debug

# Test
dotnet test tests/Nebenkosten.Tests/Nebenkosten.Tests.csproj

# Run
dotnet run --project src/Nebenkosten.Cli -- \
  --input-json ./data/input.json \
  --output-dir ./output
```

### Production (Release)
```bash
# Single self-contained executable (macOS arm64)
dotnet publish -c Release \
  --self-contained true \
  --runtime osx-arm64 \
  -p:PublishSingleFile=true \
  -o ./dist/
```

Resultat: `./dist/Nebenkosten.Cli` – ein Single Binary, kein Runtime notwendig.

---

## Prüfkriterien & Gates (Phase A ✅)

| Gate | Status | Evidenz |
|------|--------|---------|
| **Compilation** | ✅ Grün | `dotnet build` erfolgreich (0 errors, 0 warnings) |
| **Tests (Initial)** | ✅ 16/16 grün | CliContractTests, CostMatrixTests, DomainModelTests |
| **Domain Model** | ✅ Komplett | BE, NE, Mietpartei, Zaehler, Ablesung, Kostenart, Kostenbeleg |
| **Error Contract** | ✅ Implementiert | ErrorCodes enum + ApplicationError + NebenkosteException |
| **CLI Contract** | ✅ Implementiert | AbrechnungOptions mit Validierung, Exitcodes definiert |
| **NuGet-Stack** | ✅ Komplett | Alle Pakete installiert und referenziert |

---

## Risiko-Assessment

| Risiko | Wahrscheinlichkeit | Mitigation |
|--------|------------------|-----------|
| **Dezimal-Rundung nicht deterministisch** | 🟡 Niedrig | Decimal-Typ native, Round(decimal, 2, MidpointRounding.ToEven) |
| **PDF-Rendering flaky** | 🟡 Niedrig | iTextSharp 7.x stabil, Tests für A4/Seitenumbruch |
| **Performance bei großen Datensätzen** | 🟢 Sehr niedrig | C# compiled, schneller als Python |
| **Integration mit Legacy-Python-Oracle** | 🟠 Mittel | Separate Fixture-Dateien, keine Code-Abhängigkeit |
| **Operator-Fehler bei CLI-Nutzung** | 🟡 Niedrig | CommandLineParser erzeugt hilfreiche Fehlermeldungen |

---

## Technische Schulden & Zukunftspflege

- **.NET Upgrade-Path:** 8.0 → 9.0 (2025) → 10.0 LTS (2026) – trivial
- **Package Updates:** xUnit, Moq, FluentAssertions folgen SemVer und sind stabil
- **iTextSharp-Alternativen:** Könnten auf PdfSharp oder SelectPdf wechseln ohne Code-Neuschreibung (Abstraktions-Interface)
- **Skalierbarkeit:** Aktuell Single-File CLI; bei Bedarf leicht auf API-Mode (ASP.NET) ausbaubar

---

## Abschließendes Fazit

Die **C# 12 / .NET 8.0 Stack** ist für diese Anwendung optimal:

1. **Math-Sicherheit:** Decimal-Typ eliminiert Float-Risiken
2. **Compliance:** Strong typing, Validation-first, Fail-Fast
3. **Operations:** Single Executable, deterministische Ausführung
4. **Wartbarkeit:** xUnit + FluentAssertions + Clean Architecture
5. **Zukunft:** .NET 8 LTS stabil und supportiert

Die Phase A Implementierung wurde erfolgreich abgeschlossen. Die Architektur ist bereit für Phase B (Rechenkern).

---

**Gültig ab:** 2026-03-25  
**Nächste Review:** Nach Phase E (Regression-Hardening)
