using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Confuser.Core;
using Confuser.Protections.TypeScrambler.Scrambler.Analyzers;
using dnlib.DotNet;

namespace Confuser.Protections.TypeScrambler.Scrambler {
	internal sealed class ScannedMethod : ScannedItem {
		internal MethodDef TargetMethod { get; }

		private ContextAnalyzerFactory Analyzers { get; }

		private bool ScramblePublicMethods { get; }

		internal ScannedMethod(TypeService service, MethodDef target, bool scramblePublic) : base(target) {
			Debug.Assert(service != null, $"{nameof(service)} != null");
			Debug.Assert(target != null, $"{nameof(target)} != null");

			TargetMethod = target;
			ScramblePublicMethods = scramblePublic;

			Analyzers = new ContextAnalyzerFactory(this) {
				new MemberRefAnalyzer(),
				new TypeRefAnalyzer(),
				new MethodSpecAnalyzer(),
				new MethodDefAnalyzer(service)
			};
		}

		internal override void Scan() {
			// First we need to verify if it is actually acceptable to modify the method in any way.
			if (!CanScrambleMethod(TargetMethod, ScramblePublicMethods)) return;

			if (TargetMethod.HasBody) {
				foreach (var v in TargetMethod.Body.Variables) {
					RegisterGeneric(v.Type);
				}
			}

			if (TargetMethod.ReturnType != TargetMethod.Module.CorLibTypes.Void) {
				RegisterGeneric(TargetMethod.ReturnType);
			}
			foreach (var param in TargetMethod.Parameters.Where(ProcessParameter))
				RegisterGeneric(param.Type);

			if (TargetMethod.HasBody)
				foreach (var i in TargetMethod.Body.Instructions)
					if (i.Operand != null)
						Analyzers.Analyze(i);
		}

		private static bool CanScrambleMethod(MethodDef method, bool scramblePublic) {
			Debug.Assert(method != null, $"{nameof(method)} != null");

			// Unmanaged methods are very much not supported by this.
			if (!method.IsManaged) return false;

			// The entry point of the assembly must not be scrambled.
			if (method.IsEntryPoint()) return false;

			// Methods that are part of inheritance shouldn't be scrambled.
			if (method.HasOverrides || method.IsAbstract || method.IsVirtual) return false;

			// Constructors and properties will not be scrambled.
			// It may be possible for properties with some more investigation.
			if (method.IsConstructor || method.IsGetter || method.IsSetter) return false;

			// Resolving the references does not work in case the declaring type has generic paramters.
			// May be possible with some additional investigation.
			if (method.DeclaringType.HasGenericParameters) return false;

			// Skip methods with multiple overloaded signatures.
			if (method.DeclaringType.FindMethods(method.Name).Take(2).Count() > 1) return false;

			// Skip methods that are implementations of a interface.
			if (method.IsInterfaceImplementation()) return false;

			// Delegates are something we better don't scramble.
			if (method.DeclaringType.IsDelegate) return false;

			// COM imports do not like generics.
			if (method.DeclaringType.IsComImport()) return false;

			// Skip public visible methods is scrambling of public members is disabled.
			if (!scramblePublic && method.IsVisibleOutside()) return false;

			// PInvoke implementations won't work with this.
			if (method.IsPinvokeImpl) return false;

			return true;
		}

		private static bool ProcessParameter(Parameter parameter) {
			Debug.Assert(parameter != null, $"{nameof(parameter)} != null");

			// Only handle normal parameters.
			// The hidden this parameter is skipped, the return parameter is handled later.
			if (!parameter.IsNormalMethodParameter) return false;

			// Skip ref and out parameters.
			if (parameter.ParamDef?.IsOut == true) return false;

			return true;
		}

		protected override void PrepareGenerics(IEnumerable<GenericParam> scrambleParams) {
			Debug.Assert(scrambleParams != null, $"{nameof(scrambleParams)} != null");
			if (!IsScambled) return;

			TargetMethod.GenericParameters.Clear();
			foreach (var generic in scrambleParams)
				TargetMethod.GenericParameters.Add(generic);

			// The generic parameter count is not updated when adding stuff to the GenericParameters.
			// So we do that by hand.
			TargetMethod.MethodSig.GenParamCount = (ushort)TargetMethod.GenericParameters.Count;

			if (TargetMethod.HasBody) {
				foreach (var v in TargetMethod.Body.Variables) {
					v.Type = ConvertToGenericIfAvalible(v.Type);
				}
			}

			foreach (var parameter in TargetMethod.Parameters.Where(ProcessParameter)) {
				parameter.Type = ConvertToGenericIfAvalible(parameter.Type);
				Debug.Assert(parameter.Type == TargetMethod.MethodSig.Params[parameter.MethodSigIndex],
					$"{nameof(parameter)}.Type == {nameof(TargetMethod)}.MethodSig.Params[{nameof(parameter)}.MethodSigIndex]");
			}

			if (TargetMethod.ReturnType != TargetMethod.Module.CorLibTypes.Void)
				TargetMethod.ReturnType = ConvertToGenericIfAvalible(TargetMethod.ReturnType);

			Debug.Assert(TargetMethod.ReturnType == TargetMethod.MethodSig.RetType,
				$"{nameof(TargetMethod)}.ReturnType == {nameof(TargetMethod)}.MethodSig.RetType");

			// The generic flag is not automatically set. So we fix it by hand.
			TargetMethod.Signature.Generic = true;

			Debug.Assert(TargetMethod.Signature.Generic, $"({nameof(TargetMethod)}.Signature.Generic");
		}

		internal GenericInstMethodSig CreateGenericMethodSig(ScannedMethod from, TypeService srv, GenericInstMethodSig original = null) {
			var types = new List<TypeSig>(TrueTypes.Count);
			foreach (var trueType in TrueTypes) {
				if (trueType.IsGenericMethodParameter) {
					Debug.Assert(original != null, $"{nameof(original)} != null");

					var number = ((GenericSig)trueType).Number;
					Debug.Assert(number < original.GenericArguments.Count,
						$"{nameof(number)} < {nameof(original)}.GenericArguments.Count");
					var originalArgument = original.GenericArguments[(int)number];
					types.Add(originalArgument);
				} else if (from?.IsScambled == true) {
					types.Add(from.ConvertToGenericIfAvalible(trueType));
				} else if (trueType.ToTypeDefOrRef() is TypeDef def) {
					// I am sure there are cleaner and better ways to do this.
					var item = srv.GetItem(def);
					types.Add(item?.IsScambled == true ? item.CreateGenericTypeSig(null) : trueType);
				} else {
					types.Add(trueType);
				}
			}
			return new GenericInstMethodSig(types);
		}

		internal override IMemberDef GetMemberDef() => TargetMethod;

		internal override ClassOrValueTypeSig GetTarget() => TargetMethod.DeclaringType.TryGetClassOrValueTypeSig();
	}
}
