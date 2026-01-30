# ShowMeTheXAML
A WPF component making it easy to show the corresponding XAML for WPF custom styles and controls. It was built out of a need to shows the XAML for the theme library [MaterialDesignInXamlToolkit](https://github.com/ButchersBoy/MaterialDesignInXamlToolkit). You can see it working in the [demo application](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit/releases) for MaterialDesignInXamlToolkit.

[![NuGet Status](http://img.shields.io/nuget/v/ShowMeTheXAML.svg?style=flat&label=ShowMeTheXAML)](https://www.nuget.org/packages/ShowMeTheXAML/)
[![NuGet Status](http://img.shields.io/nuget/v/ShowMeTheXAML.MSBuild.svg?style=flat&label=ShowMeTheXAML.MSBuild)](https://www.nuget.org/packages/ShowMeTheXAML.MSBuild/)
[![NuGet Status](http://img.shields.io/nuget/v/ShowMeTheXAML.AvalonEdit.svg?style=flat&label=ShowMeTheXAML.AvalonEdit)](https://www.nuget.org/packages/ShowMeTheXAML.AvalonEdit/)

[![AppVeyor](https://img.shields.io/appveyor/ci/Keboo/showmethexaml.svg)](https://ci.appveyor.com/project/Keboo/showmethexaml)

## Setup
1. Install the MSBuild package. `PM> Install-Package ShowMeTheXAML.MSBuild` This will also install the `ShowMeTheXAML` package as well.
2. Add the following code in your `App.xaml.cs`
```C#
protected override void OnStartup(StartupEventArgs e)
{
    XamlDisplay.Init(
        // optional list of assemblies to look for ShowMeTheXAML data from.
        // Automatically tries to load data from Assembly.GetEntryAssembly()
    );
    base.OnStartup(e);
}
```
`XamlDisplay.Init()` does not work under Native AOT. For Native AOT support, invoke `XamlDictionary.Init()` from your `App.xaml.cs`:
```C#
protected override void OnStartup(StartupEventArgs e)
{
    XamlDictionary.Init();
    base.OnStartup(e);
}
```
*Note*: the difference between `XamlDisplay.Init()` and `XamlDictionary.Init()` is that `XamlDictionary.Init()` will look for `XamlDictionary` from `Assembly.GetEntryAssembly()` and from the list of `Assembly` instances provided to `XamlDisplay.Init()`. `XamlDictionary.Init()` does *not*.  As such, if multiple assemblies contain `XamlDictionary` types, new public APIs will need to be added to each such assembly and explicitly invoked in order for the `XamlDictionary` data to be used.

3. (Optional) The default template is pretty basic. For a better looking style add the ShowMeTheXAML.AvalonEdit package. `PM> Install-Package ShowMeTheXAML.AvalonEdit`
In App.xaml include the resource dictionary.
```XAML
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="pack://application:,,,/ShowMeTheXAML.AvalonEdit;component/Themes/xamldisplayer.xaml" />
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

## Usage
```XAML
<smtx:XamlDisplay Key="SomeUniqueString">
  <StackPanel>
    <Button Content="Some Content" />
    <TextBlock Text="Text" />
  </StackPanel>
</smtx:XamlDisplay>
```

