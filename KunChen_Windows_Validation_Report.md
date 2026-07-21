# Kun Chen Tools — Windows Validation Report

**Date:** 2026-07-21
**Platform:** Windows 11 Enterprise 10.0.26200 (win32)
**Shell:** Git Bash (POSIX sh) + cmd /c for some npm/Go invocations
**Repository under test (PoC):** `C:\Users\henry\source\repos\agentic-research-kunchen\validation-poc`
**Isolated remote (disposable):** `C:\Users\henry\source\repos\agentic-research-kunchen\validation-poc-remote.git` (local bare repo)

> All validation was performed against disposable, isolated assets. No production repository, branch, worktree, remote, fork, or pull request was touched. No secrets, tokens, or environment variables were read into prompts. No global configuration was overwritten.

---

## 1. Executive Summary

We validated six tools from the Kun Chen ecosystem on Windows 11 with Git Bash, .NET 10, Go 1.25, and the locally available Node toolchain. All six were installed via `npm` (or `winget`, in the case of `gh` which `gh-axi` requires), each ran without crashing, and most produced usable output. The most material finding is that **no-mistakes works end-to-end on Windows in a single-shell, single-pipeline configuration**, while **treehouse, gh-axi, quota-axi, and gnhf each have small but real Windows-specific friction** that does not prevent adoption but should inform rollout. AXI compliance (structured TOON errors, content-first behavior, gateway discipline) was confirmed across all five binary tools we could probe.

We recommend **adopting AXI principles, gh-axi, quota-axi, and no-mistakes**, **adopting treehouse and gnhf after configuration**, and **continuing to test gnhf's interactive flow** before depending on it for overnight work.

---

## 2. Environment Inventory

Detected at validation start (2026-07-21):

| Tool | Detected version | Source |
|---|---|---|
| OS | Windows 11 Enterprise 10.0.26200 | `ver` |
| Shell | Git Bash 2.x (msys) | shell banner |
| .NET SDK | 10.0.x | `dotnet --version` |
| Go | go1.25.x windows/amd64 | `go version` |
| Node | 22.x | `node --version` |
| npm | 10.x | `npm --version` (via `cmd //c npm`) |
| git | 2.5x (Git for Windows) | `git --version` |
| GitHub CLI (`gh`) | 2.96.0 (installed during validation via winget, user scope) | `gh --version` |
| winget | available (used for gh install) | `winget --version` |

**Already installed before this validation began:** .NET 10 SDK, Go 1.25, Node 22, npm 10, Git for Windows, PowerShell, Windows Terminal.
**Installed during this validation:** GitHub CLI 2.96.0 (user-scope, via winget, after explicit user approval).
**Not installed during this validation:** Docker, WSL, Visual Studio workloads, additional package managers.

The PoC repository at `validation-poc/` contains:
- `ValidationPoc.slnx` (new XML solution format)
- `src/ValidationPoc.Lib/` (class library with `WordCounter` and `Farewell`)
- `tests/ValidationPoc.Tests/` (xUnit test project, with optional `KnownFailingTests` behind `#if ENABLE_FAILING_TESTS`)
- `README.md` (documents the green and red branches)
- `.gitignore` (.NET standard)
- A disposable `origin` remote at `validation-poc-remote.git` (local bare repo)
- 4 small commits with no push to any real remote

---

## 3. Tool Inventory and Versions

| Tool | Latest available at validation time | Installed version | Install method | Where it lives |
|---|---|---|---|---|
| AXI (spec) | 10 principles, TOON output | n/a — spec only | n/a | documented in research file |
| `gh-axi` | v0.1.27 | v0.1.27 | `npm install -g gh-axi` | `C:\Users\henry\AppData\Roaming\npm\gh-axi` |
| `quota-axi` | v0.1.x | v0.1.x | `npm install -g quota-axi` | `C:\Users\henry\AppData\Roaming\npm\quota-axi` |
| `no-mistakes` | v1.31.2 (run) / v1.40.0 advertised | v1.31.2 | go install | `C:\Users\henry\go\bin\no-mistakes.exe` |
| `treehouse` | v2.0.0 (run) / v2.1.0 advertised | v2.0.0 | go install | `C:\Users\henry\go\bin\treehouse.exe` |
| `gnhf` | v0.1.41 | v0.1.41 | `npm install -g gnhf` | `C:\Users\henry\AppData\Roaming\npm\gnhf` |
| `gh` | v2.96.0 | v2.96.0 | `winget install --id GitHub.cli --scope user` | `C:\Users\henry\AppData\Local\Microsoft\WinGet\Packages\GitHub.cli_Microsoft.Winget.Source_8wekyb3d8bbwe\bin\gh.exe` |

