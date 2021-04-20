using System;
using System.Diagnostics.CodeAnalysis;
using FunFair.Common.DataTypes;

namespace FunFair.Labs.ScalingEthereum.DataTypes.Primitives
{
    public sealed class UserAccountId : IEquatable<UserAccountId>
    {
        private readonly Guid _id;

        public UserAccountId(Guid id)
        {
            this._id = id;
        }

        private static UserAccountId Empty { get; } = new(Guid.Empty);

        public bool Equals(UserAccountId? other)
        {
            return AreEqual(this, a2: other);
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is UserAccountId other))
            {
                return false;
            }

            return this.Equals(other);
        }

        private static bool AreEqual(UserAccountId? a1, UserAccountId? a2)
        {
            return ReferenceObjectHelpers.AreEqual(left: a1, right: a2, eq: (l, r) => l._id.Equals(r._id));
        }

        public static bool TryParse(string s, [NotNullWhen(returnValue: true)] out UserAccountId converted)
        {
            converted = Empty;

            if (!Guid.TryParse(input: s, out Guid g))
            {
                return false;
            }

            converted = new UserAccountId(g);

            return true;
        }

        public override string ToString()
        {
            return this._id.ToString();
        }

        public static explicit operator Guid(UserAccountId userAccountId)
        {
            return userAccountId._id;
        }

        public override int GetHashCode()
        {
            return this._id.GetHashCode();
        }

        public static bool operator ==(UserAccountId? a1, UserAccountId? a2)
        {
            return AreEqual(a1: a1, a2: a2);
        }

        public static bool operator !=(UserAccountId? a1, UserAccountId? a2)
        {
            return !AreEqual(a1: a1, a2: a2);
        }
    }
}