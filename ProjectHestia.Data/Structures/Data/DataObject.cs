using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHestia.Data.Structures.Data;
public class DataObject<T> where T : notnull
{
    public T Key { get; set; }
    public DateTime LastEdit { get; set; }
}
