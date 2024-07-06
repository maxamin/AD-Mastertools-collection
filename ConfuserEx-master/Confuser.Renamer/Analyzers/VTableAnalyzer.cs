using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Confuser.Core;
using Confuser.Renamer.References;
using dnlib.DotNet;

namespace Confuser.Renamer.Analyzers {
	public class VTableAnalyzer : IRenamer {
		void IRenamer.Analyze(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def) {
			switch (def) {
				case TypeDef typeDef:
					Analyze(service, context.Modules, typeDef);
					break;
				case MethodDef methodDef:
					Analyze(service, context.Modules, methodDef);
					break;
			}
		}

		public static void Analyze(INameService service, ICollection<ModuleDefMD> modules, TypeDef type) {
			if (type.IsInterface)
				return;

			var vTbl = service.GetVTables()[type];
			foreach (var ifaceVTbl in vTbl.InterfaceSlots.Values) {
				foreach (var slot in ifaceVTbl) {
					if (slot.Overrides == null)
						continue;
					Debug.Assert(slot.Overrides.MethodDef.DeclaringType.IsInterface);
					// A method in base type can implements an interface method for a
					// derived type. If the base type/interface is not in our control, we should
					// not rename the methods.
					bool baseUnderCtrl = modules.Contains(slot.MethodDef.DeclaringType.Module as ModuleDefMD);
					bool ifaceUnderCtrl = modules.Contains(slot.Overrides.MethodDef.DeclaringType.Module as ModuleDefMD);
					if ((!baseUnderCtrl && ifaceUnderCtrl) || !service.CanRename(slot.MethodDef)) {
						service.SetCanRename(slot.Overrides.MethodDef, false);
					}
					else if (baseUnderCtrl && !ifaceUnderCtrl || !service.CanRename(slot.Overrides.MethodDef)) {
						service.SetCanRename(slot.MethodDef, false);
					}
				}
			}
		}

		public static void Analyze(INameService service, ICollection<ModuleDefMD> modules, MethodDef method) {
			if (!method.IsVirtual)
				return;

			var vTbl = service.GetVTables()[method.DeclaringType];
			var slots = vTbl.FindSlots(method).ToArray();

			IMemberDef discoveredBaseMemberDef = null;
			MethodDef discoveredBaseMethodDef = null;

			bool doesOverridePropertyOrEvent = false;
			var methodProp = method.DeclaringType.Properties.Where(p => BelongsToProperty(p, method));
			foreach (var prop in methodProp) {
				foreach (var baseMethodDef in FindBaseDeclarations(service, method)) {
					var basePropDef = baseMethodDef.DeclaringType.Properties.
						FirstOrDefault(p => BelongsToProperty(p, baseMethodDef) && String.Equals(p.Name, prop.Name, StringComparison.Ordinal));

					if (basePropDef is null) continue;

					// Name of property has to line up.
					CreateOverrideReference(service, prop, basePropDef);
					CreateSiblingReference(basePropDef, ref discoveredBaseMemberDef, service);

					// Method names have to line up as well (otherwise inheriting attributes does not work).
					CreateOverrideReference(service, method, baseMethodDef);
					CreateSiblingReference(baseMethodDef, ref discoveredBaseMethodDef, service);

					doesOverridePropertyOrEvent = true;
				}
			}

			discoveredBaseMemberDef = null;
			discoveredBaseMethodDef = null;

			var methodEvent = method.DeclaringType.Events.Where(e => BelongsToEvent(e, method));
			foreach (var evt in methodEvent) {
				foreach (var baseMethodDef in FindBaseDeclarations(service, method)) {
					var baseEventDef = baseMethodDef.DeclaringType.Events.
						FirstOrDefault(e => BelongsToEvent(e, baseMethodDef) && String.Equals(e.Name, evt.Name, StringComparison.Ordinal));

					if (baseEventDef is null) continue;

					// Name of event has to line up.
					CreateOverrideReference(service, evt, baseEventDef);
					CreateSiblingReference(baseEventDef, ref discoveredBaseMemberDef, service);

					// Method names have to line up as well (otherwise inheriting attributes does not work).
					CreateOverrideReference(service, method, baseMethodDef);
					CreateSiblingReference(baseMethodDef, ref discoveredBaseMethodDef, service);

					doesOverridePropertyOrEvent = true;
				}
			}

			if (!method.IsAbstract) {
				foreach (var slot in slots) {
					if (slot.Overrides == null)
						continue;

					SetupOverwriteReferences(service, modules, slot, method.Module);
				}
			}
			else if (!doesOverridePropertyOrEvent) {
				foreach (var baseMethodDef in FindBaseDeclarations(service, method)) {
					CreateOverrideReference(service, method, baseMethodDef);
				}
			}
		}

