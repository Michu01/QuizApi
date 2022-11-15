﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QuizApi.DbContexts;

#nullable disable

namespace QuizApi.Migrations
{
    [DbContext(typeof(QuizDbContext))]
    partial class QuizDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("QuizApi.DTOs.FriendshipDTO", b =>
                {
                    b.Property<int>("MeId")
                        .HasColumnType("int");

                    b.Property<int>("TheyId")
                        .HasColumnType("int");

                    b.HasKey("MeId", "TheyId");

                    b.HasIndex("TheyId");

                    b.ToTable("Friendships");
                });

            modelBuilder.Entity("QuizApi.DTOs.QuestionDTO", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AnswerA")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("AnswerB")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("AnswerC")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("AnswerD")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("Contents")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<int>("CorrectAnswer")
                        .HasColumnType("int");

                    b.Property<int>("QuestionSetId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("QuestionSetId");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("QuizApi.DTOs.QuestionSetCategoryDTO", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("QuestionSetCategories");
                });

            modelBuilder.Entity("QuizApi.DTOs.QuestionSetDTO", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Access")
                        .HasColumnType("int");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatorId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("CreatorId");

                    b.ToTable("QuestionSets");
                });

            modelBuilder.Entity("QuizApi.DTOs.UserDTO", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("JoinDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(84)
                        .HasColumnType("nvarchar(84)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("QuizApi.DTOs.FriendshipDTO", b =>
                {
                    b.HasOne("QuizApi.DTOs.UserDTO", "Me")
                        .WithMany("Friendships")
                        .HasForeignKey("MeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("QuizApi.DTOs.UserDTO", "They")
                        .WithMany()
                        .HasForeignKey("TheyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Me");

                    b.Navigation("They");
                });

            modelBuilder.Entity("QuizApi.DTOs.QuestionDTO", b =>
                {
                    b.HasOne("QuizApi.DTOs.QuestionSetDTO", "QuestionSet")
                        .WithMany("Questions")
                        .HasForeignKey("QuestionSetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("QuestionSet");
                });

            modelBuilder.Entity("QuizApi.DTOs.QuestionSetDTO", b =>
                {
                    b.HasOne("QuizApi.DTOs.QuestionSetCategoryDTO", "Category")
                        .WithMany("QuestionSets")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("QuizApi.DTOs.UserDTO", "Creator")
                        .WithMany("QuestionSets")
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Creator");
                });

            modelBuilder.Entity("QuizApi.DTOs.QuestionSetCategoryDTO", b =>
                {
                    b.Navigation("QuestionSets");
                });

            modelBuilder.Entity("QuizApi.DTOs.QuestionSetDTO", b =>
                {
                    b.Navigation("Questions");
                });

            modelBuilder.Entity("QuizApi.DTOs.UserDTO", b =>
                {
                    b.Navigation("Friendships");

                    b.Navigation("QuestionSets");
                });
#pragma warning restore 612, 618
        }
    }
}
