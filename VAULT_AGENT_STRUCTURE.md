# DanielsVault - Agent Structure Index

Last updated: 2026-03-25
Scope: /Users/dh/Documents/DanielsVault

## Purpose
This file documents the vault structure in a stable, machine-friendly way so AI agents can:
- find the right repository quickly,
- avoid mixing unrelated domains,
- choose the right source folders for context,
- and keep future updates consistent.

## Top-Level Layout
- DanielsVault.sln
- _shared/
- ncg/
- private/
- sparkle/

## Repository Boundaries (Git Roots)
1. . (vault root)
2. _shared/shared-ai-docs
3. ncg/ncg-docs
4. private
5. sparkle

Guideline:
- Treat each git root as its own change boundary.
- Do not assume commit history or branch state is shared across nested repositories.

## Content Zones

### 1) _shared/
Primary role: shared automations, AI docs, and n8n assets.

Important paths:
- _shared/n8n/docker-compose.yml
- _shared/n8n/workflows/
- _shared/n8n/scripts/
- _shared/shared-ai-docs/_specs/
- _shared/shared-ai-docs/_plans/
- _shared/shared-ai-docs/docs/

Typical content:
- automation workflows (.json)
- project specs and implementation plans (.md)
- helper scripts and infra notes

### 2) ncg/
Primary role: NCG documentation subtree.

Important paths:
- ncg/ncg-docs/README.md
- ncg/ncg-docs/docs/Dev/
- ncg/ncg-docs/docs/Ops/
- ncg/ncg-docs/docs/Security/
- ncg/ncg-docs/docs/Specs/

Typical content:
- infra and engineering docs
- operational runbooks
- security and spec notes

### 3) private/
Primary role: personal and project-specific private knowledge base.

Important paths:
- private/README.md
- private/Energetische Sanierung/
- private/me/
- private/Portfolio/
- private/Vermietung/

Typical content:
- personal planning and admin docs
- property, renovation, and finance notes
- application and portfolio materials

### 4) sparkle/
Primary role: small standalone notes repository.

Important paths:
- sparkle/README.md
- sparkle/Jahresabschluss 2024 - Uebergabe 14.11.2025.md

## Agent Navigation Rules
1. Start from this file, then drill into exactly one domain folder first (_shared, ncg, private, sparkle).
2. Prefer local README.md files for context before reading deep files.
3. If task is about automations or AI workflows, start in _shared/.
4. If task is about infra/devops docs, start in ncg/ncg-docs/docs/.
5. If task is personal/private, stay in private/ unless explicitly asked to cross-reference.
6. Confirm git root before edits or git commands.

## Fast Routing Hints
- "n8n", "workflow", "automation" -> _shared/n8n/ or _shared/shared-ai-docs/n8n/
- "spec", "requirements", "plan" -> _shared/shared-ai-docs/_specs/ or _shared/shared-ai-docs/_plans/
- "NCG infra", "Ops", "Security docs" -> ncg/ncg-docs/docs/
- "Bewerbung", "CV", "Profil", "Vermietung", "Sanierung" -> private/

## Task-to-Path Matrix
| Task intent | Start path | Fallback path | Notes |
|---|---|---|---|
| Build or inspect n8n automation | _shared/n8n/ | _shared/shared-ai-docs/n8n/ | Check docker-compose and workflows first |
| Write/refine a spec | _shared/shared-ai-docs/_specs/ | _shared/shared-ai-docs/_plans/ | Keep spec and plan split |
| Build/refine implementation plan | _shared/shared-ai-docs/_plans/ | _shared/shared-ai-docs/_specs/ | Link plan entries to source spec |
| Search NCG infrastructure docs | ncg/ncg-docs/docs/Ops/ | ncg/ncg-docs/docs/Dev/ | Use Security and Specs as needed |
| Document security topics | ncg/ncg-docs/docs/Security/ | ncg/ncg-docs/docs/Ops/ | Keep operational and policy content separated |
| Personal profile/application updates | private/me/ | private/Portfolio/ | Keep CV/application artifacts grouped |
| Property and renovation notes | private/Energetische Sanierung/ | private/Vermietung/ | Avoid mixing with NCG docs |
| Finance or tenancy documents | private/Vermietung/ | private/Energetische Sanierung/Finanzierung/ | Keep yearly artifacts grouped by year |
| Small standalone notes | sparkle/ | private/ | Use only for compact, self-contained notes |

## Auto-Discovery Notes
- This file can have any name and still be used by an agent when explicitly referenced.
- For reliable auto-discovery by coding agents, keep an anchor file named AGENTS.md or .github/copilot-instructions.md at the repo root.
- The anchor file should point to this index and require reading it before broad search/edit steps.

## Maintenance Protocol
When structure changes, update this file in the same change set:
1. Top-level layout
2. Repository boundaries
3. Content zones (only key paths)
4. Routing hints

Keep this index concise. It should remain a high-signal entry point, not a full file catalog.
