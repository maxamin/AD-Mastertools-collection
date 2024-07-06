using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;

namespace Confuser.Core {
	internal sealed class ConfuserAssemblyResolver : IAssemblyResolver {
		internal AssemblyResolver InternalFuzzyResolver { get; } = new AssemblyResolver { FindExactMatch = false };
		internal AssemblyResolver InternalExactResolver { get; } = new AssemblyResolver { FindExactMatch = true };

		public bool EnableTypeDefCache {
			get => InternalFuzzyResolver.EnableTypeDefCache;
			set {
				InternalFuzzyResolver.EnableTypeDefCache = value;
				InternalExactResolver.EnableTypeDefCache = value;
			}
		}

		public ModuleContext DefaultModuleContext {
			get => InternalFuzzyResolver.DefaultModuleContext;
			set {
				InternalFuzzyResolver.DefaultModuleContext = value;
				InternalExactResolver.DefaultModuleContext = value;
			}
		}

		public IList<string> PostSearchPaths => new TeeList(InternalFuzzyResolver.PostSearchPaths, InternalExactResolver.PostSearchPaths);
		public IList<string> PreSearchPaths => new TeeList(InternalFuzzyResolver.PreSearchPaths, InternalExactResolver.PreSearchPaths);

		/// <inheritdoc />
		public AssemblyDef Resolve(IAssembly assembly, ModuleDef sourceModule) {
			if (assembly is AssemblyDef assemblyDef)
				return assemblyDef;

			var resolvedAssemblyDef = InternalExactResolver.Resolve(assembly, sourceModule);
			return resolvedAssemblyDef ?? InternalFuzzyResolver.Resolve(assembly, sourceModule);
		}

		public void Clear() {
			InternalExactResolver.Clear();
			InternalFuzzyResolver.Clear();
		}

		public IEnumerable<AssemblyDef> GetCachedAssemblies() => 
			InternalExactResolver.GetCachedAssemblies().Concat(InternalFuzzyResolver.GetCachedAssemblies());

		public void AddToCache(ModuleDefMD modDef) {
			InternalExactResolver.AddToCache(modDef);
			InternalFuzzyResolver.AddToCache(modDef);
		}

		private sealed class TeeList : IList<string> {
			private readonly IList<IList<string>> _lists;

			internal TeeList(params IList<string>[] lists) => _lists = lists;

			/// <inheritdoc />
			public IEnumerator<string> GetEnumerator() => _lists[0].GetEnumerator();

			/// <inheritdoc />
			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

			/// <inheritdoc />
			public void Add(string item) {
				foreach (var list in _lists) 
					list.Add(item);
			}

			/// <inheritdoc />
			public void Clear() {
				foreach (var list in _lists) 
					list.Clear();
			}

			/// <inheritdoc />
			public bool Contains(string item) => _lists[0].Contains(item);

			/// <inheritdoc />
			public void CopyTo(string[] array, int arrayIndex) => _lists[0].CopyTo(array, arrayIndex);

			/// <inheritdoc />
			public bool Remove(string item) =>
				_lists.Aggregate(true, (current, list) => current | list.Remove(item));

			/// <inheritdoc />
			public int Count => _lists[0].Count;

			/// <inheritdoc />
			public bool IsReadOnly => _lists[0].IsReadOnly;

			/// <inheritdoc />
			public int IndexOf(string item) => _lists[0].IndexOf(item);

			/// <inheritdoc />
			public void Insert(int index, string item) {
				foreach (var list in _lists) 
					list.Insert(index, item);
			}

			/// <inheritdoc />
			public void RemoveAt(int index) {
				foreach (var list in _lists) 
					list.RemoveAt(index);
			}

			/// <inheritdoc />
			public string this[int index] {
				get => _lists[0][index];
				set => _lists[0][index] = value;
			}
		}
	}
}
