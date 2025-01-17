using ASP_NET_WEEK3_Homework_Roguelike.Converters;
using ASP_NET_WEEK3_Homework_Roguelike.Model;
using System.Text.Json.Serialization;
using System.Text.Json;

public class MapConverter : JsonConverter<Map>, IConverter<Map>
{
    public override Map Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        try
        {
            var jsonObject = JsonDocument.ParseValue(ref reader).RootElement;
            var map = new Map();

            // Deserialize DiscoveredRooms
            if (jsonObject.TryGetProperty("DiscoveredRooms", out var discoveredRoomsProperty))
            {
                map.DiscoveredRooms = new Dictionary<Point, Room>();
                foreach (var roomEntry in discoveredRoomsProperty.EnumerateObject())
                {
                    var coordinates = roomEntry.Name.Split(',');
                    if (coordinates.Length != 2)
                        throw new JsonException("Invalid Point format in DiscoveredRooms key.");

                    if (!int.TryParse(coordinates[0], out var x) || !int.TryParse(coordinates[1], out var y))
                        throw new JsonException("Failed to parse Point coordinates.");

                    var roomElement = roomEntry.Value;

                    // Deserialize Room
                    var room = JsonSerializer.Deserialize<Room>(roomElement.GetRawText(), options);
                    if (room == null)
                        throw new JsonException("Failed to deserialize Room.");

                    room.Coordinates = new Point(x, y);
                    map.DiscoveredRooms[new Point(x, y)] = room;
                }
            }
            // Deserialize RoomsToDiscover
            if (jsonObject.TryGetProperty("RoomsToDiscover", out var roomsToDiscoverProperty))
            {
                map.RoomsToDiscover = JsonSerializer.Deserialize<List<RoomToDiscover>>(roomsToDiscoverProperty.GetRawText(), options) ?? new List<RoomToDiscover>();
            }
            return map;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to deserialize Map from JSON.", ex);
        }
    }
    public override void Write(Utf8JsonWriter writer, Map value, JsonSerializerOptions options)
    {
        try
        {
            writer.WriteStartObject();

            // Serialize DiscoveredRooms
            writer.WritePropertyName("DiscoveredRooms");
            writer.WriteStartObject();
            foreach (var kvp in value.DiscoveredRooms)
            {
                var point = kvp.Key;
                var room = kvp.Value;
                if (room == null)
                {
                    throw new Exception($"Room at {point} is null.");
                }
                var key = $"{point.X},{point.Y}";
                writer.WritePropertyName(key);
                writer.WriteStartObject();
                writer.WriteNumber("X", room.Coordinates.X);
                writer.WriteNumber("Y", room.Coordinates.Y);
                writer.WriteString("EventStatus", room.EventStatus);
                writer.WriteBoolean("IsExplored", room.IsExplored);

                // Serialize Exits
                writer.WritePropertyName("Exits");
                writer.WriteStartObject();
                foreach (var exit in room.Exits)
                {
                    writer.WritePropertyName(exit.Key.ToString());
                    writer.WriteNullValue(); // avoids circular references
                }
                writer.WriteEndObject();
                writer.WriteEndObject();
            }
            writer.WriteEndObject();

            // Serialize RoomsToDiscover
            writer.WritePropertyName("RoomsToDiscover");
            JsonSerializer.Serialize(writer, value.RoomsToDiscover, options);
            writer.WriteEndObject();
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to serialize Map to JSON.", ex);
        }
    }
}