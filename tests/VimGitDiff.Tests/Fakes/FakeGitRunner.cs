using System;
using System.Collections.Generic;

using VimGitDiff.Git;
using VimGitDiff.Io;

namespace VimGitDiff.Tests.Fakes;

public sealed class FakeGitRunner : IGitRunner
{
    public List<RecordedInvocation> Invocations { get; } = new();

    public Func<IReadOnlyList<string>, string?, ProcessResult> Responder { get; set; }
        = (_, _) => new ProcessResult(0, "", "");

    public ProcessResult Run(IReadOnlyList<string> arguments, string? workingDirectory)
    {
        Invocations.Add(new RecordedInvocation(new List<string>(arguments), workingDirectory));

        return Responder(arguments, workingDirectory);
    }

    public sealed record RecordedInvocation(IReadOnlyList<string> Arguments, string? WorkingDirectory);
}
