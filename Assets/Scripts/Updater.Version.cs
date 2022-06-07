using System;
using System.Text.RegularExpressions;

namespace Elektronik
{
    public partial class Updater
    {
        /// <summary> Version of Electronik. </summary>
        public readonly struct Version : IComparable<Version>
        {
            /// <summary> Major version. </summary>
            public readonly int Major;

            /// <summary> Minor version </summary>
            public readonly int Minor;

            /// <summary> Fix number. </summary>
            public readonly int Fix;

            /// <summary> Number of release candidate. </summary>
            public readonly int Rc;

            /// <summary> Work-In-Progress flag. </summary>
            public readonly bool Wip;

            /// <summary> Constructor. Parses version from given string. </summary>
            /// <param name="version"> Version string. </param>
            public Version(string version)
            {
                const string pattern = @"v?(\d+)(?:\.(\d+))?(?:\.(\d+))?(?:-rc(\d+))?(-WIP)?";
                var match = Regex.Match(version, pattern);

                Major = Parse(match.Groups[1].Value);
                Minor = Parse(match.Groups[2].Value);
                Fix = Parse(match.Groups[3].Value);
                Rc = Parse(match.Groups[4].Value);
                Wip = !string.IsNullOrEmpty(match.Groups[5].Value);
            }

            /// <inheritdoc />
            public int CompareTo(Version other)
            {
                if (Major > other.Major) return 1;
                if (Major < other.Major) return -1;

                if (Minor > other.Minor) return 1;
                if (Minor < other.Minor) return -1;

                if (Fix > other.Fix) return 1;
                if (Fix < other.Fix) return -1;

                if (Rc == 0 && other.Rc > 0) return 1;
                if (other.Rc == 0 && Rc > 0) return -1;

                if (Rc > other.Rc) return 1;
                if (Rc < other.Rc) return -1;

                if (Wip) return -1;
                if (other.Wip) return 1;

                return 0;
            }

            public bool Equals(Version other)
            {
                return Major == other.Major && Minor == other.Minor && Fix == other.Fix && Rc == other.Rc
                        && Wip == other.Wip;
            }

            /// <inheritdoc />
            public override bool Equals(object obj)
            {
                return obj is Version other && Equals(other);
            }

            /// <inheritdoc />
            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = Major;
                    hashCode = (hashCode * 397) ^ Minor;
                    hashCode = (hashCode * 397) ^ Fix;
                    hashCode = (hashCode * 397) ^ Rc;
                    hashCode = (hashCode * 397) ^ Wip.GetHashCode();
                    return hashCode;
                }
            }

            public static bool operator >(Version first, Version second)
            {
                return first.CompareTo(second) > 0;
            }

            public static bool operator <(Version first, Version second)
            {
                return first.CompareTo(second) < 0;
            }

            public static bool operator ==(Version first, Version second)
            {
                return first.Equals(second);
            }

            public static bool operator !=(Version first, Version second)
            {
                return !(first == second);
            }

            /// <inheritdoc />
            public override string ToString()
            {
                var rc = Rc > 0 ? $"-rc{Rc}" : "";
                var wip = Wip ? "-WIP" : "";
                return $"v{Major}.{Minor}.{Fix}{rc}{wip}";
            }

            private static int Parse(string str) => int.TryParse(str, out var res) ? res : 0;
        }
    }
}