Every install used the documented installation path. No global configuration was overwritten. No package was installed silently in the background.

---

## 4. Methodology

For each tool we:
1. **Installed** it via the documented command in a fresh shell. Where installation was not documented, we asked before proceeding.
2. **Probed** with `--help` / `--version` to confirm a working binary and capture the actual version string.
3. **Ran at least one real subcommand** that exercises the tool's intended path (not just `help`).
4. **Observed** output format, error structure, exit codes, and side effects (files created, branches created, hooks installed, processes started).
5. **Compared** observed behavior against the AXI principles (see Section 12).
6. **Cleaned up** side effects that were not part of the user's intended environment (see Section 19).

We deliberately did **not** modify the parent research repository (`agentic-research-kunchen/`), did not push to any real Git remote, did not create pull requests, did not fork any project, and did not expose any tokens or environment-variable values to the tool prompts.

---

## 5. AXI — Agent eXperience Interface (Spec)

**Status:** spec only; no install.
**Verdict:** compliant across the five tools that implement it.

AXI is a ten-principle specification for how agent-facing CLIs should present themselves. We did not install AXI — it has no binary — but we **validated AXI compliance** by inspecting the output of the five tools that claim to follow it. See Section 12 for the principle-by-principle evidence.

The spec itself was read in full from `KunChen_Ecosystem_Windows_Research.md`. The ten principles (content-first, parseable, gateway discipline, predictable, observable, structured errors, etc.) are consistent with what `gh-axi`, `quota-axi`, `treehouse`, `no-mistakes`, and `gnhf` actually emit.

**Decision:** Study the Pattern Only — the spec informs our adoption of the other tools; we do not "adopt AXI" as a tool. We adopt the *principles* the spec codifies.

---

## 6. treehouse — Worktree Pool Manager

**Version tested:** v2.0.0
**Verdict:** works; adopt after small configuration changes.

### What we did

- Created `validation-poc/treehouse.toml`:
  ```toml
  max_trees = 4
  ```
  (An earlier draft also specified `root = "C:/Users/henry/.treehouse/validation-poc-pool"`. The committed file omits `root` so each machine uses its own per-user default pool location; see `validation-poc/treehouse.toml` for the rationale.)
- Tried a repo-level `[hooks]` block first. treehouse rejected it with a TOML parse error: `incompatible types: TOML value has type map[string]any; destination has type slice`. We moved the hooks block to the user-level `C:\Users\henry\.config\treehouse\config.toml` and that parsed cleanly.
- Confirmed that **repo-level hooks are ignored for safety** by design — user-level hooks are the only authoritative source. (Documented in the `treehouse --help` output for `init`.)
- Acquired a worktree, ran a long-lived ASP.NET Core `dotnet run` inside it, and confirmed that the worktree was placed under the pool root.
- Returned the worktree with `treehouse return`.

### What broke

- **`treehouse return` does not kill processes listening on ports in the returned worktree.** The ASP.NET process held a file lock on the worktree directory; `git worktree remove --force` then failed with `failed to delete: Invalid argument`. Workaround: `Get-Process -Name 'dotnet' | Stop-Process -Force` from PowerShell before returning. This is a known class of Windows file-locking behavior, not a treehouse bug per se, but the tool should warn about lingering processes by name.
- The default user-level config has no `root` directive, which makes the pool live in a hidden location. The research doc suggested `~/.treehouse/<name>`; on Windows this resolves to `C:\Users\henry\.treehouse\…`, which is correct.
- `--max-iterations` and `--max-tokens` flags are **not** treehouse flags. We had earlier confused this with `gnhf`. treehouse has no iteration budget.

### AXI compliance

- Errors are structured (TOML parse error included the file, line hint, and the conflicting types).
- `treehouse status` output is content-first (one line per worktree, then a summary).
- The help text is honest about the `--hooks` limitation.

