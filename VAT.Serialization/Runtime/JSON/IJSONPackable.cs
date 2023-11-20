using Newtonsoft.Json.Linq;

namespace VAT.Serialization.JSON
{
    /// <summary>
    /// Interface for an object that can be packed and unpacked from a json file.
    /// </summary>
    public interface IJSONPackable {
        void Pack(JSONPacker packer, JObject json);

        void Unpack(JSONUnpacker unpacker, JToken token);
    }
}
