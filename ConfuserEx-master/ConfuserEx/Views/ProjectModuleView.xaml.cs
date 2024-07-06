using System;
using System.Windows;
using System.Windows.Controls;
using ConfuserEx.ViewModel;
using Ookii.Dialogs.Wpf;

namespace ConfuserEx.Views {
	public partial class ProjectModuleView : Window {
		readonly ProjectModuleVM module;

		public ProjectModuleView(ProjectModuleVM module) {
			InitializeComponent();
			this.module = module;
			DataContext = module;
		}

		void Done(object sender, RoutedEventArgs e) {
			DialogResult = true;
		}

		void ChooseSNKey(object sender, RoutedEventArgs e) => 
			module.SNKeyPath = ChooseKey();

		void ChooseSNSigKey(object sender, RoutedEventArgs e) => 
			module.SNSigKeyPath = ChooseKey();

		void ChooseSNPublicKey(object sender, RoutedEventArgs e) =>
			module.SNPubKeyPath = ChooseKey();

		void ChooseSNPublicSigKey(object sender, RoutedEventArgs e) =>
			module.SNPubSigKeyPath = ChooseKey();

		string ChooseKey() {
			var ofd = new VistaOpenFileDialog {
				Filter = "Supported Key Files (*.snk, *.pfx)|*.snk;*.pfx|All Files (*.*)|*.*"
			};

			return ofd.ShowDialog() ?? false ? ofd.FileName : null;
		}
	}
}
