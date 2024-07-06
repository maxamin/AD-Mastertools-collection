using System.IO;
using System.Linq;
using System.Xml;
using Confuser.Core;
using Confuser.Core.Project;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Confuser.MSBuild.Tasks {
	public sealed class ConfuseTask : Task {
		[Required]
		public ITaskItem Project { get; set; }

		[Required]
		public ITaskItem OutputAssembly { get; set; }

		[Output]
		public ITaskItem[] ConfusedFiles { get; set; }

		public override bool Execute() {
			var project = new ConfuserProject();
			var xmlDoc = new XmlDocument();
			xmlDoc.Load(Project.ItemSpec);
			project.Load(xmlDoc);
			project.OutputDirectory = Path.GetDirectoryName(Path.GetFullPath(OutputAssembly.ItemSpec));

			var logger = new MSBuildLogger(Log);
			var parameters = new ConfuserParameters {
				Project = project,
				Logger = logger
			};

			ConfuserEngine.Run(parameters).Wait();

			ConfusedFiles = project.Select(m => new TaskItem(Path.Combine(project.OutputDirectory, m.Path))).Cast<ITaskItem>().ToArray();

			return !logger.HasError;
		}
	}
}
