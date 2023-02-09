### Description 
- Copy the following folder to your project https://github.com/PureWeen/ShanedlerSamples/tree/main/ShanedlerSamples/Library
- then you can use the builder to opt into all workarounds or just pick a few

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