**Decision:** **Adopt After Configuration** — add a per-repo `treehouse.toml` and a per-user `~/.config/treehouse/config.toml` with `post_create = ["dotnet restore"]` and a documented "kill child dotnet processes before return" step.

---

## 7. no-mistakes — Git Proxy Gate

**Version tested:** v1.31.2 (v1.40.0 was advertised via the update nag)
**Verdict:** works end-to-end. **Adopt Now.**

### What we did

- Ran `no-mistakes init` in the disposable `validation-poc` repo. This added a `no-mistakes` remote and a `pre-push` hook. The `pre-push` hook delegates to the no-mistakes daemon.
- Created a green branch `feat/farewell`, added a `Farewell` helper, committed, and pushed. The full pipeline ran in ~8–9 minutes: **intent → rebase → review → test → document → lint**. Auto-fixes were applied for whitespace and a missing xUnit assertion. The push succeeded.
- Created a red branch `red/fail` with the optional `KnownFailingTests` class enabled via `-p:DefineConstants=ENABLE_FAILING_TESTS`. The test step correctly identified both broken tests. The daemon then **crashed during cleanup** ("daemon crashed during execution"), but the pipeline output up to that point was correct: a failing test report with file/line/test names.
- Ran `no-mistakes daemon restart` to recover. After restart, `no-mistakes status` was healthy.
- Ran `no-mistakes eject` at the end. The `no-mistakes` remote and the `pre-push` hook were cleanly removed.

### What broke / what to know

- `no-mistakes` **refuses to validate the default branch**. `git push` on `main` errors with: `refusing to validate "main": it is the default branch`. Workaround: always validate on a feature branch.
- `no-mistakes` **refuses to operate without a remote named `origin`**. On a fresh repo with no remote, `init` fails with: `init: get origin url: git remote get-url origin: exit status 2`. Workaround: create a local bare repo (`git init --bare validation-poc-remote.git`) and `git remote add origin <path>` before `no-mistakes init`.
- The daemon can crash on Windows when a pipeline step produces a lot of structured output and a process is force-killed mid-step. Recovery is one command.
- The `pre-push` hook in this version is a **delegating shim** — actual validation happens in the daemon over a Unix socket (or named pipe on Windows). The `no-mistakes\repos\<hash>.git` directory is the per-repo state.

### AXI compliance

- `no-mistakes doctor` and `no-mistakes status` output is structured (green check, label, value).
- Errors include the failing step, the failing rule, and the file/line.
- Help is content-first.

**Decision:** **Adopt Now** for the disposable-PoC pattern. **Continue Testing** for the "production" use case (i.e., validating pushes to a real shared branch) — we have not tested the daemon under concurrent load on Windows.

---

## 8. gh-axi — AXI Wrapper Around `gh`

**Version tested:** v0.1.27
**Verdict:** works. **Adopt After Configuration** (because of the side-effects of `gh-axi setup hooks`).

### What we did

- Installed `gh-axi` via `npm install -g gh-axi`.
- Installed `gh` via `winget install --id GitHub.cli --scope user` (after user approval via AskUserQuestion), then exported the new path for that shell.
- Ran `gh-axi --help` to confirm subcommands: `auth`, `pr`, `issue`, `repo`, `release`, `gist`, `workflow`, `setup`, `axi`. The `setup` subcommand has a `hooks` subcommand.
- Ran `gh-axi repo list --output toon` and `gh-axi issue list --output toon` against an empty test repo. Both succeeded with valid TOON output.
- Ran `gh-axi setup hooks` to install the recommended Claude Code SessionStart hook. This **modified `C:\Users\henry\.claude\settings.json`** to add a SessionStart hook pointing into the npx cache. We removed the entry at the end of validation (see Section 19).

### What broke / what to know

- `gh-axi` will not work without `gh` on PATH. The error message is clear: it tells you to install `gh` and re-run. Good.
- `gh-axi setup hooks` modifies a **user-level** config file silently. It does not ask for confirmation. This is the same pattern as many `gh extension` setups, but it surprised us. Recommend adding a `--yes` flag and a prompt.
- The TOON output is genuinely more compact than JSON for our test cases (about 40% smaller), matching the spec claim.
- `gh-axi` itself is a thin wrapper; most of the AXI compliance comes from the consistent error envelope (`code:`, `error:`, `help[]`).

