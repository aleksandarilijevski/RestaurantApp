﻿// <auto-generated />
using System;
using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EntityFramework.Migrations
{
    [DbContext(typeof(EFContext))]
    partial class EFContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ArticleTable", b =>
                {
                    b.Property<int>("ArticlesID")
                        .HasColumnType("int");

                    b.Property<int>("TablesID")
                        .HasColumnType("int");

                    b.HasKey("ArticlesID", "TablesID");

                    b.HasIndex("TablesID");

                    b.ToTable("ArticleTable");
                });

            modelBuilder.Entity("EntityFramework.Models.Article", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<long>("Barcode")
                        .HasColumnType("bigint");

                    b.Property<int?>("BillID")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DataEntryID")
                        .HasColumnType("int");

                    b.Property<DateTime>("ModifiedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("BillID");

                    b.HasIndex("DataEntryID");

                    b.ToTable("Articles");
                });

            modelBuilder.Entity("EntityFramework.Models.ArticleDetails", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<int?>("ArticleID")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("EntryPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("ModifiedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("ArticleID");

                    b.ToTable("ArticleDetails");
                });

            modelBuilder.Entity("EntityFramework.Models.Bill", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ModifiedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("TotalPrice")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("ID");

                    b.ToTable("Bills");
                });

            modelBuilder.Entity("EntityFramework.Models.DataEntry", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("DataEntryNumber")
                        .HasColumnType("int");

                    b.Property<DateTime>("ModifiedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("TotalAmount")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("ID");

                    b.ToTable("DataEntries");
                });

            modelBuilder.Entity("EntityFramework.Models.Table", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<bool>("Available")
                        .HasColumnType("bit");

                    b.Property<int?>("BillID")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ModifiedDateTime")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.HasIndex("BillID");

                    b.ToTable("Tables");
                });

            modelBuilder.Entity("EntityFramework.Models.Waiter", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<long>("Barcode")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("FirstAndLastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("JMBG")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("ModifiedDateTime")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.ToTable("Waiters");
                });

            modelBuilder.Entity("ArticleTable", b =>
                {
                    b.HasOne("EntityFramework.Models.Article", null)
                        .WithMany()
                        .HasForeignKey("ArticlesID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EntityFramework.Models.Table", null)
                        .WithMany()
                        .HasForeignKey("TablesID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("EntityFramework.Models.Article", b =>
                {
                    b.HasOne("EntityFramework.Models.Bill", null)
                        .WithMany("BoughtArticles")
                        .HasForeignKey("BillID");

                    b.HasOne("EntityFramework.Models.DataEntry", null)
                        .WithMany("Articles")
                        .HasForeignKey("DataEntryID");
                });

            modelBuilder.Entity("EntityFramework.Models.ArticleDetails", b =>
                {
                    b.HasOne("EntityFramework.Models.Article", "Article")
                        .WithMany("ArticleDetails")
                        .HasForeignKey("ArticleID");

                    b.Navigation("Article");
                });

            modelBuilder.Entity("EntityFramework.Models.Table", b =>
                {
                    b.HasOne("EntityFramework.Models.Bill", "Bill")
                        .WithMany()
                        .HasForeignKey("BillID");

                    b.Navigation("Bill");
                });

            modelBuilder.Entity("EntityFramework.Models.Article", b =>
                {
                    b.Navigation("ArticleDetails");
                });

            modelBuilder.Entity("EntityFramework.Models.Bill", b =>
                {
                    b.Navigation("BoughtArticles");
                });

            modelBuilder.Entity("EntityFramework.Models.DataEntry", b =>
                {
                    b.Navigation("Articles");
                });
#pragma warning restore 612, 618
        }
    }
}
