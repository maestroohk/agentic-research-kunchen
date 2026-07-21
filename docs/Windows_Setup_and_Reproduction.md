# Windows Setup and Reproduction Guide

This document explains how to reproduce the validated Kun Chen ecosystem
setup on a fresh Windows 11 machine. The intent is that the validation
results in `KunChen_Windows_Validation_Report.md` are **re-runnable**,
not just re-readable. None of the steps require credentials from this
machine or this repository; everything is reproducible from public
downloads and a clean shell.

## 1. Prerequisites

A fresh Windows 11 Enterprise (or Windows 11 Pro) machine with the
following installed and on `PATH`:

| Tool | Minimum version | How to verify |
|---|---|---|
| PowerShell 7 | 7.4 or later | `pwsh --version` |
| Git for Windows | 2.40 or later | `git --version` |
| .NET SDK | 10.0 (preview at validation time) | `dotnet --version` |
| Node.js | 20 LTS or later | `node --version` |
| npm | 10.x (ships with Node 20+) | `npm --version` |
| Go | 1.22 or later | `go version` |
| GitHub CLI | 2.50 or later (only if using `gh-axi`) | `gh --version` |
| winget | 1.7 or later (only if installing `gh` via winget) | `winget --version` |

PowerShell 7 ships with Windows 11 22H2 and later. The other tools
are installed with the commands below.

## 2. Tool versions validated

These are the exact versions exercised on the validation machine.
Newer versions are likely to work; older versions may not. If you
are validating reproducibility, install these versions.

| Tool | Version tested | Source |
|---|---|---|
| `gh-axi` | v0.1.27 | `npm install -g gh-axi` |
| `quota-axi` | v0.1.x | `npm install -g quota-axi` |
| `gnhf` | v0.1.41 | `npm install -g gnhf` |
| `no-mistakes` | v1.31.2 | `go install github.com/kun-chen/no-mistakes/cmd/no-mistakes@v1.31.2` |
| `treehouse` | v2.0.0 | `go install github.com/kun-chen/treehouse/cmd/treehouse@v2.0.0` |
| `gh` | v2.96.0 | `winget install --id GitHub.cli --scope user` |
| `dotnet` SDK | 10.0 (preview) | https://dotnet.microsoft.com/download/dotnet/10.0 |

The AXI specification itself has no install; it is documented in
`KunChen_Ecosystem_Windows_Research.md`.

## 3. Installation commands

Run these in a fresh PowerShell 7 or Git Bash shell. Each tool
installs to the **user scope** — no admin rights are required and
no system-wide configuration is touched.

### 3.1 .NET SDK 10

Download and run the installer from
https://dotnet.microsoft.com/download/dotnet/10.0. After install:

```bash
dotnet --version   # should print 10.0.x
```

### 3.2 Node.js 20 LTS

Download and run the installer from https://nodejs.org. After install:

```bash
node --version
npm --version
```

### 3.3 Go 1.22+

Download and run the MSI from https://go.dev/dl/. After install,
add `%USERPROFILE%\go\bin` to your `PATH` for new shells.

```bash
go version
```

### 3.4 GitHub CLI (only if you plan to use `gh-axi`)

```bash
winget install --id GitHub.cli --scope user
```

After install, new shells need `gh` on `PATH`. The binary is installed
under `AppData\Local\Microsoft\WinGet\Packages\…`. Either re-open your
terminal, or add the path manually:

```bash
export PATH="/c/Users/<you>/AppData/Local/Microsoft/WinGet/Packages/GitHub.cli_Microsoft.Winget.Source_8wekyb3d8bbwe/bin:$PATH"
```

### 3.5 The Kun Chen ecosystem tools

In Git Bash or PowerShell:

```bash
# npm-based (user-scope)
npm install -g gh-axi
npm install -g quota-axi
npm install -g gnhf

# Go-based (user-scope, requires Go 1.22+ on PATH)
go install github.com/kun-chen/no-mistakes/cmd/no-mistakes@v1.31.2
go install github.com/kun-chen/treehouse/cmd/treehouse@v2.0.0
```

Verify:

```bash
gh-axi --version
quota-axi --version
gnhf --version
no-mistakes --version
treehouse --version
```

Each should print its version and exit 0. The first run of each Go
tool may print a "newer version available" notice — that is normal
and informational.

## 4. Authentication

None of the validation tools require credentials **to install or to
smoke-test**. Some require interactive login **to exercise their
intended use**:

