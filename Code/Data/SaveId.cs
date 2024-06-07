using System;

namespace SaveBattle.Data
{
    /// <summary>
    /// A JSON serializable version of a <see cref="LorId"/>
    /// </summary>
    [Serializable]
    public class SaveId : IEquatable<SaveId>, IComparable<SaveId>, IEquatable<LorId>, IComparable<LorId>
    {
        public string packageId { get; set; }

        public int id { get; set; }

        public SaveId()
        {
            id = -1;
        }

        public SaveId(int id)
        {
            this.id = id;
        }

        public SaveId(string packageId, int id)
        {
            this.packageId = packageId;
            this.id = id;
        }

        public SaveId(LorId lorId)
        {
            packageId = lorId.packageId;
            id = lorId.id;
        }

        public LorId GetLorId()
        {
            if ( id != -1 )
            {
                return new LorId(packageId, id);
            }
            else
            {
                return LorId.None;
            }
        }

        public int CompareTo(SaveId other)
        {
            if (other == null) return 1;

            int packageIdComparison = string.Compare(packageId, other.packageId, StringComparison.Ordinal);
            if (packageIdComparison != 0) return packageIdComparison;

            return id.CompareTo(other.id);
        }

        public int CompareTo(LorId other)
        {
            if (other == null) return 1;

            int packageIdComparison = string.Compare(packageId, other.packageId, StringComparison.Ordinal);
            if (packageIdComparison != 0) return packageIdComparison;

            return id.CompareTo(other.id);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SaveId) || Equals(obj as LorId);
        }

        public bool Equals(SaveId other)
        {
            return other != null &&
                   packageId == other.packageId &&
                   id == other.id;
        }

        public bool Equals(LorId other)
        {
            return other != null &&
                   packageId == other.packageId &&
                   id == other.id;
        }

        public static bool operator ==(SaveId a, LorId b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (a is null || b is null)
                return false;

            return a.packageId == b.packageId && a.id == b.id;
        }

        public static bool operator ==(LorId a, SaveId b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (a is null || b is null)
                return false;

            return a.packageId == b.packageId && a.id == b.id;
        }

        public static bool operator !=(SaveId a, LorId b)
        {
            return !(a == b);
        }

        public static bool operator !=(LorId a, SaveId b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + (packageId?.GetHashCode() ?? 0);
                hash = hash * 23 + id.GetHashCode();
                return hash;
            }
        }
    }
}
