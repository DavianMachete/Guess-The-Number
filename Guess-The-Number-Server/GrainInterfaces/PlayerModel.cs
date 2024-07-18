namespace GrainInterfaces;

[GenerateSerializer]
public class PlayerModel
{
    public int Points { get; set; }
    public string Name { get; set; }
    
    public PlayerModel(string name)
    {
        Name = name;
    }
}