# Milky

Have you always wanted to make your own checkers?

Milky is here to make your dream come true! (And also net you some profit for sure)

Milky is the first library allowing you to create a C# checker with only a few lines of code.

It manages everything such as console, user inputs, loops, output, statistics, requests, captures and much more.

This is a crazy time saver, which is why Milky is the most used library that the best checkers on the market are using.

No more asking people to code checkers for you, now you can do it by yourself!

## Purchase

To purchase Milky, contact [Laiteux#1337](https://discordapp.com/users/551547196047360020) on Discord.

Price is $50, I only accept Bitcoin and Amazon.com Gift Cards.

## Requirements

You will have to import the following to your project references in order to get everything working well.

- [``MilkyNet.dll``](https://github.com/Laiteux/Milky/raw/master/Requirements/MilkyNet.dll)
- [``Newtonsoft.Json.dll``](https://github.com/Laiteux/Milky/raw/master/Requirements/Newtonsoft.Json.dll)

## Documentation

Here you will find pretty much everything you need on how to use the library, its functionalities and features.

### MilkyManager

I highly suggest you create a class named ``MilkyManager.cs``  like [this one](https://github.com/Laiteux/Milky/blob/master/MilkyManager.cs) in your project, don't forget to edit the namespace as well.

This is what we are going to use for the documentation.

### Initializing

First of all, make sure your ``Main`` class has ``[STAThread]`` attribute, that's required to communicate with COM components, and so use ``OpenFileDialog``.

To start, you have to initialize a ``MilkyManager`` instance, and your program informations.

Let's call this instance ``Milky`` so it's easier for the following examples.

```csharp
MilkyManager Milky = new MilkyManager();
Milky.ProgramManager.Initialize("LoL Checker", "1.0.0", "Laiteux");
```

Optionally you can specify an url to retrieve author information from, for example [https://pastebin.com/raw/QW82zeqi](https://pastebin.com/raw/QW82zeqi) which will return my Discord.

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
Please check if the method returns true before doing anything else, otherwise every call to a Milky method will print an error message in console and won't process.

### User Input

To make a user choose settings, there are 3 built-in methods you need to know.

#### To ask the user for a String input

```csharp
string username = Milky.UserUtils.AskString("Username");
```

#### To ask the user for an Integer input

```csharp
int threads = Milky.UserUtils.AskInteger("Threads");
```

#### To ask the user to select a choice

```csharp
string proxyProtocol = Milky.UserUtils.AskChoice("Proxy Protocol", new string[] { "HTTP", "SOCKS4", "SOCKS5" });
```

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

### Custom Statistics

Custom statistics allow you to store, update/edit and increment a value that you can re-use, display, etc., anywhere.

To create a custom statistic, you have to give it an alias (which will be used to identify it later) and optionally a value (default : 0)
```csharp
Milky.CustomStatistics.AddCustomStatistic("totalPoints");
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
void Milky.ConsoleSettings.SetTitleStyle(bool showFree, bool showPercentages);
```

### Lists / Files

There are pre-made methods to make the user import a combo-list and a proxy-list, using an ``OpenFileDialog``.

```csharp
Milky.FileUtils.LoadCombos();
Milky.FileUtils.LoadProxies("SOCKS5");
```
You can optionally specify a combo type for ``LoadCombos``. This is only to tell the user what kind of combos they should load, it won't filter them or anything.

You can optionally specify a proxy type for ``LoadProxies`` (see example). This is only to tell the user what kind of proxies they should load, it won't filter them or anything.

#### Values you can use

- **%program.name%** : Program's name (``Milky.ProgramInformations.name`` : ``LoL Checker``)
- **%program.version%** : Program's version (``Milky.ProgramInformations.version`` : ``1.0.0``)
- **%program.author%** : Program's author (``Milky.ProgramInformations.author`` : ``Laiteux``)


- **%lists.combos%** : Size of loaded combo-list, count of loaded combo-lines
- **%lists.proxies%** : Size of loaded proxy-list, count of loaded proxies


- **%run.ran%** : Count of ran combo-lines
- **%run.remaining%** : Count of remaining/left combo-lines to be ran
- **%run.hits%** : Count of combo-hits
- **%run.free%** : Count of free combo-hits


- **%statistics.rpm%** : RPM means Ran Per Minute, same as CPM (Checked Per Minute)
- **%statistics.elapsed%** : Elapsed Time (``TimeSpan.FromSeconds`` Format : ``00:00:00``)
- **%statistics.estimated%** : Estimated remaining/left time (``TimeSpan.FromSeconds`` Format : ``00:00:00``)

#### Percentage values

Format : ``0,00%``

- **%run.ran.percentage%** : run.ran/lists.combos
- **%run.hits.percentage%** : run.hits/run.ran
- **%run.free.percentage%** : run.free/run.hits

### Run

#### Start

This is required, it will basically put your program in checking mode by setting the ``RunStatus`` to ``Running``, starting all loops.
```csharp
Milky.RunManager.StartRun();
```

#### Process

You will then have to run through the user combo-list and process each combo-line, this is an example of how to do it, with a ``Parallel.ForEach`` loop to multi-thread the process :
```csharp
Parallel.ForEach(Milky.RunLists.combos, new ParallelOptions { MaxDegreeOfParallelism = Milky.RunSettings.threads }, combo =>
{
    OutputType outputType = OutputType.Invalid;
    CaptureDictionary captures = new CaptureDictionary();

    // Your checking, capture process ...
});
```
Here, we are setting our ``OutputType`` to ``Invalid`` by default, you will then be able to set it to ``Hit`` or ``Free`` depending on your check.

##### Capture

``CaptureDictionary`` is simply a ``Dictionary<string, string>``, it works and you can use it exactly the same.

Example to add a capture :
```csharp
captures.Add("Points", points.ToString())
```
Assuming ``points`` is an ``Integer``.

#### Submit

Then you will have to submit your combo result using ``Milky.RunManager.SubmitComboResult()``
```csharp
Milky.RunManager.SubmitComboResult(combo, outputType, captures);
```

Note that submitting a ``CaptureDictionary`` is optional, I did it in the example to show how these work but you don't have to send any if you don't want any capture.

This is the ``SubmitComboResult`` method so you can see what else you can do with it :
```csharp
void SubmitComboResult(string combo, OutputType outputType, CaptureDictionary captures = null, bool outputResult = true, string file = null, string directory = null)
```
``outputResult`` is to decide whether or not you wanna output the combo result in the console and in a file.

``file`` is the file name (.txt will automatically be added) you wanna output the combo and its capture in. ``null`` will be "Hits.txt" or "Free.txt" if ``OutputType`` is ``Free``.

``directory`` is the directory name we will write the file in, null will be formatted as such : ``Jan 01, 2019 - 20.30.00``

#### Finish

Once your checking process is done, you will call ``Milky.RunManager.FinishRun()`` which will set the ``RunStatus`` to ``Finished``.

Example :
```csharp
Milky.RunManager.FinishRun();
Thread.Sleep(-1);
```
Tip : You can add ``Thread.Sleep(-1)`` (see example)  at the very end of your code to prevent your program/console from closing/exiting.

### Utils

Milky Library also contains a lot of built-in utils that may be helpful to you, if you don't wanna write them yourself.

#### Console

Can be accessed through ``Milky.Utils.ConsoleUtils``
```csharp
void Write(string text, ConsoleColor color = ConsoleColor.White)
void WriteLine(string text, ConsoleColor color = ConsoleColor.White)
void Exit(string message, ConsoleColor color = ConsoleColor.White, int delay = 3000) // Writes defined message then closes program
```

#### File

Can be accessed through ``Milky.Utils.FileUtils``
```csharp
void CreateFile(string file, string directory) // Creates defined file in defined directory
void WriteLine(string text, string file, string directory) // Writes a line in defined file in defined directory
```

#### Hash

Can be accessed through ``Milky.Utils.HashUtils``
```csharp
CreateMD5 // Converts a string to its equivalent representation that is hashed with the MD5 hash algorithm.
```

#### List

Can be accessed through ``Milky.Utils.ListUtils``
```csharp
string GetRandomCombo(ComboList combos = null) // Returns a random combo from defined ComboList or Milky.Lists.combos if null
string GetRandomProxy(ProxyList proxies = null) // Returns a random proxy from defined ProxyList or Milky.Lists.proxies if null
```

#### Request

Can be accessed through ``Milky.Utils.RequestUtils``
```csharp
MilkyRequest SetProxy(MilkyRequest request, string proxy = null, string protocol = null, int timeout = -1) // Adds a proxy to your request, will pick from Milky.RunSettings/Milky.Lists.proxies for null values
MilkyResponse Execute(MilkyRequest request, HttpMethod method, string url, string payload = null) // Executes a request and returns its response
```

#### String

Can be accessed through ``Milky.Utils.StringUtils``
```csharp
string RandomString(int length, string characters) // Creates a random string of defined length with defined characters
string RandomIPV4address() // Generates a random IPv4 address (1-255;0-255;1-255;0-255).
string Escape(string text) // Escapes a set of characters by replacing them with their escape codes (System.Text.RegularExpressions.Regex).
string Unescape(string text) // Converts any escaped characters in the input string.
string EncodeBase64(string text) // Converts a string to its equivalent representation that is encoded with base-64.
string DecodeBase64(string text) // Converts a string to its equivalent base-64 decoded.
int CountOccurences(string text, string find) // Returns the amount of occurences in a string
```

#### User

Can be accessed through ``Milky.Utils.UserUtils``
```csharp
string AskString(string asked) // Asks the user for a String input
int AskInteger(string asked) // Asks the user for an Integer input
string AskChoice(string asked, string[] choices) // Asks the user to select a choice
```

## Examples

You can find some "Checker" Examples in the [Examples folder](https://github.com/Laiteux/Milky/tree/master/Examples).

## Contribute

Yes you can contribute, satisfying everyone needs is my main goal for this library.

Any idea, feature, request, suggestion or anything else ? Feel free to contact [Laiteux#1337](https://discordapp.com/users/551547196047360020) on Discord.

You're welcome !
