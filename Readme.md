## Getting Started

* Available on NuGet: https://www.nuget.org/packages/PureWeen.Maui.FixesAndWorkarounds

### Description 
- Copy the following folder to your project https://github.com/PureWeen/ShanedlerSamples/tree/main/ShanedlerSamples/Library
- then you can use the builder to opt into all workarounds or just pick a few


### Things Fixes
- Android TabbedPage navigation blank https://github.com/dotnet/maui/issues/9743
- iOS Toolbar back button isn't updating correctly
- TitleView on iOS fixes
- iOS will navigate to the next field now when you've set it to next.
- Modal measuring when you push a modal and type in a box
- Various Frame Issues

### Features added
- Android, tapping off into nothingess closes keyboard (need to use included behaviors for this to work)
- FocusAndOpenKeyboard if you want to open the keyboard

#### All workarounds

```C#
builder.ConfigureMauiWorkarounds();
```


#### Pick some

```C#
builder.ConfigureMauiWorkarounds(false);
builder.ConfigureShellWorkarounds();
builder.ConfigureTabbedPageWorkarounds();
builder.ConfigureEntryNextWorkaround();
```

