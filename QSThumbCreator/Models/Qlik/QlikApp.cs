using System;
using System.Collections.Generic;

namespace QSThumbCreator.Models.Qlik
{
    public class QlikApp
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string AppId { get; set; }
        public DateTime PublishTime { get; set; }
        public bool Published { get; set; }
        public QlikStream Stream { get; set; }
        public string SavedInProductVersion { get; set; }
        public string MigrationHash { get; set; }
        public int AvailabilityStatus { get; set; }
        public IList<string> Privileges { get; set; }

        public string StreamName => Stream?.Name;
    }
}