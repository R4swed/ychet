using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ychet
{
    internal class DB
    {

        public string connectionString { get; set; }
        public string connection;

        public void getConnection()
        {
            connection = @"Data Source=DB.db";
            connectionString = connection;
        }
    }
}
