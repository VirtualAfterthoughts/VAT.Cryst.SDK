using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Cryst
{
    public interface IUpdateable {
        void OnUpdate();
    }

    public interface IFixedUpdateable {
        void OnFixedUpdate();
    }

    public interface ILateUpdateable {
        void OnLateUpdate();
    }
}
