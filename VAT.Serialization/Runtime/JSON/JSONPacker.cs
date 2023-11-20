using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace VAT.Serialization.JSON
{
    /// <summary>
    /// Utility for packing a json file.
    /// </summary>
    public class JSONPacker {
        public const int VERSION = 1;

        private const int RECURSION_CAP = 8;

        private const string REFERENCE_PREFIX = "r:";
        private const string TYPE_PREFIX = "t:";

        private readonly Dictionary<ReferenceId, IJSONPackable> _references;
        private readonly HashSet<IJSONPackable> _referenceSet;

        private readonly Dictionary<TypeId, Type> _types;
        private readonly Dictionary<Type, TypeId> _typesInverse;

        private readonly JObject _jsonDocument;

        private bool _hasPackedRoot = false;

        private JSONPacker(JObject jsonDocument) {
            _references = new Dictionary<ReferenceId, IJSONPackable>();
            _referenceSet = new HashSet<IJSONPackable>();

            _types = new Dictionary<TypeId, Type>();
            _typesInverse = new Dictionary<Type, TypeId>();

            _jsonDocument = jsonDocument;
        }

        public JSONPacker() : this(new JObject()) { }

        /// <summary>
        /// Packs the uppermost packable into a formatted json document.
        /// </summary>
        /// <typeparam name="TPackable"></typeparam>
        /// <param name="root"></param>
        /// <returns></returns>
        public JObject PackRoot<TPackable>(TPackable root) where TPackable : IJSONPackable {
            if (_hasPackedRoot)
                throw new Exception("Root for document was already packed.");

            _jsonDocument.Add("version", VERSION);
            _jsonDocument.Add("root", PackReference(root));

            // Serialize objects
            var objectsJson = new JObject();
            List<IJSONPackable> packedJson = new List<IJSONPackable>();

            for (var i = 0; i < RECURSION_CAP; i++) {
                var referenceCopy = new Dictionary<ReferenceId, IJSONPackable>(_references);

                foreach (var pair in referenceCopy) {
                    if (packedJson.Contains(pair.Value))
                        continue;

                    // Pack the packable into a json object
                    var newObject = new JObject();
                    pair.Value.Pack(this, newObject);

                    // Pack the type as well if we can
                    if (_typesInverse.TryGetValue(pair.Value.GetType(), out var id)) {
                        var typeObject = new JObject();
                        typeObject.Add("type", id.ToString());

                        newObject.Add("isType", typeObject);
                    }

                    // Add to the final json output
                    objectsJson.Add(pair.Key.ToString(), newObject);
                    packedJson.Add(pair.Value);
                }
            }

            _jsonDocument.Add("references", objectsJson);

            // Serialize types
            var typesJson = new JObject();
            foreach (var pair in _types) {
                var newObject = new JObject();
                newObject.Add("type", pair.Key.ToString());
                newObject.Add("typeName", pair.Value.AssemblyQualifiedName);

                typesJson.Add(pair.Key.ToString(), newObject);
            }

            _jsonDocument.Add("types", typesJson);

            _hasPackedRoot = true;

            return _jsonDocument;
        }

        /// <summary>
        /// Packs a reference to a packable, so that it is not duplicated.
        /// </summary>
        /// <typeparam name="TPackable"></typeparam>
        /// <param name="packable"></param>
        /// <returns></returns>
        public JObject PackReference<TPackable>(TPackable packable) where TPackable : IJSONPackable {
            return new JObject {
                ["ref"] = GetPackedReference(packable).ToString(),
                ["type"] = GetPackedType(packable.GetType()).ToString(),
            };
        }

        // ID creation
        private int _lastReferenceId;
        private ReferenceId CreateReferenceId() => new ReferenceId($"{REFERENCE_PREFIX}{_lastReferenceId++}");

        private ReferenceId GetPackedReference(IJSONPackable packable) {
            if (_referenceSet.Contains(packable)) {
                return _references.FirstOrDefault((p) => p.Value == packable).Key;
            }
            else {
                var id = CreateReferenceId();
                _references.Add(id, packable);
                _referenceSet.Add(packable);
                return id;
            }
        }

        private int _lastTypeId;
        private TypeId CreateTypeId() => new TypeId($"{TYPE_PREFIX}{_lastTypeId++}");

        private TypeId GetPackedType(Type type) {
            if (_typesInverse.ContainsKey(type)) 
                return _typesInverse[type];

            var id = CreateTypeId();
            _types.Add(id, type);
            _typesInverse.Add(type, id);

            return id;
        }
    }
}
