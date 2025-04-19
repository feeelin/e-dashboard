﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tulahack.API.Context;

#nullable disable

namespace Tulahack.API.Context.Migrations
{
    [DbContext(typeof(TulahackContext))]
    [Migration("20250419170411_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.4");

            modelBuilder.Entity("Tulahack.API.Models.Account", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("About")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("Blocked")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Firstname")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Lastname")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Middlename")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PhotoUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Role")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TelegramAccount")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Accounts", (string)null);

                    b.HasDiscriminator<int>("Role").HasValue(0);

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Tulahack.API.Models.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("ActualTime")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("AnalyticsTime")
                        .HasColumnType("TEXT");

                    b.Property<int>("BugsAfterReleaseCounter")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DevelopmentTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("PlannedTime")
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TaskLifecycle")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TeamCapacity")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TeamEfficiency")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TeamWorkload")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("TestingTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("TimelineLabel")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("Tulahack.API.Models.ProjectTask", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Actual")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ActualTime")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Assignee")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Estimated")
                        .HasColumnType("INTEGER");

                    b.Property<int>("EstimationAccuracy")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Priority")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ProjectId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TaskLabel")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TaskType")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TimeTracked")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("ProjectTasks");
                });

            modelBuilder.Entity("Tulahack.API.Models.StorageFile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Filename")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Filepath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("Owner")
                        .HasColumnType("TEXT");

                    b.Property<int>("Purpose")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Revision")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("StorageFiles", (string)null);
                });

            modelBuilder.Entity("Tulahack.API.Models.Team", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AssigneeWithOverdueTasks")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MembersCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("UnassignedMembers")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("Tulahack.API.Models.TimelineItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Extra")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("ItemType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("ProjectId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("TimelineItems");
                });

            modelBuilder.Entity("Tulahack.API.Models.Manager", b =>
                {
                    b.HasBaseType("Tulahack.API.Models.Account");

                    b.Property<bool>("CertificateNeeded")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ManagerNumber")
                        .HasColumnType("INTEGER");

                    b.HasDiscriminator().HasValue(1);
                });

            modelBuilder.Entity("Tulahack.API.Models.Superuser", b =>
                {
                    b.HasBaseType("Tulahack.API.Models.Account");

                    b.HasDiscriminator().HasValue(2);
                });

            modelBuilder.Entity("Tulahack.API.Models.ProjectTask", b =>
                {
                    b.HasOne("Tulahack.API.Models.Project", null)
                        .WithMany("ProjectTasks")
                        .HasForeignKey("ProjectId");
                });

            modelBuilder.Entity("Tulahack.API.Models.TimelineItem", b =>
                {
                    b.HasOne("Tulahack.API.Models.Project", null)
                        .WithMany("TimelineItems")
                        .HasForeignKey("ProjectId");
                });

            modelBuilder.Entity("Tulahack.API.Models.Project", b =>
                {
                    b.Navigation("ProjectTasks");

                    b.Navigation("TimelineItems");
                });
#pragma warning restore 612, 618
        }
    }
}
