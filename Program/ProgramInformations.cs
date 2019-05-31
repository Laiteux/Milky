using Milky.Utils;
using MilkyNet;

namespace Milky.Program
{
    public class ProgramInformations
    {
        private RequestUtils _requestUtils;

        public string
            _name = null,
            _version = null,
            _author = null;
        
        public void SetProgramName(string name)
            => _name = name;

        public void SetProgramVersion(string version)
            => _version = version;

        public void SetProgramAuthor(string author, string authorURL = null)
        {
            _requestUtils = RequestUtils.GetOrNewInstance();

            if (authorURL != null)
            {
                try
                {
                    author = _requestUtils.Execute(new MilkyRequest(), HttpMethod.GET, authorURL).ToString();
                }
                catch { }
            }

            _author = author;
        }

        private static ProgramInformations _classInstance;
        public static ProgramInformations GetOrNewInstance()
        {
            return _classInstance ?? (_classInstance = new ProgramInformations());
        }
    }
}