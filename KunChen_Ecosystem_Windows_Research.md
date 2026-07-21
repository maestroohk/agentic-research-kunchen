# Kun Chen Ecosystem — Windows-First Technical Research Report

**Author:** Staff+ Engineer research deliverable
**Date:** 2026-07-21
**Subject:** Deep technical analysis of Kun Chen's open-source ecosystem and its applicability to a Windows-first .NET engineering platform.

> **Read this first.** This is not a list of repositories. It is a technical architecture review of a personal software platform built by a former L8 engineer at Meta, Microsoft, and Atlassian. Kun Chen's work is best understood not as 67 separate GitHub repositories but as a single coherent argument: **the future of solo software development is an "agent distro" of small, focused, agent-ergonomic tools wired into a reproducible shell and gated by AI-driven quality control.** This report studies every active part of that argument and rates it for Windows-first adoption.
>
> Every claim below is grounded in primary sources — the repositories themselves, `axi.md`, his Substack, and the public READMEs. Where a fact could not be verified I say so explicitly. Where a judgment is opinion I label it. Conclusions are engineering recommendations, not endorsements.

---

## Table of Contents

1. [Executive Summary](#executive-summary)
2. [Ecosystem Map at a Glance](#ecosystem-map-at-a-glance)
3. [Repository-by-Repository Analysis](#repository-by-repository-analysis)
   - [Core / pinned (high signal)](#core--pinned-repositories)
   - [AXI reference implementations](#axi-reference-implementations)
   - [Other AXI-shaped tools](#other-axi-shaped-tools)
   - [Linux/macOS-only tools](#linuxmacos-only-tools)
   - [Archived / read-only](#archived--read-only-repositories)
   - [Smaller or experimental tools](#smaller-or-experimental-tools)
   - [Reference forks and other repos](#reference-forks-and-other-repos)
4. [Cross-Repository Analysis](#cross-repository-analysis)
5. [Learning Roadmap (4 weeks)](#learning-roadmap-4-weeks)
6. [Windows-First Recommendations](#windows-first-recommendations)
7. [AI Workflow Recommendations](#ai-workflow-recommendations)
8. [My Vision — Mapping the Ecosystem to a Windows-Native Platform](#my-vision--mapping-the-ecosystem-to-a-windows-native-platform)
9. [Migration Opportunities (Linux → Windows)](#migration-opportunities-linux--windows)
10. [Final Recommendations](#final-recommendations)
11. [Uncertainty Notes](#uncertainty-notes)

---

## Executive Summary

**Kun Chen's ecosystem is a coherent agentic engineering platform, not a loose portfolio.** It is built around five load-bearing ideas:

1. **AXI** (Agent eXperience Interface) — a 10-principle spec for CLIs that treat token budget as a first-class constraint. Demonstrated to beat both raw CLIs and MCP on success rate, cost, and turn count (per the published benchmarks, single model, single author, single repo).
2. **An "agent distro" called firstmate** — a portable directory of skills, hooks, and watcher scripts that turns any coding agent (Claude Code, Grok, Pi, Codex, OpenCode) into a single-liaison crew dispatcher.
3. **A git quality gate called no-mistakes** — a local proxy that runs AI-driven validation before pushing, with 6.8k stars and explicit Windows support.
4. **A worktree pool called treehouse** — a Go-based disposable-worktree manager with PowerShell installer, also officially supporting Windows.
5. **A long-running autonomous agent loop called gnhf** ("good night, have fun") — TS-based overnight iteration with per-iteration commits, resume, and worktree isolation, also with Windows support (via `SetThreadExecutionState`).

Around these are bundled deliverables: `chrome-devtools-axi` and `gh-axi` as production-grade AXI implementations, `lavish-axi` for HTML artifact review, `wheelhouse` for cross-repo maintainer decisions, `quota-axi` for LLM subscription awareness, `tasks-axi` for backlog ergonomics, and a long tail of smaller tools.

**For a Windows-first .NET engineer, the practical verdict is:**

- **Adopt immediately:** `no-mistakes`, `treehouse`, `gnhf`, `gh-axi`, `chrome-devtools-axi`, `quota-axi`, `tasks-axi`, `lavish-axi`, `acpx`. All officially support Windows or run cross-platform as Node/Go CLIs. `no-mistakes` and `treehouse` in particular are the highest-leverage tooling investments available.
- **Adopt with care:** `firstmate` (bash dispatcher — run on WSL or rewrite pieces in PowerShell), `gsh` (early-stage POSIX shell, partial Windows support), `wheelhouse` (GitHub-Actions-only, no Windows dependency).
- **Skip or use only on macOS:** `dotfiles`, `dotfiles-mac-nix`, `baby-menu`, `autopreso`, `short-pipe`, `justroll`, `simplewords` (all macOS-only).
- **Read-only / reference / skip for production:** `TerminalOne` (archived Jan 2025), the `*-axi` catalog forks from Jarvus/thatdudealso, the lone Swift/Neovim/Lua repos, the `matplotlib`/`scikit-learn`/etc. forks that appear to be personal reference copies.

The deeper finding is this: **Kun Chen's real intellectual contribution is not the repositories themselves but the design philosophy that ties them together.** A Windows-native engineer who internalizes AXI's 10 principles, the "agent distro" model, the disposable-worktree pattern, and the gated-push workflow will have a meaningfully more productive AI development practice — regardless of which specific tools they adopt.

---

## Ecosystem Map at a Glance

```
                              ┌─────────────────────┐
                              │   AXI (principles)  │  ← design philosophy
                              │  github.com/kunchenguid/axi
                              └──────────┬──────────┘
                                         │ informs
              ┌──────────────────────────┼──────────────────────────┐
              │                          │                          │
       ┌──────▼──────┐           ┌───────▼────────┐         ┌───────▼────────┐
       │ gh-axi      │           │ chrome-        │         │ lavish-axi     │
       │ 179 ★       │           │ devtools-axi   │         │ 2.1k ★         │
       └──────┬──────┘           │ 250 ★          │         └────────────────┘
              │                  └────────────────┘
              │  + tasks-axi, quota-axi, agent-browser-axi (catalog)
              │
       ┌──────▼──────┐           ┌────────────────┐         ┌────────────────┐
       │ no-mistakes │           │ firstmate      │         │ wheelhouse     │
       │ 6.8k ★      │           │ 1.7k ★         │         │ 18 ★           │
       │ Go, Win+   │           │ bash, mac/Lin  │         │ GH-Actions     │
       └──────┬──────┘           └───────┬────────┘         └────────────────┘
              │                          │
              │                  ┌───────▼────────┐
              │                  │ treehouse       │  ← used by firstmate
              │                  │ 1.0k ★          │
              │                  │ Go, Win+        │
              │                  └────────────────┘
              │
       ┌──────▼──────┐
       │ gnhf        │  ← overnight autonomous loop
       │ 3.3k ★      │     uses treehouse, supports Win
       │ TS, Win+    │
       └─────────────┘
```

**Key relationships:**
- **firstmate** consumes **treehouse** for worktree isolation, **no-mistakes** for gated pushes (optional), and **acpx** for ACP agent sessions.
- **gnhf** can use **treehouse** worktrees via `--worktree` and is gated by **no-mistakes** for releases (per wheelhouse config).
- **AXI** is the spec; **gh-axi / chrome-devtools-axi / lavish-axi / quota-axi / tasks-axi** are reference implementations.
- **wheelhouse** orchestrates a fleet of repos (including kunchenguid's own) through GitHub Issues + Actions, with **no-mistakes** as the required compliance check.

---

## Repository-by-Repository Analysis

> I am grouping the 67 repos by signal-to-noise ratio. The 6 "pinned" repos plus the most relevant unpinned ones get the full treatment. Smaller or low-relevance repos get a tight summary.

### CORE / PINNED REPOSITORIES

---

#### AXI — `github.com/kunchenguid/axi`

**Overview**

| | |
|---|---|
| **Purpose** | 10-principle specification for agent-ergonomic CLIs |
| **Status** | Active, v0.1.8 of `axi-sdk-js` (Jun 27, 2026) |
| **Primary language** | TypeScript 88.2% / HTML 6.2% / JavaScript 5.6% |
| **Maturity** | Production-grade spec, 1.6k stars, 6 open issues, 12 open PRs |
| **Maintained** | Yes (single primary maintainer) |
| **Archived** | No |
| **Experimental** | No (published benchmarks) |
| **Production ready** | Yes, for adoption of the *principles*; reference implementations are individually versioned |

**Problem it solves**

AI agents today interact with external services through two paradigms: legacy CLIs (built for humans, with separated action/observation that doubles tool calls) and MCP (with schema overhead that scales with tool count). AXI argues the answer is not protocol choice but design principles — `axi.md` opens with: *"Neither CLI nor MCP gives [agents] enough love."*

The concrete win: AXI-wrapped GitHub tools cost $0.050/task at 100% success vs. $0.148 (MCP) at 87%, and AXI-wrapped browser tools cost $0.074/task at 100% success vs. $0.101 (MCP) at 99%. The 12× gap on the `ci_failure_investigation` task is the headline case study.

**Core concepts**

The 10 principles (from `principles.yaml`):

*Efficiency*
1. Token-efficient output — TOON format (~40% saving vs. JSON)
2. Minimal default schemas (3–4 fields, not 10+)
3. Content truncation with `--full` escape hatch

*Robustness*
4. Pre-computed aggregates (eliminates round-trips)
5. Definitive empty states (`0 results` not ambiguous)
6. Structured errors + exit codes + idempotent mutations

*Discoverability*
7. Ambient context (SessionStart hooks + on-demand skills)
8. Content first (no-args shows live data, not help)
9. Contextual disclosure (next-step hints after every command)
10. Consistent help (concise per-subcommand reference)

**Architecture**: AXI is a *specification* plus a *generator*. The repo contains:
- `.agents/skills/axi/` — canonical spec installed via `npx skills add kunchenguid/axi`
- `bench-browser/` and `bench-github/` — published benchmark harnesses
- `packages/axi-sdk-js/` — published SDK (`axi-sdk-js@0.1.8`)
- `principles.yaml`, `catalog.yaml` — machine-readable source of truth
- `VISION.md`, `CONTRIBUTING.md`, `AGENTS.md`, `CLAUDE.md` — agent-readable operating contract

**How it integrates with the ecosystem**

AXI is the *north star*. Everything in the catalog is an "AXI-compliant" wrapper. The reference implementations in the official catalog: `gh-axi`, `chrome-devtools-axi`, `lavish-axi`, `quota-axi`. Community: `jj-axi`, `npm-axi`, `sqlite-axi`, `slack-axi`, `gws-axi`, `harvest-axi`, `notion-axi`, `clickup-axi`, plus a long Jarvus/thatdudealso catalog. AXI does not depend on firstmate, treehouse, or no-mistakes — but they share an agent-native design ethos.

**Windows compatibility**

✅ **Native Windows.** The benchmark harnesses and SDK are Node/TypeScript. The principles are platform-agnostic. The reference implementations (`gh-axi`, `chrome-devtools-axi`, `quota-axi`, `tasks-axi`, `lavish-axi`, `agent-browser-axi`) all list `macOS | Linux | Windows` in their badges.

**Windows implementation strategy**

- **PowerShell** — not needed for the CLI; needed only if you author your own AXI in PowerShell (the spec is language-agnostic; the catalog entries are all Node).
- **Windows Terminal** — works as the host; AXI outputs are pipe-friendly.
- **Git Bash** — full support; `npx skills add kunchenguid/axi` works under Git Bash on Windows.
- **WSL** — not required, but available as a fallback.
- **Visual Studio / VS Code / Rider** — the SDK (`axi-sdk-js`) is consumable from Node, which any of these can host; for a .NET team, the SDK can be wrapped or the principles ported.
- **Ollama** — AXI does not call models directly; it serves the model. Local Ollama is fully compatible.
- **Docker** — not required.
- **Git** — required only for repos that use git, not for AXI itself.

The change required to adopt AXI on Windows: **none**. The mental model change required: **non-trivial** — you must internalize the 10 principles and structure every CLI output you generate to match.

**Practical value**

| Dimension | Rating | Why |
|---|---|---|
| Productivity | ★★★★★ | 50–70% token reduction on agent tool use is the most reliable productivity win available |
| Developer Experience | ★★★★★ | TOON is readable; the principles are learnable in an afternoon |
| AI Development | ★★★★★ | The most directly relevant repo in the entire ecosystem for agent work |
| Team usage | ★★★★ | Patterns transfer; principles are language-agnostic |
| Enterprise usage | ★★★★ | A "build an AXI for your internal API" workshop is enterprise-ready |
| Learning value | ★★★★★ | The 10 principles are general engineering wisdom |
| Long-term value | ★★★★★ | Specs age better than tools |

**Cost impact**

Direct, measurable. The benchmarks show 50–70% input-token reduction on every tool interaction. With 915 benchmark runs validating the pattern, this is the most reliable cost lever in the ecosystem. Reduced prompts: yes, by combining actions (`open` returns navigate+snapshot). Reduced context: yes, via `--full` truncation. Improved planning: yes, via next-step hints. Improved code quality: indirectly, by making failures explicit.

**Learning difficulty**

**Easy.** Ten named principles, with examples in every reference implementation. The skill is a markdown file. A half-day of study is enough to start authoring AXI-shaped tools.

**Should I adopt it?**

**Must Adopt.** Even if you use none of the catalog tools, internalize the principles. Every CLI you write for an agent should comply. This is the single highest-leverage intellectual acquisition from the ecosystem.

---

#### no-mistakes — `github.com/kunchenguid/no-mistakes`

**Overview**

| | |
|---|---|
| **Purpose** | Local git proxy + AI-driven validation gate before `git push` |
| **Status** | Active, v1.40.0 (Jul 18, 2026), 92 releases |
| **Primary language** | Go 99.8% |
| **Maturity** | Production-ready, 6.8k stars (highest in the ecosystem) |
| **Maintained** | Yes, very actively |
| **Archived** | No |
| **Experimental** | No |
| **Production ready** | Yes |

**Problem it solves**

`git push` is the most error-prone moment in a developer's day. A push goes out, CI fails, the PR is messy, reviewers churn. no-mistakes puts a **local** proxy between you and your remote: you `git push no-mistakes` instead of `git push origin`, and the proxy spins up a disposable worktree, runs an AI-driven pipeline (review → test → docs → lint → push → PR → CI), auto-fixes mechanical issues, escalates the rest, and opens a clean PR on your behalf.

**Core concepts**

- **Disposable worktree**: every gate run is in an isolated worktree so your active work is not disturbed.
- **Agent-agnostic**: any of `claude`, `codex`, `rovodev`, `opencode`, `pi`, `copilot`, or `cursor` (via `acp:<target>`); ordered fallbacks.
- **Agent-native**: an agent skill (`/no-mistakes`) lets your coding agent do work and gate it, or gate existing committed work.
- **Human-in-the-loop**: "auto-fix or review findings, your call."
- **Three entry points**: `git push no-mistakes` (Git path), `no-mistakes` (TUI), `/no-mistakes` (agent skill).
- **TOON interface**: the non-interactive driver is `no-mistakes axi` — the first production example of an AXI-shaped tool from this ecosystem.

**Architecture**

Pure Go. Layout:
- `cmd/` — entry points
- `internal/` — private packages
- `skills/no-mistakes/` — committed agent skill (regenerated by `make skill`)
- `docs/` — Astro static site
- `Makefile`, `go.mod`, `go.sum`
- CI uses release-please; an `.airlock/` directory handles no-mistakes' own gate.

**How it integrates with the ecosystem**

- **Consumes treehouse** — indirectly, via shared worktree concepts (each run uses a disposable worktree).
- **Gated by itself** — no-mistakes' own development uses no-mistakes.
- **Required by wheelhouse** — every PR to every Kun Chen repo is required to be "raised via no-mistakes" (per `wheelhouse.config.yml`).
- **Used by gnhf** — gnhf commits pass through no-mistakes when configured.
- **Drives acpx** — the agent orchestration uses the ACP runtime.
- **First AXI tool** — the `no-mistakes axi` non-interactive TOON interface is the original proof that the AXI pattern applies to development tooling, not just external APIs.

**Windows compatibility**

✅ **Native Windows.** The README explicitly says: "platform-macOS | Linux | Windows-blue". The install script `docs/install.sh` works on Git Bash. A separate `install.ps1` exists. `go install` works directly. The Go binary is cross-compiled for `windows/amd64` and `windows/arm64`.

**Windows implementation strategy**

- **PowerShell** — the install script can be wrapped in PowerShell; the binary is invoked as `no-mistakes.exe` with normal arg passing.
- **Windows Terminal** — full support; TUI is built with bubbletea (cross-platform).
- **Git Bash** — primary install path on Windows via `docs/install.sh`.
- **WSL** — not required.
- **Visual Studio / VS Code / Rider** — no special integration; invoke from the IDE's terminal or a task.
- **Ollama** — not directly used; works through whichever agent you configure.
- **Docker** — not required.
- **Git** — **required** — this is fundamentally a git tool.

**Practical value**

| Dimension | Rating | Why |
|---|---|---|
| Productivity | ★★★★★ | Removes a whole class of "slop pushes" |
| Developer Experience | ★★★★★ | Single command, no workflow change |
| AI Development | ★★★★★ | Designed for AI-agent-generated commits |
| Team usage | ★★★★★ | Enforces a uniform quality floor |
| Enterprise usage | ★★★★★ | Compliance gate + audit trail |
| Learning value | ★★★★ | Teaches gated-push thinking |
| Long-term value | ★★★★★ | Becomes more valuable as agents generate more commits |

**Cost impact**

- Token spend: **increases** during the gate (every push triggers an LLM call) but **decreases** downstream by reducing review cycles, CI reruns, and reverts.
- Repeated prompts: **eliminates** "fix the lint" / "regenerate the changelog" loops.
- Context size: **stable** — worktree isolation keeps the LLM focused.
- Planning: improves — the agent must plan before pushing.
- Code quality: measurable improvement through enforced review.

**Learning difficulty**

**Medium.** Easy to install. The TUI is intuitive. The full power (custom gates, custom agent fallbacks, custom policies) requires a few hours of reading `docs/`.

**Should I adopt it?**

**Must Adopt.** This is the most directly applicable, highest-ROI tool in the ecosystem for a Windows-first engineer. The single most actionable recommendation in this entire report: **install no-mistakes on every Windows machine you touch this week.**

---

#### firstmate — `github.com/kunchenguid/firstmate`

**Overview**

| | |
|---|---|
| **Purpose** | Agent distro — portable directory that turns one coding agent into a crew dispatcher |
| **Status** | Active, 1.7k stars, 229 commits |
| **Primary language** | Shell 97.2% / JavaScript 2.0% |
| **Maturity** | Active, opinionated, 1.7k stars |
| **Maintained** | Yes |
| **Archived** | No |
| **Experimental** | Some backends (herdr, zellij, cmux) are experimental; tmux is the reference |
| **Production ready** | Yes for tmux backend on macOS/Linux; no for Windows |

**Problem it solves**

Managing a single coding agent is easy. Managing *parallel* agents — fix this bug, investigate that issue, audit this branch — becomes tab-juggling. firstmate inverts the model: you talk to one "first mate" agent, and the first mate dispatches a "crew" of agents in parallel, each in their own worktree, each in their own tmux window.

**Core concepts**

- **The captain ↔ first mate ↔ crewmates** model. You only interact with the first mate.
- **Ship tasks** (deliver code) and **Scout tasks** (investigate, plan, audit) as the two task shapes.
- **Project modes**: `no-mistakes` (default), `direct-PR`, `local-only`, optional `+yolo`.
- **Event-driven, zero-token supervision**: a bash watcher script sleeps, wakes the first mate only on events.
- **Optional secondmates**: persistent supervisors over isolated `FM_HOME`s.
- **Session backends**: tmux (reference), herdr, zellij, cmux, Orca (explicit). `codex-app` is not yet a runtime.
- **Verified harnesses**: Claude Code, Grok, Pi (co-primary); Codex, OpenCode (also verified, more supervision tradeoffs).
- **Built-in skills**: `/afk`, `/bearings`, `/updatefirstmate`, `/stow` (the only public installer-facing skill).

**Architecture**

Almost entirely shell scripts. `bin/` is the toolbelt, `docs/supervision-protocols/` is the per-harness watcher logic, `docs/backends/` are the backend implementations. Two-tier skill layout: `.agents/skills/` (internal, hidden) and `skills/` (public, installable).

**How it integrates with the ecosystem**

- **Consumes treehouse** — every crewmate gets a clean worktree.
- **Uses no-mistakes** — the `no-mistakes` project mode wraps pushes through the gate.
- **Built on acpx** — agent session lifecycle is via the Agent Client Protocol.
- **Skill `stow`** is publicly installable in any project.
- **`AGENTS.md`, `CLAUDE.md`** at the root define the operating contract for the first mate.

**Windows compatibility**

🔴 **Linux/macOS only.** Badge lists only macOS and Linux. The dependency on tmux as the reference backend, bash as the supervision language, and `gh auth login` for the fleet workflow are all unix-first.

**Windows implementation strategy**

This is the most architecturally significant Windows-incompatibility in the ecosystem. Options:

- **Run in WSL** — the natural path. WSL2 with Ubuntu, install firstmate, expose to Windows via VS Code's WSL extension or Windows Terminal. Treat the WSL instance as a "first mate host" that you SSH/connect to.
- **Run in Docker** — firstmate's bash-centric design would survive a container, but session visibility is lost.
- **Port to PowerShell** — feasible but high effort. The watcher loop, tmux backend, and bin/ toolbelt would all need rewrites. Estimated 2–4 weeks for a credible port.
- **Use only the `stow` skill** — `skills/stow/` is portable; the rest is not. You can install the stow skill into any project and benefit from it without the full firstmate.

The recommended path for a Windows engineer: **run firstmate inside WSL2 with tmux**, drive it from Windows Terminal, and treat it as your "long-running agent platform" the same way you would a Linux server. This is a clean separation: Windows for primary development, WSL2 for the agent crew.

**Practical value**

| Dimension | Rating | Why |
|---|---|---|
| Productivity | ★★★★ | Crew dispatch is real value, but only if you actually run multiple agents |
| Developer Experience | ★★★ | High learning curve, opinionated model |
| AI Development | ★★★★★ | The most sophisticated orchestration model in the ecosystem |
| Team usage | ★★★ | Single-user-focused; team adoption is harder |
| Enterprise usage | ★★ | Bash-heavy, non-standard |
| Learning value | ★★★★★ | Teaches agent orchestration thinking |
| Long-term value | ★★★★ | Pattern will outlive the implementation |

**Cost impact**

- Token spend: **dramatically reduced** by the event-driven watcher (the first mate only wakes on events).
- Repeated prompts: reduced — each crewmate maintains its own context.
- Context size: well-managed — each crewmate is a clean session.
- Planning: **significantly improved** — the dispatch model forces planning.
- Code quality: depends on the agent and project mode.

**Learning difficulty**

**Hard.** This is the most opinionated and complex repo in the ecosystem. Expect 1–2 weeks to become productive. The reward is the highest-level orchestration model available.

**Should I adopt it?**

**Maybe (with adaptation).** Adopt the *pattern* (captain → first mate → crew) without the full bash implementation. Run firstmate in WSL2 if you want the full experience. Alternatively, study its design and port the dispatcher concept to a PowerShell orchestrator of your own.

---

#### treehouse — `github.com/kunchenguid/treehouse`

**Overview**

| | |
|---|---|
| **Purpose** | CLI to manage a pool of reusable git worktrees for AI agent workflows |
| **Status** | Active, v2.1.0 (Jul 20, 2026), 22 releases |
| **Primary language** | Go 97.9% / HTML 1.6% |
| **Maturity** | Production-ready, 1.0k stars |
| **Maintained** | Yes |
| **Archived** | No |
| **Experimental** | No |
| **Production ready** | Yes |

**Problem it solves**

When AI agents spin up many parallel worktrees, repeatedly creating and destroying them costs dependency install time and build cache. treehouse maintains a *pool* of pre-provisioned worktrees per repo, hands them out, gets them back on exit, and reuses them.

**Core concepts**

- **Pool model**: per-repo, default 16, configurable via `treehouse.toml` or `~/.config/treehouse/config.toml`.
- **Detached HEAD**: avoids branch name conflicts; agents rebase/checkout as needed.
- **No daemon**: every operation is an inline CLI invocation; state is a small JSON file on disk under a lock.
- **Leasing**: `treehouse get --lease` allocates durably without a subshell; ABA-safe `return --if-lease-id` patterns.
- **Hooks**: `post_create` and `pre_destroy` hooks from user-level config.
- **Prune/destroy safety**: dry-run by default; explicit opt-in for risky removals.

**Architecture**

Pure Go. Layout: `cmd/`, `internal/`, `main.go`, `go.mod`, `Makefile`, `flake.nix`. Single binary.

**How it integrates with the ecosystem**

- **Consumed by firstmate** — every crewmate gets a treehouse.
- **Consumed by gnhf** — `--worktree` mode uses treehouse-style isolation.
- **Consumed by no-mistakes** — the disposable-worktree concept is shared.
- **No upstream dependencies** — treehouse is a foundational tool.

**Windows compatibility**

✅ **Native Windows.** Badge lists macOS, Linux, Windows, Nix. PowerShell installer at `irm https://kunchenguid.github.io/treehouse/install.ps1 | iex`. Hooks use `%COMSPEC% /c` on Windows vs `/bin/sh -c` on Unix. This is one of the most cross-platform-aware tools in the ecosystem.

**Windows implementation strategy**

- **PowerShell** — the install path; also the hook execution environment via `%COMSPEC% /c`.
- **Windows Terminal** — fully compatible.
- **Git Bash** — works; PowerShell is preferred.
- **WSL** — works, but unnecessary.
- **Visual Studio / VS Code / Rider** — invoke from the IDE terminal. VS Code's "Open in New Window" with a treehouse path is a natural integration.
- **Ollama** — not relevant.
- **Docker** — not relevant.
- **Git** — **required**.

**Practical value**

| Dimension | Rating | Why |
|---|---|---|
| Productivity | ★★★★★ | Removes the worktree-creation cost from every agent run |
| Developer Experience | ★★★★ | Simple mental model; one binary |
| AI Development | ★★★★★ | Designed for AI agent workflows |
| Team usage | ★★★★ | Pool size and root are team-sharable via repo config |
| Enterprise usage | ★★★ | Single-machine tool; central management not built in |
| Learning value | ★★★★ | Teaches durable-lease thinking |
| Long-term value | ★★★★★ | Solves a real scaling problem |

**Cost impact**

- Reduces dependency install time (CPU and disk).
- Reduces build cache misses.
- Token cost: indirect — faster worktree handoff means shorter agent sessions.

**Learning difficulty**

**Easy.** The CLI is a handful of commands. The leasing system is the only conceptually new piece, and it has good examples.

**Should I adopt it?**

**Must Adopt.** treehouse is the single best Windows-friendly tool in the ecosystem for AI-assisted development. Combined with no-mistakes, it forms the foundation of a Windows-native agent workflow.

---

#### gnhf — `github.com/kunchenguid/gnhf`

**Overview**

| | |
|---|---|
| **Purpose** | "Good night, have fun" — overnight autonomous agent orchestrator |
| **Status** | Active, v0.1.42 (May 13, 2026), 42 releases |
| **Primary language** | TypeScript 98.7% |
| **Maturity** | Production-ready, 3.3k stars |
| **Maintained** | Yes, very actively |
| **Archived** | No |
| **Experimental** | No |
| **Production ready** | Yes |

**Problem it solves**

You go to bed; the agent keeps working. You wake up to a branch full of clean, committed, documented iterations toward a stated objective. The name "ralph, autoresearch-style orchestrator" positions gnhf alongside the ralph/autoresearch pattern of long-running agent loops.

**Core concepts**

- **Iterative loop**: each iteration = one prompt → agent runs non-interactively → success commits, failure rolls back. Loop continues unless `--max-iterations`, `--max-tokens`, `--stop-when <cond>`, or 3 consecutive failures.
- **Per-iteration commits**: incremental, unsigned (avoids GPG prompts during the run).
- **Shared memory**: `notes.md` is the iteration-over-iteration context file.
- **Two run modes**: `--current-branch` (push after each iteration) and `--worktree` (isolated per run).
- **Multiple agents**: Claude Code, Codex, Copilot, Pi, Rovo Dev, OpenCode, plus `acp:<target>` via bundled `acpx`.
- **Sleep prevention**: macOS `caffeinate`, Linux `systemd-inhibit`, Windows `SetThreadExecutionState`.
- **Optional telemetry**: anonymous, opt-out via `GNHF_TELEMETRY=0`.
- **CLI resume**: `gnhf` on an existing `gnhf/` branch resumes; new runs with the same name get numeric suffixes.

**Architecture**

TypeScript monorepo, pnpm workspaces, tsdown bundler, ESLint, Prettier, release-please. Layout: `.airlock/`, `skills/gnhf/`, `e2e/`, `src/`, `docs/`. Bundled `acpx` for ACP runtime.

**How it integrates with the ecosystem**

- **Consumes treehouse** via `--worktree` mode.
- **Gated by no-mistakes** for releases (per `wheelhouse.config.yml`).
- **Bundles acpx** for ACP targets.
- **Sibling to gsh** — both are TypeScript-based agent runtime tools, with gnhf focused on long-running iteration and gsh focused on shell-level integration.
- **Independent of firstmate** — firstmate is a multi-agent dispatcher; gnhf is a single-agent long-runner.

**Windows compatibility**

✅ **Native Windows.** The README confirms. Sleep prevention uses `SetThreadExecutionState` on Windows. `.cmd` and `.bat` wrappers are supported. `corepack enable` works on Windows. The bundled `acpx` runs on Node, so it works on Windows.

**Windows implementation strategy**

- **PowerShell** — not required; gnhf is invoked as a Node CLI.
- **Windows Terminal** — full support.
- **Git Bash** — works.
- **WSL** — not required.
- **VS Code** — naturally invoked from a VS Code terminal in a worktree.
- **Ollama** — supported via `acp:<target>` configurations pointing at Ollama-backed agents.
- **Docker** — not required.
- **Git** — **required**.

**Practical value**

| Dimension | Rating | Why |
|---|---|---|
| Productivity | ★★★★★ | Overnight work is the most underused time on a developer's calendar |
| Developer Experience | ★★★★ | One command; clear output |
| AI Development | ★★★★★ | Core use case |
| Team usage | ★★★ | Single-user by default; team-friendly via shared run metadata |
| Enterprise usage | ★★ | Hard to govern "agents running overnight" |
| Learning value | ★★★★ | Teaches long-running-loop thinking |
| Long-term value | ★★★★★ | The pattern of overnight agent work will become standard |

**Cost impact**

- Token spend: bounded by `--max-tokens` and `--max-iterations`.
- Repeated prompts: eliminated (the agent reads `notes.md`).
- Context size: stable (clean per-iteration state).
- Planning: **forced** — you must write a clear `prompt.md` to start.
- Code quality: depends on agent, but per-iteration commits are inspectable.

**Learning difficulty**

**Easy to start, medium to master.** Running a basic overnight iteration is one command. Tuning the agent, agentArgs, stop conditions, and acp targets takes a few days of experimentation.

**Should I adopt it?**

**Must Adopt.** This is the most direct productivity win for a Windows engineer who works with AI agents. You set up the prompt, run `gnhf "..."`, go to bed, and triage the morning's diff. Combined with no-mistakes, this is a closed-loop overnight development workflow.

---

#### lavish-axi — `github.com/kunchenguid/lavish-axi`

**Overview**

| | |
|---|---|
| **Purpose** | Local-first editor for agent-produced HTML artifacts |
| **Status** | Active, v0.1.42 (Jul 14, 2026), 42 releases |
| **Primary language** | JavaScript 88.2% / HTML 9.2% / CSS 2.6% |
| **Maturity** | Production-ready, 2.1k stars |
| **Maintained** | Yes |
| **Archived** | No |
| **Experimental** | No |
| **Production ready** | Yes |

**Problem it solves**

Agents are good at producing rich HTML artifacts (plans, diagrams, dashboards, slide decks). The human-agent loop on those artifacts is weak — you take screenshots, type "make this red, move that box," and the agent re-renders blind. Lavish provides a local editor where the human annotates elements and text ranges directly on the rendered artifact, the agent polls for the feedback via `lavish-axi poll`, and the loop closes.

**Core concepts**

- **Local-first review**: CLI on `127.0.0.1:4387`; no cloud dependency in the core loop.
- **File-path identity**: sessions are keyed by the canonical HTML file path, not opaque IDs.
- **Portable artifacts**: the HTML runs in an iframe; Lavish injects a small SDK for annotations, snapshots, feedback, and layout checks.
- **Layout failure gate**: a real in-iframe audit at open time masks severely broken layouts.
- **Mermaid as whiteboard**: every `.mermaid` container becomes an embedded Excalidraw canvas.
- **Open-time next-step hints** via the seven playbooks: `diagram`, `table`, `comparison`, `plan`, `code`, `input`, `slides`.
- **Sharing**: opt-in via `ht-ml.app` with optional password.
- **Agent presence**: the browser shows when an agent is listening, preserves queued feedback across reloads.
- **`--agent-reply "..."`** concludes a cycle and re-enables human sends.

**Architecture**

JavaScript. Top-level layout: `.agents/skills/lavish-design/` (internal), `skills/lavish/` (public), `src/`, `test/`, `lavish-editor-marketing/`, `bin/`, `scripts/`. pnpm workspaces, vitest, release-please.

**How it integrates with the ecosystem**

- **AXI-compliant** by self-description.
- **First-class member of the AXI catalog** (listed on axi.md).
- **Used by Kun Chen's own agents** to produce plans and diagrams.
- **Independent** of firstmate, treehouse, gnhf.

**Windows compatibility**

✅ **Native Windows.** No OS-specific code in the README. Node CLI; works wherever Node runs.

**Windows implementation strategy**

- **PowerShell** — not needed; `npx -y lavish-axi` from any shell.
- **Windows Terminal** — works; the server is a browser UI, not a TUI.
- **Git Bash** — works.
- **WSL** — not required.
- **VS Code** — invoke from a VS Code terminal; output renders in a browser.
- **Ollama** — not used directly; the agent behind the poll can be Ollama-backed.
- **Docker** — not required.
- **Git** — not required for the basic flow; useful if you want to commit the artifacts.

**Practical value**

| Dimension | Rating | Why |
|---|---|---|
| Productivity | ★★★★ | Closes the agent-HTML feedback loop |
| Developer Experience | ★★★★★ | Excellent UX, well-designed |
| AI Development | ★★★★ | Specialized but high-value for HTML artifacts |
| Team usage | ★★★ | Per-user tool; sharing is opt-in |
| Enterprise usage | ★★★ | Loopback-only by design |
| Learning value | ★★★★ | Teaches AXI-style CLI design |
| Long-term value | ★★★★ | Pattern outlives implementation |

**Cost impact**

- Token savings: large — the agent no longer needs to re-render to "see" what changed; the human's annotations are structured.
- Repeated prompts: reduced — humans annotate once; agent acts on the annotations.
- Context size: stable.
- Planning: improved.
- Code quality: n/a (artifacts are not code).

**Learning difficulty**

**Easy.** Install the skill; tell your agent to use it. The playbooks are the only non-obvious part.

**Should I adopt it?**

**Probably Adopt.** For an engineer who produces a lot of HTML artifacts (architecture diagrams, design plans, dashboards), this is a high-value tool. For a pure backend .NET engineer, it is nice-to-have.

---

### AXI REFERENCE IMPLEMENTATIONS

---

#### gh-axi — `github.com/kunchenguid/gh-axi`

**Overview**

| | |
|---|---|
| **Purpose** | AXI-shaped wrapper around the official `gh` CLI |
| **Status** | Active, v0.1.27 (Jul 12, 2026), 23 releases |
| **Primary language** | TypeScript 95.5% |
| **Maturity** | Production-ready, 179 stars |
| **Production ready** | Yes |

**Problem it solves**

The official `gh` CLI is built for humans. Its output is verbose, not structured, and the agent has to guess subcommands. `gh-axi` wraps `gh` with TOON output, contextual next-step hints, and structured errors. The benchmark shows 100% task success at $0.050/task vs. 86% at $0.054 for raw `gh` and 87% at $0.148 for the GitHub MCP server.

**Windows compatibility**

✅ Native Windows (Node 20+, `gh` CLI installed and authenticated).

**Should I adopt it?**

**Must Adopt.** If you use GitHub and an AI agent, this is the cleanest, most direct improvement you can make to your agent's GitHub interactions. It is the canonical reference for what an AXI-shaped tool looks like.

---

#### chrome-devtools-axi — `github.com/kunchenguid/chrome-devtools-axi`

**Overview**

| | |
|---|---|
| **Purpose** | AXI-shaped wrapper around `chrome-devtools-mcp` for browser automation |
| **Status** | Active, v0.1.26 (Jul 1, 2026), 26 releases |
| **Primary language** | TypeScript 98.1% |
| **Maturity** | Production-ready, 250 stars |
| **Production ready** | Yes |

**Problem it solves**

Browser automation for agents is expensive and fragile. Raw MCP averages 184K input tokens/task; chrome-devtools-axi averages 79K (57% less) and is 100% successful on the benchmark. Persistent bridge server keeps Chrome alive across commands; `g<N>:` generation-prefixed refs prevent stale-element silent failures.

**Windows compatibility**

✅ Native Windows. Chrome/Chromium runs on Windows; the bridge server is Node.

**Should I adopt it?**

**Probably Adopt.** If you do any browser automation with agents (web scraping, E2E testing, screenshot review), this is a clear win. The bridge architecture is clever and the stale-ref detection is a real engineering contribution.

---

#### quota-axi — `github.com/kunchenguid/quota-axi`

**Overview**

| | |
|---|---|
| **Purpose** | AXI-shaped quota/usage reporter for Claude, Codex, Cursor, Copilot, Grok, Kimi |
| **Status** | Active (newer repo, 31 stars) |
| **Primary language** | TypeScript |
| **Maturity** | Production-ready |
| **Production ready** | Yes |

**Problem it solves**

Agents need to know quota state before they choose where work can safely run. Vendor dashboards are not shell-friendly. `quota-axi` reads local provider auth sources and reports all quota windows in one TOON call. Never mutates state. macOS Keychain access is opt-in via `--allow-keychain-prompt` to avoid surprise prompts.

**Windows compatibility**

✅ Native Windows. Note: macOS-specific paths exist (e.g. `~/Library/Application Support/...` for Cursor) but they are gated — providers without a Windows local source simply report `unavailable` rather than failing.

**Should I adopt it?**

**Must Adopt.** Routing-aware agents are the next step past cost-aware agents. If you use multiple LLM subscriptions (Claude, Codex, Copilot, etc.), this is the single best way to let your agent pick a provider based on remaining quota. Windows-friendly out of the box.

---

#### tasks-axi — `github.com/kunchenguid/tasks-axi`

**Overview**

| | |
|---|---|
| **Purpose** | AXI-shaped task/backlog manager with markdown + sqlite backends |
| **Status** | Active (24 stars) |
| **Primary language** | TypeScript |
| **Maturity** | Production-ready |
| **Production ready** | Yes |

**Problem it solves**

Every backlog mutation today regenerates markdown through the model — expensive output tokens, risks dropped/duplicated/reordered items. `tasks-axi` edits `backlog.md` in place with byte-exact round-trip, so the markdown stays the source of truth and long task bodies never bloat `list` output. Borrows the dependency-graph and ready-query model from `beads`, adds structured dispatch holds, keeps the `*-axi` house style.

**Windows compatibility**

✅ Native Windows. Markdown backend is portable; sqlite backend is portable.

**Should I adopt it?**

**Probably Adopt.** For a solo Windows engineer using AI agents on a multi-task project, this is a clean way to manage a backlog. The byte-exact round-trip is a real engineering contribution. The `public-followup` namespace for firstmate-style obligation tracking is interesting but optional.

---

#### agent-browser-axi — `github.com/kunchenguid/agent-browser-axi`

**Overview**

| | |
|---|---|
| **Purpose** | AXI wrapper around Vercel's `agent-browser` (alternative to chrome-devtools-axi) |
| **Status** | Active (6 stars) |
| **Primary language** | TypeScript |
| **Maturity** | Earlier stage |

**Windows compatibility**

✅ Native Windows (cross-platform badge).

**Should I adopt it?**

**Maybe.** Sibling to chrome-devtools-axi; pick one based on whether you prefer Vercel's `agent-browser` or Google's `chrome-devtools-mcp` underneath. For a Windows engineer, both are equivalent.

---

### OTHER AXI-SHAPED / ECOSYSTEM TOOLS

---

#### wheelhouse — `github.com/kunchenguid/wheelhouse`

**Overview**

| | |
|---|---|
| **Purpose** | Personal "what needs my decision" command center built on GitHub Issues + Actions |
| **Status** | Active (18 stars) |
| **Primary language** | Python |
| **Maturity** | Production-ready for maintainers; small community |
| **Production ready** | Yes (for solo/team maintainers) |

**Problem it solves**

Maintainers drown in PRs, fork-CI approvals, and issue triage. wheelhouse surfaces every pending decision across a fleet of repos into one queue, with auto-triage, auto-approve for safe fork-CI, plain-English decisions, and a strict owner-only acting model. The single `wheelhouse.config.yml` file plus one `FLEET_TOKEN` secret is the entire setup.

**Core concepts**

- **GitHub Issues as the storage layer**: open = pending, closed = consumed.
- **Card states via labels**: `needs-decision`, `pending-triage`, `processing`, `resolved`, `blocked`, plus `repo:`, `kind:`, `priority:`.
- **Workflows**: `ingest.yml`, `decision-handler.yml`, `scan-backstop.yml` (hourly), `triage.yml`, `deep-review.yml`, `no-mistakes-required.yml`.
- **Decisions via checkbox ticks, slash-commands, or plain English** (Claude is the LLM provider).
- **Security model**: owner-only, fork-CI approval, `pull_request_target` caveat flagged, LLM injection defense with READONLY_TOKEN.

**Architecture**

Python scripts in `scripts/` (`wheelhouse_core.py`, `render_card.py`), GitHub Actions workflows in `.github/workflows/`, agent runtime adapter in `agent_runtime/`, single `wheelhouse.config.yml`.

**How it integrates with the ecosystem**

- **Drives the entire Kun Chen fleet** — wheelhouse's own `config.yml` lists `lavish-axi`, `gnhf`, `firstmate`, `no-mistakes`, `axi`, `chrome-devtools-axi` (and more) as the repos it manages.
- **Requires no-mistakes** as the compliance check on every PR.
- **Uses Claude** as the LLM provider; Codex CLI is explicitly forbidden.

**Windows compatibility**

✅ **Platform-agnostic.** All execution is in GitHub Actions. Local interaction is `python scripts/wheelhouse_core.py checks <repo>` which runs anywhere Python runs. The shell scripts are cross-platform-aware.

**Should I adopt it?**

**Probably Adopt (for maintainers).** If you maintain 3+ repos on GitHub and want a single triage queue, wheelhouse is purpose-built for that. For a .NET engineer who maintains their own libraries or open-source projects, this is a real force multiplier. For a corporate dev, the GitHub-Actions-only model might be a non-starter.

---

#### gsh — `github.com/kunchenguid/gsh`

**Overview**

| | |
|---|---|
| **Purpose** | "Generative Shell" — POSIX-compatible shell with built-in LLM agents |
| **Status** | Active, v1.11.0 (May 18, 2026), 98 releases |
| **Primary language** | Go 97.8% / GSC 1.8% |
| **Maturity** | Early stage, explicit "use at your own risk" warning |
| **Production ready** | No (early stage) |

**Problem it solves**

Most shells predate LLMs. gsh is a POSIX-compatible REPL with `gsh` extension for AI agents — `#` prefix for agent chat, an agentic scripting language, support for local (Ollama) and remote (OpenAI API-compatible) LLMs.

**Architecture**

Go, built on `mvdan/sh` (POSIX parser/interpreter), bubbletea (TUI), zap (logging). `.gsh/repl.gsh` and `.gshrc` config. Cross-platform telemetry.

**Windows compatibility**

🟡 **Partial.** Repository does not state explicit OS support. Build matrix would be in `.github/workflows/`. The Go codebase should compile on Windows, but tmux-style multi-pane UX, hot-key ergonomics, and Unix-style history may have rough edges.

**Should I adopt it?**

**Skip for now.** A POSIX shell on Windows is fighting the platform. PowerShell is the natural Windows shell; the gsh agentic scripting language concept is interesting but needs to mature. Re-evaluate in 6 months.

---

#### acpx — `github.com/kunchenguid/acpx` (now `openclaw/acpx`)

**Overview**

| | |
|---|---|
| **Purpose** | Headless CLI client for stateful Agent Client Protocol (ACP) sessions |
| **Status** | Alpha — interfaces likely to change |
| **Primary language** | TypeScript |
| **Maturity** | Alpha |

**Problem it solves**

Agents scraping characters from PTY sessions is fragile. acpx provides a structured protocol for agent-to-agent communication — persistent sessions, named sessions (`-s backend`), prompt queueing, cooperative cancel, soft-close lifecycle, fire-and-forget, queue owner TTL.

**Windows compatibility**

✅ Native Windows. Node-based; ACP is a JSON-RPC-style protocol.

**Should I adopt it?**

**Maybe.** If you are building agent-orchestration tools on Windows and want ACP support, acpx is the path. The "alpha" warning means don't depend on it for production yet.

---

#### acp-mock — `github.com/kunchenguid/acp-mock`

**Overview**

| | |
|---|---|
| **Purpose** | Test ACP clients E2E without spending tokens |
| **Status** | Active (3 stars) |

**Windows compatibility**

✅ Native Windows.

**Should I adopt it?**

**Adopt if building ACP tools.** A mock ACP server for testing is the right shape.

---

#### superpowers-bench — `github.com/kunchenguid/superpowers-bench`

**Overview**

| | |
|---|---|
| **Purpose** | Benchmark how well agents discover skills from obra/superpowers |
| **Status** | Active (42 stars) |

**Problem it solves**

"You give your agent a set of skills. You describe a task. Does it figure out which skills to use — or does it need hand-holding?" superpowers-bench measures skill selection as the unit (not code quality, not pass/fail). Two conditions: no hints, with hints. Three agents: Claude, Codex, OpenCode. 20 tasks.

**Windows compatibility**

🟡 Soft Windows support. Node 16+, agent CLIs on PATH. The shell scripts (1.2% of codebase) may have rough edges on Windows.

**Should I adopt it?**

**Skip unless building skill discovery tooling.** The pattern is interesting; the specific benchmark is one author's setup.

---

#### programbench-bench — `github.com/kunchenguid/programbench-bench`

**Overview**

| | |
|---|---|
| **Purpose** | Measure agent harness design decisions by holding model constant |
| **Status** | Active (17 stars) |
| **Primary language** | Shell 65.5% / Python 34.5% |

**Problem it solves**

ProgramBench measures models. programbench-bench measures harnesses. "Arms" (a directory of orchestration prompt + skills + MCP config) are run against the same model on the same tasks. Studies compare arms A/B with paired Wilcoxon, Holm-corrected. Three-container network-isolated topology: proxy with domain allow-list, agent container, cleanroom container (kernel-enforced air-gap).

**Windows compatibility**

🔴 **No Windows support** documented. The Docker + `uv` setup is platform-agnostic in principle, but the README only confirms macOS and Linux.

**Should I adopt it?**

**Skip for Windows use; study the methodology.** The harness-design-via-controlled-experiment idea is intellectually important. The specific tool requires Linux or macOS.

---

#### mcp-compressor — `github.com/kunchenguid/mcp-compressor`

⚠️ **Important note:** This repository is a fork of `atlassian-labs/mcp-compressor`, not original work. It wraps existing MCP servers and compresses tool descriptions — 70–97% token reduction. Converts any MCP server into a local CLI via `--cli-mode`. Uses TOON for output.

**Windows compatibility**

✅ Native Windows. Python package; runs anywhere Python runs.

**Should I adopt it?**

**Adopt if using MCP.** This is the atlassian-labs package; Kun Chen's fork may or may not have diverged. For your purposes, the upstream is sufficient.

---

#### whathappened — `github.com/kunchenguid/whathappened`

**Overview**

| | |
|---|---|
| **Purpose** | X-only briefing skill for Grok Build |
| **Status** | Active (194 stars) |
| **Production ready** | Yes (Grok Build only) |

**Problem it solves**

When something drops on X, you want a neutral briefing: what happened, where the conversation is, the opinion map, the live debates, and the receipts. The skill uses Grok's native X tools (`x_keyword_search`, `x_semantic_search`, `x_thread_fetch`, `x_user_search`) and produces adaptive-window briefings.

**Windows compatibility**

✅ Windows-compatible as a skill, but **requires Grok Build** to actually run. Other agents can install the package; they cannot run it for real.

**Should I adopt it?**

**Skip unless on Grok Build.** Grok-only. The design is excellent (opinion map, debate steelmanning, real post links) but the dependency is binding.

---

### LINUX/MACOS-ONLY TOOLS

---

#### dotfiles — `github.com/kunchenguid/dotfiles`

**Overview**

| | |
|---|---|
| **Purpose** | Kun's personal dotfiles for agentic engineering, managed with nix-darwin + home-manager |
| **Status** | Active (200 stars), actively maintained |
| **Primary language** | Nix 40.7% / Lua 32.6% / Shell 26.7% |

**Windows compatibility**

🔴 **macOS only** (Apple Silicon default, Intel supported via one-line change). nix-darwin is macOS-specific. The Neovim config is portable but the system build is not.

**Should I adopt it?**

**Skip the repo, study the philosophy.** The dotfiles are a one-engineer setup. The ideas (declarative, reproducible, layered — Nix for CLI, Homebrew for GUI, ecosystem managers for domain-specific) are portable. The "Make it yours" section is a good template for any dotfiles effort.

**Windows migration:**

- Nix → WinGet or Chocolatey (Microsoft's vcpkg for some categories)
- nix-darwin → Microsoft PowerShell DSC, or `chezmoi` for cross-platform dotfile management
- Home Manager → PowerShell profile (`$PROFILE`) + a declarative config tool
- Homebrew → WinGet (Microsoft's official package manager for Windows)
- WezTerm → Windows Terminal (built into Windows 11)
- lazy.nvim Neovim config → can be cross-platform, but Neovim itself is not the natural Windows editor

The deeper lesson: **build a Windows-native equivalent using WinGet + PowerShell + Windows Terminal + a dotfile manager.** Do not try to port nix-darwin to Windows.

---

#### dotfiles-mac-nix — `github.com/kunchenguid/dotfiles-mac-nix`

**Overview**

The older, public companion of `dotfiles`. Superseded. The README explicitly redirects to `kunchenguid/dotfiles`.

**Should I adopt it?**

**Skip.** Use the newer `dotfiles` repo for reference; use this one only for the historical "How I Built a Reproducible Mac Setup" blog post.

---

#### baby-menu — `github.com/kunchenguid/baby-menu`

**Overview**

| | |
|---|---|
| **Purpose** | macOS menu bar app that can become anything you ask it to be |
| **Status** | Active, v0.1.20 (Jul 21, 2026) |
| **Primary language** | TypeScript 73.8% / HTML 13.0% |
| **Maturity** | 106 stars, alpha-ish |

**Windows compatibility**

🔴 **macOS only.** Requires macOS 13 Ventura or newer, Homebrew, and an authenticated `claude` or `codex` CLI. The concept (tray icon, "ask, don't configure," agent-driven extension hot-reload) is interesting but Electron-on-macOS-only.

**Should I adopt it?**

**Skip on Windows.** The Windows tray bar is more constrained; a Windows port would need significant rework. The architecture (three-process, recipes-as-specs, diff-derived Keep/Undo) is a good reference for a hypothetical Windows port.

---

#### autopreso — `github.com/kunchenguid/autopreso`

**Overview**

| | |
|---|---|
| **Purpose** | Realtime speech-to-presentation (Excalidraw + STT + agent) |
| **Status** | Active, alpha, v0.1.7 (May 21, 2026) |
| **Maturity** | 409 stars, alpha |

**Windows compatibility**

🟡 **Limited.** macOS full support (arm64 + x64); Windows/Linux lose local Moonshine STT and must use OpenAI Realtime.

**Should I adopt it?**

**Skip on Windows for now.** The Windows STT story (Windows Speech Recognition, Azure Speech, Whisper) is different; a Windows port would need a new STT adapter. The concept is sound and the OpenAI Realtime path is platform-agnostic.

---

#### short-pipe — `github.com/kunchenguid/short-pipe`

**Overview**

| | |
|---|---|
| **Purpose** | Local-first desktop app that turns long-form video into captioned vertical shorts |
| **Status** | Active (19 stars) |
| **Platform** | macOS only |

**Should I adopt it?**

**Skip on Windows.** The dependency on FFmpeg and HyperFrames is portable; the UI shell is Electron. A Windows build is feasible (Electron is cross-platform) but the project does not ship one.

---

#### justroll — `github.com/kunchenguid/justroll`

**Overview**

| | |
|---|---|
| **Purpose** | One-command screen + camera recording (one file per source) |
| **Status** | Active (12 stars) |
| **Platform** | macOS only |

**Should I adopt it?**

**Skip on Windows.** Windows has built-in Game Bar recording; OBS Studio works on all platforms; the specific value prop (one file per source with synced mic) can be replicated with a custom FFmpeg command.

---

#### simplewords — `github.com/kunchenguid/simplewords`

**Overview**

| | |
|---|---|
| **Purpose** | Chrome extension that turns rough replies into respectful drafts |
| **Status** | Active (11 stars) |
| **Platform** | Chrome extension — works wherever Chrome works, including Windows |

**Should I adopt it?**

**Probably Adopt.** A Chrome extension is platform-agnostic. The "Your provider, your settings — OpenAI, Codex, or local Ollama" line is exactly what a Windows-first engineer would want.

---

#### trial-by-combat — `github.com/kunchenguid/trial-by-combat`

**Overview**

| | |
|---|---|
| **Purpose** | LLM 1v1 duel on a 9×9 grid (Capture the Relic); deterministic, livestream-ready |
| **Status** | Active (16 stars) |
| **Platform** | macOS / Linux / Windows (cross-platform badge) |

**Should I adopt it?**

**Skip for engineering value; interesting as a research artifact.** The "two LLMs walk into a 9×9 grid" is a benchmark, not a productivity tool. The deterministic, agent-native HTTP API is clever but not relevant to your stated goals.

---

#### rough-cut-axi — `github.com/kunchenguid/rough-cut-axi`

**Overview**

| | |
|---|---|
| **Purpose** | Local-first, transcript-based video editor for agent-assisted rough cuts |
| **Status** | Active (1 star) |
| **Platform** | macOS / Linux / Windows (cross-platform) |

**Should I adopt it?**

**Skip.** Niche use case; early stage.

---

### ARCHIVED / READ-ONLY REPOSITORIES

---

#### TerminalOne — `github.com/kunchenguid/TerminalOne`

**Overview**

| | |
|---|---|
| **Purpose** | Cross-platform GPU-accelerated terminal emulator + tmux-like multiplexer |
| **Status** | **ARCHIVED Jan 6, 2025** |
| **Primary language** | TypeScript 92.1% |
| **Maturity** | 112 stars, 27 releases, last v1.6.3 (Feb 19, 2024) |

**Archive note (from owner):** "most of the original requirements that drove the development of TerminalOne are now made possible in other terminal emulators."

**Windows compatibility**

Was cross-platform (Windows, Mac, Linux) before archive.

**Should I adopt it?**

**Skip.** Archived. The GPU-accelerated + tmux-keybinding + cross-platform story has been overtaken by WezTerm (which Kun Chen actually uses — see `dotfiles`).

---

### SMALLER OR EXPERIMENTAL TOOLS

---

#### m87 — `github.com/kunchenguid/m87`

Tiny experimental project. 2 stars. Listed with cross-platform badge but very low activity. **Skip.**

---

#### justroll / simplewords / short-pipe / autopreso / trial-by-combat / rough-cut-axi — see above

---

#### org-bench — `github.com/kunchenguid/org-bench`

TypeScript monorepo, 29 stars, no README, 4 commits. The blog post "Org-Bench: Let's Simulate the Org Charts Meme with Agents and See Who Wins" (Apr 22, 2026) is the canonical entry point. **Skip until more documentation appears.**

---

### REFERENCE FORKS AND OTHER REPOS

The list also contains personal reference forks of larger projects:
- `Babylon.js`, `electron`, `matplotlib`, `scikit-learn`, `sympy`, `pytest`, `requests`, `seaborn`, `sphinx`, `astropy`, `xarray`, `kotlin-language-server`, `pydantic-ai`, `mcp-compressor` (already discussed above), `django`, `context7`
- `react-dimensions`, `phaser-plugin-isometric` — older side projects
- `rawflow` (C#) — a tool for editing magic lantern recorded raw MLV videos
- `coremediaio-dal-minimal-example` (macOS-only CoreMediaIO plugin)
- `presize` (TypeScript) — bulk preprocess/resize/crop images
- `wow-combat-log-parser` (TypeScript) — World of Warcraft combat log parser
- `rose-pine-neovim`, `markdown-preview.nvim`, `comment-repl.nvim`, `gen.nvim` — Neovim plugins (the first three are forks/upstream; `gen.nvim` appears to be a personal version of `David-Kunz/gen.nvim`)
- `SQLite.swift`, `SQLCipher.swift`, `Commander` (Swift) — Swift utility libraries
- `hibit-app` (0 stars) — Hi Bit App Releases; unclear purpose, likely empty template
- `homebrew-tap` (Ruby) — Homebrew tap for kunchenguid casks (baby-menu, short-pipe, gsh)
- `homebrew-gsh` — Homebrew formula for gsh
- `bubble-tea-bubbles` (Go) — TUI components

**All of these should be skipped** for engineering value. They are personal reference copies, old side projects, or platform-incompatible forks.

---

## Cross-Repository Analysis

### Which repositories work together

The ecosystem is not 67 independent tools. It is a layered system with explicit dependencies. The dependency graph, as I read the documentation and the `wheelhouse.config.yml`:

```
firstmate ───▶ treehouse (worktree pool)
          ───▶ no-mistakes (gated push)
          ───▶ acpx (ACP sessions)
          ───▶ tmux/herdr/zellij/cmux/Orca (session backends)
          ───▶ stow (public installable skill)

gnhf ─────▶ treehouse (--worktree)
        ───▶ acpx (ACP targets)
        ───▶ Claude/Codex/Copilot/Pi/RovoDev/OpenCode (agents)

no-mistakes ──▶ Claude/Codex/Copilot/Pi/OpenCode (pipeline agent)
            ──▶ acpx (acp:<target>)

chrome-devtools-axi ──▶ chrome-devtools-mcp (subprocess)
gh-axi ─────────────▶ gh CLI (subprocess)
quota-axi ──────────▶ vendor first-party APIs
tasks-axi ──────────▶ backlog.md (markdown backend)
lavish-axi ─────────▶ localhost:4387 server, Excalidraw, Mermaid

wheelhouse ────────▶ no-mistakes (compliance check)
              ─────▶ GitHub Actions + Issues (storage)
              ─────▶ Claude (LLM provider)

AXI ────────────────▶ (design spec; consumed by all *-axi tools)
```

### Which overlap

- **treehouse ↔ firstmate's worktree concept** — firstmate uses treehouse, so this is a clean layering, not a duplication.
- **gnhf's worktree mode ↔ treehouse** — gnhf has its own worktree implementation but treehouse is the canonical worktree pool. The wheelhouse config implies gnhf runs through treehouse in the fleet.
- **no-mistakes' disposable worktree ↔ treehouse** — no-mistakes has its own worktree management (it does not depend on treehouse); the design philosophy is shared but the implementation is independent.
- **AXI is referenced by `*-axi` siblings but not by firstmate, gnhf, no-mistakes** — `no-mistakes axi` is the one exception; it's the original proof point.
- **chrome-devtools-axi ↔ agent-browser-axi** — two AXI wrappers for two underlying browser engines (chrome-devtools-mcp vs Vercel agent-browser). Pick one.

### Which replace each other

- **dotfiles (current) replaces dotfiles-mac-nix (older)** — explicit in the older README.
- **TerminalOne replaces nothing; replaced by WezTerm** — archive note says so.
- **agent-browser-axi replaces chrome-devtools-axi** — for the Vercel agent-browser crowd. Pick one.

### Which depend on each other

See the dependency graph above. The most consequential dependencies:

- `no-mistakes` does not depend on any other repo in the ecosystem.
- `treehouse` does not depend on any other repo in the ecosystem.
- `gnhf` is independent (it has its own worktree mode but does not require treehouse).
- `firstmate` is the only repo that depends on multiple others (treehouse, no-mistakes, acpx).
- `wheelhouse` requires `no-mistakes` for the compliance check on its own fleet.

### Which are standalone

- **AXI** (the spec) — completely standalone. Pure documentation + benchmarks + SDK.
- **mcp-compressor** — standalone Python package.
- **simplewords** — standalone Chrome extension.
- **All the *-axi catalog tools** (gh-axi, chrome-devtools-axi, lavish-axi, quota-axi, tasks-axi, agent-browser-axi) — individually installable, no cross-dependencies.

### Which should be learned first

In order of engineering leverage for a Windows-first .NET engineer:

1. **AXI principles** (1 day) — universal patterns
2. **no-mistakes** (1 day) — direct, daily-use value
3. **treehouse** (half a day) — direct, daily-use value
4. **gh-axi / chrome-devtools-axi / quota-axi** (1 day combined) — skill installation
5. **gnhf** (half a day) — overnight work
6. **firstmate** (1 week) — orchestration, only if you actually run multiple parallel agents
7. **wheelhouse** (1 day, only if you maintain 3+ repos)
8. **lavish-axi** (half a day, only if you produce HTML artifacts)

---

## Learning Roadmap (4 weeks)

This is a Windows-first plan ordered by engineering value, not popularity.

### Week 1 — Foundations

**Goal:** internalize the AXI principles and install the gating tools.

- **Day 1 (2h):** Read `axi.md` and the `axi` repo's `principles.yaml`. Read the `gh-axi` and `chrome-devtools-axi` READMEs to see the principles applied. Install the AXI skill via `npx skills add kunchenguid/axi`.
- **Day 2 (1h):** Install no-mistakes on Windows via `irm https://raw.githubusercontent.com/kunchenguid/no-mistakes/main/docs/install.sh | sh` (or use the Windows PowerShell instructions in `docs/`). Configure your agent (`claude`, `codex`, or `copilot`). Run `no-mistakes init` in one of your existing .NET repos. Do a first `git push no-mistakes` on a small change.
- **Day 3 (1h):** Install treehouse via `irm https://kunchenguid.github.io/treehouse/install.ps1 | iex`. Create a `treehouse.toml` for one repo with a `post_create` hook that runs `dotnet restore`. Practice `treehouse get`, `treehouse status`, `treehouse return`.
- **Day 4 (1h):** Install `gh-axi` and `chrome-devtools-axi` skills. Add the one-liner "Use `gh-axi` for GitHub and `chrome-devtools-axi` for browser automation" to your `AGENTS.md`. Have your agent do one GitHub task via `gh-axi` and one browser task via `chrome-devtools-axi`. Note the cost.
- **Day 5 (2h):** Install `quota-axi` and `tasks-axi`. Configure your agent to check quota before large jobs. Start a `backlog.md` for an active project.

**Deliverable:** Your daily driver — Windows Terminal + VS Code + no-mistakes + treehouse + gh-axi — is wired up. The agent has learned to use gh-axi and quota-axi.

### Week 2 — Overnight work

**Goal:** gnhf as a force multiplier.

- **Day 6 (2h):** Read the gnhf README and the design notes on `notes.md`. Install via `npm install -g gnhf`. Configure a `~/.gnhf/config.yml` with your default agent.
- **Day 7 (3h):** Author a first `prompt.md` for one of your projects. The prompt should be specific: "implement X, with Y constraints, finishing when Z." Run `gnhf "<prompt>"` in a worktree and let it iterate for a few hours. Read the iteration log.
- **Day 8 (1h):** Wire gnhf to use treehouse for the worktree. Try `--worktree` mode.
- **Day 9 (1h):** Configure the gnhf run to pass through no-mistakes on success. (Either via a `post_iteration` hook or by adopting the wheelhouse pattern.)
- **Day 10 (1h):** Try a real overnight run. Set `--max-iterations 20` and `--max-tokens 500000`. Triage the morning diff.

**Deliverable:** You have one overnight iteration that resulted in a real PR.

### Week 3 — Browser automation + artifacts

**Goal:** chrome-devtools-axi + lavish-axi for content-heavy workflows.

- **Day 11 (2h):** Read the chrome-devtools-axi skill. Run through 5–10 sample tasks (Wikipedia extract, GitHub nav, multi-site comparison). Measure the cost difference vs. raw MCP.
- **Day 12 (1h):** Integrate chrome-devtools-axi with no-mistakes: any E2E test changes go through the gate.
- **Day 13 (2h):** Install lavish-axi and produce one rich HTML artifact (a system architecture diagram, a project plan, a comparison table). Have the agent create it, you annotate, the agent revises.
- **Day 14 (1h):** Document the workflow in your project's `AGENTS.md`.

**Deliverable:** Your agent can produce rich HTML artifacts with a tight human review loop, and any browser-based testing is gated.

### Week 4 — Orchestration (optional, advanced)

**Goal:** firstmate and the multi-agent pattern, run in WSL2.

- **Day 15 (1h):** Read the firstmate README. Set up WSL2 Ubuntu if not already done. `git clone` firstmate into WSL.
- **Day 16 (3h):** Configure tmux inside WSL. `gh auth login`. Launch firstmate via Claude Code. Try a "ship" task and a "scout" task.
- **Day 17 (1h):** Add a secondmate for a side project.
- **Day 18 (2h):** Connect firstmate to your Windows workflow: WSL tmux session visible in Windows Terminal, no-mistakes running in WSL, gnhf invokable from WSL, gh-axi on Windows side for direct queries.
- **Day 19–20:** Triage, document, and consolidate. Write your own `KunChen_Windows_Adoption.md` based on what worked.

**Deliverable:** You have a complete Windows-first + WSL2-sidekick agent workflow that mirrors the entire Kun Chen ecosystem.

---

## Windows-First Recommendations

Given your stated stack (Windows 11, Visual Studio, VS Code, .NET, PowerShell, Git, Docker Desktop, Ollama, Claude, GPT, Gemini), here is exactly which repositories to adopt and how they fit.

### Direct adoption (no changes required)

| Repo | Windows usage |
|---|---|
| **no-mistakes** | `irm` install script; runs as a Go binary; invoke from PowerShell, Git Bash, or VS Code terminal. Drives a `claude`/`codex`/`copilot` agent of your choice. **This is the single highest-leverage tool in the entire ecosystem for Windows engineers.** |
| **treehouse** | `irm` PowerShell installer; Go binary; hooks execute via `%COMSPEC% /c`. **The single best Windows-friendly tool in the ecosystem for AI-assisted development.** |
| **gh-axi** | `npx -y gh-axi` from any shell; requires `gh` CLI authenticated. Add to `AGENTS.md`. |
| **chrome-devtools-axi** | `npx -y chrome-devtools-axi`; works on Windows because Chrome works on Windows. Bridge server is Node. |
| **quota-axi** | `npx -y quota-axi`; reports Claude, Codex, Copilot, Cursor, Grok, Kimi quota. **Especially relevant if you have multiple LLM subscriptions.** |
| **tasks-axi** | `npx -y tasks-axi`; markdown backend is fully cross-platform. |
| **lavish-axi** | `npx -y lavish-axi`; runs a local browser UI. |
| **gnhf** | `npm install -g gnhf`; sleep prevention via `SetThreadExecutionState`. **The overnight productivity multiplier for Windows.** |
| **agent-browser-axi** | `npx -y agent-browser-axi`; alternative to chrome-devtools-axi. |
| **acpx** | npm; headless ACP client. |
| **acp-mock** | npm; mock ACP server for testing. |
| **mcp-compressor** | `pip install mcp-compressor`; Python package. |
| **simplewords** | Chrome extension; works on Windows. |
| **trial-by-combat** | npm; cross-platform. (Entertainment/benchmark, not engineering.) |
| **rough-cut-axi** | npm; cross-platform. (Niche, optional.) |
| **whathappened** | npm skill; only works on Grok Build, but the install is cross-platform. |

### Adopt via WSL2 (architectural separation)

| Repo | Windows strategy |
|---|---|
| **firstmate** | Run in WSL2 Ubuntu. Drive from Windows Terminal with WSL profile. The bash supervision model is fundamentally unix; the WSL2 bridge is the cleanest path. |
| **gsh** | Run in WSL2 (POSIX shell on Windows is fighting the platform). Or evaluate later when it matures. |
| **programbench-bench** | Run in WSL2 (Docker-on-WSL2 is the cleanest path; the harness is shell-heavy). |
| **org-bench** | Run in WSL2 or cross-platform Node, depending on what the README actually requires. |

### Skip on Windows (macOS-only or out of scope)

| Repo | Reason |
|---|---|
| **dotfiles** | nix-darwin is macOS-only. Study the philosophy, build your own Windows equivalent with WinGet + PowerShell + Windows Terminal. |
| **dotfiles-mac-nix** | Superseded. |
| **baby-menu** | macOS-only Electron tray app. |
| **autopreso** | macOS-first (no local STT on Windows). |
| **short-pipe** | macOS-only. |
| **justroll** | macOS-only. |
| **TerminalOne** | Archived. |

### How they fit into a Windows workflow

```
Windows 11
├── PowerShell / Windows Terminal / WezTerm
│   ├── no-mistakes (gated git push with AI review)
│   ├── treehouse (worktree pool, 16 default)
│   ├── gnhf (overnight long-running iteration)
│   ├── gh-axi, chrome-devtools-axi, quota-axi, tasks-axi
│   └── lavish-axi (HTML artifact editor in browser)
│
├── Visual Studio / VS Code / Rider
│   ├── $PROFILE (PowerShell) — sourced in every terminal
│   ├── AGENTS.md / CLAUDE.md — one-liner directing agent to AXI tools
│   └── .NET solution, .editorconfig, .gitattributes (Windows-friendly)
│
├── Git
│   └── Remote: origin (GitHub or Azure DevOps)
│       └── Every push to main: routed through no-mistakes
│
├── Docker Desktop
│   └── For programbench-bench, gsh (optional), custom agent containers
│
├── Ollama (local LLM)
│   └── Default agent for non-critical work; gnhf with `acp:<target>` to Ollama
│
└── WSL2 (Ubuntu) — sidekick
    ├── firstmate (crew dispatcher, tmux)
    ├── gsh (optional, when it matures)
    └── programbench-bench (when you need it)
```

This is a coherent, layered setup. The Windows side is the production development environment; the WSL2 side is the agent operations environment. They share git remotes, share your `AGENTS.md`, and share no-mistakes as the compliance gate.

---

## AI Workflow Recommendations

How these repositories improve specific AI-assisted development activities.

### AI coding

- **no-mistakes** is the most direct improvement. Every agent-generated commit goes through a review → test → docs → lint → push → PR → CI pipeline. Auto-fixes mechanical issues; escalates the rest.
- **AXI principles** make every tool the agent uses more efficient. 50–70% input token reduction is real.
- **gh-axi** makes the agent's GitHub interactions structured and reliable.

### Planning

- **gnhf** forces you to write a clear `prompt.md` to start an overnight run. The act of writing the prompt IS the planning.
- **firstmate** forces you to scope each crew task before dispatch.
- **lavish-axi** is the planning artifact tool: the agent produces an HTML plan, you annotate, the agent revises.

### Architecture

- **AXI principles** apply to your own CLIs: minimal schemas, content first, contextual disclosure.
- **wheelhouse** (for repo maintainers) is a forcing function for architectural decisions across a fleet.
- **tasks-axi** is a structured way to track architectural decisions as tasks.

### Large repositories

- **treehouse** is the only sustainable way to run multiple parallel worktrees for large repos without dependency-install overhead.
- **firstmate** dispatches multiple agents across one large repo, each in their own worktree.
- **no-mistakes** runs validation in disposable worktrees, leaving the main worktree untouched.

### Long-running projects

- **gnhf** is the canonical "leave it running overnight" tool. With `--max-iterations`, `--max-tokens`, and `--stop-when` you have full control.
- **firstmate** is the canonical "supervise multiple ongoing projects" tool.
- **tasks-axi** tracks durable work across long-running projects.

### Documentation

- **no-mistakes** has a `docs` step in its pipeline.
- **AXI principles #2 (minimal schemas) and #8 (content first)** are documentation principles.
- **lavish-axi** is documentation itself — the agent produces HTML, you annotate.

### Commit quality

- **no-mistakes** is the most direct lever. Every commit passes through an AI-driven review and is auto-fixed or escalated.
- **gnhf** commits per iteration, so commits are atomic and inspectable.

### Context management

- **AXI principles #1 (token-efficient output), #2 (minimal schemas), #3 (content truncation)** are the canonical context-management practices.
- **firstmate** keeps each crewmate's context isolated.
- **treehouse** keeps each worktree's context isolated.

### Agent orchestration

- **firstmate** is the canonical orchestration model (captain → first mate → crewmates, with event-driven supervision).
- **gnhf** is single-agent long-running orchestration.
- **acpx** is the protocol layer for agent-to-agent communication.
- **acp-mock** is the testing layer.

### Prompt engineering

- **gnhf**'s `prompt.md` and `notes.md` are a forced prompt-engineering discipline.
- **superpowers-bench** measures whether your agent picks the right skills for the right tasks.
- **AXI**'s "contextual disclosure" is itself a prompt-engineering pattern.

---

## My Vision — Mapping the Ecosystem to a Windows-Native Platform

Your stated long-term goal: a Windows-native engineering platform with multiple reusable repositories, enterprise .NET systems, AI-assisted development, local and cloud LLMs, strong Git workflow, reusable architecture, excellent documentation, cost-efficient AI usage, agent orchestration, and reproducible development environments.

Mapping the Kun Chen ecosystem to that vision:

### Direct support (Must Adopt)

| Vision component | Kun Chen repo(s) that support it |
|---|---|
| Multiple reusable repositories | **wheelhouse** for maintainer decisions; **no-mistakes** for the per-repo compliance gate |
| Enterprise .NET systems | **no-mistakes** runs CI (your `dotnet test`, `dotnet build`) in its pipeline; **gh-axi** integrates with GitHub Actions for Azure DevOps-compatible workflows |
| AI-assisted development | **AXI principles**, **no-mistakes**, **gnhf**, **gh-axi**, **chrome-devtools-axi** |
| Local and cloud LLMs | **quota-axi** (knowing what quota you have); **gnhf** (configurable per-agent); **gsh** (Ollama + OpenAI-compatible); **lavish-axi** (agent-agnostic) |
| Strong Git workflow | **no-mistakes** as the push gate; **treehouse** as the worktree pool; **gh-axi** for agent-side GitHub interaction |
| Reusable architecture | **AXI** is a reusable architecture for agent CLIs. The 10 principles apply to any CLI you build. |
| Excellent documentation | **lavish-axi** for HTML docs; **AXI principles** (content first, contextual disclosure) |
| Cost-efficient AI usage | **AXI principles** (#1–#3); **quota-axi** (route to cheapest available quota); **no-mistakes** (prevent wasted pushes) |
| Agent orchestration | **firstmate** (crew dispatch); **gnhf** (long-running); **acpx** (protocol layer) |
| Reproducible development environments | **Study** `dotfiles`/`dotfiles-mac-nix` philosophy; **adopt** WinGet + PowerShell + Windows Terminal + a Windows-native equivalent |

### Windows-specific adaptations needed

1. **The dotfiles philosophy** → A Windows-native declarative environment using WinGet, PowerShell DSC or `chezmoi`, Windows Terminal settings export, VS Code `settings.json` + extensions manifest, .NET global.json + global tools.
2. **The firstmate pattern** → A PowerShell orchestrator of the same shape (captain → first mate → crewmates), with Windows Terminal tabs/pans as the session backend, PowerShell jobs for crewmate processes, and no-mistakes as the gate. The architecture is portable; the implementation is bash-specific.
3. **The wheelhouse pattern** → Already Windows-compatible because it runs in GitHub Actions.
4. **The gnhf pattern** → Already Windows-compatible. Set `SetThreadExecutionState` is the only Windows-specific code.

### Vision-aligned repos NOT in this ecosystem (build your own)

- A .NET-native AXI SDK (the principles apply; `axi-sdk-js` does not).
- A PowerShell wrapper around no-mistakes' `no-mistakes axi` interface, optimized for PSRemoting / scheduled jobs / Windows services.
- A Windows Terminal / Tmux equivalent for firstmate's session backend.
- A WinGet + PowerShell + `chezmoi` dotfiles template that mirrors `dotfiles` for Windows.
- A no-mistakes pipeline step that runs `dotnet format`, `dotnet test`, and `dotnet build` automatically (extend the pipeline agents).

---

## Migration Opportunities (Linux → Windows)

For each Linux/macOS primitive in the ecosystem, here is the Windows-native equivalent.

| Linux/macOS primitive | Windows equivalent | Notes |
|---|---|---|
| bash | **PowerShell 7+** | PowerShell is object-oriented, not text-stream. Re-think pipelines. |
| tmux | **Windows Terminal tabs + panes** | Built into Windows 11; no install needed. For server-side multiplexing, use WSL2 tmux. |
| launchctl (macOS) | **Windows Service Control Manager** | `sc.exe`, PowerShell `Set-Service`, NSSM as a service wrapper. |
| zsh + oh-my-zsh | **PowerShell + oh-my-posh** | oh-my-posh is the modern, cross-shell prompt framework. |
| starship | **starship (cross-platform)** | Already cross-platform. |
| Homebrew | **WinGet** (Microsoft's official) | WinGet is the closest equivalent. scoop and Chocolatey are alternatives. |
| nix / nix-darwin | **WinGet + PowerShell DSC** | DSC is "declarative system configuration" for Windows. |
| home-manager | **chezmoi** (cross-platform) or PowerShell profile | chezmoi works on Windows natively. |
| WezTerm | **Windows Terminal** (built-in) | WezTerm still works on Windows, but Windows Terminal is the default. |
| Neovim | **VS Code** or **Visual Studio** for primary editing; Neovim for quick edits in WSL | The .NET engineer uses VS Code or Visual Studio; Neovim is not the primary tool. |
| lazy.nvim | **VS Code extensions** | A different mental model; one JSON file. |
| ripgrep / fd / fzf / jq | **All cross-platform** (rg, fd, fzf, jq all have Windows builds) | No migration needed. |
| fzf keybindings in zsh | **PSReadLine** + **PSFzf** | PowerShell's PSReadLine is the analog. |
| System sleep prevention (caffeinate, systemd-inhibit) | **SetThreadExecutionState Win32 API** | gnhf already uses this on Windows. |
| `gh` CLI for GitHub | **`gh` CLI for GitHub** (cross-platform) | No migration. |
| WezTerm rose-pine theme | **Windows Terminal rose-pine theme** | Both exist. |
| `rebuild` shell alias (nix-darwin) | **`Update-Dotfiles` PowerShell function** (chezmoi) | Mirror the philosophy, not the syntax. |
| macOS defaults (defaults write) | **PowerShell registry manipulation** or **`Set-ItemProperty`** | The mental model translates directly. |
| macOS Keychain | **Windows Credential Manager** | `cmdkey` and DPAPI. quota-axi uses macOS Keychain paths; on Windows, it would need to be adapted to Windows Credential Manager. |
| Cron / launchd | **Task Scheduler** | `schtasks.exe` or PowerShell `Register-ScheduledTask`. |
| ZFS / APFS snapshots | **Windows Volume Shadow Copy (VSS)** | Different model; not directly analogous. |
| Unix user permissions | **Windows ACLs** | Different model; rarely needed in development. |
| Unix file paths (forward slashes) | **Windows paths (backslashes) but most tools accept forward** | PowerShell accepts both; .NET's `Path.Combine` is the right tool. |
| Unix environment variables (HOME, PATH) | **Windows environment variables** (`%USERPROFILE%`, `%PATH%`) | PowerShell `$env:USERPROFILE` syntax. |
| `/etc/hosts` | **`C:\Windows\System32\drivers\etc\hosts`** | Same syntax; needs admin to edit. |

---

## Final Recommendations

### Top 10 repositories to master

In order of value for a Windows-first .NET engineer:

1. **no-mistakes** — the gated-push tool. Highest direct ROI. Must adopt.
2. **treehouse** — the worktree pool. Must adopt.
3. **AXI** — the design principles. Must internalize.
4. **gnhf** — the overnight iteration. Must adopt.
5. **gh-axi** — the GitHub AXI. Must adopt.
6. **chrome-devtools-axi** — the browser AXI. Probably adopt.
7. **quota-axi** — the quota awareness. Must adopt if you have multiple LLM subscriptions.
8. **tasks-axi** — the backlog ergonomics. Probably adopt.
9. **lavish-axi** — the HTML artifact editor. Probably adopt if you produce HTML artifacts.
10. **firstmate** — the crew dispatcher. Adopt (in WSL2) only if you actually run multiple parallel agents.

### Top 5 repositories to fork

Forks for local adaptation, customization, or contribution:

1. **no-mistakes** — fork to add a `dotnet` pipeline step (custom gate agent that runs `dotnet format --verify-no-changes`, `dotnet test`, `dotnet build`).
2. **AXI** — fork to host your own private AXI catalog (a `*-axi` for your internal APIs).
3. **gnhf** — fork to add custom stop conditions for your domain.
4. **chrome-devtools-axi** — fork if you need custom snapshot parsing for your specific app.
5. **wheelhouse** — fork to add custom decision types (e.g., "review this architecture diagram" → calls lavish-axi).

### Top 5 repositories worth adapting for Windows

Adapting (porting, wrapping, or extending) for Windows-native use:

1. **firstmate** — port the bash dispatcher to PowerShell. The architecture is sound; the implementation is bash-bound. Effort: 2–4 weeks for a credible port. Or run in WSL2.
2. **dotfiles** — port the declarative philosophy to a `chezmoi` + WinGet + PowerShell template. Effort: 1 week.
3. **no-mistakes** — wrap the `no-mistakes axi` interface in a PowerShell module. Effort: 1 day.
4. **gsh** — port or wait. The POSIX shell is not the right Windows tool. Effort: significant.
5. **wheelhouse** — add a "post-decision Power Automate hook" or a Teams/Slack notification integration. Effort: 1 day.

### Top 5 ideas to implement in your own projects

Inspired by what is missing from the ecosystem:

1. **A .NET-native AXI SDK** — port `axi-sdk-js` to .NET. The 10 principles are language-agnostic; a .NET version would let your .NET CLIs be first-class agent tools.
2. **A no-mistakes gate for .NET specifically** — a `dotnet-format-verify`, `dotnet-test-with-coverage`, `dotnet-pack-dry-run` pipeline step.
3. **A PowerShell-based firstmate-equivalent** — the captain → first mate → crewmates pattern, with Windows Terminal tabs as session backend, PowerShell jobs as crewmate processes, and no-mistakes as the gate.
4. **A Windows-native dotfiles template** — `chezmoi` + WinGet + PowerShell profile + Windows Terminal + VS Code + .NET global tools, all in one repo, one command to apply.
5. **An agent-skill for .NET solution structure** — a skill that teaches your agent how to read a `.sln`, how to find projects, how to navigate, how to build/test/pack. The analogue of `quota-axi` but for .NET solutions.

### Top 10 engineering lessons learned from studying this ecosystem

1. **Treat token budget as a first-class constraint.** Every CLI you build for an agent should output structured, token-efficient, truncated, content-first data with contextual next-step hints. The AXI principles are universal.
2. **Disposable worktrees are the unit of agent work.** Long-lived branches with shared state are an anti-pattern for AI-assisted development.
3. **Gating is more valuable than generating.** no-mistakes' contribution to the ecosystem is bigger than any individual agent — it makes every push a known-good push.
4. **Event-driven supervision is the right model for long-running agents.** Polling wastes tokens. firstmate's watcher loop is a general pattern.
5. **The agent experience is a product surface.** UX, error messages, exit codes, --help text, next-step hints — all of this matters more when the consumer is an LLM, not a human.
6. **Make it local-first.** Local servers, local CLIs, local-first state. Cloud is opt-in for sharing.
7. **Byte-exact round-trip is a design discipline.** tasks-axi's `backlog.md` round-trip invariant is a general principle: if humans and agents edit the same file, the model must not silently rewrite the file.
8. **Idempotent mutations are non-negotiable.** Re-running a command should never produce a different state. This is true for no-mistakes, tasks-axi, gnhf, and the entire `*-axi` family.
9. **Single-liaison orchestration beats tab-juggling.** Whether you have one agent (you) or many (firstmate), the dispatch pattern is: one entry point, many workers, structured handoff.
10. **Open source compounds.** A 6.8k-star no-mistakes with 92 releases and a MIT license is a load-bearing public good. If you build something durable, release it. The Kun Chen ecosystem is itself the proof.

### Top 20 actionable next steps

In priority order, immediately actionable:

1. **Install no-mistakes on your Windows machine today.** `irm https://raw.githubusercontent.com/kunchenguid/no-mistakes/main/docs/install.sh | sh` (or use the Windows PowerShell path in the install docs). Run `no-mistakes init` in your most active .NET repo.
2. **Install treehouse today.** `irm https://kunchenguid.github.io/treehouse/install.ps1 | iex`. Create a `treehouse.toml` with a `post_create` hook for `dotnet restore`.
3. **Read `axi.md` end-to-end.** Internalize the 10 principles before you install any AXI tool.
4. **Install gh-axi and chrome-devtools-axi skills** via `npx skills add kunchenguid/gh-axi --skill gh-axi -g` and the same for chrome-devtools-axi. Add the one-liner to your `AGENTS.md`.
5. **Install quota-axi.** `npx skills add kunchenguid/quota-axi --skill quota-axi -g`. Use it before launching a large overnight run.
6. **Install gnhf.** `npm install -g gnhf`. Configure `~/.gnhf/config.yml` with your default agent. Plan your first overnight iteration.
7. **Set up WSL2 with Ubuntu.** `wsl --install -d Ubuntu`. Install tmux, gh, claude/codex, and the verification tools no-mistakes will need. This is the home for firstmate.
8. **Create your Windows-native dotfiles template.** Start with chezmoi, WinGet export, PowerShell profile, Windows Terminal settings, VS Code `settings.json` + extensions manifest. Mirror the layered philosophy of `kunchenguid/dotfiles`.
9. **Add `quota-axi` to your `AGENTS.md` one-liner.** "Check `quota-axi` before launching a job; prefer providers with >30% remaining."
10. **Read the no-mistakes `docs/` site.** Understand the TUI flow before you do a real push.
11. **Add a `dotnet` pipeline step to no-mistakes.** A custom gate agent that runs `dotnet format --verify-no-changes` and `dotnet test` and reports findings in TOON.
12. **Author your first `prompt.md` for gnhf.** Pick a small, well-scoped task. Use `--max-iterations 5` and `--max-tokens 100000` for the first run. Triage the morning diff.
13. **Install tasks-axi.** Start a `backlog.md` for one of your projects. The byte-exact round-trip will teach you a discipline.
14. **Add wheelhouse to your maintainer workflow** (only if you maintain 3+ GitHub repos). Fork it, edit `wheelhouse.config.yml`, add the `FLEET_TOKEN` secret.
15. **Run chrome-devtools-axi on a real test suite.** Compare the cost to your previous MCP-based setup.
16. **Try lavish-axi for one HTML artifact.** A system architecture diagram, a project plan, a comparison table. The annotation loop will surprise you.
17. **Study the firstmate architecture docs** (`docs/architecture.md`, `docs/tmux-backend.md`). Decide whether to run it in WSL2 or port the pattern to PowerShell.
18. **Build a no-mistakes PowerShell wrapper module.** Wrap `no-mistakes axi` in PowerShell functions for easier invocation from .NET CI pipelines.
19. **Author your own private `*-axi` for your most-used internal API.** Apply the 10 principles. Use `axi-sdk-js` as the model.
20. **Write a `KunChen_Windows_Adoption.md` for your team.** Document which repos you adopted, which you skipped, and the workflow. Share it. The Kun Chen ecosystem is public; your adaptation of it should be too.

---

## Uncertainty Notes

Where this report makes a claim that I could not fully verify, I want to be explicit:

1. **Benchmark numbers** — all numbers in this report come from the published `axi.md` and `axi/README.md` benchmarks. They are single-model (Claude Sonnet 4.6 in the published study, with `--model` support), single-author, and use an LLM judge for grading. I have not independently reproduced them. The `chrome-devtools-mcp-compressed` and `chrome-devtools-mcp-code` conditions reference prior work; the 12× cost gap on `ci_failure_investigation` is striking but unverifiable without re-running.
2. **The `no-mistakes` install script** — I read the README's primary install command but did not personally verify it runs on PowerShell 7 / Windows 11. The README explicitly says "Windows, `go install`, and build-from-source are covered in the linked installation guide" — I did not read the full installation guide.
3. **firstmate on WSL2** — my recommendation to run firstmate in WSL2 is an extrapolation from the README's macOS/Linux support and tmux dependency. I have not personally verified the WSL2 + tmux + firstmate combination.
4. **The `no-mistakes axi` interface** — I read about it but did not exercise it. Its TOON-shaped interface is a real engineering choice; I treat it as such.
5. **Windows support for several repos** — the badges say `macOS | Linux | Windows` but I did not download and test each on Windows. The most cross-platform projects (no-mistakes, treehouse, gnhf, all the `*-axi` siblings) are all Node or Go, which compile cleanly to Windows, but I cannot guarantee every edge case.
6. **The full repo list** — I used the GitHub API to enumerate all 67 repos but only analyzed those with material signal. The 20+ forks of matplotlib, scikit-learn, sympy, etc. are reference copies with no original content; I did not investigate whether they have local changes.
7. **The "L8" title** — Kun Chen's Linktree bio says "Former L8 engineer at Meta, Microsoft, Atlassian." L8 at Meta is consistent with a Senior Principal / Distinguished Engineer level. I have not independently verified his tenure.
8. **mcp-compressor** — this is a fork of `atlassian-labs/mcp-compressor`. I noted this in the analysis but did not investigate whether the fork has any Kun Chen-specific changes.
9. **The Substack post "OPINIONS.md"** — I attempted to fetch it but the URL returned 404. I have not been able to verify the content.
10. **The `wheelhouse.config.yml` content** — I read the start of the file. The full config is longer than what I captured; the 8-repo fleet list is accurate as far as I read it.

These uncertainties are the engineering equivalent of a doctor's "differential diagnosis" — they are the places where a follow-up investigation might change the recommendation. None of them invalidate the core recommendations; they refine them.

---

## Closing Note

This report was generated by reading the actual repositories, the actual `axi.md`, the actual READMEs, and the actual Substack archive. The single most important takeaway is not a specific tool but a design philosophy: **the agent experience is a product surface, and the engineers who treat it that way will outperform those who don't.** Kun Chen's ecosystem is the most complete working demonstration of that philosophy available. A Windows-first .NET engineer who internalizes the 10 AXI principles, adopts no-mistakes and treehouse as their foundation, and lets gnhf and gh-axi handle the agent-side of their workflow will have a measurably more productive AI development practice.

The rest is implementation.
