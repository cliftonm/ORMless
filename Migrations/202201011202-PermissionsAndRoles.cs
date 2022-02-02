using FluentMigrator;

using Clifton;

namespace Migrations
{
    /*
        The idea here is that users have roles with certain permissions.
        Entities (tables) permissions associated with one or more roles.
        The user's permission on a particular entity is based on the user having a role that also exists for the entity.
        Multiple user roles and multiple entity roles resolve to the combination of all allowable permissions.
        No permissions are given for any table that does not exist in the Entity collection with the exception 
            of when the IsSysAdmin flag is set on a user.

        Example:
            Lets say we have tables that users in the Accounting department can access.
            And we have tables that users in the Sales department can access.
            We create two roles:
                Accounting
                Sales
            We map users accordingly to their role.  Some users might have both roles assigned.
            We map entities according to what role they are used for.
            An Accounting user would be able to access only Accounting tables, not Sales tables.
            Conversely, a Sales user would be able to access only Sales tables, not Accounting tables.

            This moves permissions out of specific endpoint calls (the usual implementation) and instead maps permissions to the entities themselves.
    */

    [Migration(202201011202)]
    public class _202201011202_PermissionsAndRoles : Migration
    {
        public override void Up()
        {
            Create.Table("Role")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("Name").AsString().NotNullable()
                .WithColumn("CanCreate").AsBoolean().NotNullable()
                .WithColumn("CanRead").AsBoolean().NotNullable()
                .WithColumn("CanUpdate").AsBoolean().NotNullable()
                .WithColumn("CanDelete").AsBoolean().NotNullable()
                .WithColumn("Deleted").AsBoolean().NotNullable();

            Create.Table("User")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("Username").AsString().NotNullable()
                .WithColumn("Password").AsString().NotNullable()
                
                // The salt can and should be stored right next to the salted and hashed password.  The Internet never lies, right?
                .WithColumn("Salt").AsString().Nullable()               

                .WithColumn("AccessToken").AsString().Nullable()
                .WithColumn("RefreshToken").AsString().Nullable()
                
                // Hmmm.  Dangerous?
                .WithColumn("IsSysAdmin").AsBoolean().NotNullable()     

                .WithColumn("LastLogin").AsDateTime().Nullable()
                .WithColumn("ExpiresIn").AsInt32().Nullable()
                .WithColumn("ExpiresOn").AsInt64().Nullable()
                .WithColumn("Deleted").AsBoolean().NotNullable();

            Create.Table("UserRole")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("RoleId").AsInt32().NotNullable().ForeignKey("Role", "ID")
                .WithColumn("UserId").AsInt32().NotNullable().ForeignKey("User", "ID")
                .WithColumn("Deleted").AsBoolean().NotNullable();

            Create.Table("Entity")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("TableName").AsString().NotNullable()
                .WithColumn("Deleted").AsBoolean().NotNullable();

            Create.Table("EntityRole")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("RoleId").AsInt32().NotNullable().ForeignKey("Role", "ID")
                .WithColumn("EntityId").AsInt32().NotNullable().ForeignKey("Entity", "ID")
                .WithColumn("Deleted").AsBoolean().NotNullable();

            var salt = Hasher.GenerateSalt();
            var pwd = "SysAdmin";
            var hashedPassword = Hasher.HashPassword(salt, pwd);

            Insert.IntoTable("User").Row(new { Username = "SysAdmin", Password = hashedPassword, Salt = salt, IsSysAdmin = true, Deleted = false });
        }

        public override void Down()
        {

        }
    }
}
