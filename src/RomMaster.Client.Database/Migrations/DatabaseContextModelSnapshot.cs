﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RomMaster.Client.Database;

namespace RomMaster.Client.Database.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846");

            modelBuilder.Entity("RomMaster.Client.Database.Models.Dat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author");

                    b.Property<string>("Category");

                    b.Property<DateTime>("Date");

                    b.Property<string>("Description");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Version")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Dat");
                });

            modelBuilder.Entity("RomMaster.Client.Database.Models.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("DatId");

                    b.Property<string>("Description");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int?>("Year");

                    b.HasKey("Id");

                    b.HasIndex("DatId");

                    b.ToTable("Game");
                });

            modelBuilder.Entity("RomMaster.Client.Database.Models.Rom", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Crc")
                        .IsRequired();

                    b.Property<int?>("GameId");

                    b.Property<string>("Md5")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Sha1")
                        .IsRequired();

                    b.Property<int>("Size");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("Rom");
                });

            modelBuilder.Entity("RomMaster.Client.Database.Models.Game", b =>
                {
                    b.HasOne("RomMaster.Client.Database.Models.Dat")
                        .WithMany("Games")
                        .HasForeignKey("DatId");
                });

            modelBuilder.Entity("RomMaster.Client.Database.Models.Rom", b =>
                {
                    b.HasOne("RomMaster.Client.Database.Models.Game")
                        .WithMany("Roms")
                        .HasForeignKey("GameId");
                });
#pragma warning restore 612, 618
        }
    }
}
