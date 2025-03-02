namespace AntaresPay.Models;

public class UnitData
{
    private List<string> units =
    [
        "phoenix",
        "scorpion",
        "andromeda",
        "europa",
        "hercules",
        "alpha",
        "lyra",
        "shaula",
        "bellatrix",
        "pegasus"
    ];

    public required string Name { get; set; }
    public required string Id { get; set; }
    public int Balance { get; set; }

    public bool IsValidUnit()
    {
        return units.Contains(Id);
    }
}
