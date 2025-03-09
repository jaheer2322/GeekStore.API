﻿// <auto-generated />
using System;
using GeekStore.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GeekStore.API.Migrations
{
    [DbContext(typeof(GeekStoreDbContext))]
    [Migration("20250112061424_Adding images table")]
    partial class Addingimagestable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("GeekStore.API.Models.Domains.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            Id = new Guid("6a3fb4b3-2c2b-4f0e-8cbb-9b4d914729b1"),
                            Name = "CPU"
                        },
                        new
                        {
                            Id = new Guid("1992b5e0-7888-476b-a46d-ce812e8d7b6d"),
                            Name = "GPU"
                        },
                        new
                        {
                            Id = new Guid("9e336f6c-e645-49a7-bd6f-38f79cdf548a"),
                            Name = "PSU"
                        },
                        new
                        {
                            Id = new Guid("8499e196-2cb1-45ad-b7bd-a82a0bb48745"),
                            Name = "Motherboard"
                        },
                        new
                        {
                            Id = new Guid("5ec4a3f7-b00a-47a3-aa3d-d946030ca55c"),
                            Name = "Ram"
                        },
                        new
                        {
                            Id = new Guid("be730ab1-9f45-41ab-a094-bc1b8a301a03"),
                            Name = "Graphics card"
                        },
                        new
                        {
                            Id = new Guid("a24ad4ff-ad4a-4dd7-8ac0-53a6216ab93f"),
                            Name = "Miscellaneous"
                        });
                });

            modelBuilder.Entity("GeekStore.API.Models.Domains.Image", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("FileDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FileExtension")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("FileSizeInBytes")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("GeekStore.API.Models.Domains.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<Guid>("TierId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("TierId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("GeekStore.API.Models.Domains.Tier", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Tiers");

                    b.HasData(
                        new
                        {
                            Id = new Guid("05547253-358b-4923-b34f-9abf8b96fb61"),
                            Name = "Low end"
                        },
                        new
                        {
                            Id = new Guid("d43469ab-503e-453e-a35a-075752fe84d6"),
                            Name = "Mid end"
                        },
                        new
                        {
                            Id = new Guid("bec04c25-6ba3-46f9-9dd5-273a042cba80"),
                            Name = "High end"
                        });
                });

            modelBuilder.Entity("GeekStore.API.Models.Domains.Product", b =>
                {
                    b.HasOne("GeekStore.API.Models.Domains.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GeekStore.API.Models.Domains.Tier", "Tier")
                        .WithMany()
                        .HasForeignKey("TierId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Tier");
                });
#pragma warning restore 612, 618
        }
    }
}
