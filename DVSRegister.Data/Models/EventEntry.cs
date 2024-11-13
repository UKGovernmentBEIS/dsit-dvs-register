using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

namespace DVSRegister.Data.Models
{
    internal class EventEntry
    {
        public EventEntry(EntityEntry entry)
        {
            Entry = entry;
        }
        public EntityEntry Entry { get; }
        public string ActorId { get; set; }       
        public TeamEnum Team { get; set; }
        public EventTypeEnum EventType{ get; set; }
        public ActionEnum Action { get; set; }
        public string TableName { get; set; }
        public Dictionary<string, object> KeyValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();       
        public List<string> ChangedColumns { get; } = new List<string>();
        public Event ToEventLogs()
        {
            var eventObj = new Event
            {
                ActorId = ActorId,
                Team = Team.ToString(),
                EventType = EventType.ToString(),
                TableName = TableName,
                Action = Action.ToString(),
                DateTime = DateTime.UtcNow,
                PrimaryKey = ConvertToJsonDocument(KeyValues),
                OldValues = ConvertToJsonDocument(OldValues),
                NewValues = ConvertToJsonDocument(NewValues),
                AffectedColumns = ConvertToJsonDocument(ChangedColumns)
            };
            return eventObj;
        }


        private JsonDocument ConvertToJsonDocument(Dictionary<string, object> keyValues)
        {
            string jsonString = JsonSerializer.Serialize(keyValues);
            JsonDocument jsonDocument = JsonDocument.Parse(jsonString);
            return jsonDocument;
        }

        private JsonDocument ConvertToJsonDocument(List<string> values)
        {
            string jsonString = JsonSerializer.Serialize(values);
            JsonDocument jsonDocument = JsonDocument.Parse(jsonString);
            return jsonDocument;
        }
    }
    }



