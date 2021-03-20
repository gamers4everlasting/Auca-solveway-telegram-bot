using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using TelegramBot.DAL.Entities;

namespace TelegramBot.DAL.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(key => key.Id);
            builder.HasIndex(index => index.SolvewayUserId).IsUnique();
            builder.HasIndex(index => index.TelegramUserId).IsUnique();
        }
    }
}
