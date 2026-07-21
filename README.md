# agentic-research-kunchen

Research and Windows-validation evidence for the Kun Chen agent-tooling
ecosystem (`AXI`, `treehouse`, `no-mistakes`, `gh-axi`, `quota-axi`,
`gnhf`). This repository exists to preserve the research, the
validation findings, and a reproducible proof-of-concept setup so the
work can be continued from any Windows machine.

> ⚠️ **Safety first.** The tools validated here can clone, commit,
> push, and create pull requests. Always exercise them against
> disposable repositories with a local bare `origin`. Never push
> validation results to a real remote without explicit approval.

## Documents in this repository

| Path | What it is |
|---|---|
| `KunChen_Ecosystem_Windows_Research.md` | The pre-existing research document — ecosystem overview, AXI principles, per-tool descriptions, day-by-day adoption plan. |
| `KunChen_Windows_Validation_Report.md` | The Windows 11 validation report — versions tested, observed behavior, errors encountered, AXI-compliance matrix, adoption matrix, final decisions. |
| `docs/Windows_Setup_and_Reproduction.md` | How to reproduce the validated setup on a fresh Windows 11 machine. |
| `validation-poc/` | The proof-of-concept .NET 10 solution, xUnit tests, and ASP.NET Core fixture used to exercise the tools. |
| `validation-poc/README.md` | PoC-specific build, test, and tool-usage instructions. |
| `.agents/skills/axi/` | The AXI skill (project-scoped) — installable guidance for building AXI-compliant CLIs. |

## Recommended reading order

1. **`KunChen_Ecosystem_Windows_Research.md`** — read this first to
   understand what the tools are and why they exist.
2. **`KunChen_Windows_Validation_Report.md`** — read this second to
   see what was actually observed on Windows 11, including the
   Windows-specific gotchas (file locking, Git Bash `npm` PATH, etc.).
3. **`docs/Windows_Setup_and_Reproduction.md`** — read this third
   when you want to reproduce the validation on another machine.
4. **`validation-poc/README.md`** — read this when you actually run
   the PoC, to see the green and red branches and the file-lock
   fixture.

## Tools validated

| Tool | Version | Verdict |
|---|---|---|
| `gh-axi` | v0.1.27 | Adopt After Configuration |
| `quota-axi` | v0.1.x | Continue Testing |
| `gnhf` | v0.1.41 | Adopt After Configuration / Continue Testing |
| `no-mistakes` | v1.31.2 | Adopt Now |
| `treehouse` | v2.0.0 | Adopt After Configuration |
| `gh` (GitHub CLI) | v2.96.0 | Required dependency for `gh-axi` |
| AXI (spec) | n/a | Study the Pattern Only |

Full per-tool reasoning is in `KunChen_Windows_Validation_Report.md`,
Section 16 (Adoption Matrix) and Section 17 (Final Decisions).

## Current conclusions (one paragraph)

The AXI principles are well-implemented across the binary tools
(structured TOON errors, content-first output, predictable exit
codes, no silent side effects). `no-mistakes` is the most
production-ready on Windows: its full pipeline runs end-to-end in
~8–9 minutes against a disposable .NET PoC and detects the
intentionally failing tests. `treehouse`, `gh-axi`, and `gnhf` all
work, but each has a Windows-specific friction point that
rollout should plan for (file-lock on worktree return, silent
settings.json write from `setup hooks`, mock mode that is not
faithful to real behavior). `quota-axi` is a good discovery tool
but its live quota reporting path was not exercised because it
would have required a real provider credential, which is out of
scope for a portable validation.

## How to reproduce

See `docs/Windows_Setup_and_Reproduction.md`. The short version:

```bash
git clone https://github.com/maestroohk/agentic-research-kunchen.git
cd agentic-research-kunchen

# PoC
dotnet restore ./validation-poc/ValidationPoc.slnx
dotnet build  ./validation-poc/ValidationPoc.slnx --no-restore
dotnet test   ./validation-poc/ValidationPoc.slnx --no-build
# Expected: 12 tests, 12 passed.

# Deliberately failing test (red branch)
dotnet build ./validation-poc/ValidationPoc.slnx -p:DefineConstants=ENABLE_FAILING_TESTS
dotnet test  ./validation-poc/ValidationPoc.slnx --no-build
# Expected: 12 pass, 2 fail (these are the KnownFailingTests).
```

## What is not in this repository

- Tool binaries. The Go and npm-installed binaries live in your
  per-user `~/go/bin/` and `%AppData%\npm\` respectively. Install
  them with the commands in `docs/Windows_Setup_and_Reproduction.md`.
- Tool runtime state. `.treehouse/`, `.no-mistakes/`, `.gnhf/`, and
  disposable bare remotes are intentionally excluded.
- Secrets, tokens, and credentials. None were observed, none were
  committed, none should be added.

## License / contribution

The research and validation documents are notes for personal
study. The PoC source code in `validation-poc/` is intentionally
trivial and is offered for the same purpose — to give the tools
something small and disposable to operate on. There is no
contribution policy at this time.
