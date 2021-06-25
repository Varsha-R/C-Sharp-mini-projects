using Microsoft.WindowsAPICodePack.Net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace NetworkUtility.Utilities
{
    public class NetworkUtil
    {
        public static List<string> activeAdapterIds { get; private set; } = null;
        public bool IsNetworkConnectedToInternet = false;

        private List<string> GetActivePhysicalAdapters()
        {
            List<string> adapterIds = new List<string>();
            ManagementObjectSearcher mos = null;
            try
            {
                // filtering on the Manufacturer and PNPDeviceID not starting with "ROOT\"
                // Physical devices have PNPDeviceID starting with "PCI\" or something else besides "ROOT\"
                mos = new ManagementObjectSearcher(@"SELECT * 
                                     FROM   Win32_NetworkAdapter 
                                     WHERE  Manufacturer != 'Microsoft' 
                                            AND NOT PNPDeviceID LIKE 'ROOT\\%'");
                // Get the physical adapters and sort them by their index. 
                // This is needed because they're not sorted by default
                IList<ManagementObject> managementObjectList = mos.Get()
                                                                  .Cast<ManagementObject>()
                                                                  .OrderBy(p => Convert.ToUInt32(p.Properties["Index"].Value, CultureInfo.InvariantCulture))
                                                                  .ToList();

                // Let's just show all the properties for all physical adapters.
                foreach (ManagementObject mo in managementObjectList)
                {
                    foreach (PropertyData pd in mo.Properties)
                    {
                        if (pd.Name.Equals("GUID", StringComparison.InvariantCultureIgnoreCase))
                        {
                            string id = pd.Value.ToString();
                            id = id.Substring(1, id.Length - 2);
                            id = id.ToUpper(CultureInfo.InvariantCulture);
                            adapterIds.Add(id);
                        }
                    }
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                Console.WriteLine("Error occurred while fetching physical adapters" + e);
                return adapterIds;
            }
            finally
            {
                if (mos != null)
                {
                    mos.Dispose();
                }
            }
            return adapterIds;
        }

        public bool GetNetworkStatus()
        {
            try
            {
                if (activeAdapterIds == null)
                {
                    activeAdapterIds = GetActivePhysicalAdapters();
                }
                var networks = NetworkListManager.GetNetworks(NetworkConnectivityLevels.Connected);
                NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface netInterface in networkInterfaces)
                {
                    if (netInterface.OperationalStatus == OperationalStatus.Up && netInterface.NetworkInterfaceType != NetworkInterfaceType.Tunnel
                       && netInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    {
                        foreach (Microsoft.WindowsAPICodePack.Net.Network network in networks)
                        {
                            foreach (NetworkConnection conn in network.Connections)
                            {
                                string id = netInterface.Id;
                                id = id.Substring(1, id.Length - 2);
                                id = id.ToUpper(CultureInfo.InvariantCulture);

                                if (id.Equals(conn.AdapterId.ToString().ToUpper(CultureInfo.InvariantCulture), StringComparison.InvariantCultureIgnoreCase)
                                    && activeAdapterIds.Contains(id))
                                {
                                    IsNetworkConnectedToInternet = conn.Network.IsConnectedToInternet;
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            return IsNetworkConnectedToInternet;
        }

        public void SubscribeNetworkEvents()
        {
            NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(NetworkAvailabilityChangedEventHandler);
            NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(NetworkAddressChangedEventHandler);
        }

        private void NetworkAvailabilityChangedEventHandler(object sender, EventArgs e)
        {
            Console.WriteLine("Network Availability changed: NetworkListManager? "+ NetworkListManager.IsConnectedToInternet);
        }

        private void NetworkAddressChangedEventHandler(object sender, EventArgs e)
        {
            Console.WriteLine("Network Address changed: NetworkListManager? " + NetworkListManager.IsConnectedToInternet);
        }

    }
}
