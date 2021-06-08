using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace FetchUserProfiles
{
    public static class WmiQueryExecutor
    {
        private static readonly Type classType = typeof(WmiQueryExecutor);
        public static IEnumerable<ManagementObject> RunQuery(string query)
        {
            ValidateArguements(query);

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
            {
                return searcher.Get().OfType<ManagementObject>();
            }
        }

        private static void ValidateArguements(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentNullException(nameof(query));
            }
        }
    }
}
