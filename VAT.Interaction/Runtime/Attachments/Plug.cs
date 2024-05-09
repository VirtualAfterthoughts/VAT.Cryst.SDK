using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Interaction.Attachments
{
    public enum PlugState
    {
        NONE,
        INSERTING,
        INSERTED,
        EJECTING,
    }

    public abstract class Plug : MonoBehaviour
    {
        private Socket _insertedSocket = null;
        private bool _isLocked = false;
        private PlugState _state = PlugState.NONE;

        public Socket InsertedSocket => _insertedSocket;
        public bool IsLocked => _isLocked;
        public PlugState State => _state;

        public void ConfirmInsert(Socket socket)
        {
            _insertedSocket = socket;
            _isLocked = false;
            _state = PlugState.INSERTING;

            socket.RegisterPlug(this);

            OnBeginInsert(socket);
        }
        
        public void CompleteInsert()
        {
            _isLocked = true;
            _state = PlugState.INSERTED;

            _insertedSocket.LockPlug(this);

            OnCompleteInsert(_insertedSocket);
        }

        public void ConfirmEject()
        {
            _isLocked = false;
            _state = PlugState.EJECTING;

            _insertedSocket.UnlockPlug(this);

            OnBeginEject(_insertedSocket);
        }

        public void CompleteEject()
        {
            _insertedSocket.UnregisterPlug(this);

            OnCompleteEject(_insertedSocket);

            _insertedSocket = null;
            _state = PlugState.NONE;
        }

        protected abstract void OnBeginInsert(Socket socket);

        protected abstract void OnCompleteInsert(Socket socket);

        protected abstract void OnBeginEject(Socket socket);

        protected abstract void OnCompleteEject(Socket socket);
    }
}
