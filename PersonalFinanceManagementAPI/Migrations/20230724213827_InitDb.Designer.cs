﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PersonalFinanceManagementAPI.Database;

#nullable disable

namespace PersonalFinanceManagementAPI.Migrations
{
    [DbContext(typeof(PFMDbContext))]
    [Migration("20230724213827_InitDb")]
    partial class InitDb
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PersonalFinanceManagementAPI.Database.Entities.CategoriesEntity", b =>
                {
                    b.Property<string>("Code")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(3)
                        .HasColumnType("character varying(3)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<string>("ParentCode")
                        .IsRequired()
                        .HasMaxLength(1)
                        .HasColumnType("character varying(1)");

                    b.HasKey("Code");

                    b.ToTable("categories", (string)null);
                });

            modelBuilder.Entity("PersonalFinanceManagementAPI.Database.Entities.SplitsEntity", b =>
                {
                    b.Property<string>("TransactionId")
                        .HasColumnType("character varying(32)");

                    b.Property<string>("Catcode")
                        .HasColumnType("text");

                    b.Property<double>("Amount")
                        .HasColumnType("double precision");

                    b.Property<string>("categoryCode")
                        .IsRequired()
                        .HasColumnType("character varying(3)");

                    b.HasKey("TransactionId", "Catcode");

                    b.HasIndex("categoryCode");

                    b.ToTable("splitsTransactions", (string)null);
                });

            modelBuilder.Entity("PersonalFinanceManagementAPI.Database.Entities.TransactionsEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<double>("Amount")
                        .HasColumnType("double precision");

                    b.Property<string>("BeneficiaryName")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<string>("Catcode")
                        .HasColumnType("text");

                    b.Property<string>("Currency")
                        .IsRequired()
                        .HasMaxLength(3)
                        .HasColumnType("character varying(3)");

                    b.Property<DateTime>("Date")
                        .HasMaxLength(20)
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(1024)
                        .HasColumnType("character varying(1024)");

                    b.Property<string>("Direction")
                        .IsRequired()
                        .HasColumnType("character varying(64)");

                    b.Property<string>("Kind")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("Mcc")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("transactions", (string)null);
                });

            modelBuilder.Entity("PersonalFinanceManagementAPI.Database.Entities.SplitsEntity", b =>
                {
                    b.HasOne("PersonalFinanceManagementAPI.Database.Entities.TransactionsEntity", "transaction")
                        .WithMany("Splits")
                        .HasForeignKey("TransactionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PersonalFinanceManagementAPI.Database.Entities.CategoriesEntity", "category")
                        .WithMany("Splits")
                        .HasForeignKey("categoryCode")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("category");

                    b.Navigation("transaction");
                });

            modelBuilder.Entity("PersonalFinanceManagementAPI.Database.Entities.CategoriesEntity", b =>
                {
                    b.Navigation("Splits");
                });

            modelBuilder.Entity("PersonalFinanceManagementAPI.Database.Entities.TransactionsEntity", b =>
                {
                    b.Navigation("Splits");
                });
#pragma warning restore 612, 618
        }
    }
}