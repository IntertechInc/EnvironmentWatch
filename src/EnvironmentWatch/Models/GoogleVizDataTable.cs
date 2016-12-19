using System.Collections.Generic;
using System.Linq;

namespace EnvironmentWatch.Models
{
    /// <summary>
    /// Class used to facilitate JSON serialization into format required by Google
    /// to create a DataTable. 
    /// </summary>
    public class GoogleVizDataTable
    {
        public IList<Col> cols { get; set; } = new List<Col>();

        public IList<Row> rows { get; set; } = new List<Row>();

        public class Col
        {
            public string label { get; set; }
            public string type { get; set; }
        }

        public class Row
        {
            public IEnumerable<RowValue> c { get; set; }

            public class RowValue
            {
                public object v;
            }
        }
    }
}
