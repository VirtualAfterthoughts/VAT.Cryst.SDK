#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace VAT.Packaging
{
    public abstract class DataShard : Shard
    {
        protected Crystal _crystal;
        public Crystal Crystal
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying && _crystal == null)
                {
                    ValidateCrystal();
                }
#endif

                return _crystal;
            }
            set
            {
                _crystal = value;
            }
        }

        public override ICrystal MainCrystal { get => Crystal; set => Crystal = value as Crystal; }

#if UNITY_EDITOR
        public virtual void OnEditorInspectorGUI(SerializedObject serializedObject) { }

        protected void ValidateCrystal()
        {
            AssetPackager.HookOnReady(() =>
            {
                foreach (var crystal in AssetPackager.Instance.GetCrystals())
                {
                    if (crystal.Shards.Contains(this))
                    {
                        Crystal = crystal;
                        break;
                    }
                }
            });
        }
#endif
    }
}