		static void CreateSiblingReference<T>(T basePropDef, ref T discoveredBaseMemberDef, INameService service) where T : class, IMemberDef {
			if (discoveredBaseMemberDef is null)
				discoveredBaseMemberDef = basePropDef;
			else {
				var references = service.GetReferences(discoveredBaseMemberDef)
					.OfType<MemberSiblingReference>()
					.ToArray();
				if (references.Length > 0) {
					discoveredBaseMemberDef = (T)references[0].OldestSiblingDef;
					foreach (var siblingRef in references.Skip(1)) {
						// Redirect all the siblings to the new oldest reference
						RedirectSiblingReferences(siblingRef.OldestSiblingDef, discoveredBaseMemberDef, service);
					}
				}

				// Check if the discovered base type is the current type. If so, nothing needs to be done.
				if (ReferenceEquals(basePropDef, discoveredBaseMemberDef)) return;

				service.AddReference(basePropDef, new MemberSiblingReference(basePropDef, discoveredBaseMemberDef));
				UpdateOldestSiblingReference(discoveredBaseMemberDef, basePropDef, service);
			}
		}

		static void UpdateOldestSiblingReference(IMemberDef oldestSiblingMemberDef, IMemberDef basePropDef, INameService service) {
			var reverseReference = service.GetReferences(oldestSiblingMemberDef).OfType<MemberOldestSiblingReference>()
				.SingleOrDefault();
			if (reverseReference is null) {
				service.AddReference(oldestSiblingMemberDef, new MemberOldestSiblingReference(oldestSiblingMemberDef, basePropDef));
				PropagateRenamingRestrictions(service, oldestSiblingMemberDef, basePropDef);
			}
			else if (!reverseReference.OtherSiblings.Contains(basePropDef)) {
				reverseReference.OtherSiblings.Add(basePropDef);
				PropagateRenamingRestrictions(service, reverseReference.OtherSiblings);
			}
		}

		static void RedirectSiblingReferences(IMemberDef oldMemberDef, IMemberDef newMemberDef, INameService service) {
			if (ReferenceEquals(oldMemberDef, newMemberDef)) return;

			var referencesToUpdate = service.GetReferences(oldMemberDef)
				.OfType<MemberOldestSiblingReference>()
				.SelectMany(r => r.OtherSiblings)
				.SelectMany(service.GetReferences)
				.OfType<MemberSiblingReference>()
				.Where(r => ReferenceEquals(r.OldestSiblingDef, oldMemberDef));

			foreach (var reference in referencesToUpdate) {
				reference.OldestSiblingDef = newMemberDef;
				UpdateOldestSiblingReference(newMemberDef, reference.ThisMemberDef, service);
			}
			UpdateOldestSiblingReference(newMemberDef, oldMemberDef, service);
		}

		static void CreateOverrideReference(INameService service, IMemberDef thisMemberDef, IMemberDef baseMemberDef) {
			var overrideRef = new MemberOverrideReference(thisMemberDef, baseMemberDef);
			service.AddReference(thisMemberDef, overrideRef);

			PropagateRenamingRestrictions(service, thisMemberDef, baseMemberDef);
		}

		static void PropagateRenamingRestrictions(INameService service, params object[] objects) =>
			PropagateRenamingRestrictions(service, (IList<object>)objects);

		static void PropagateRenamingRestrictions(INameService service, IList<object> objects) {
			if (!objects.All(service.CanRename)) {
				foreach (var o in objects) {
					service.SetCanRename(o, false);
				}
			}
			else {
				var minimalRenamingLevel = objects.Max(service.GetRenameMode);
				foreach (var o in objects) {
					service.ReduceRenameMode(o, minimalRenamingLevel);
				}
			}
		}

		private static IEnumerable<MethodDef> FindBaseDeclarations(INameService service, MethodDef method) {
			var unprocessed = new Queue<MethodDef>();
			unprocessed.Enqueue(method);

			var vTables = service.GetVTables();

			while (unprocessed.Any()) {
				var currentMethod = unprocessed.Dequeue();

				var vTbl = vTables[currentMethod.DeclaringType];
				var slots = vTbl.FindSlots(currentMethod).Where(s => s.Overrides != null);

				bool slotsExists = false;
				foreach (var slot in slots) {
					unprocessed.Enqueue(slot.Overrides.MethodDef);
					slotsExists = true;
				}
				
				if (!slotsExists && method != currentMethod)
					yield return currentMethod;
			}
		}

		private static bool BelongsToProperty(PropertyDef propertyDef, MethodDef methodDef) =>
			propertyDef.GetMethods.Contains(methodDef) || propertyDef.SetMethods.Contains(methodDef) ||
			(propertyDef.HasOtherMethods && propertyDef.OtherMethods.Contains(methodDef));

		private static bool BelongsToEvent(EventDef eventDef, MethodDef methodDef) =>
			Equals(eventDef.AddMethod, methodDef) || Equals(eventDef.RemoveMethod, methodDef) || Equals(eventDef.InvokeMethod, methodDef) ||
			(eventDef.HasOtherMethods && eventDef.OtherMethods.Contains(methodDef));

