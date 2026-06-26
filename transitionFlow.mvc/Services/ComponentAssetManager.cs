
namespace TransitFlow.mvc.Services
{
    public class ComponentAssetManager : IComponentAssetManager
    {
        private readonly List<string> _styles = new List<string>();
        private readonly List<string> _scripts = new List<string>();

        public void RegisterStyle(string src)
        {
            if (!_styles.Contains(src))
            {
                _styles.Add(src);
            }
        }

        public void RegisterScript(string src)
        {
            if (!_scripts.Contains(src))
            {
                _scripts.Add(src);
            }
        }

        public List<string> GetStyles()
        {
            return _styles;
        }

        public List<string> GetScripts()
        {
            return _scripts;
        }
    }
}
