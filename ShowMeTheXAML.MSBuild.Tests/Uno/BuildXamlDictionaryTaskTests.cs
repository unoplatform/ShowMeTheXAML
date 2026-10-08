using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Build.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShowMeTheXAML.MSBuild.Tests;

[TestClass]
public class BuildXamlDictionaryTaskTests
{
	// Uno 7.0 renamed Uno.UI.Toolkit to Uno.UI.Extras; the displayed XAML must carry the new namespace.
	private const string ExtrasPage = """
		<Page xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
		      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
		      xmlns:smtx="using:ShowMeTheXAML"
		      xmlns:extras="using:Uno.UI.Extras">
			<StackPanel>
				<smtx:XamlDisplay UniqueKey="Extras_Sample">
					<Grid extras:VisibleBoundsPadding.PaddingMask="Top">
						<TextBlock Text="Hello" />
					</Grid>
				</smtx:XamlDisplay>
			</StackPanel>
		</Page>
		""";

	[TestMethod]
	public void When_Uno_XamlDisplay_Then_Found()
	{
		var location = new BuildXamlDictionaryTask()
			.ParseXamlFile(XDocument.Parse(ExtrasPage), "Page.xaml")
			.Single();

		Assert.AreEqual("Extras_Sample", location.UniqueKey);
		StringAssert.Contains(location.XamlData, @"extras:VisibleBoundsPadding.PaddingMask=""""Top""""");
	}

	[TestMethod]
	public void When_Extras_Namespace_Used_Then_Declaration_Kept()
	{
		var location = new BuildXamlDictionaryTask()
			.ParseXamlFile(XDocument.Parse(ExtrasPage), "Page.xaml")
			.Single();

		StringAssert.Contains(location.XamlData, @"xmlns:extras=""""using:Uno.UI.Extras""""");
		Assert.IsFalse(location.XamlData.Contains("Uno.UI.Toolkit"));
	}

	[TestMethod]
	public void When_Wpf_XamlDisplay_Then_Ignored()
	{
		const string wpfPage = """
			<Page xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
			      xmlns:smtx="clr-namespace:ShowMeTheXAML;assembly=ShowMeTheXAML">
				<smtx:XamlDisplay UniqueKey="Wpf_Sample">
					<TextBlock Text="Hello" />
				</smtx:XamlDisplay>
			</Page>
			""";

		var locations = new BuildXamlDictionaryTask()
			.ParseXamlFile(XDocument.Parse(wpfPage), "Page.xaml");

		Assert.IsFalse(locations.Any());
	}

	[TestMethod]
	public void When_Executed_Then_Generated_Dictionary_Has_Extras_Xaml()
	{
		var directory = Directory.CreateTempSubdirectory("smtx-").FullName;
		try
		{
			var pagePath = Path.Combine(directory, "Page.xaml");
			File.WriteAllText(pagePath, ExtrasPage);

			BuildXamlDictionaryTask task = new()
			{
				PageMarkup = [new TaskItem(pagePath)],
				OutputPath = directory,
			};

			Assert.IsTrue(task.Execute());

			var generated = File.ReadAllText(task.GeneratedCodeFiles.Single().ItemSpec);
			StringAssert.Contains(generated, @"XamlResolver.Set(""Extras_Sample"", @""");
			StringAssert.Contains(generated, @"xmlns:extras=""""using:Uno.UI.Extras""""");
		}
		finally
		{
			Directory.Delete(directory, recursive: true);
		}
	}
}
