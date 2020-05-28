using System;

namespace QSThumbCreator.Models.Qlik
{
    public class QlikSheet
    {
        public string Id { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset ModifiedDate { get; set; }
        public string ModifiedByUserName { get; set; }
        public QlikOwner Owner { get; set; }
        public string EngineObjectType { get; set; }
        public string Description { get; set; }
        public string Attributes { get; set; }
        public string ObjectType { get; set; }
        public DateTimeOffset PublishTime { get; set; }
        public bool Published { get; set; }
        public bool Approved { get; set; }
        public object Tags { get; set; }
        public string SourceObject { get; set; }
        public string DraftObject { get; set; }
        public string Name { get; set; }
        public QlikApp App { get; set; }
        public string AppObjectBlobId { get; set; }
        public string EngineObjectId { get; set; }
        public string ContentHash { get; set; }
        public long Size { get; set; }
        public object Privileges { get; set; }
        public string SchemaPath { get; set; }
    }
}