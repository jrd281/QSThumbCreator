using System.Collections.Generic;

namespace QSThumbCreator.Models.Qlik
{
    public class QlikContentLibrary
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public IList<object> Privileges { get; set; }
    }
}