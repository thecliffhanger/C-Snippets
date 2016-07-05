using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Text;

namespace SampleProject.Utils
{
    public class LDAPUtil
    {
        private const string DisplayNameProperty = "displayname";

        public static string GetActiveDirectoryDetails(string id, string domain, string propertyName)
        {
            using (var entry = new DirectoryEntry("LDAP://" + domain))
            {
                var search = new DirectorySearcher(entry) {Filter = "(&(objectClass=user)(anr=" + id + "))"};

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
        public static string GetFullName(string idwithDomain)
        {
            var input = idwithDomain.Split(new[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);

            // Fetching the LDAP query results and caching them as well
            var displayName = MemoryCacheUtil.AddOrGetExisting(id + DisplayNameProperty, 
                                  () => GetActiveDirectoryDetails(input[1], input[0], DisplayNameProperty));
            
            // converting "lastname, firstname" to "FirstName LastName"
            var names = displayName.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            return new StringBuilder(names[1]).Append(" ").Append(names[0]).ToString();
        }
    }
}
