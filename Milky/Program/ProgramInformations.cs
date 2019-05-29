using Milky.Utils;
using MilkyNet;

namespace Milky.Program
{
    public class ProgramInformations
    {
        private RequestUtils _requestUtils;

        public string
            name = null,
            version = null,
            author = null;
        
        public void SetProgramName(string _name)
            => name = _name;

        public void SetProgramVersion(string _version)
            => version = _version;

        public void SetProgramAuthor(string _author, string authorURL = null)
        {
            _requestUtils = RequestUtils.GetOrNewInstance();

            if (authorURL != null)
            {
                try
                {
                    _author = _requestUtils.Execute(new MilkyRequest(), HttpMethod.GET, authorURL).ToString();
                }
                catch { }
            }

            author = _author;
        }

        private static ProgramInformations _classInstance;
        public static ProgramInformations GetOrNewInstance()
        {
            return _classInstance ?? (_classInstance = new ProgramInformations());
        }
    }
}