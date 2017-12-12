using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;

namespace InlaksIB
{
    public class ActiveDirectoryInterface
    {
        private readonly string _ldapPath;

        public ActiveDirectoryInterface(string ldapPath)
        {
            _ldapPath = ldapPath.Trim();
        }


        public bool Authenticate(string userID, string Password)
        {

            PrincipalContext pc = new PrincipalContext(ContextType.Domain, _ldapPath.Replace(@"LDAP://", ""));

            // validate the credentials
            bool valid = pc.ValidateCredentials(userID, Password);

            return valid;

        }
    }
}