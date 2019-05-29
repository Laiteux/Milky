using Milky.Utils;

namespace Milky.Program
{
    public class ProgramManager
    {
        private ProgramInformations _programInformations;
        private ConsoleUtils _consoleUtils;

        public void Initialize(string programName = null, string programVersion = null, string programAuthor = null, string authorURL = null)
        {
            _programInformations = ProgramInformations.GetOrNewInstance();
            _consoleUtils = ConsoleUtils.GetOrNewInstance();

            _programInformations.SetProgramName(programName);
            _programInformations.SetProgramVersion(programVersion);
            _programInformations.SetProgramAuthor(programAuthor, authorURL);

            _consoleUtils.UpdateTitle();
        }

        private static ProgramManager _classInstance;
        public static ProgramManager GetOrNewInstance()
        {
            return _classInstance ?? (_classInstance = new ProgramManager());
        }
    }
}