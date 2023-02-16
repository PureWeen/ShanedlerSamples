### Description 
- Copy the following folder to your project https://github.com/PureWeen/ShanedlerSamples/tree/main/ShanedlerSamples/Library
- then you can use the builder to opt into all workarounds or just pick a few


### Things Fixes
- Android TabbedPage navigation blank https://github.com/dotnet/maui/issues/9743
- iOS Toolbar back button isn't updating correctly
- TitleView on iOS fixes
- iOS will navigate to the next field now when you've set it to next.

### Features added
- Android, tapping off into nothingess closes keyboard (need to use included behaviors for this to work)


#### All workarounds

```C#
builder.ConfigureShanedler();
```


#### Pick some

```C#
builder.ConfigureShanedler(false);
builder.ConfigureShellWorkarounds();
builder.ConfigureTabbedPageWorkarounds();
builder.ConfigureEntryNextWorkaround();
```

