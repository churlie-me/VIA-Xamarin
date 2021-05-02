using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Via.Helpers
{
    public interface IDBInterface
    {
        SQLiteConnection CreateConnection();
    }
}
