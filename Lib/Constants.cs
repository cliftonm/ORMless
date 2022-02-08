namespace Lib
{
    public static class Constants
    {
        public const string CRLF = "\r\n";
        public const string AUTHORIZATION = "Authorization";
        public const string TOKEN_PREFIX = "Bearer";

        public const string VERSION_INFO = "VersionInfo";

        public const string ID = "ID";
        public const string DELETED = "Deleted";
        public const string TABLE_NAME_PARAM = "@_tableName";

        public const string AUDIT_INSERT = "Insert";
        public const string AUDIT_UPDATE = "Update";
        public const string AUDIT_DELETE = "Delete";

        public const int ONE_DAY_IN_SECONDS = 24 * 60 * 60;
        public const int REFRESH_VALID_DAYS = 90;

        public const string ENTITY_AUTHORIZATION_SCHEME = "entityAuthorization";

    }
}
