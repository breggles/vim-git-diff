using System;
using System.Collections.Generic;

using VimGitDiff.Io;

namespace VimGitDiff.Tests.Fakes;

public sealed class FakeProcessRunner : IProcessRunner
{
    public List<RecordedInvocation> Invocations { get; } = new();

    public Func<string, IReadOnlyList<string>, string?, ProcessResult> Responder { get; set; }
        = (_, _, _) => new ProcessResult(0, "", "");

    public ProcessResult Run(string fileName, IReadOnlyList<string> arguments, string? workingDirectory)
    {
        Invocations.Add(new RecordedInvocation(fileName, new List<string>(arguments), workingDirectory));

        return Responder(fileName, arguments, workingDirectory);
    }

    public sealed record RecordedInvocation(string FileName, IReadOnlyList<string> Arguments, string? WorkingDirectory);
}
