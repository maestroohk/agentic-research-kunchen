# ValidationPoc

A deliberately minimal .NET proof-of-concept repository used to validate
the Kun Chen ecosystem tools (`AXI`, `treehouse`, `no-mistakes`,
`gh-axi`, `quota-axi`, `gnhf`) on Windows.

This repository is **not** a production codebase. It exists only to
exercise the tools under test. The `src/ValidationPoc.Web/` project is
a deliberately long-lived ASP.NET Core process used to verify that
`treehouse return` and `git worktree remove` correctly handle the
Windows file-lock case; the `tests/ValidationPoc.Tests/KnownFailingTests.cs`
file is the deliberately failing test that exercises the red branch of
the no-mistakes quality gate.

## Original PoC commit history

When this directory lived as an independent git repository, the following
commits were made. They are recorded here for traceability; the source
files below are the same as those commits, integrated into the parent
repository.

```
fd4bf93 chore: create isolated Windows validation PoC (solution + lib + tests)
ea3cd9b docs: clarify whitespace handling in WordCounter
0b3dfbc feat: add ASP.NET Core web project for locked-file test
215af8a feat: add Farewell helper for no-mistakes test
```

The nested `.git` directory was removed before integration; the parent
repository is now the single source of truth.

## Structure

```
ValidationPoc.slnx
src/ValidationPoc.Lib/         class library (WordCounter, Greeter, Farewell)
src/ValidationPoc.Web/         ASP.NET Core web app (file-lock fixture)
tests/ValidationPoc.Tests/     xUnit test project
treehouse.toml                 treehouse pool config (per-user root)
```

## Build & test

```bash
dotnet restore
dotnet build
dotnet test
```

The default `dotnet test` run is green: it covers `WordCounter`,
`Greeter`, and `Farewell`. Six tests pass.

## Intentional validation failure

`tests/ValidationPoc.Tests/KnownFailingTests.cs` is **compiled out by
default** via `#if ENABLE_FAILING_TESTS`. Two tests in that file
expect wrong values on purpose, so they would fail if compiled in.
This is how the no-mistakes red branch is exercised.

To enable the failing tests, define the preprocessor symbol when
building:

```bash
dotnet build -p:DefineConstants=ENABLE_FAILING_TESTS
dotnet test --no-build
```

You should see two failing tests. Do not "fix" the assertions — the
mismatches are intentional and the values are recorded in the
validation report (`KunChen_Windows_Validation_Report.md`).

## Long-running process fixture

`src/ValidationPoc.Web/` is a minimal ASP.NET Core 10 app used to
verify Windows file-lock behavior. From `validation-poc/`:

```bash
dotnet run --project src/ValidationPoc.Web
```

The default `applicationUrl` is `http://localhost:5286`. While this
process is running, attempting `git worktree remove` on the worktree
that contains `src/ValidationPoc.Web/bin` or `src/ValidationPoc.Web/obj`
will fail with `Invalid argument` because Windows cannot unlink a
directory that has open file handles. Stop the process before
returning a worktree to the pool.
