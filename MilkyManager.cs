using Milky.Loops;
using Milky.Output;
using Milky.Program;
using Milky.Run;
using Milky.Settings;
using Milky.Statistics;
using Milky.Utils;

namespace Milky_Checker
{
    class MilkyManager
    {
        public KeyboardListener KeyboardListener;
		
        public ConsoleLoops ConsoleLoops;
        public LoopsManager LoopsManager;
        public StatisticsLoops StatisticsLoops;
        
        public OutputSettings OutputSettings;
        
        public ProgramInformations ProgramInformations;
        public ProgramManager ProgramManager;
        
        public RunInformations RunInformations;
        public RunLists RunLists;
        public RunManager RunManager;
        
        public ConsoleSettings ConsoleSettings;
        public RunSettings RunSettings;
        
        public CustomStatistics CustomStatistics;
        public RunStatistics RunStatistics;
        
        public ConsoleUtils ConsoleUtils;
        public DateTimeUtils DateTimeUtils;
        public FileUtils FileUtils;
        public FormatUtils FormatUtils;
        public HashUtils HashUtils;
        public ListUtils ListUtils;
        public RequestUtils RequestUtils;
        public StringUtils StringUtils;
        public UserUtils UserUtils;
        
        public MilkyManager()
        {
            KeyboardListener = KeyboardListener.GetOrNewInstance();
            
            ConsoleLoops = ConsoleLoops.GetOrNewInstance();
            LoopsManager = LoopsManager.GetOrNewInstance();
            StatisticsLoops = StatisticsLoops.GetOrNewInstance();
            
            OutputSettings = OutputSettings.GetOrNewInstance();
            
            ProgramInformations = ProgramInformations.GetOrNewInstance();
            ProgramManager = ProgramManager.GetOrNewInstance();
            
            RunInformations = RunInformations.GetOrNewInstance();
            RunLists = RunLists.GetOrNewInstance();
            RunManager = RunManager.GetOrNewInstance();
            
            ConsoleSettings = ConsoleSettings.GetOrNewInstance();
            RunSettings = RunSettings.GetOrNewInstance();
            
            CustomStatistics = CustomStatistics.GetOrNewInstance();
            RunStatistics = RunStatistics.GetOrNewInstance();
            
            ConsoleUtils = ConsoleUtils.GetOrNewInstance();
            DateTimeUtils = DateTimeUtils.GetOrNewInstance();
            FileUtils = FileUtils.GetOrNewInstance();
            FormatUtils = FormatUtils.GetOrNewInstance();
            HashUtils = HashUtils.GetOrNewInstance();
            ListUtils = ListUtils.GetOrNewInstance();
            RequestUtils = RequestUtils.GetOrNewInstance();
            StringUtils = StringUtils.GetOrNewInstance();
            UserUtils = UserUtils.GetOrNewInstance();
        }
    }
}
