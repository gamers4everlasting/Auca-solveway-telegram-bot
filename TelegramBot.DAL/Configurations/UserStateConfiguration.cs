using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using TelegramBot.DAL.Entities;

namespace TelegramBot.DAL.Configurations
{
    public class UserStateConfiguration : IEntityTypeConfiguration<UserState>
    {
        public void Configure(EntityTypeBuilder<UserState> builder)
        {
            builder.HasKey(key => key.Id);

            builder.HasIndex(index => index.UserId).IsUnique(true);
        }
    }
}