| Tool | Auth required? | What it does |
|---|---|---|
| `gh-axi` | Yes, for live `gh` operations | Run `gh auth login` once. `gh-axi` inherits `gh`'s session. |
| `quota-axi` | Yes, for live quota reporting | Configure provider credentials per `quota-axi doctor` output. Each provider has its own config location; `quota-axi` does **not** read environment variables. |
| `no-mistakes` | No, but needs a remote | Add any local or remote `git remote add origin <url>` before `no-mistakes init`. A local bare repo on disk is sufficient for testing. |
| `treehouse` | No | Pool is per-user. |
| `gnhf` | Only for real-agent runs | Mock mode (`--mock`) needs no credentials. Real-agent runs need provider credentials, configured per `--agent`. |
| `gh` | Yes, for any non-readonly `gh` call | `gh auth login`. |

This guide does **not** include any credentials. Do not commit
credentials, tokens, or `.env` files to this repository. The
repository's `.gitignore` already excludes common credential
patterns.

## 5. PoC usage

The `validation-poc/` directory is the proof-of-concept .NET
solution. It is intentionally small — two source files, four test
files, one ASP.NET Core app — and exists only to exercise the tools.

### 5.1 Restore, build, and test (green branch)

From the repository root:

```bash
dotnet restore ./validation-poc/ValidationPoc.slnx
dotnet build  ./validation-poc/ValidationPoc.slnx --no-restore
dotnet test   ./validation-poc/ValidationPoc.slnx --no-build
```

Expected: **12 tests, 12 passed, 0 failed, 0 skipped**.

### 5.2 Run the deliberately failing test (red branch)

The file `validation-poc/tests/ValidationPoc.Tests/KnownFailingTests.cs`
is compiled out by default via `#if ENABLE_FAILING_TESTS`. To
exercise the red branch:

```bash
dotnet build ./validation-poc/ValidationPoc.slnx -p:DefineConstants=ENABLE_FAILING_TESTS
dotnet test  ./validation-poc/ValidationPoc.slnx --no-build
```

Expected: **12 pass + 2 fail**. The two failing tests are:

- `KnownFailingTests.Greeter_ShouldProduceExpectedGreeting_ForKnownName`
  — expects "Good morning, Henry" but `Greeter.Greet("Henry")` returns
  "Hello, Henry!".
- `KnownFailingTests.WordCounter_ShouldCountWordsCorrectly_ForKnownInput`
  — expects 99 but the actual count of `one two three` is 3.

Do **not** "fix" the failing assertions. The mismatches are recorded
evidence of the red-branch behavior in the validation report.

After running the red branch, rebuild without the flag to leave
the working tree green:

```bash
dotnet build ./validation-poc/ValidationPoc.slnx
```

### 5.3 Run the ASP.NET Core test application

The `src/ValidationPoc.Web/` project is a deliberately long-lived
ASP.NET Core 10 app used to verify Windows file-lock behavior. It
binds to `http://localhost:5286` by default. Start it:

```bash
dotnet run --project ./validation-poc/src/ValidationPoc.Web
```

While it is running, `git worktree remove` on a worktree that
contains its `bin/` or `obj/` will fail with `Invalid argument` on
Windows because the process holds open file handles. Stop the
process (Ctrl-C, or `Stop-Process -Name dotnet` from PowerShell)
before cleaning up a worktree.

### 5.4 Test treehouse

In the repository root:

```bash
# initialize the per-user config (one time)
treehouse init --user

# in a disposable working copy, add a treehouse.toml:
#   max_trees = 4
# (the committed file in this repo is a working example)

# acquire a worktree
treehouse get ./validation-poc
# … do work …
treehouse return   # this terminates lingering processes owned by the worktree
```

The first acquire will create the per-user pool under
`~/.treehouse/<repo-name>/` (on Windows, `%USERPROFILE%\.treehouse\<repo-name>`).
The path is **not** committed; each machine uses its own per-user
default.

### 5.5 Test no-mistakes safely

Use a disposable clone, not the parent repository:

```bash
git clone <this-repo-url> nm-test
cd nm-test
git checkout -b feat/your-change
# … make a small change, commit …
git push
```

If `no-mistakes init` complains that there is no `origin`, create a
local bare remote:

```bash
git init --bare ../nm-test-remote.git
git remote add origin ../nm-test-remote.git
no-mistakes init
```

no-mistakes refuses to validate the default branch (`main`). Always
push a feature branch. The first push will trigger the full pipeline
(intent → rebase → review → test → document → lint), which can take
5–10 minutes on a small project. Auto-fixes are applied for common
issues; the pipeline does not rewrite git history.

