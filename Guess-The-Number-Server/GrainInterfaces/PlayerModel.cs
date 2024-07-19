namespace GrainInterfaces;

[GenerateSerializer]
public record class PlayerModel(string UserName)
{
    [Id(0)] public int Wins { get; set; }
    [Id(1)] public int Loses { get; set; }
    [Id(3)] public string UserName { get; set; } = UserName;
}