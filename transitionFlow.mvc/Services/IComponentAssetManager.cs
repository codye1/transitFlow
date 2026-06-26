namespace TransitFlow.mvc.Services
{
    public interface IComponentAssetManager
    {
        void RegisterStyle(string src);
        void RegisterScript(string src);
        List<string> GetStyles();
        List<string> GetScripts();
    }
}
