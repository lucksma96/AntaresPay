using AntaresPay.Enums;
using SQLite;

namespace AntaresPay.Persistence.Entities;

public class Operation : BaseEntity
{
    public OperationTypeEnum Type { get; set; }

    [MaxLength(20)]
    public required string UnitName { get; set; }
}
