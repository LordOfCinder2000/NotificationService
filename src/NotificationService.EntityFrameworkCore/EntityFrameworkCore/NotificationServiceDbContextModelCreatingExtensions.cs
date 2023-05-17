using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NotificationService.Notifications;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.Identity;

namespace NotificationService.EntityFrameworkCore;

public static class NotificationServiceDbContextModelCreatingExtensions
{
    public static void ConfigureNotificationService(
        this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        /* Configure all entities here. Example:

        builder.Entity<Question>(b =>
        {
            //Configure table & schema name
            b.ToTable(NotificationServiceDbProperties.DbTablePrefix + "Questions", NotificationServiceDbProperties.DbSchema);

            b.ConfigureByConvention();

            //Properties
            b.Property(q => q.Title).IsRequired().HasMaxLength(QuestionConsts.MaxTitleLength);

            //Relations
            b.HasMany(question => question.Tags).WithOne().HasForeignKey(qt => qt.QuestionId);

            //Indexes
            b.HasIndex(q => q.CreationTime);
        });
        */
        builder.Entity<Notification>(b =>
        {
            //Configure table & schema name
            b.ToTable(NotificationServiceDbProperties.DbTablePrefix + "Notifications", NotificationServiceDbProperties.DbSchema);

            b.ConfigureByConvention();

            //Properties
            b.Property(q => q.NotificationName).IsRequired().HasMaxLength(NotificationServiceConsts.MaxNotificationNameLength);
            b.Property(q => q.Data).HasMaxLength(NotificationServiceConsts.MaxDataLength);
            b.Property(q => q.DataTypeName).HasMaxLength(NotificationServiceConsts.MaxDataTypeNameLength);
            b.Property(q => q.EntityTypeName).HasMaxLength(NotificationServiceConsts.MaxEntityTypeNameLength);
            b.Property(q => q.EntityTypeAssemblyQualifiedName).HasMaxLength(NotificationServiceConsts.MaxEntityTypeAssemblyQualifiedNameLength);
            b.Property(q => q.EntityId).HasMaxLength(NotificationServiceConsts.MaxEntityIdLength);
            b.Property(q => q.Severity).HasConversion(new EnumToStringConverter<NotificationSeverity>());
            b.Property(q => q.UserIds).HasMaxLength(NotificationServiceConsts.MaxUserIdsLength);
            b.Property(q => q.ExcludedUserIds).HasMaxLength(NotificationServiceConsts.MaxUserIdsLength);
            b.Property(q => q.TenantIds).HasMaxLength(NotificationServiceConsts.MaxTenantIdsLength);
            b.Property(q => q.TargetNotifiers).HasMaxLength(NotificationServiceConsts.MaxTargetNotifiersLength);

            //Indexes
            b.HasIndex(q => q.NotificationName);
        });

        builder.Entity<NotificationSubscription>(b =>
        {
            //Configure table & schema name
            b.ToTable(NotificationServiceDbProperties.DbTablePrefix + "NotificationSubscriptions", NotificationServiceDbProperties.DbSchema);

            b.ConfigureByConvention();

            //Properties
            b.Property(q => q.NotificationName).IsRequired().HasMaxLength(NotificationServiceConsts.MaxNotificationNameLength);
            b.Property(q => q.EntityTypeName).HasMaxLength(NotificationServiceConsts.MaxEntityTypeNameLength);
            b.Property(q => q.EntityTypeAssemblyQualifiedName).HasMaxLength(NotificationServiceConsts.MaxEntityTypeAssemblyQualifiedNameLength);
            b.Property(q => q.EntityId).HasMaxLength(NotificationServiceConsts.MaxEntityIdLength);
            
            //Relations
            b.HasOne<IdentityUser>().WithMany().HasForeignKey(x => x.UserId).IsRequired();

            //Indexes
            b.HasIndex(q => q.UserId);
            b.HasIndex(q => q.NotificationName);
        });

        builder.Entity<UserNotification>(b =>
        {
            //Configure table & schema name
            b.ToTable(NotificationServiceDbProperties.DbTablePrefix + "UserNotifications", NotificationServiceDbProperties.DbSchema);

            b.ConfigureByConvention();

            //Properties
            b.Property(q => q.TargetNotifiers).HasMaxLength(NotificationServiceConsts.MaxTargetNotifiersLength);
            b.Property(q => q.State).HasConversion(new EnumToStringConverter<UserNotificationState>());

            //Relations
            b.HasOne<IdentityUser>().WithMany().HasForeignKey(x => x.UserId).IsRequired();
            b.HasOne<TenantNotification>().WithMany().HasForeignKey(x => x.TenantNotificationId).IsRequired();

            //Indexes
            b.HasIndex(q => q.UserId);
            b.HasIndex(q => q.TenantNotificationId);
        });

        builder.Entity<TenantNotification>(b =>
        {
            //Configure table & schema name
            b.ToTable(NotificationServiceDbProperties.DbTablePrefix + "TenantNotifications", NotificationServiceDbProperties.DbSchema);

            b.ConfigureByConvention();

            //Properties
            b.Property(q => q.NotificationName).IsRequired().HasMaxLength(NotificationServiceConsts.MaxNotificationNameLength);
            b.Property(q => q.Data).IsRequired().HasMaxLength(NotificationServiceConsts.MaxDataLength);
            b.Property(q => q.DataTypeName).IsRequired().HasMaxLength(NotificationServiceConsts.MaxDataTypeNameLength);
            b.Property(q => q.EntityTypeName).HasMaxLength(NotificationServiceConsts.MaxEntityTypeNameLength);
            b.Property(q => q.EntityTypeAssemblyQualifiedName).HasMaxLength(NotificationServiceConsts.MaxEntityTypeAssemblyQualifiedNameLength);
            b.Property(q => q.EntityId).HasMaxLength(NotificationServiceConsts.MaxEntityIdLength);
            b.Property(q => q.Severity).HasConversion(new EnumToStringConverter<NotificationSeverity>());

            //Indexes
            b.HasIndex(q => q.NotificationName);
        });
    }
}
