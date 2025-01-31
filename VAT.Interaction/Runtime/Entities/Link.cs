using System.Collections.Generic;

namespace VAT.Interaction.Entities
{
    public class Link<T>
    {
        public enum LinkType
        {
            PRIMARY,
            SECONDARY
        }

        public delegate void LinkCallback(Link<T> from, Link<T> to, LinkType type);

        private readonly List<Link<T>> _primaryLinks = new();
        public IReadOnlyList<Link<T>> PrimaryLinks => _primaryLinks;

        private readonly List<Link<T>> _secondaryLinks = new();
        public IReadOnlyList<Link<T>> SecondaryLinks => _secondaryLinks;

        public event LinkCallback OnLinkConnected, OnLinkDisconnected;

        public T Origin { get; }

        public Link(T origin)
        {
            Origin = origin;
        }

        private void HookLink(Link<T> link)
        {
            link.OnLinkConnected += LinkConnected;
            link.OnLinkDisconnected += LinkDisconnected;
        }

        private void UnhookLink(Link<T> link)
        {
            link.OnLinkConnected -= LinkConnected;
            link.OnLinkDisconnected -= LinkDisconnected;
        }

        public void Connect(Link<T> other)
        {
            HookLink(other);
            other.HookLink(this);

            ConnectOneWay(other);
            other.ConnectOneWay(this);
        }

        private void ConnectOneWay(Link<T> other)
        {
            _primaryLinks.Add(other);

            OnLinkConnected?.Invoke(this, other, LinkType.PRIMARY);
        }

        public void Disconnect(Link<T> other)
        {
            DisconnectOneWay(other);
            other.DisconnectOneWay(this);

            UnhookLink(other);
            other.UnhookLink(this);
        }

        private void DisconnectOneWay(Link<T> other)
        {
            _primaryLinks.Remove(other);

            OnLinkDisconnected?.Invoke(this, other, LinkType.PRIMARY);
        }

        private void LinkConnected(Link<T> from, Link<T> to, LinkType type)
        {
            if (type != LinkType.PRIMARY)
            {
                return;
            }

            if (to == this)
            {
                return;
            }

            _secondaryLinks.Add(to);

            OnLinkConnected?.Invoke(this, to, LinkType.SECONDARY);
        }

        private void LinkDisconnected(Link<T> from, Link<T> to, LinkType type)
        {
            if (type != LinkType.PRIMARY)
            {
                return;
            }

            if (to == this)
            {
                return;
            }

            _secondaryLinks.Remove(to);

            OnLinkDisconnected?.Invoke(this, to, LinkType.SECONDARY);
        }
    }
}
