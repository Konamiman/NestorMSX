using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Konamiman.NestorMSX.Z80Debugger.Console
{
    public abstract class TokenWithName
    {
        public string FullName { get; }
        public bool HasValidName { get; }
        private readonly string[] nameParts;
        private readonly string lastNamePart;

        private static readonly string[] dot = {"."};

        protected TokenWithName(string fullName)
        {
            FullName = fullName;
            if(fullName == null) {
                HasValidName = false;
                return;
            }

            HasValidName = Regex.IsMatch(fullName, @"^ *[A-Za-z0-9_\.]+ *$");
            if (!HasValidName)
                return;

            nameParts = fullName
                .Trim()
                .Split(dot, StringSplitOptions.RemoveEmptyEntries)
                .Select(n => n.ToLower())
                .ToArray();

            if (nameParts.Any(p => char.IsDigit(p[0])))
                HasValidName = false;

            lastNamePart = nameParts.Last();
        }

        protected bool Equals(TokenWithName other)
        {
            return nameParts.Equals(other.nameParts);
        }

        public override bool Equals(object obj)
        {
            if (!HasValidName)
                throw new InvalidOperationException($"Invalid name: {FullName}");

            //short-circuit most common case
            if ((obj as string)?.Equals(lastNamePart, StringComparison.InvariantCultureIgnoreCase) == true)
                return true;

            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() == this.GetType()) return ((TokenWithName)obj).FullName == this.FullName;
            if (obj.GetType() != typeof(string)) return false;

            var tokens = ((string)obj)
                .Split(dot, StringSplitOptions.RemoveEmptyEntries)
                .Select(n => n.ToLower())
                .ToArray();

            if (tokens.Length > nameParts.Length) return false;

            var relevantNameParts = nameParts.Skip(nameParts.Length - tokens.Length).ToArray();
            for(int i=0; i<tokens.Length; i++)
                if (!relevantNameParts[i].StartsWith(tokens[i])) return false;

            return true;
        }

        public override int GetHashCode()
        {
            return FullName.GetHashCode();
        }

        public static bool operator ==(TokenWithName left, string right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TokenWithName left, string right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}
