using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace DVSRegister.Data.Entities
{
    public class Event
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string ActorId { get; set; }    
        public string Team { get; set; }
        public string EventType { get; set; }    // add provider, service, certificate review, pi check etc
        public string TableName { get; set; }
        public string Action { get; set; } // CRUD operations      
        public JsonDocument OldValues { get; set; }
        public JsonDocument NewValues { get; set; }
        public JsonDocument AffectedColumns { get; set; }
        public JsonDocument PrimaryKey { get; set; }
        public DateTime DateTime { get; set; }
    }
}
