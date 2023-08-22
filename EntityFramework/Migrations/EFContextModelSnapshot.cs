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

            modelBuilder.HasSequence("TableArticleQuantitySequence");

            modelBuilder.Entity("ArticleDetailsTableArticleQuantity", b =>
                {
                    b.Property<int>("ArticleDetailsID")
                        .HasColumnType("int");

                    b.Property<int>("TableArticleQuantitiesID")
                        .HasColumnType("int");

                    b.HasKey("ArticleDetailsID", "TableArticleQuantitiesID");

                    b.HasIndex("TableArticleQuantitiesID");

                    b.ToTable("ArticleDetailsTableArticleQuantity");
                });

            modelBuilder.Entity("EntityFramework.Models.Article", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<long>("Barcode")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("CreatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DataEntryID")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ModifiedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2 )");

                    b.HasKey("ID");

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

                    b.Property<DateTime?>("CreatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("EntryPrice")
                        .HasColumnType("decimal(18,2 )");

                    b.Property<DateTime?>("ModifiedDateTime")
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

                    b.Property<decimal>("Cash")
                        .HasColumnType("decimal(18,2 )");

                    b.Property<decimal>("Change")
                        .HasColumnType("decimal(18,2 )");

                    b.Property<DateTime?>("CreatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ModifiedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("PaymentType")
                        .HasColumnType("int");

                    b.Property<string>("RegistrationNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TableID")
                        .HasColumnType("int");

                    b.Property<decimal>("TotalPrice")
                        .HasColumnType("decimal(18,2 )");

                    b.HasKey("ID");

                    b.HasIndex("TableID");

                    b.ToTable("Bills");
                });

            modelBuilder.Entity("EntityFramework.Models.Configuration", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<int>("BillCounter")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CurrentDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ModifiedDateTime")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.ToTable("Configurations");
                });

            modelBuilder.Entity("EntityFramework.Models.DataEntry", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<DateTime?>("CreatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("DataEntryNumber")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ModifiedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("TotalAmount")
                        .HasColumnType("decimal(18,2 )");

                    b.HasKey("ID");

                    b.ToTable("DataEntries");
                });

            modelBuilder.Entity("EntityFramework.Models.Table", b =>
                {
                    b.Property<int>("ID")
                        .HasColumnType("int");

                    b.Property<int?>("ArticleID")
                        .HasColumnType("int");

                    b.Property<bool>("Available")
                        .HasColumnType("bit");

                    b.HasKey("ID");

                    b.HasIndex("ArticleID");

                    b.ToTable("Tables");
                });

            modelBuilder.Entity("EntityFramework.Models.TableArticleQuantity", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValueSql("NEXT VALUE FOR [TableArticleQuantitySequence]");

                    SqlServerPropertyBuilderExtensions.UseSequence(b.Property<int>("ID"));

                    b.Property<int>("ArticleID")
                        .HasColumnType("int");

                    b.Property<int?>("BillID")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("TableID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("ArticleID");

                    b.HasIndex("BillID");

                    b.HasIndex("TableID");

                    b.ToTable("TableArticleQuantity");

                    b.UseTpcMappingStrategy();
                });

            modelBuilder.Entity("EntityFramework.Models.Waiter", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<long>("Barcode")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("CreatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("FirstAndLastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("JMBG")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("ModifiedDateTime")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.ToTable("Waiters");
                });

            modelBuilder.Entity("EntityFramework.Models.SoldTableArticleQuantity", b =>
                {
                    b.HasBaseType("EntityFramework.Models.TableArticleQuantity");

                    b.ToTable("SoldTableArticleQuantity");
                });

            modelBuilder.Entity("ArticleDetailsTableArticleQuantity", b =>
                {
                    b.HasOne("EntityFramework.Models.ArticleDetails", null)
                        .WithMany()
                        .HasForeignKey("ArticleDetailsID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EntityFramework.Models.TableArticleQuantity", null)
                        .WithMany()
                        .HasForeignKey("TableArticleQuantitiesID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("EntityFramework.Models.Article", b =>
                {
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

            modelBuilder.Entity("EntityFramework.Models.Bill", b =>
                {
                    b.HasOne("EntityFramework.Models.Table", "Table")
                        .WithMany()
                        .HasForeignKey("TableID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Table");
                });

            modelBuilder.Entity("EntityFramework.Models.Table", b =>
                {
                    b.HasOne("EntityFramework.Models.Article", null)
                        .WithMany("Tables")
                        .HasForeignKey("ArticleID");
                });

            modelBuilder.Entity("EntityFramework.Models.TableArticleQuantity", b =>
                {
                    b.HasOne("EntityFramework.Models.Article", "Article")
                        .WithMany()
                        .HasForeignKey("ArticleID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EntityFramework.Models.Bill", "Bill")
                        .WithMany()
                        .HasForeignKey("BillID");

                    b.HasOne("EntityFramework.Models.Table", "Table")
                        .WithMany("TableArticleQuantities")
                        .HasForeignKey("TableID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Article");

                    b.Navigation("Bill");

                    b.Navigation("Table");
                });

            modelBuilder.Entity("EntityFramework.Models.Article", b =>
                {
                    b.Navigation("ArticleDetails");

                    b.Navigation("Tables");
                });

            modelBuilder.Entity("EntityFramework.Models.DataEntry", b =>
                {
                    b.Navigation("Articles");
                });

            modelBuilder.Entity("EntityFramework.Models.Table", b =>
                {
                    b.Navigation("TableArticleQuantities");
                });
#pragma warning restore 612, 618
        }
    }
}
