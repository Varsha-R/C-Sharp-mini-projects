using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneDriveFolderPath
{
    class Program
    {
        static void Main(string[] args)
        {
            string userFolderPath = @"C:\Users\Varsha.Ravindra\OneDrive\OD.exe";
            List<string> selectedPaths = new List<string>(new string[] {"C:", @"C:\Users", @"C:\Users\Varsha.Ravindra\OneDrive"}); //, @"C:\Users", @"C:\Users\Varsha.Ravindra", @"C:\Users\Varsha.Ravindra\source"
            List<string> unselectedPaths = new List<string>(new string[] { @"C:\Pro", @"C:\Users\Varsha.Ravindra\Programs" });// { @"C:\Users\public", @"C:\Users\Varsha.Ravindra\OneDrive" }

            string oneDriveRoot = Path.GetPathRoot(userFolderPath);
            int rootValue = selectedPaths.Count(x => x.StartsWith(oneDriveRoot, StringComparison.InvariantCultureIgnoreCase));
            bool userFolderInSelectedPaths = selectedPaths.Any(x => userFolderPath.StartsWith(x, StringComparison.InvariantCulture) 
                                            && !string.Equals(x, oneDriveRoot, StringComparison.InvariantCultureIgnoreCase)) 
                                            && !unselectedPaths.Any(x => userFolderPath.StartsWith(x, StringComparison.InvariantCulture));
            if (rootValue == 1 || userFolderInSelectedPaths)
            {
                //OD is selected
                //TODO: Send packet to OldPC to restart OneDrive
            }
            //bool userFolderNotInSelectedPaths = ;
            //if(userFolderNotInSelectedPaths)
            //{
            //    //OD is not selected
            //}
        }
    }
}