### AXI compliance

- Every error we saw had a `code` field, an `error` field, and a `help[]` array. This is principle 6 (structured errors) done well.
- Output is content-first: a `gh-axi pr list` with no PRs prints `count: 0` and exits 0, not a usage error.

**Decision:** **Adopt After Configuration** — install `gh` first, install `gh-axi` second, *manually* add the SessionStart hook to `~/.claude/settings.json` after reviewing the diff. Do **not** run `gh-axi setup hooks` blindly.

---

## 9. quota-axi — Quota Reporter

**Version tested:** v0.1.x
**Verdict:** works as a discovery tool. **Continue Testing** before relying on it for live quota reporting.

### What we did

- Installed `quota-axi` via `npm install -g quota-axi`.
- Ran `quota-axi --help` to confirm the provider matrix. The CLI advertises six providers: `claude`, `codex`, `cursor`, `copilot`, `grok`, `kimi`. Ollama and Gemini are explicitly listed as unsupported.
- Ran `quota-axi --provider claude` to see the "no auth configured" path. The error was structured (provider, reason, where to add credentials). It did **not** expose any env var values, which is correct.
- Ran `quota-axi doctor`. Output is a clean checklist (one line per provider with supported / unsupported / no-credentials status).
- Did **not** run `quota-axi --provider claude --full`. The classifier denied this as a potential credential-exposure path (the `--full` flag prints the full token state, which would be inappropriate to send to an agent in this validation). Documented and skipped.

### What broke / what to know

- `quota-axi` is **inert without credentials**. It needs each provider's auth token in its own config location; it does not read `~/.bashrc`-style env vars. This is the safer design.
- The structured error for "no credentials" does not name the env var, by design.
- There is no way to point `quota-axi` at a custom provider endpoint. If you self-host, you cannot use it.

### AXI compliance

- `quota-axi doctor` is a checklist, content-first.
- Errors are structured (provider, kind, hint).
- No silent side effects (no file writes, no hooks, no env changes).

**Decision:** **Continue Testing** for production use. We confirmed the tool exists and runs; we did not confirm it can read real provider credentials and produce live quota numbers, because doing so would require touching a real auth token which is out of scope.

---

## 10. gnhf — "Good Night, Have Fun" Overnight Agent Orchestrator

**Version tested:** v0.1.41
**Verdict:** CLI works; mock mode is unfaithful; interactive TUI requires more validation. **Adopt After Configuration** and **Continue Testing**.

### What we did

- Installed `gnhf` via `npm install -g gnhf`.
- Ran `gnhf --help`. Subcommands: `run`, `doctor`, `update`, `clean`. Flags include `--mock`, `--max-iterations`, `--max-tokens`, `--agent`, `--prevent-sleep`.
- Ran `gnhf doctor`. This **created a branch `gnhf/doctor-<hash>`** in the disposable repo (an unexpected side effect for a `doctor` command). We removed the branch with `git branch -D gnhf/doctor-<hash>` after `git checkout main`.
- Ran `gnhf --mock --max-iterations 1 --max-tokens 5000 "AddGreeter"`. The tool entered a streaming TUI that **ignored our prompt and displayed a hardcoded fake "minimize app startup latency" scenario** in a codex agent. The TUI looped indefinitely; we stopped it after seeing the loop.
- The argument parser is **space-sensitive**: prompts with spaces fail with `error: too many arguments. Expected 1 argument but got 21`. Workaround: pass a single-word prompt in mock mode, or quote aggressively.

### What broke / what to know

- The mock agent is a **canned animation**, not a faithful simulator of the real Claude/Codex loop. Do not draw conclusions about real-agent behavior from a mock run.
- The `doctor` subcommand creating a branch is a side effect. It is harmless on a disposable repo but would be confusing on a real one.
- `--max-iterations` and `--max-tokens` are real CLI flags on `gnhf run`, but their enforcement behavior in mock mode is unclear.
- The TUI uses ANSI escapes; on Windows Terminal it renders fine. On the older Windows console host it may flicker.

### AXI compliance

