# Milky Library

A library to easily create your own credential stuffing programs.

## Documentation

You will find here pretty everything on how to use the library and its functionalities, features.

### MilkyManager

I highly suggest to create a class named ``MilkyManager.cs``  like [this one](https://github.com/Laiteux/Milky-Library/blob/master/MilkyManager.cs) in your project, don't forget to edit the namespace as well.

### Initializing

First of all, you have to initialize a ``MilkyManager`` instance, and your program informations.
Let's call this instance ``Milky`` so it's easier for the following examples.

```csharp
MilkyManager Milky = new MilkyManager();
Milky.ProgramManager.Initialize("Minecraft Checker", "1.0.0", "Laiteux");
```

### Lists

There are pre-made methods to make the user import a combo-list and a proxy-list, using an ``OpenFileDialog``.

```csharp
Milky.FileUtils.LoadCombos(ComboType.Any);
Milky.FileUtils.LoadProxies();
```

### Run Settings

Milky allows you to set different run settings, that you will be able to call later.

#### Threads
```csharp
Milky.RunSettings.threads = 100;
```

#### Proxy Protocol
```csharp
Milky.RunSettings.proxyProtocol = "HTTP";
```

#### Proxy Timeout
```csharp
Milky.RunSettings.proxyTimeout = 5000;
```

### User Input

To make an user choose his settings, there are 3 built-in methods you need to know.

#### To ask the user for a String input
```csharp
string username = Milky.UserUtils.AskString("Username");
```

#### To ask the user for an Integer input
```csharp
int threads = Milky.UserUtils.AskInteger("Threads");
```

#### To ask the user to make a choice
```csharp
string proxyProtocol = Milky.UserUtils.AskChoice("Proxy Protocol", new string[] { "HTTP", "SOCKS4", "SOCKS5" });
```

### Console Settings

There are only console title settings available for now.
These let you edit your console title format depending on the run status.
You can find the default values below.

```csharp
Milky.ConsoleSettings.idleTitleFormat = "%program.name% %program.version% by %program.author%",
Milky.ConsoleSettings.runningTitleFormat =
	"%program.name% %program.version% by %program.author% - Running | " +
	"Ran : %run.ran% - Remaining : %run.remaining% - Hits : %run.hits% | " +
	"RPM : %statistics.rpm% - Elapsed : %statistics.elapsed% - Estimated : %statistics.estimated%",
Milky.ConsoleSettings.finishedTitleFormat =
	"%program.name% %program.version% by %program.author% - Finished | " +
	"Ran : %run.ran% - Hits : %run.hits% | " +
	"Elapsed : %statistics.elapsed%";
```

You can also choose to display or not "Free" Hits, and to show or not percentages (Example : ``Ran : 100 (10,0%)``)
```csharp
Milky.ConsoleSettings.SetTitleStyle(bool showFree, bool showPercentages);
```

#### Values you can use

##### %program.name%
Program's name (``Milky.ProgramInformations.name`` : ``Minecraft Checker``)
##### %program.version%
Program's version (``Milky.ProgramInformations.version`` : ``1.0.0``)
##### %program.author%
Program's author (``Milky.ProgramInformations.author`` : ``Laiteux``)

##### %lists.combos%
Size of loaded combo-list
##### %lists.proxies%
Size of loaded proxy-list

##### %run.ran%
Count of ran combo-lines
##### %run.remaining%
Count of remaining/left combo-lines to be ran
##### %run.hits%
Count of combo-hits
##### %run.free%
Count of free combo-hits

##### %statistics.rpm%
RPM means Ran Per Minute, same as CPM (Checked Per Minute)
##### %statistics.elapsed%
Elapsed Time (``TimeSpan.FromSeconds`` Format : ``00:00:00``)
##### %statistics.estimated%
Estimated remaining/left time (``TimeSpan.FromSeconds`` Format : ``00:00:00``)

#### Percentage values
Format : ``0,0%``

##### %run.ran.percentage%
run.ran/lists.combos
##### %run.hits.percentage%
run.hits/run.ran
##### %run.free.percentage%
run.free/run.hits

### Output Settings

This is basically to edit your output/result format, available values are all below.

```csharp
Milky.OutputSettings.outputFormat = "%combo%";
Milky.OutputSettings.outputWithCaptureFormat = "%combo% %separator% %capture%";
Milky.OutputSettings.captureFormat = "%name% = %value%";
Milky.OutputSettings.comboCaptureSeparator = "|";
Milky.OutputSettings.capturesSeparator = " | ";
```

### Custom Statistics

Custom statistics allows you to store, update/edit and increment a value that you can re-use, display ... anywhere.

To create a custom statistic, you have to give it an alias (which will be used to identify it later) and optionally a value (default = 0)
```csharp
Milky.CustomStatistics.AddCustomStatistic("totalPoints", 0);
```

To update a custom statistic, you have to identify it by its name, and choose the new value to set to it
```csharp
Milky.CustomStatistics.UpdateCustomStatistic("totalPoints", 10);
```

To increment a custom statistic, you have to identify it by its name, and choose the value to add up to it
```csharp
Milky.CustomStatistics.IncrementCustomStatistic("totalPoints", 100);
```
