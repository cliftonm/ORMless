using FluentMigrator;

namespace DMS.Migrations
{
    [Migration(202201011201)]
    public class _202201011201_CreateTables : Migration
    {
        public override void Up()
        {
            Create.Table("Test")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("IntField").AsInt32().Nullable()
                .WithColumn("StringField").AsString().Nullable()
                .WithColumn("DateField").AsDate().Nullable()
                .WithColumn("DateTimeField").AsDateTime().Nullable()
                .WithColumn("TimeField").AsTime().Nullable()
                .WithColumn("BitField").AsBoolean().Nullable()
                .WithColumn("Deleted").AsBoolean().NotNullable();

            Create.Table("Audit")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("Entity").AsString().NotNullable()
                .WithColumn("EntityId").AsInt32().NotNullable()
                .WithColumn("RecordBefore").AsString(int.MaxValue).Nullable()
                .WithColumn("RecordAfter").AsString(int.MaxValue).Nullable()
                .WithColumn("Action").AsString().NotNullable()
                .WithColumn("ActionBy").AsString().NotNullable()
                .WithColumn("ActionDate").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("Deleted").AsBoolean().NotNullable();
        }

        public override void Down()
        {
        }
    }
}
