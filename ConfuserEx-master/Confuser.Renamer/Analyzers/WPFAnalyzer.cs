using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Resources;
using System.Text.RegularExpressions;
using System.Web;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.Renamer.BAML;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.IO;

namespace Confuser.Renamer.Analyzers {
	internal class WPFAnalyzer : IRenamer {
		static readonly object BAMLKey = new object();

		static readonly Regex ResourceNamePattern = new Regex("^.*\\.g\\.resources$");
		internal static readonly Regex UriPattern = new Regex("^(?:PACK\\://(?:COMPONENT|APPLICATION)\\:,,,)?(?:/(.+?)(?:;V\\d+\\.\\d+\\.\\d+\\.\\d+)?;COMPONENT)?(/?[^/].*\\.[BX]AML)$");
		BAMLAnalyzer analyzer;

		internal Dictionary<string, List<IBAMLReference>> bamlRefs = new Dictionary<string, List<IBAMLReference>>(StringComparer.OrdinalIgnoreCase);
		public event Action<BAMLAnalyzer, BamlElement> AnalyzeBAMLElement;

		public void Analyze(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def) {
			var method = def as MethodDef;
			if (method != null) {
				if (!method.HasBody)
					return;
				AnalyzeMethod(context, service, method);
			}

			var module = def as ModuleDefMD;
			if (module != null) {
				AnalyzeResources(context, service, module);
			}
		}

		public void PreRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def) {
			if (!(def is ModuleDefMD module) || !parameters.GetParameter(context, def, "renXaml", true))
				return;

			var renameMode = parameters.GetParameter(context, def, "renXamlMode", RenameMode.Letters);
			if (renameMode < RenameMode.Letters) {
				var illegalValues = Enum.GetValues(typeof(RenameMode)).Cast<RenameMode>().Where(m => m < RenameMode.Letters);
				context.Logger.Warn("The renaming modes " + String.Join(", ", illegalValues) + " are not allowed for XAML resources. Letters mode will be used.");
				renameMode = RenameMode.Letters;
			}

			var wpfResInfo = context.Annotations.Get<Dictionary<string, Dictionary<string, BamlDocument>>>(module, BAMLKey);
			if (wpfResInfo == null)
				return;

			foreach (var res in wpfResInfo.Values)
				foreach (var doc in res.Values) {
					var decodedName = HttpUtility.UrlDecode(doc.DocumentName);
					var encodedName = doc.DocumentName;
					if (bamlRefs.TryGetValue(decodedName, out var references)) {
						var decodedDirectory = decodedName.Substring(0, decodedName.LastIndexOf('/') + 1);
						var encodedDirectory = encodedName.Substring(0, encodedName.LastIndexOf('/') + 1);

						var fileName = service.RandomName(renameMode).ToLowerInvariant();
						if (decodedName.EndsWith(".BAML", StringComparison.OrdinalIgnoreCase))
							fileName += ".baml";
						else if (decodedName.EndsWith(".XAML", StringComparison.OrdinalIgnoreCase))
							fileName += ".xaml";

						string decodedNewName = decodedDirectory + fileName;
						string encodedNewName = encodedDirectory + fileName;

						context.Logger.Debug(String.Format("Preserving virtual paths. Replaced {0} with {1}", decodedName, decodedNewName));

						bool renameOk = references.All(r => r.CanRename(module, decodedName, decodedNewName) || r.CanRename(module, encodedName, encodedNewName));

						if (renameOk) {
							foreach (var bamlRef in references) {
								bamlRef.Rename(module, decodedName, decodedNewName);
								bamlRef.Rename(module, encodedName, encodedNewName);
							}
							doc.DocumentName = encodedNewName;
						}
					}
				}
		}

