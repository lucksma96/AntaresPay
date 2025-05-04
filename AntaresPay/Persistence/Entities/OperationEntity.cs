using AntaresPay.Enums;
using SQLite;

namespace AntaresPay.Persistence.Entities;

[Table("Operations")]
public class OperationEntity : BaseEntity
{
    public OperationTypeEnum Type { get; set; }

    [MaxLength(20)]
    public string? UnitName { get; set; }
    public int Value { get; set; }
}
