using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confuser.Core;
using dnlib.DotNet;
using dnlib.DotNet.Pdb;

namespace Confuser.Renamer {
	internal class RenamePhase : ProtectionPhase {
		public RenamePhase(NameProtection parent)
			: base(parent) { }

		public override ProtectionTargets Targets {
			get { return ProtectionTargets.AllDefinitions; }
		}

		public override string Name {
			get { return "Renaming"; }
		}

		protected override void Execute(ConfuserContext context, ProtectionParameters parameters) {
			var service = (NameService)context.Registry.GetService<INameService>();

			context.Logger.Debug("Renaming...");
			foreach (IRenamer renamer in service.Renamers) {
				foreach (IDnlibDef def in parameters.Targets)
					renamer.PreRename(context, service, parameters, def);
				context.CheckCancellation();
			}

			var targets = parameters.Targets.ToList();
			service.GetRandom().Shuffle(targets);
			var pdbDocs = new HashSet<string>();
			foreach (IDnlibDef def in GetTargetsWithDelay(targets, context, service).WithProgress(targets.Count, context.Logger)) {
				if (def is ModuleDef && parameters.GetParameter(context, def, "rickroll", false))
					RickRoller.CommenceRickroll(context, (ModuleDef)def);

				bool canRename = service.CanRename(def);
				RenameMode mode = service.GetRenameMode(def);

				if (def is MethodDef) {
					var method = (MethodDef)def;
					if ((canRename || method.IsConstructor) && parameters.GetParameter(context, def, "renameArgs", true)) {
						foreach (ParamDef param in ((MethodDef)def).ParamDefs)
							param.Name = null;
					}

					if (parameters.GetParameter(context, def, "renPdb", false) && method.HasBody) {
						foreach (var instr in method.Body.Instructions) {
							if (instr.SequencePoint != null && !pdbDocs.Contains(instr.SequencePoint.Document.Url)) {
								instr.SequencePoint.Document.Url = service.ObfuscateName(instr.SequencePoint.Document.Url, mode);
								pdbDocs.Add(instr.SequencePoint.Document.Url);
							}
						}
						foreach (var local in method.Body.Variables) {
							if (!string.IsNullOrEmpty(local.Name))
								local.Name = service.ObfuscateName(local.Name, mode);
						}

						if (method.Body.HasPdbMethod)
							method.Body.PdbMethod.Scope = new PdbScope();
					}
				}

				if (!canRename)
					continue;

				service.SetIsRenamed(def);

				IList<INameReference> references = service.GetReferences(def);
				bool cancel = references.Any(r => r.ShouldCancelRename);
				if (cancel)
					continue;

				if (def is TypeDef) {
					var typeDef = (TypeDef)def;
					if (parameters.GetParameter(context, def, "flatten", true)) {
						typeDef.Name = service.ObfuscateName(typeDef.FullName, mode);
						typeDef.Namespace = "";
					}
					else {
						var nsFormat = parameters.GetParameter(context, def, "nsFormat", "{0}");
						typeDef.Namespace = service.ObfuscateName(nsFormat, typeDef.Namespace, mode);
						typeDef.Name = service.ObfuscateName(typeDef.Name, mode);
					}
					foreach (var param in typeDef.GenericParameters)
						param.Name = ((char)(param.Number + 1)).ToString();
				}
				else if (def is MethodDef) {
					foreach (var param in ((MethodDef)def).GenericParameters)
						param.Name = ((char)(param.Number + 1)).ToString();

					def.Name = service.ObfuscateName(def.Name, mode);
				}
				else
					def.Name = service.ObfuscateName(def.Name, mode);

				int updatedReferences = -1;
				do {
					var oldUpdatedCount = updatedReferences;
					// This resolves the changed name references and counts how many were changed.
					var updatedReferenceList = references.Where(refer => refer.UpdateNameReference(context, service)).ToArray();
					updatedReferences = updatedReferenceList.Length;
					if (updatedReferences == oldUpdatedCount) {
						var errorBuilder = new StringBuilder();
						errorBuilder.AppendLine("Infinite loop detected while resolving name references.");
						errorBuilder.Append("Processed definition: ").AppendDescription(def, service).AppendLine();
						errorBuilder.Append("Assembly: ").AppendLine(context.CurrentModule.FullName);
						errorBuilder.AppendLine("Faulty References:");
						foreach (var reference in updatedReferenceList) {
							errorBuilder.Append(" - ").AppendLine(reference.ToString(service));
						}
						context.Logger.Error(errorBuilder.ToString().Trim());
						throw new ConfuserException();
					}
					context.CheckCancellation();
				} while (updatedReferences > 0);
			}
		}

		private static IEnumerable<IDnlibDef> GetTargetsWithDelay(IList<IDnlibDef> definitions, ConfuserContext context, INameService service) {
			var delayedItems = new List<IDnlibDef>();
			var currentList = definitions;
			var lastCount = -1;
			while (currentList.Any()) {
				foreach (var def in currentList) {
					if (service.GetReferences(def).Any(r => r.DelayRenaming(service)))
						delayedItems.Add(def);
					else
						yield return def;
				}

				if (delayedItems.Count == lastCount) {
					var errorBuilder = new StringBuilder();
					errorBuilder.AppendLine("Failed to rename all targeted members, because the references are blocking each other.");
					errorBuilder.AppendLine("Remaining definitions: ");
					foreach (var def in delayedItems) {
						errorBuilder.Append("• ").AppendDescription(def, service).AppendLine();
					}
					context.Logger.Warn(errorBuilder.ToString().Trim());
					yield break;
				}
				lastCount = delayedItems.Count;
				currentList = delayedItems;
				delayedItems = new List<IDnlibDef>();
			}
		}
	}
}
