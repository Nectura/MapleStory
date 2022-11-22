﻿// <auto-generated />
using System;
using Common.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Common.Migrations
{
    [DbContext(typeof(EntityContext))]
    partial class EntityContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Common.Database.Models.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("AccountType")
                        .HasColumnType("int");

                    b.Property<int>("CharacterSlots")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("HasAcceptedEula")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("LastKnownIpAddress")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("LastLoggedInAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("LastWorldId")
                        .HasColumnType("int");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<byte[]>("PasswordSaltHash")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<byte[]>("PicHash")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<byte[]>("PicSaltHash")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<byte[]>("PinHash")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<byte[]>("PinSaltHash")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<Guid?>("RestrictionId")
                        .HasColumnType("char(36)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("RestrictionId")
                        .IsUnique();

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("Common.Database.Models.AccountRestriction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("AccountId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("ExpirationTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("IssuedByAccountId")
                        .HasColumnType("int");

                    b.Property<string>("Reason")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("AccountRestrictions");
                });

            modelBuilder.Entity("Common.Database.Models.Character", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<ushort>("AbilityPoints")
                        .HasColumnType("smallint unsigned");

                    b.Property<int>("AccountId")
                        .HasColumnType("int");

                    b.Property<byte>("BuddyLimit")
                        .HasColumnType("tinyint unsigned");

                    b.Property<byte>("CashSlots")
                        .HasColumnType("tinyint unsigned");

                    b.Property<ushort>("Dexterity")
                        .HasColumnType("smallint unsigned");

                    b.Property<byte>("EquipmentSlots")
                        .HasColumnType("tinyint unsigned");

                    b.Property<byte>("EtceteraSlots")
                        .HasColumnType("tinyint unsigned");

                    b.Property<uint>("Experience")
                        .HasColumnType("int unsigned");

                    b.Property<bool>("ExperienceLocked")
                        .HasColumnType("tinyint(1)");

                    b.Property<uint>("Face")
                        .HasColumnType("int unsigned");

                    b.Property<ushort>("Fame")
                        .HasColumnType("smallint unsigned");

                    b.Property<ushort>("Foothold")
                        .HasColumnType("smallint unsigned");

                    b.Property<uint>("GachaponExperience")
                        .HasColumnType("int unsigned");

                    b.Property<int>("Gender")
                        .HasColumnType("int");

                    b.Property<byte>("HairColor")
                        .HasColumnType("tinyint unsigned");

                    b.Property<uint>("HairStyle")
                        .HasColumnType("int unsigned");

                    b.Property<ushort>("HitPoints")
                        .HasColumnType("smallint unsigned");

                    b.Property<ushort>("Intelligence")
                        .HasColumnType("smallint unsigned");

                    b.Property<ushort>("Job")
                        .HasColumnType("smallint unsigned");

                    b.Property<byte>("Level")
                        .HasColumnType("tinyint unsigned");

                    b.Property<bool>("LevelLocked")
                        .HasColumnType("tinyint(1)");

                    b.Property<ushort>("Luck")
                        .HasColumnType("smallint unsigned");

                    b.Property<ushort>("ManaPoints")
                        .HasColumnType("smallint unsigned");

                    b.Property<uint>("MapId")
                        .HasColumnType("int unsigned");

                    b.Property<ushort>("MaxHitPoints")
                        .HasColumnType("smallint unsigned");

                    b.Property<ushort>("MaxManaPoints")
                        .HasColumnType("smallint unsigned");

                    b.Property<uint>("Mesos")
                        .HasColumnType("int unsigned");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<byte>("SetupSlots")
                        .HasColumnType("tinyint unsigned");

                    b.Property<ushort>("SkillPoints")
                        .HasColumnType("smallint unsigned");

                    b.Property<byte>("SkinColor")
                        .HasColumnType("tinyint unsigned");

                    b.Property<byte>("SpawnPoint")
                        .HasColumnType("tinyint unsigned");

                    b.Property<byte>("Stance")
                        .HasColumnType("tinyint unsigned");

                    b.Property<ushort>("Strength")
                        .HasColumnType("smallint unsigned");

                    b.Property<ushort>("SubJob")
                        .HasColumnType("smallint unsigned");

                    b.Property<byte>("UsableSlots")
                        .HasColumnType("tinyint unsigned");

                    b.Property<int>("WorldId")
                        .HasColumnType("int");

                    b.Property<ushort>("X")
                        .HasColumnType("smallint unsigned");

                    b.Property<ushort>("Y")
                        .HasColumnType("smallint unsigned");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("Characters");
                });

            modelBuilder.Entity("Common.Database.Models.Account", b =>
                {
                    b.HasOne("Common.Database.Models.AccountRestriction", "Restriction")
                        .WithOne("Account")
                        .HasForeignKey("Common.Database.Models.Account", "RestrictionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Restriction");
                });

            modelBuilder.Entity("Common.Database.Models.Character", b =>
                {
                    b.HasOne("Common.Database.Models.Account", "Account")
                        .WithMany("Characters")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("Common.Database.Models.Account", b =>
                {
                    b.Navigation("Characters");
                });

            modelBuilder.Entity("Common.Database.Models.AccountRestriction", b =>
                {
                    b.Navigation("Account");
                });
#pragma warning restore 612, 618
        }
    }
}
