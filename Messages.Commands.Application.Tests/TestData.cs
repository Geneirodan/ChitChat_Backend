namespace Messages.Commands.Application.Tests;

public static class TestData
{
    internal static readonly Guid[] ValidIds =
    [
        new("44c726d6-0758-42d3-ab04-1526b7de55f2"),
        new("5c3467f1-d2f3-4cb9-9e4c-22819010b784"),
        new("a844e0ff-1585-4a5f-a2f6-6bea96afe6cb"),
        new("6f05babc-1aae-45ca-9e91-fd65310594ad"),
        new("c829af4c-3227-48e1-a28c-b61cd9855134"),
        new("17144647-0abf-4fa2-b663-799cce41cb8e"),
        new("7f41d545-f40c-4370-a81c-eb979d3f54e4"),
        new("5e9d1f21-f77d-4027-bb0e-6d260eaa462f"),
        new("6d3c90d6-423d-4aaf-aa80-ff3ba0f15ae2"),
        new("82305454-671c-49b9-a084-6ae3d966318c")
    ];

    internal static readonly Message[] ValidMessages = ValidIds
        .Select(x => Message.CreateInstance(x, string.Empty, DateTime.Now, Guid.NewGuid(), Guid.NewGuid()))
        .Select(x => x.Message)
        .ToArray();
}