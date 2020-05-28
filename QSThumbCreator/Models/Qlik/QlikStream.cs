using System.Collections.Generic;

namespace QSThumbCreator.Models.Qlik
{
    public class QlikStream
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IList<string> Privileges { get; set; }
        public IList<QlikApp> QlikApps { get; set; }
    }
}