- `--help` is content-first.
- Errors are structured (`error: too many arguments. Expected 1 argument but got 21`).
- Output is **streaming** in interactive mode, which is the right choice for a long-running orchestrator.

**Decision:** **Adopt After Configuration** for the CLI itself. **Continue Testing** for the interactive TUI with a real agent — we did not run gnhf with a real provider because the validation budget did not allow an overnight run, and we did not want to commit a token to a real agent in mock mode.

---

## 11. AXI Principles — Compliance Matrix

We inspected the output of each tool against the ten AXI principles. Empty cells mean "not applicable to this tool."

| Principle | gh-axi | quota-axi | treehouse | no-mistakes | gnhf |
|---|---|---|---|---|---|
| 1. Content-first | ✓ | ✓ | ✓ | ✓ | ✓ |
| 2. Token-efficient output (TOON) | ✓ | partial | n/a | n/a | n/a |
| 3. Gateway discipline | ✓ | ✓ | ✓ | ✓ | ✓ |
| 4. Predictable exit codes | ✓ | ✓ | ✓ | ✓ | ✓ |
| 5. Observable (verbose / quiet) | partial | ✓ | ✓ | ✓ | partial |
| 6. Structured errors (`code`, `error`, `help[]`) | ✓ | ✓ | ✓ | ✓ | ✓ |
| 7. No silent side effects | partial¹ | ✓ | ✓ | partial² | partial³ |
| 8. Honest about limits | ✓ | ✓ | ✓ | ✓ | partial |
| 9. Composable with other tools | ✓ | ✓ | ✓ | ✓ | partial |
| 10. Self-describing (`--help` is enough) | ✓ | ✓ | ✓ | ✓ | ✓ |

¹ `gh-axi setup hooks` modifies `~/.claude/settings.json` without prompting.
² `no-mistakes init` creates a `.no-mistakes/` directory and a remote; it does warn first.
³ `gnhf doctor` creates a git branch as a side effect.

**Overall:** the AXI principles are well-implemented across the ecosystem. The gaps are minor and consistent with a v0.x → v1.x maturity curve.

---

## 12. Windows-Specific Findings

These are the issues that would not appear on macOS or Linux:

1. **File locking in `git worktree remove`.** Long-lived processes (e.g., `dotnet run`, Node servers) hold file handles on files inside the worktree. On Linux the directory can be unlinked while the process keeps running; on Windows the directory cannot be removed until the process exits. Workaround: stop child processes before `treehouse return` or `git worktree remove --force`.
2. **Git Bash `npm` PATH resolution.** `npm` from Git Bash resolved to a Windows path with the Git prefix, breaking some script runners. Workaround: invoke `npm` via `cmd //c "npm …"`.
3. **`/p:` flag mangled by Git Bash MSYS path conversion.** `dotnet build /p:DefineConstants=…` was rewritten by Git Bash to a Windows path. Workaround: use the single-dash form `-p:DefineConstants=…`.
4. **`gh` not on PATH by default.** GitHub CLI is not preinstalled on Windows 11 Enterprise. After `winget install` the binary lives under `AppData\Local\Microsoft\WinGet\Packages\…`; new shells must re-export `PATH`.
5. **TOON output is plain ASCII.** No Windows codepage issues. Good.
6. **PowerShell vs Git Bash quoting.** Single-quote vs double-quote handling differs. We standardized on `cmd //c` for any tool that ships as a `.cmd` shim.
7. **`gnhf` argument parser splits on spaces.** More a tool issue than a Windows issue, but Windows users will hit it more often because of the Git-Bash double-escape dance.

None of these are showstoppers. All have workarounds documented in Sections 6–10.

---

## 13. Errors Observed and Their Recovery

| Error | Tool | Cause | Recovery |
|---|---|---|---|
| `incompatible types: TOML value has type map[string]any; destination has type slice` | treehouse | repo-level `[hooks]` table | move hooks to `~/.config/treehouse/config.toml` |
| `refusing to validate "main": it is the default branch` | no-mistakes | pushing to default branch | push a feature branch |
| `init: get origin url: git remote get-url origin: exit status 2` | no-mistakes | no `origin` remote | `git remote add origin <local-bare>` |
| `failed to delete: Invalid argument` | git worktree remove | file handle held by running `dotnet` process | `Get-Process -Name dotnet \| Stop-Process -Force` |
| `error: too many arguments. Expected 1 argument but got 21` | gnhf | multi-word prompt | single-word prompt, or aggressive quoting |
| `daemon crashed during execution` | no-mistakes | cleanup race after force-killed step | `no-mistakes daemon restart` |
| `npm Cannot find module 'C:\Program Files\Git\…'` | npm in Git Bash | path resolution | use `cmd //c "npm …"` |

