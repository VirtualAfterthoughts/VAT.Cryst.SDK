using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Interaction.Attachments;

namespace VAT.Props.Ammo
{
    public class AmmoSocket : Socket
    {
        [SerializeField]
        private Transform _outsidePoint;

        [SerializeField]
        private Transform _insidePoint;

        public Transform OutsidePoint => _outsidePoint;

        public Transform InsidePoint => _insidePoint;
    }
}
