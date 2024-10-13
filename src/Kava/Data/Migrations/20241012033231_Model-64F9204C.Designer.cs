﻿// <auto-generated />
using System;
using Kava.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Kava.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241012033231_Model-64F9204C")]
    partial class Model64F9204C
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.10");

            modelBuilder.Entity("Kava.Core.Models.Attachment", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(26)
                        .HasColumnType("TEXT");

                    b.Property<string>("CardId")
                        .IsRequired()
                        .HasMaxLength(26)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("MimeType")
                        .IsRequired()
                        .HasMaxLength(25)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<ulong>("Size")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CardId");

                    b.ToTable("Attachments");
                });

            modelBuilder.Entity("Kava.Core.Models.Board", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(26)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Boards");
                });

            modelBuilder.Entity("Kava.Core.Models.Card", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(26)
                        .HasColumnType("TEXT");

                    b.Property<string>("CategoryId")
                        .IsRequired()
                        .HasMaxLength(26)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Cards");
                });

            modelBuilder.Entity("Kava.Core.Models.Category", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(26)
                        .HasColumnType("TEXT");

                    b.Property<string>("BoardId")
                        .IsRequired()
                        .HasMaxLength(26)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("BoardId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("Kava.Core.Models.Comment", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(26)
                        .HasColumnType("TEXT");

                    b.Property<string>("CardId")
                        .IsRequired()
                        .HasMaxLength(26)
                        .HasColumnType("TEXT");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CardId");

                    b.ToTable("Comment");
                });

            modelBuilder.Entity("Kava.Core.Models.Attachment", b =>
                {
                    b.HasOne("Kava.Core.Models.Card", "Card")
                        .WithMany("Attachments")
                        .HasForeignKey("CardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Card");
                });

            modelBuilder.Entity("Kava.Core.Models.Card", b =>
                {
                    b.HasOne("Kava.Core.Models.Category", "Category")
                        .WithMany("Cards")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("Kava.Core.Models.Category", b =>
                {
                    b.HasOne("Kava.Core.Models.Board", "Board")
                        .WithMany("Categories")
                        .HasForeignKey("BoardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Board");
                });

            modelBuilder.Entity("Kava.Core.Models.Comment", b =>
                {
                    b.HasOne("Kava.Core.Models.Card", "Card")
                        .WithMany("Comments")
                        .HasForeignKey("CardId");

                    b.Navigation("Card");
                });

            modelBuilder.Entity("Kava.Core.Models.Board", b =>
                {
                    b.Navigation("Categories");
                });

            modelBuilder.Entity("Kava.Core.Models.Card", b =>
                {
                    b.Navigation("Attachments");

                    b.Navigation("Comments");
                });

            modelBuilder.Entity("Kava.Core.Models.Category", b =>
                {
                    b.Navigation("Cards");
                });
#pragma warning restore 612, 618
        }
    }
}