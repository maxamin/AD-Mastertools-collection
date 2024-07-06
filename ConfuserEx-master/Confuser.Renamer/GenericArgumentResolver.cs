﻿using System;
using System.Collections.Generic;
using dnlib.DotNet;

namespace Confuser.Renamer {
	/// <summary>
	///     Resolves generic arguments
	/// </summary>
	public ref struct GenericArgumentResolver {
		private GenericArguments _genericArguments;
		private RecursionCounter _recursionCounter;

		/// <summary>
		///     Resolves the type signature with the specified generic arguments.
		/// </summary>
		/// <param name="typeSig">The type signature.</param>
		/// <param name="typeGenArgs">The type generic arguments.</param>
		/// <returns>Resolved type signature.</returns>
		/// <exception cref="System.ArgumentException">No generic arguments to resolve.</exception>
		public static TypeSig Resolve(TypeSig typeSig, IList<TypeSig> typeGenArgs) {
			if (typeGenArgs == null) throw new ArgumentException("No generic arguments to resolve.");

			var resolver = new GenericArgumentResolver();
			resolver._genericArguments = new GenericArguments();
			resolver._recursionCounter = new RecursionCounter();
			resolver._genericArguments.PushTypeArgs(typeGenArgs);

			return resolver.ResolveGenericArgs(typeSig);
		}

		/// <summary>
		///     Resolves the method signature with the specified generic arguments.
		/// </summary>
		/// <param name="methodSig">The method signature.</param>
		/// <param name="typeGenArgs">The type generic arguments.</param>
		/// <returns>Resolved method signature.</returns>
		/// <exception cref="System.ArgumentException">No generic arguments to resolve.</exception>
		public static MethodSig Resolve(MethodSig methodSig, IList<TypeSig> typeGenArgs) {
			if (typeGenArgs == null)
				throw new ArgumentException("No generic arguments to resolve.");

			var resolver = new GenericArgumentResolver();
			resolver._genericArguments = new GenericArguments();
			resolver._recursionCounter = new RecursionCounter();
			resolver._genericArguments.PushTypeArgs(typeGenArgs);

			return resolver.ResolveGenericArgs(methodSig);
		}

		private bool ReplaceGenericArg(ref TypeSig typeSig) {
			var newTypeSig = _genericArguments.Resolve(typeSig);
			if (newTypeSig == typeSig) return false;

			typeSig = newTypeSig;
			return true;

		}

		private MethodSig ResolveGenericArgs(MethodSig sig) {
			if (sig == null)
				return null;
			if (!_recursionCounter.Increment())
				return null;

			var result = ResolveGenericArgs(new MethodSig(sig.GetCallingConvention()), sig);

			_recursionCounter.Decrement();
			return result;
		}

		private MethodSig ResolveGenericArgs(MethodSig sig, MethodSig old) {
			sig.RetType = ResolveGenericArgs(old.RetType);
			foreach (var p in old.Params)
				sig.Params.Add(ResolveGenericArgs(p));
			sig.GenParamCount = old.GenParamCount;
			if (sig.ParamsAfterSentinel != null) {
				foreach (var p in old.ParamsAfterSentinel)
					sig.ParamsAfterSentinel.Add(ResolveGenericArgs(p));
			}
			return sig;
		}

		private TypeSig ResolveGenericArgs(TypeSig typeSig) {
			if (!_recursionCounter.Increment())
				return null;

			if (ReplaceGenericArg(ref typeSig)) {
				_recursionCounter.Decrement();
				return typeSig;
			}

			TypeSig result;
			switch (typeSig.ElementType) {
				case ElementType.Ptr:
					result = new PtrSig(ResolveGenericArgs(typeSig.Next));
					break;
				case ElementType.ByRef:
					result = new ByRefSig(ResolveGenericArgs(typeSig.Next));
					break;
				case ElementType.Var:
					result = new GenericVar(((GenericVar) typeSig).Number);
					break;
				case ElementType.ValueArray:
					result = new ValueArraySig(ResolveGenericArgs(typeSig.Next), ((ValueArraySig) typeSig).Size);
					break;
				case ElementType.SZArray:
					result = new SZArraySig(ResolveGenericArgs(typeSig.Next));
					break;
				case ElementType.MVar:
					result = new GenericMVar((typeSig as GenericMVar).Number);
					break;
				case ElementType.CModReqd:
					result = new CModReqdSig((typeSig as ModifierSig).Modifier, ResolveGenericArgs(typeSig.Next));
					break;
				case ElementType.CModOpt:
					result = new CModOptSig((typeSig as ModifierSig).Modifier, ResolveGenericArgs(typeSig.Next));
					break;
				case ElementType.Module:
					result = new ModuleSig((typeSig as ModuleSig).Index, ResolveGenericArgs(typeSig.Next));
					break;
				case ElementType.Pinned:
					result = new PinnedSig(ResolveGenericArgs(typeSig.Next));
					break;
				case ElementType.FnPtr:
					throw new NotSupportedException("FnPtr is not supported.");

				case ElementType.Array:
					var arraySig = (ArraySig)typeSig;
					var sizes = new List<uint>(arraySig.Sizes);
					var lBounds = new List<int>(arraySig.LowerBounds);
					result = new ArraySig(ResolveGenericArgs(typeSig.Next), arraySig.Rank, sizes, lBounds);
					break;
				case ElementType.GenericInst:
					var gis = (GenericInstSig)typeSig;
					var genArgs = new List<TypeSig>(gis.GenericArguments.Count);
					foreach (var ga in gis.GenericArguments)
						genArgs.Add(ResolveGenericArgs(ga));
					
					result = new GenericInstSig(ResolveGenericArgs(gis.GenericType) as ClassOrValueTypeSig, genArgs);
					break;

				default:
					result = typeSig;
					break;
			}

			_recursionCounter.Decrement();

			return result;
		}
	}
}