using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Text;

namespace SampleProject.Utils
{
    public class LDAPUtil
    {
        private const string DisplayNameProperty = "displayname";

        public static string GetActiveDirectoryDetails(string id, string propertyName)
        {
            // To extract only user id excluding the domain
            var input = id.Split(new[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);

            using (var entry = new DirectoryEntry("LDAP://" + input[0]))
            {
                var search = new DirectorySearcher(entry) {Filter = "(&(objectClass=user)(anr=" + input[1] + "))"};

                //search.PropertiesToLoad.Add("displayname");

                SearchResult result = search.FindOne();

                //var propertyNames = new string[result.Properties.PropertyNames.Count]; 
                //result.Properties.PropertyNames.CopyTo(propertyNames, 0);
                
                //var props = propertyNames.ToDictionary(name => name, name => result.Properties[name][0].ToString());

                var propertyValue = result.Properties[propertyName][0].ToString();
                return propertyValue;
            }
        }

        // Example method to use LDAP queries
        public static string GetFullName(string id)
        {
            // Fetching the LDAP query results and caching them as well
            var displayName = MemoryCacheUtil.AddOrGetExisting(id + DisplayNameProperty, 
                                  () => GetActiveDirectoryDetails(id, DisplayNameProperty));
            
            // converting "lastname, firstname" to "FirstName LastName"
            var names = displayName.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            return new StringBuilder(names[1]).Append(" ").Append(names[0]).ToString();
        }
    }
}
