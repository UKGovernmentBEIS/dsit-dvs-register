﻿// <auto-generated />
using System;
using DVSRegister.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    [DbContext(typeof(DVSRegisterDbContext))]
    partial class DVSRegisterDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DVSRegister.Data.Entities.PreAssessment", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ID"));

                    b.Property<bool?>("ASP")
                        .HasColumnType("boolean");

                    b.Property<string>("AltContactEmail")
                        .HasColumnType("varchar(254)");

                    b.Property<string>("AltContactRole")
                        .HasColumnType("text");

                    b.Property<string>("AltContactTelephoneNumber")
                        .HasColumnType("text");

                    b.Property<string>("CompanyRegistrationNumber")
                        .IsRequired()
                        .HasColumnType("varchar(8)");

                    b.Property<int>("ConfirmAccuracy")
                        .HasColumnType("integer");

                    b.Property<int>("ConfirmLegalRequirements")
                        .HasColumnType("integer");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DIPForeignJurisdictionID")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("GeographicalAreas")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool?>("IDSP")
                        .HasColumnType("boolean");

                    b.Property<string>("ModifiedBy")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("ModifiedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool?>("OSP")
                        .HasColumnType("boolean");

                    b.Property<bool?>("Other")
                        .HasColumnType("boolean");

                    b.Property<string>("OtherRoleDescription")
                        .HasColumnType("text");

                    b.Property<int>("PreAssessmentStatus")
                        .HasColumnType("integer");

                    b.Property<string>("RegisteredCompanyName")
                        .IsRequired()
                        .HasColumnType("varchar(160)");

                    b.Property<string>("SROEmail")
                        .IsRequired()
                        .HasColumnType("varchar(254)");

                    b.Property<string>("SROFullName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SRORole")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SROTelephoneNumber")
                        .HasColumnType("text");

                    b.Property<string>("URN")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool?>("WalletProvider")
                        .HasColumnType("boolean");

                    b.HasKey("ID");

                    b.ToTable("PreAssessment");
                });
#pragma warning restore 612, 618
        }
    }
}
