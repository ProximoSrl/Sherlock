using System.Threading.Tasks;

namespace Sherlock.Host.Models
{
    public sealed class NavigationLink
    {
        public NavigationLink(string name, string url)
        {
            Name = name;
            Url = url;
        }

        public string Name { get; private set; }
        public string Url { get; private set; }
    }
}