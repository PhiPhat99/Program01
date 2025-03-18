using System;
using System.Collections.Generic;
using Ivi.Visa.Interop;

namespace Program01
{
    public class ResourceManagers
    {
        private static readonly Dictionary<string, ResourceManagers> ResourceManager = new Dictionary<string, ResourceManagers>();

        public static ResourceManagers GetResourceManager(string DeviceName)
        {
            if (!ResourceManager.ContainsKey(DeviceName))
            {
                ResourceManager[DeviceName] = new ResourceManagers();
            }

            return ResourceManager[DeviceName];
        }

        private ResourceManagers() { }
    }
}
