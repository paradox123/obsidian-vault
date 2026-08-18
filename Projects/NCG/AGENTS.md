# NCG Project Context

## What is this project about?

NCG is the Nagel Car Group automotive/e-commerce platform. It is a .NET
microservice landscape with APIs, service-owned databases, external
integration proxies, an API gateway, and IdentityServer/STS-based identity
flows. The documented platform currently covers 33 deployable backend
services across 59 projects.

The work is not limited to feature development. It also includes
containerized local and Hetzner environments, GitLab CI/CD, deployment and
check-build workflows, monitoring, incident diagnosis, secrets and
certificate handling, service dependencies, and operational documentation.

## What is the project team?

- Daniel Hecht is the documented infrastructure lead and works hands-on
  across backend development, DevOps, and platform engineering.
- Richard is the documented certificate manager.
- A GitLab administrator role is documented, but the person's name is kept
  in the NCG contact vault rather than in project notes.
- Other NCG developers and operational stakeholders contribute to the
  platform. The current documentation does not contain a complete,
  authoritative named team roster.

Do not invent names, reporting lines, or ownership. For current participants,
prefer the active meeting metadata, recent NCG meeting summaries, or verified
repository history. Treat a name mentioned in one implementation artifact as
a contributor, not automatically as a permanent team member or owner.

## What is Daniel's position in the project?

Daniel's documented roles are:

- Backend Developer
- DevOps Engineer
- Platform Engineer
- Infrastructure Lead

His responsibilities span backend services and APIs, containerized runtime
environments, CI/CD and deployments, platform and operations standards,
observability, incident analysis, Security Token Service and IdentityServer
topics, secrets and certificate hygiene, and technical documentation.

Daniel operates across feature delivery and production-readiness concerns.
Summaries should preserve this hands-on cross-cutting role and should not
inflate it into formal personnel management or overall project ownership
unless a meeting explicitly establishes that.

## Where can the agent find further information?

Start with the narrowest relevant source:

| Question                             | Source                                                                                                                                              |
| ------------------------------------ | --------------------------------------------------------------------------------------------------------------------------------------------------- |
| Documentation working rules          | `/Users/dh/Documents/DanielsVault/ncg/ncg-docs/AGENTS.md`                                                                                           |
| Operations overview and contacts     | `/Users/dh/Documents/DanielsVault/ncg/ncg-docs/docs/Ops/README.md`                                                                                  |
| Developer onboarding                 | `/Users/dh/Documents/DanielsVault/ncg/ncg-docs/docs/Ops/Guides/Quick-Start-Developers.md`                                                           |
| DevOps onboarding                    | `/Users/dh/Documents/DanielsVault/ncg/ncg-docs/docs/Ops/Guides/Quick-Start-DevOps.md`                                                               |
| Environments and infrastructure      | `/Users/dh/Documents/DanielsVault/ncg/ncg-docs/docs/Ops/Environments/` and `/Users/dh/Documents/DanielsVault/ncg/ncg-docs/docs/Ops/Infrastructure/` |
| Services and dependencies            | `/Users/dh/Documents/DanielsVault/ncg/ncg-docs/docs/Ops/Services/`                                                                                  |
| Security, certificates, and secrets  | `/Users/dh/Documents/DanielsVault/ncg/ncg-docs/docs/Ops/Security/` and `/Users/dh/Documents/DanielsVault/ncg/ncg-docs/docs/Security/`               |
| Active and historical specifications | `/Users/dh/Documents/DanielsVault/ncg/ncg-docs/docs/Specs/`                                                                                         |
| Daniel's role and project history    | `/Users/dh/Documents/DanielsVault/private/me/cv.md` and `/Users/dh/Documents/DanielsVault/private/me/profile.md`                                    |
| Past meeting outcomes                | `/Users/dh/Documents/DanielsVault/Meetings/Summaries/` filtered to summaries whose `projects` frontmatter contains `NCG`                            |

For an unfamiliar XYZ topic:

1. Read the nearest README or index in `ncg/ncg-docs`.
2. Search the `ncg-docs` QMD collection for concepts; use exact lexical
   search for service names, hosts, settings, routes, and identifiers.
3. Read the matching Ops, Security, or Specs source before drawing a
   conclusion.
4. Use current meeting evidence for people, commitments, deadlines, and
   status because those facts change faster than architecture notes.

## Meeting-assistant rules

- Use the exact project value `NCG` in meeting frontmatter.
- Distinguish proposals, decisions, action items, and verified runtime facts.
- Record an owner or due date only when the meeting or a cited source states
  it.
- Never copy secrets, tokens, passwords, private keys, or sensitive values
  into summaries or project files. Preserve vault references instead.
- Do not claim that a documented operational change is deployed or live
  without runtime, pipeline, or meeting evidence.
- Prefer links to authoritative NCG documents over duplicating long technical
  detail in project context.