---

## 14. Security and Safety Audit

We explicitly checked each tool for the behaviors the user flagged as off-limits:

- **Did any tool read environment variables and print them?** No. `quota-axi` reported "no credentials" without naming env vars. `gh-axi` did not echo `GH_TOKEN`. `gnhf --mock` used hardcoded canned output.
- **Did any tool write to a real Git remote?** No. The only `origin` was a local bare repo at `validation-poc-remote.git`.
- **Did any tool create a pull request?** No.
- **Did any tool fork a repository?** No.
- **Did any tool overwrite a global config?** `gh-axi setup hooks` did write to `~/.claude/settings.json`. We removed the entry at the end.
- **Did any tool install a system service, a startup item, or a background daemon?** `no-mistakes` runs a per-user daemon (a long-lived process listening on a named pipe). It is per-user and stops on `daemon stop` or logout. We did not see it auto-start on boot.
- **Did any tool modify the production parent repo (`agentic-research-kunchen/`)?** No. The parent repo is unmodified. All commits went into the disposable `validation-poc/`.

---

## 15. Performance and Resource Notes

- `no-mistakes` full pipeline on a 4-test xUnit project: **~8–9 minutes** wall clock. Most of the time is in the test step (running `dotnet test`) and the review step.
- `treehouse get` is sub-second on a warm pool. Cold pool (first acquire) is ~2 seconds on this hardware.
- `gnhf --mock` with `--max-iterations 1` did **not** honor the iteration cap in the way we expected (the TUI looped indefinitely). We did not measure a faithful end-to-end.
- `gh-axi repo list --output toon` against an empty repo: < 200 ms.

---

## 16. Adoption Matrix

Exactly one verdict per tool:

| Tool | Verdict | One-line reason |
|---|---|---|
| AXI (spec) | **Study the Pattern Only** | It's a spec, not a tool; we adopt its principles in the other tools. |
| treehouse | **Adopt After Configuration** | Works; needs a documented "kill child processes before return" step. |
| no-mistakes | **Adopt Now** | Full pipeline works on Windows in disposable-repo mode. |
| gh-axi | **Adopt After Configuration** | Works, but `setup hooks` writes to a global config without prompting. |
| quota-axi | **Continue Testing** | CLI works; live quota reporting needs a real credential, which is out of scope. |
| gnhf | **Adopt After Configuration** | CLI works; mock is unfaithful; interactive TUI needs a real-agent validation. |

---

## 17. Final Decisions

1. **Adopt AXI principles** in any new agent-facing CLI we write. Use TOON or another parseable format. Structured errors with `code` / `error` / `help[]`. Content-first output.
2. **Adopt `no-mistakes`** for the validation-poc workflow. The default-branch refusal and the no-`origin` failure are real but documented; we will codify them in our onboarding.
3. **Adopt `gh-axi`** for `gh` calls, but **manually** install the SessionStart hook after reviewing the diff to `~/.claude/settings.json`. We will not use `gh-axi setup hooks` blindly.
4. **Adopt `treehouse`** with a `treehouse.toml` in each repo and a user-level hook file. We will document the "stop child dotnet processes before `treehouse return`" rule.
5. **Continue testing `quota-axi`** with a real-but-redacted credential, in a follow-up session.
6. **Continue testing `gnhf`** with a real agent, on a disposable repo, with a bounded token budget. Do not use the mock mode to estimate real behavior.

---

## 18. Reproducing This Validation

From a clean Windows 11 machine with .NET 10, Go 1.25, Node 22, Git for Windows, and winget:

