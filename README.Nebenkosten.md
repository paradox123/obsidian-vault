# Nebenkostenabrechnung – C# Implementation

Deterministische Erzeugung von Nebenkostenabrechnungen nach deutschem Recht (§ 556 ff. BGB, BetrKV).
Implementierung aus Spec (Iteration 3) in C# 12 / .NET 8.

**Location:** `/private/Vermietung/nebenkosten-abrechnung/Nebenkosten.sln` (nicht im DanielsVault Root)
**Status:** Phase A – Artefaktvertrag & Baseline ✅

## Wichtig: Domain-Struktur

Dieses Projekt folgt der **DanielsVault Domain Struktur**:

```
DanielsVault/ (Root, nicht für Nebenkosten!)
├── DanielsVault.sln ← Nur für MarkdownToPdf/n8n (nicht für Nebenkosten)
├── VAULT_AGENT_STRUCTURE.md
└── private/ ← Git-Root für private Domain
    └── Vermietung/
        └── nebenkosten-abrechnung/
            ├── Nebenkosten.sln ← Die tatsächliche C#-Solution!
            ├── README.md
            ├── dotnet/ (Nebenkosten.Core, Nebenkosten.Cli, Nebenkosten.Rendering)
            ├── tests/ (Nebenkosten.Tests)
            └── src/ (Python Legacy)
```

**Kernprinzip:** Jede Domain (`_shared`, `ncg`, `private`, `sparkle`) hat ihre eigenen Grenzen. Nebenkosten gehört zu `private/Vermietung` und hat dort eine lokale Solution (`Nebenkosten.sln`).

## Setup & Ausführung

```bash
cd /Users/dh/Documents/DanielsVault/private/Vermietung/nebenkosten-abrechnung

# Build
dotnet build -c Release

# Tests
dotnet test tests/Nebenkosten.Tests/Nebenkosten.Tests.csproj

# Ausführung (Phase A – Placeholders)
dotnet run --project dotnet/Nebenkosten.Cli -- \
  --input-json data/input.json \
  --output-dir ./output \
  [--skip-pdf] \
  [--billing-date 2024-12-31]
```

## Implementation Roadmap

### Phase A: Artefaktvertrag & Validierung ✅ Abgeschlossen
- ✅ Fehlervergabe (ExitCodes, ErrorCodes, ApplicationError)
- ✅ CLI-Contract (AbrechnungOptions, Validierung)
- ✅ Domänenmodell (BE, NE, Mietpartei, Zaehler, Kostenart, ...)
- ✅ Kostenarten-Matrix (11 Kostenarten, Scope, Umlage)
- ✅ Unit Tests für Contract & Domain (16/16 grün)
- ✅ Build erfolgreich (0 Fehler, 0 Warnungen)

### Phase B: Rechenkern ohne PDF ⏳ Nächster Schritt
- ⏳ ConsumptionCalculator (ista, manual, proration, rest-consumption, Wärmepumpe)
- ⏳ AllocationCalculator (Schlüssel-, Verbrauchs-, Mischumlage)
- ⏳ RoundingAllocator (Kaufmännische Rundung, Restcent-Tie-Breaker)

### Phase C: JSON-Output (`einzelabrechnung.json`)
- ⏳ StatementAssembler (Zwischenergebnis mit Herleitungen)
- ⏳ Artefakt-Output (Verzeichnisstruktur, Dateibenennung)

### Phase D: Rendering (HTML/PDF)
- ⏳ HtmlRenderer (Scriban Templates, Mieter-Sektionen)
- ⏳ PdfRenderer (iTextSharp, A4-Tauglichkeit)
- ⏳ Dokumenten-Tests (Pflichtfelder, Seitenumbruch, Labels)

### Phase E: Regression & Oracle-Hardening
- ⏳ Regression2024Tests (Excel-Oracle, NE1-NE4; NE5 separate Prüfung)
- ⏳ Dokumentation: Spec-Delta-Gründe

## Fehlervertrag (Spec Iteration 3)

| Code | Kategorie | Exitcode | Beispiel |
|------|-----------|----------|----------|
| `NK-SCHEMA-*` | JSON-Struktur | 2 | Fehlendes Feld, falscher Typ |
| `NK-REF-*` | Referenzen | 2 | NE-ID nicht definiert |
| `NK-CONSISTENCY-*` | Logische Konsistenz | 2 | Scope nicht in Matrix |
| `NK-CALC-*` | Berechnung | 3 | Negativer Restverbrauch |
| `NK-RENDER-*` | Rendering | 4 | PDF-Generierung fehlgeschlagen |
| `NK-IO-*` | Datei-Fehler | 4 | Eingabedatei nicht lesbar |

## Testabdeckung

| Test-Gruppe | Fokus | Anzahl Tests | Status |
|-------------|-------|----------------|--------|
| CliContractTests | ExitCodes, ErrorCodes, Exceptions | 7 | ✅ Grün |
| CostMatrixTests | Kostenarten-Konfiguration | 5 | ✅ Grün |
| DomainModelTests | Basis-Domänenmodell | 4 | ✅ Grün |
| ValidationTests | Schema, Referenzen, Konsistenz | [TBD] | ⏳ |
| ConsumptionCalculatorTests | 5 Verbrauchsfallgruppen | [TBD] | ⏳ |
| AllocationCalculatorTests | Umlage-Logik | [TBD] | ⏳ |
| RoundingAllocatorTests | Rundung & Restcent | [TBD] | ⏳ |
| RenderingContractTests | Artefakt-Struktur | [TBD] | ⏳ |
| Regression2024Tests | Oracle-Vergleich | [TBD] | ⏳ |

**Gesamt:** 16/100+ Tests grün.

## Referenzen

- **Verbindliche Spec:** [2026-03-24 Nebenkostenabrechnung Applikation.md](_shared/shared-ai-docs/_specs/2026-03-24%20Nebenkostenabrechnung%20Applikation.md)
- **Implementierungsplan:** [2026-03-25 Implementierungsplan](_shared/shared-ai-docs/_plans/2026-03-25%20Nebenkostenabrechnung%20Applikation%20Implementierungsplan.md#iteration-3)
- **Legacy Python-Oracle:** [private/Vermietung/nebenkosten_pipeline/](private/Vermietung/nebenkosten_pipeline/)

## Status

**Phase:** A – Artefaktvertrag & Baseline ✅ Abgeschlossen  
**Location:** `/private/Vermietung/nebenkosten-abrechnung/Nebenkosten.sln` (nicht Root)
**Compilation:** ✅ Erfolgreich (.NET 8.0, Debug & Release)  
**Tests:** ✅ 16/16 grün  
**Next:** Phase B – Rechenkern-Implementierung  

---

Maintained in DanielsVault / private / Vermietung / nebenkosten-abrechnung
