using System.Collections.Generic;

[System.Serializable]
public class Character
{
    public string name;
    public List<string> traits = new List<string>();

    public Character(string name, List<string> traits)
    {
        this.name = name;
        this.traits = traits;
    }
}