To remove the gate when you are done:

```bash
no-mistakes eject
```

### 5.6 Test gh-axi (read-only)

With `gh` authenticated:

```bash
gh-axi repo list --output toon
gh-axi issue list --output toon
gh-axi pr list --output toon
```

These are read-only and safe to run against any repo you have access
to. **Do not** run `gh-axi setup hooks` unless you have read the
diff it will apply to `~/.claude/settings.json`. If you do install
the hook, remove it when you are done:

```bash
# open ~/.claude/settings.json and remove the gh-axi entry under
# hooks.SessionStart
```

### 5.7 Test quota-axi

```bash
quota-axi doctor
```

This prints a per-provider checklist (supported / unsupported /
no-credentials) without exposing any credential values. For a live
quota reading, configure provider credentials per the doctor output
and re-run `quota-axi --provider <name>`. **Do not** pass `--full`
to an agent in a non-interactive context.

### 5.8 Run a bounded gnhf experiment

Mock mode is unfaithful — it displays a canned scenario that ignores
your prompt. Use mock mode only to verify the TUI renders correctly
on your terminal.

```bash
gnhf --mock --max-iterations 1 --max-tokens 5000 AddGreeter
```

A real-agent run requires provider credentials and a disposable
repository. Keep the token budget tight for the first run:

```bash
gnhf run --agent claude --max-iterations 3 --max-tokens 50000 "your task"
```

The TUI uses ANSI escapes; Windows Terminal renders it correctly.
Do not run a real-agent overnight without first doing a bounded
smoke test.

## 6. Local-only files and directories

These are **not** committed and must be recreated locally:

| Path | Why it is excluded |
|---|---|
| `validation-poc/.gnhf/` | gnhf runtime state (per-machine) |
| `validation-poc/bin/`, `obj/` | .NET build output (machine-specific) |
| `validation-poc/.vs/` | Visual Studio user files |
| `validation-poc/TestResults/` | dotnet test output |
| `~/.treehouse/<name>/` | treehouse worktree pool (per-user) |
| `~/.no-mistakes/` | no-mistakes daemon state (per-user) |
| `~/.config/treehouse/config.toml` | treehouse user hooks (per-user) |
| `validation-poc-remote.git/` (or any local bare repo) | disposable test remotes; never commit a bare repo |
| `.env`, `secrets.json`, `appsettings.Development.json` | credential-bearing config |

The repository's `.gitignore` already excludes all of the above.

## 7. Cleanup

When you are done validating, you can remove the local-only state:

```bash
# Stop any running dotnet / treehouse / no-mistakes processes
Get-Process -Name dotnet,treehouse,no-mistakes,gnhf -ErrorAction SilentlyContinue |
    Stop-Process -Force

# Remove per-user tool state
Remove-Item -Recurse -Force "$env:USERPROFILE\.treehouse"        -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "$env:USERPROFILE\.no-mistakes"     -ErrorAction SilentlyContinue
Remove-Item -Force           "$env:USERPROFILE\.config\treehouse\config.toml" -ErrorAction SilentlyContinue

# Remove any disposable bare repos
Remove-Item -Recurse -Force "validation-poc-remote.git" -ErrorAction SilentlyContinue

# Remove the PoC's local runtime state
Remove-Item -Recurse -Force "validation-poc\.gnhf"    -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "validation-poc\bin"      -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "validation-poc\obj"      -ErrorAction SilentlyContinue
Get-ChildItem -Recurse -Directory -Path "validation-poc" -Filter TestResults |
    Remove-Item -Recurse -Force
```

To uninstall the global tools:

```bash
npm uninstall -g gh-axi quota-axi gnhf
go clean -i github.com/kun-chen/...
winget uninstall --id GitHub.cli   # if you no longer need gh
```

This does **not** remove the source repositories; the .NET SDK, Go,
and Node.js runtimes will need to be uninstalled via "Add or Remove
Programs" if you want a fully clean machine.

## 8. Safety reminders

- **Do not** validate against a production or important repository.
  Always use a disposable clone, ideally with a local bare `origin`.
- **Do not** push to a real remote without explicit approval.
- **Do not** create pull requests against real repositories.
- **Do not** expose secrets, tokens, or environment variables in
  agent prompts or commit messages.
- **Do not** install tools globally if the project instructions
  forbid it. This guide installs everything at the user scope.
- **Do not** rewrite or redesign the tools. If something does not
  work, document the blocker and stop.
