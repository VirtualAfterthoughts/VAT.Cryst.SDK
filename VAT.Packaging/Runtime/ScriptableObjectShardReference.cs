using System;

namespace VAT.Packaging
{
    [Serializable]
    public class ScriptableObjectShardReference : ShardReferenceT<IScriptableObjectShard>
    {
#if UNITY_EDITOR
        public override Type EditorShardType => typeof(StaticScriptableObjectShard);
#endif
    }
}