		private static void AddImportReference(INameService service, ICollection<ModuleDefMD> modules, ModuleDef module, MethodDef method, MemberRef methodRef) {
			if (method.Module != module && modules.Contains((ModuleDefMD)module)) {
				var declType = (TypeRef)methodRef.DeclaringType.ScopeType;
				service.AddReference(method.DeclaringType, new TypeRefReference(declType, method.DeclaringType));
				service.AddReference(method, new MemberRefReference(methodRef, method));

				var typeRefs = methodRef.MethodSig.Params.SelectMany(param => param.FindTypeRefs()).ToList();
				typeRefs.AddRange(methodRef.MethodSig.RetType.FindTypeRefs());
				typeRefs.AddRange(methodRef.DeclaringType.ToTypeSig().FindTypeRefs());
				foreach (var typeRef in typeRefs) {
					SetupTypeReference(service, modules, module, typeRef);
				}
			}
		}

		private static void SetupTypeReference(INameService service, ICollection<ModuleDefMD> modules, ModuleDef module, ITypeDefOrRef typeDefOrRef) {
			if (!(typeDefOrRef is TypeRef typeRef)) return;

			var def = typeRef.ResolveTypeDef();
			if (!(def is null) && def.Module != module && modules.Contains((ModuleDefMD)def.Module))
				service.AddReference(def, new TypeRefReference(typeRef, def));
		}

		private static void SetupSignatureReferences(INameService service, ICollection<ModuleDefMD> modules,
			ModuleDef module, GenericInstSig typeSig) {
			SetupSignatureReferences(service, modules, module, typeSig.GenericType);
			foreach (var genericArgument in typeSig.GenericArguments)
				SetupSignatureReferences(service, modules, module, genericArgument);
		}

		private static void SetupSignatureReferences(INameService service, ICollection<ModuleDefMD> modules, ModuleDef module, TypeSig typeSig) {
			var asTypeRef = typeSig.TryGetTypeRef();
			if (asTypeRef != null) {
				SetupTypeReference(service, modules, module, asTypeRef);
			}
		}

		private static void SetupOverwriteReferences(INameService service, ICollection<ModuleDefMD> modules, VTableSlot slot, ModuleDef module) {
			var methodDef = slot.MethodDef;
			var baseSlot = slot.Overrides;
			var baseMethodDef = baseSlot.MethodDef;

			var overrideRef = new OverrideDirectiveReference(slot, baseSlot);
			service.AddReference(methodDef, overrideRef);
			service.AddReference(slot.Overrides.MethodDef, overrideRef);

			var importer = new Importer(module, ImporterOptions.TryToUseTypeDefs);

			IMethodDefOrRef target;
			if (baseSlot.MethodDefDeclType is GenericInstSig declType) {
				MemberRef targetRef = new MemberRefUser(module, baseMethodDef.Name, baseMethodDef.MethodSig, declType.ToTypeDefOrRef());
				targetRef = importer.Import(targetRef);
				service.AddReference(baseMethodDef, new MemberRefReference(targetRef, baseMethodDef));
				SetupSignatureReferences(service, modules, module, targetRef.DeclaringType.ToTypeSig() as GenericInstSig);

				target = targetRef;
			}
			else {
				target = baseMethodDef;
				if (target.Module != module) {
					target = (IMethodDefOrRef)importer.Import(baseMethodDef);
					if (target is MemberRef memberRef)
						service.AddReference(baseMethodDef, new MemberRefReference(memberRef, baseMethodDef));
				}
			}

			if (target is MemberRef methodRef)
				AddImportReference(service, modules, module, baseMethodDef, methodRef);

			if (methodDef.Overrides.Any(impl => IsMatchingOverride(impl, target)))
				return;

			methodDef.Overrides.Add(new MethodOverride(methodDef, target));
		}

		private static bool IsMatchingOverride(MethodOverride methodOverride, IMethodDefOrRef targetMethod) {
			SigComparer comparer = default;

			var targetDeclTypeDef = targetMethod.DeclaringType.ResolveTypeDef();
			var overrideDeclTypeDef = methodOverride.MethodDeclaration.DeclaringType.ResolveTypeDef();
			if (!comparer.Equals(targetDeclTypeDef, overrideDeclTypeDef))
				return false;

			var targetMethodSig = targetMethod.MethodSig;
			var overrideMethodSig = methodOverride.MethodDeclaration.MethodSig;
			if (methodOverride.MethodDeclaration.DeclaringType is TypeSpec spec && spec.TypeSig is GenericInstSig genericInstSig) {
				overrideMethodSig = GenericArgumentResolver.Resolve(overrideMethodSig, genericInstSig.GenericArguments);
			}

			return comparer.Equals(targetMethodSig, overrideMethodSig);
		}

		public void PreRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def) {
			//
		}

		public void PostRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def) {
			var method = def as MethodDef;
			if (method == null || !method.IsVirtual || method.Overrides.Count == 0)
				return;

			method.Overrides
				  .RemoveWhere(impl => MethodEqualityComparer.CompareDeclaringTypes.Equals(impl.MethodDeclaration, method));
		}
	}
}