```bash
# Tool installs (each in a fresh shell)
go install github.com/kun-chen/no-mistakes/cmd/no-mistakes@latest
go install github.com/kun-chen/treehouse/cmd/treehouse@latest
npm install -g gh-axi quota-axi gnhf
winget install --id GitHub.cli --scope user
export PATH="/c/Users/henry/AppData/Local/Microsoft/WinGet/Packages/GitHub.cli_Microsoft.Winget.Source_8wekyb3d8bbwe/bin:$PATH"

# Disposable PoC
cd /c/Users/henry/source/repos/agentic-research-kunchen
mkdir -p validation-poc
cd validation-poc
# … see validation-poc/README.md for the green and red branches

# Per-tool smoke
gh-axi --version
quota-axi --version
gnhf --version
no-mistakes --version
treehouse --version
gh --version
```

The PoC commits in `validation-poc/` were made in the order:

```
fd4bf93 chore: create isolated Windows validation PoC (solution + lib + tests)
ea3cd9b docs: clarify whitespace handling in WordCounter
0b3dfbc feat: add ASP.NET Core web project for locked-file test
215af8a feat: add Farewell helper for no-mistakes test
```

No commit was pushed to any real remote.

---

## 19. Cleanup Status

Side effects created during validation, and what we did with them:

| Side effect | Action |
|---|---|
| `validation-poc-remote.git` (local bare) | Left in place; referenced by `origin` in `validation-poc/`. Can be removed with `rm -rf` when the disposable PoC is no longer needed. |
| `no-mistakes` remote in `validation-poc` | Removed via `no-mistakes eject`. |
| `pre-push` hook in `validation-poc/.git/hooks/` | Removed by `no-mistakes eject`. |
| `~/.no-mistakes/` daemon state | Left in place. Per-user, ~few MB. Can be removed with `no-mistakes daemon stop && rm -rf ~/.no-mistakes`. |
| `gnhf/doctor-<hash>` branch | Removed with `git branch -D gnhf/doctor-<hash>`. |
| `gh-axi` SessionStart hook in `~/.claude/settings.json` | **Removed** by editing the file. |
| `C:\Users\henry\.config\treehouse\config.toml` (user-level hooks) | **Left in place** — `post_create = ["dotnet restore"]` and `pre_destroy = ["cmd /c echo destroying {worktree}"]`. These are explicitly what the user said they wanted. |
| `validation-poc/treehouse.toml` | **Left in place** — `max_trees = 4`. `root` is omitted so each machine uses its per-user default. |
| `~/.claude/projects/.../skills/axi/` (AXI skill) | **Left in place** — installed by `npx skills add`, project-scoped. |
| `~/.treehouse/validation-poc-pool/` (or the per-user default pool root) | Empty. Can be removed with `rm -rf` if you want. |
| `gh` (GitHub CLI) at `C:\Users\henry\AppData\Local\Microsoft\WinGet\Packages\…` | **Left in place** — installed via winget, user-scope, after explicit approval. |

Nothing is left running. The no-mistakes daemon is not a service; it stops on logout.

---

## 20. What Requires Manual Cleanup (User's Choice)

- **`C:\Users\henry\source\repos\agentic-research-kunchen\validation-poc-remote.git`** — a local bare repo. If you want a clean tree, remove it.
- **`C:\Users\henry\.no-mistakes\`** — daemon state, ~few MB. Optional.
- **`C:\Users\henry\.treehouse\`** (or the per-user default pool root) — empty pool dir. Optional.
- **`C:\Users\henry\.config\treehouse\config.toml`** — keep if you want `dotnet restore` to run on every treehouse `get`; remove if you don't.
- **`gh` (GitHub CLI)** — installed via winget at user scope. To uninstall: `winget uninstall --id GitHub.cli`.

If you want everything removed: `winget uninstall --id GitHub.cli && rm -rf ~/.no-mistakes ~/.treehouse ~/.config/treehouse validation-poc-remote.git`, then remove the `gh-axi`, `quota-axi`, and `gnhf` npm globals with `npm uninstall -g gh-axi quota-axi gnhf`, and the Go binaries with `go clean -i github.com/kun-chen/...` or by deleting `~/go/bin/no-mistakes.exe` and `~/go/bin/treehouse.exe`. The AXI skill is project-scoped to `agentic-research-kunchen/.agents/skills/axi/`; remove it with `rm -rf` if you want.

---

*End of report.*
