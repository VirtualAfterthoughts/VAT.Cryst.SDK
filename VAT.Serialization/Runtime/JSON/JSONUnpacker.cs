using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace VAT.Serialization.JSON {
    /// <summary>
    /// Utility for unpacking a json file.
    /// </summary>
    public class JSONUnpacker {
        private readonly JObject _jsonDocument;

        private readonly Dictionary<ReferenceId, IJSONPackable> _references;
        private readonly Dictionary<TypeId, Type> _types;

        public JSONUnpacker(JObject jsonDocument) {
            _references = new Dictionary<ReferenceId, IJSONPackable>();
            _types = new Dictionary<TypeId, Type>();

            _jsonDocument = jsonDocument;
        }

        /// <summary>
        /// Unpacks the uppermost packable into an instance.
        /// </summary>
        /// <typeparam name="TPackable"></typeparam>
        /// <param name="root"></param>
        /// <param name="constructor"></param>
        public void UnpackRoot<TPackable>(out TPackable root, Func<Type, TPackable> constructor) where TPackable : IJSONPackable {
            JObject rootObj = _jsonDocument["root"].ToObject<JObject>();
            JObject objectsObj = _jsonDocument["references"].ToObject<JObject>();
            JObject typesObj = _jsonDocument["types"].ToObject<JObject>();
            
            ConstructTypes(typesObj);

            if (TryCreateFromReference(rootObj, out root, constructor)) {
                root.Unpack(this, objectsObj[rootObj["ref"].ToString()]);
            }
        }

        /// <summary>
        /// Unpacks a reference into its reference id and type id.
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="referenceId"></param>
        /// <param name="typeId"></param>
        public void UnpackReference(JToken reference, out ReferenceId referenceId, out TypeId typeId) {
            referenceId = new ReferenceId(reference["ref"].ToString());
            typeId = new TypeId(reference["type"].ToString());
        }

        /// <summary>
        /// Gets or creates an instance of a packable by reference.
        /// </summary>
        /// <typeparam name="TPackable"></typeparam>
        /// <param name="reference"></param>
        /// <param name="packable"></param>
        /// <param name="constructor"></param>
        /// <returns></returns>
        public bool TryCreateFromReference<TPackable>(JToken reference, out TPackable packable, Func<Type, TPackable> constructor) where TPackable : IJSONPackable
        {
            packable = default;

            UnpackReference(reference, out var referenceId, out var typeId);

            if (_references.ContainsKey(referenceId)) {
                packable = (TPackable)_references[referenceId];
                return true;
            }
            if (!_types.ContainsKey(typeId))
                return false;

            var type = _types[typeId];
            packable = constructor(type);
            _references.Add(referenceId, packable);

            var objectToken = _jsonDocument["references"][referenceId.ToString()];
            packable.Unpack(this, objectToken);

            return true;
        }

        private void ConstructTypes(JObject types) {
            foreach (var typeObj in types) {
                var typeId = new TypeId(typeObj.Key);
                var typeName = typeObj.Value["typeName"].ToString();

                var type = Type.GetType(typeName);
                if (type == null) {
                    continue;
                }

                _types.Add(typeId, type);
            }
        }
    }
}
