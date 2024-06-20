using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku1
{
    /// <summary>
    /// Publiczny interfejs ISavable, któy zawiera metodę Save, która może być stosowana do zapisu plików
    /// </summary>
    public interface ISavable
    {
        public void Save();
    }
}
