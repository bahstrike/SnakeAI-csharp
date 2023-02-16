using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeAI
{
    public class TableRow
    {
        public void setInt(string s, int i)
        {

        }

        public void setFloat(string s, double f)
        {

        }
    }

    public class Table
    {
        public int getColumnCount()
        {
            return 0;
        }

        public double getFloat(int i, string s)
        {
            return 0.0;
        }

        public int getInt(int i, string s)
        {
            return 0;
        }

        public void addColumn(string s)
        {

        }

        public TableRow addRow()
        {
            return null;
        }
    }
}
