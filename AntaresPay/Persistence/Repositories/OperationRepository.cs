using AntaresPay.Persistence.Entities;

namespace AntaresPay.Persistence.Repositories;

public class OperationRepository(SQLiteDatabase database) : BaseRepository<OperationEntity>(database)
{
}
