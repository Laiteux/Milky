# Milky

Have you always wanted to make your own checkers ?

Milky is there to make your dream come true ! And also get you some profit for sure.

Milky is the first library allowing you to create a C# checker with only a few lines of code.

It manages everything such as console, user inputs, loops, output, statistics, requests, captures and much more.

This is a crazy time gain, and Milky is the library most of the best checkers on the market are using.

What are you waiting for ? Come and try it :D

## Purchase

To purchase Milky, contact [Laiteux#1337](https://discordapp.com/users/551547196047360020) on Discord.

Price is $50, I only accept Bitcoin and Amazon.com Gift Cards.

## Requirements

You will have to import them to your project references in order to get everything working well.

- [``MilkyNet.dll``](https://github.com/Laiteux/Milky/raw/master/Requirements/MilkyNet.dll)
- [``Newtonsoft.Json.dll``](https://github.com/Laiteux/Milky/raw/master/Requirements/Newtonsoft.Json.dll)

## Documentation

You will find here pretty much everything on how to use the library, its functionalities and features.

### MilkyManager

I highly suggest you create a class named ``MilkyManager.cs``  like [this one](https://github.com/Laiteux/Milky/blob/master/MilkyManager.cs) in your project, don't forget to edit the namespace as well.

This is what we are gonna use for the documentation.

### Initializing

First of all, make sure your ``Main`` class has ``[STAThread]`` attribute, that's required to communicate with COM components, and so use ``OpenFileDialog``.

To start, you have to initialize a ``MilkyManager`` instance, and your program informations.

Let's call this instance ``Milky`` so it's easier for the following examples.

```csharp
MilkyManager Milky = new MilkyManager();
Milky.ProgramManager.Initialize("LoL Checker", "1.0.0", "Laiteux");
```

You can also optionaly specify an url to retrieve author from, for example [https://pastebin.com/raw/QW82zeqi](https://pastebin.com/raw/QW82zeqi) which will return my Discord.

Like that :
```csharp
MilkyManager Milky = new MilkyManager();
Milky.ProgramManager.Initialize("LoL Checker", "1.0.0", "Laiteux", "https://pastebin.com/raw/QW82zeqi");
```
In case it fails retrieving author from the specified URL, static one will be used ("Laiteux" here).

### Authenticating

You have to authenticate to Milky Library with your Milky Key using ``Milky.Authentication.Authenticate()`` in order to be able to use it.
```csharp
Milky.Authentication.Authenticate("ExampleKey");
```
Please check if the method returns true before doing anything, because otherwise every call to a Milky method will print an error message in console, and also won't even work.

### Lists

There are pre-made methods to make the user import a combo-list and a proxy-list, using an ``OpenFileDialog``.

```csharp
Milky.FileUtils.LoadCombos();
Milky.FileUtils.LoadProxies("SOCKS5");
```
You can optionaly specify a combo type for ``LoadCombos``. This is only to tell the user about what kind of combos should he load, it won't filter them or anything.

You can optionaly specify a proxy type for ``LoadProxies`` (see example). This is only to tell the user about what kind of proxies should he load, it won't filter them or anything.

### Run Settings

Milky allows you to set different run settings that you will be able to call later.

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

### Output Settings

This is basically to edit your output/result format, available values are all below.

```csharp
Milky.OutputSettings.outputFormat = "%combo%";
Milky.OutputSettings.outputWithCaptureFormat = "%combo% %separator% %capture%";
Milky.OutputSettings.captureFormat = "%name% = %value%";
Milky.OutputSettings.comboCaptureSeparator = "|";
Milky.OutputSettings.capturesSeparator = " | ";
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

You can also choose to display or not "Free" Hits, and to show or not percentages (Example : ``Ran : 100 (10,00%)``)
```csharp
Milky.ConsoleSettings.SetTitleStyle(bool showFree, bool showPercentages);
```

#### Values you can use

**%program.name%** Program's name (``Milky.ProgramInformations.name`` : ``LoL Checker``)

**%program.version%** Program's version (``Milky.ProgramInformations.version`` : ``1.0.0``)

**%program.author%** Program's author (``Milky.ProgramInformations.author`` : ``Laiteux``)


**%lists.combos%** Size of loaded combo-list, count of loaded combo-lines

**%lists.proxies%** Size of loaded proxy-list, count of loaded proxies


**%run.ran%** Count of ran combo-lines

**%run.remaining%** Count of remaining/left combo-lines to be ran

**%run.hits%** Count of combo-hits

**%run.free%** Count of free combo-hits


**%statistics.rpm%** RPM means Ran Per Minute, same as CPM (Checked Per Minute)

**%statistics.elapsed%** Elapsed Time (``TimeSpan.FromSeconds`` Format : ``00:00:00``)

**%statistics.estimated%** Estimated remaining/left time (``TimeSpan.FromSeconds`` Format : ``00:00:00``)

#### Percentage values

Format : ``0,00%``

**%run.ran.percentage%** run.ran/lists.combos

**%run.hits.percentage%** run.hits/run.ran

**%run.free.percentage%** run.free/run.hits

### User Input

To make a user choose his settings, there are 3 built-in methods you need to know.

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

### Custom Statistics

Custom statistics are allowing you to store, update/edit and increment a value that you can re-use, display ... anywhere.

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

Tip : You can retrieve a custom statistic percentage / hits for your console title : ``%custom.totalPoints.percentage%``

### Run

#### Process

This is required, it will basically put your program in checking mode by setting the ``RunStatus`` to ``Running``, starting all loops.
```csharp
Milky.RunManager.StartRun();
```

You will then have to run through the user combo-list, process each combo and submit its result using ``Milky.RunManager.SubmitComboResult()``, this is an example of how to do it, with a ``Parallel.ForEach`` loop to multi-thread the process :

```csharp
Parallel.ForEach(Milky.RunLists.combos, new ParallelOptions { MaxDegreeOfParallelism = Milky.RunSettings.threads }, combo =>
{
    OutputType outputType = OutputType.Invalid;
	CaptureDictionary captures = new CaptureDictionary();

    // Your checking, capture process ...

    Milky.RunManager.SubmitComboResult(combo, outputType, captures);
});
```
Here, we are setting our ``OutputType`` to ``Invalid`` by default, you will then be able to set it to ``Hit`` or ``Free`` depending on your check.

Note that submitting a ``CaptureDictionary`` is optional, I did it in the example to show how these work but you don't have to send any if you don't want any capture.

This is the ``SubmitComboResult`` method so you can see what else you can do with it :
```csharp
void SubmitComboResult(string combo, OutputType outputType, CaptureDictionary captures = null, bool outputResult = true, string file = null, string directory = null)
```
``outputResult`` is to decide whether or not you wanna output the combo result in the console and in a file.

``file`` is the file name (.txt will automatically be added) you wanna output the combo and its capture in. ``null`` will be "Hits.txt" or "Free.txt" if ``OutputType`` is ``Free``.

``directory`` is the directory name we will write the file in, null will be formatted as such : ``Jan 01, 2019 - 20.30.00``


#### Capture

``CaptureDictionary`` is simply a ``Dictionary<string, string>``, it works and you can use it exactly the same.

Example to add a capture :
```csharp
captures.Add("Points", points.ToString())
```
Assuming ``points`` is an ``Integer``.

#### Finish

Once your checking process is done, you will call ``Milky.RunManager.FinishRun()`` which will set the ``RunStatus`` to ``Finished``.

Example :
```csharp
Milky.RunManager.FinishRun();
Thread.Sleep(-1);
```
Tip : You can add ``Thread.Sleep(-1)`` (see example)  at the very end of your code to prevent your program/console from closing/exiting.

### Utils

Milky Library also contains a lot of built-in utils that may be helpful to you, in case you don't wanna write them yourself.

Feel free to take a look at them by writing ``Milky.Utils.`` and checking proposals (space + enter).

## Examples

You can find some "Checker" Examples in the [Examples folder](https://github.com/Laiteux/Milky/tree/master/Examples).
