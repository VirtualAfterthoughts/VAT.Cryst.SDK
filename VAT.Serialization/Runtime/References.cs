namespace VAT.Serialization {
    /// <summary>
    /// The id pointing towards an object in a json file.
    /// </summary>
    public struct ReferenceId {
        public readonly string id;

        public ReferenceId(string id) {
            this.id = id;
        }

        public override string ToString() => id;
    }

    /// <summary>
    /// The id pointing towards the assembly qualified type of an object in a json file.
    /// </summary>
    public struct TypeId
    {
        public readonly string id;

        public TypeId(string id)
        {
            this.id = id;
        }

        public override string ToString() => id;
    }
}