		public void PostRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def) {
			var module = def as ModuleDefMD;
			if (module == null)
				return;

			var wpfResInfo = context.Annotations.Get<Dictionary<string, Dictionary<string, BamlDocument>>>(module, BAMLKey);
			if (wpfResInfo == null)
				return;

			var newResources = new List<EmbeddedResource>();

			foreach (EmbeddedResource res in module.Resources.OfType<EmbeddedResource>()) {
				Dictionary<string, BamlDocument> resInfo;

				if (!wpfResInfo.TryGetValue(res.Name, out resInfo))
					continue;

				var stream = new MemoryStream();
				var writer = new ResourceWriter(stream);

				var reader = new ResourceReader(res.CreateReader().AsStream());
				IDictionaryEnumerator enumerator = reader.GetEnumerator();
				while (enumerator.MoveNext()) {
					var name = (string)enumerator.Key;
					string typeName;
					byte[] data;
					reader.GetResourceData(name, out typeName, out data);

					BamlDocument document;
					if (resInfo.TryGetValue(name, out document)) {
						var docStream = new MemoryStream();
						docStream.Position = 4;
						BamlWriter.WriteDocument(document, docStream);
						docStream.Position = 0;
						docStream.Write(BitConverter.GetBytes((int)docStream.Length - 4), 0, 4);
						data = docStream.ToArray();
						name = document.DocumentName;
					}

					writer.AddResourceData(name, typeName, data);
				}
				writer.Generate();
				newResources.Add(new EmbeddedResource(res.Name, stream.ToArray(), res.Attributes));
			}

			foreach (EmbeddedResource res in newResources) {
				int index = module.Resources.IndexOfEmbeddedResource(res.Name);
				module.Resources[index] = res;
			}
		}

		void AnalyzeMethod(ConfuserContext context, INameService service, MethodDef method) {
			var dpRegInstrs = new List<Tuple<bool, Instruction>>();
			var routedEvtRegInstrs = new List<Instruction>();
			for (int i = 0; i < method.Body.Instructions.Count; i++) {
				Instruction instr = method.Body.Instructions[i];
				if ((instr.OpCode.Code == Code.Call || instr.OpCode.Code == Code.Callvirt)) {
					var regMethod = (IMethod)instr.Operand;

					if (regMethod.DeclaringType.FullName == "System.Windows.DependencyProperty" &&
						regMethod.Name.String.StartsWith("Register")) {
						dpRegInstrs.Add(Tuple.Create(regMethod.Name.String.StartsWith("RegisterAttached"), instr));
					}
					else if (regMethod.DeclaringType.FullName == "System.Windows.EventManager" &&
							 regMethod.Name.String == "RegisterRoutedEvent") {
						routedEvtRegInstrs.Add(instr);
					}
				}
				else if (instr.OpCode.Code == Code.Newobj) {
					var methodRef = (IMethod)instr.Operand;

					if (methodRef.DeclaringType.FullName == "System.Windows.Data.PropertyGroupDescription" &&
						methodRef.Name == ".ctor" && i - 1 >= 0 && method.Body.Instructions[i - 1].OpCode.Code == Code.Ldstr) {
						foreach (var property in analyzer.LookupProperty((string)method.Body.Instructions[i - 1].Operand))
							service.SetCanRename(property, false);
					}
				}
				else if (instr.OpCode == OpCodes.Ldstr) {
					var operand = ((string)instr.Operand).ToUpperInvariant();
					if (operand.EndsWith(".BAML") || operand.EndsWith(".XAML")) {
						var match = UriPattern.Match(operand);
						var refModule = method.Module;
						if (match.Success) {
							var resourceAssemblyName = match.Groups[1].Success ? match.Groups[1].Value : string.Empty;
							// Check if the expression contains a resource name (group 1)
							// If it does, check if it is this assembly.
							if (!string.IsNullOrWhiteSpace(resourceAssemblyName) &&
								!resourceAssemblyName.Equals(method.Module.Assembly.Name.String, StringComparison.OrdinalIgnoreCase)) {
								// Let's see if we can find this assembly.
								refModule = context.Modules.FirstOrDefault(m =>
									resourceAssemblyName.Equals(m.Assembly.Name.String,
										StringComparison.OrdinalIgnoreCase));

								if (refModule == null) {
									// This resource points to an assembly that is not part of the obfuscation.
									// Leave it alone!
									return;
								}
							}
							operand = match.Groups[2].Value;
						}
						else if (operand.Contains("/"))
							context.Logger.WarnFormat("Fail to extract XAML name from '{0}'.", instr.Operand);

						var reference = new BAMLStringReference(refModule, instr);
						operand = WebUtility.UrlDecode(operand.TrimStart('/'));
						var baml = operand.Substring(0, operand.Length - 5) + ".BAML";
						var xaml = operand.Substring(0, operand.Length - 5) + ".XAML";
						bamlRefs.AddListEntry(baml, reference);
						bamlRefs.AddListEntry(xaml, reference);
					}
				}
			}

			if (dpRegInstrs.Count == 0)
				return;

			var traceSrv = context.Registry.GetService<ITraceService>();
			MethodTrace trace = traceSrv.Trace(method);

			bool erred = false;
			foreach (var instrInfo in dpRegInstrs) {
				int[] args = trace.TraceArguments(instrInfo.Item2);
				if (args == null) {
					if (!erred)
						context.Logger.WarnFormat("Failed to extract dependency property name in '{0}'.", method.FullName);
					erred = true;
					continue;
				}
				Instruction ldstr = method.Body.Instructions[args[0]];
				if (ldstr.OpCode.Code != Code.Ldstr) {
					if (!erred)
						context.Logger.WarnFormat("Failed to extract dependency property name in '{0}'.", method.FullName);
					erred = true;
					continue;
				}

				var name = (string)ldstr.Operand;
				TypeDef declType = method.DeclaringType;
				bool found = false;
				if (instrInfo.Item1) // Attached DP
				{
					MethodDef accessor;
					if ((accessor = declType.FindMethod("Get" + name)) != null && accessor.IsStatic) {
						service.SetCanRename(accessor, false);
						found = true;
					}
					if ((accessor = declType.FindMethod("Set" + name)) != null && accessor.IsStatic) {
						service.SetCanRename(accessor, false);
						found = true;
					}
				}

				// Normal DP
				// Find CLR property for attached DP as well, because it seems attached DP can be use as normal DP as well.
				PropertyDef property = null;
				if ((property = declType.FindProperty(name)) != null) {
					service.SetCanRename(property, false);

					found = true;
					if (property.GetMethod != null)
						service.SetCanRename(property.GetMethod, false);

					if (property.SetMethod != null)
						service.SetCanRename(property.SetMethod, false);

					if (property.HasOtherMethods) {
						foreach (MethodDef accessor in property.OtherMethods)
							service.SetCanRename(accessor, false);
					}
				}
				if (!found) {
					if (instrInfo.Item1)
						context.Logger.WarnFormat("Failed to find the accessors of attached dependency property '{0}' in type '{1}'.",
												  name, declType.FullName);
					else
						context.Logger.WarnFormat("Failed to find the CLR property of normal dependency property '{0}' in type '{1}'.",
												  name, declType.FullName);
				}
			}

			erred = false;
			foreach (Instruction instr in routedEvtRegInstrs) {
				int[] args = trace.TraceArguments(instr);
				if (args == null) {
					if (!erred)
						context.Logger.WarnFormat("Failed to extract routed event name in '{0}'.", method.FullName);
					erred = true;
					continue;
				}
				Instruction ldstr = method.Body.Instructions[args[0]];
				if (ldstr.OpCode.Code != Code.Ldstr) {
					if (!erred)
						context.Logger.WarnFormat("Failed to extract routed event name in '{0}'.", method.FullName);
					erred = true;
					continue;
				}

				var name = (string)ldstr.Operand;
				TypeDef declType = method.DeclaringType;

				EventDef eventDef = null;
				if ((eventDef = declType.FindEvent(name)) == null) {
					context.Logger.WarnFormat("Failed to find the CLR event of routed event '{0}' in type '{1}'.",
											  name, declType.FullName);
					continue;
				}
				service.SetCanRename(eventDef, false);

				if (eventDef.AddMethod != null)
					service.SetCanRename(eventDef.AddMethod, false);

				if (eventDef.RemoveMethod != null)
					service.SetCanRename(eventDef.RemoveMethod, false);

				if (eventDef.InvokeMethod != null)
					service.SetCanRename(eventDef.InvokeMethod, false);

				if (eventDef.HasOtherMethods) {
					foreach (MethodDef accessor in eventDef.OtherMethods)
						service.SetCanRename(accessor, false);
				}
			}
		}

		void AnalyzeResources(ConfuserContext context, INameService service, ModuleDefMD module) {
			if (analyzer == null) {
				analyzer = new BAMLAnalyzer(context, service);
				analyzer.AnalyzeElement += AnalyzeBAMLElement;
			}

			var wpfResInfo = new Dictionary<string, Dictionary<string, BamlDocument>>();

			foreach (EmbeddedResource res in module.Resources.OfType<EmbeddedResource>()) {
				Match match = ResourceNamePattern.Match(res.Name);
				if (!match.Success)
					continue;

				var resInfo = new Dictionary<string, BamlDocument>();

				var reader = new ResourceReader(res.CreateReader().AsStream());
				IDictionaryEnumerator enumerator = reader.GetEnumerator();
				while (enumerator.MoveNext()) {
					var name = (string)enumerator.Key;
					if (!name.EndsWith(".baml"))
						continue;

					string typeName;
					byte[] data;
					reader.GetResourceData(name, out typeName, out data);
					BamlDocument document = analyzer.Analyze(module, name, data);
					document.DocumentName = name;
					resInfo.Add(name, document);
				}

				if (resInfo.Count > 0)
					wpfResInfo.Add(res.Name, resInfo);
			}
			if (wpfResInfo.Count > 0)
				context.Annotations.Set(module, BAMLKey, wpfResInfo);
		}
	}
}
