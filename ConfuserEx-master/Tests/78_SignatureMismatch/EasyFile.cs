using System;

namespace SignatureMismatch {
	public abstract class File<T> : IEquatable<File<T>>
		where T : class {
		public string Name { get; set; } = "";

		public T Data { get; }

		public File(T data) {
			Data = data ?? throw new ArgumentNullException(nameof(data));
		}

		public override bool Equals(object obj) => Equals(obj as File<T>);

		public bool Equals(File<T> other) {
			if (ReferenceEquals(this, other)) {
				return true;
			}

			if (other is null) {
				return false;
			}

			if (string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(other.Name)) {
				return Data.Equals(other.Data);
			}

			return Name == other.Name;
		}

		public override int GetHashCode() {
			return string.IsNullOrEmpty(Name)
				? Data.GetHashCode()
				: Name.GetHashCode();
		}
	}

	public class TextFile : File<string> {
		public TextFile(string name, string code)
			: base(code) {
			Name = name ?? throw new ArgumentNullException(nameof(name));
		}

		public override string ToString() => !string.IsNullOrEmpty(Name)
			? Name
			: Data;
	}
}
