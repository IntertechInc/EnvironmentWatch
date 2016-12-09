using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EnvironmentWatch.Migrations
{
    public partial class FirstMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceType",
                columns: table => new
                {
                    DeviceTypeId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceType", x => x.DeviceTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    LocationId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.LocationId);
                });

            migrationBuilder.CreateTable(
                name: "MeasurementType",
                columns: table => new
                {
                    MeasurementTypeId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementType", x => x.MeasurementTypeId);
                });

            migrationBuilder.CreateTable(
                name: "ReportingDevice",
                columns: table => new
                {
                    ReportingDeviceId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    DeviceTypeId = table.Column<int>(nullable: false),
                    LastIpAddress = table.Column<string>(nullable: true),
                    LocationId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportingDevice", x => x.ReportingDeviceId);
                    table.ForeignKey(
                        name: "FK_ReportingDevice_DeviceType_DeviceTypeId",
                        column: x => x.DeviceTypeId,
                        principalTable: "DeviceType",
                        principalColumn: "DeviceTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportingDevice_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "LocationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Measurement",
                columns: table => new
                {
                    MeasurementId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LocationId = table.Column<int>(nullable: false),
                    MeasuredDate = table.Column<DateTime>(nullable: false),
                    MeasuredValue = table.Column<decimal>(nullable: false),
                    MeasurementTypeId = table.Column<int>(nullable: false),
                    ReportingDeviceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Measurement", x => x.MeasurementId);
                    table.ForeignKey(
                        name: "FK_Measurement_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "LocationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Measurement_MeasurementType_MeasurementTypeId",
                        column: x => x.MeasurementTypeId,
                        principalTable: "MeasurementType",
                        principalColumn: "MeasurementTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Measurement_ReportingDevice_ReportingDeviceId",
                        column: x => x.ReportingDeviceId,
                        principalTable: "ReportingDevice",
                        principalColumn: "ReportingDeviceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Measurement_LocationId",
                table: "Measurement",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Measurement_MeasurementTypeId",
                table: "Measurement",
                column: "MeasurementTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Measurement_ReportingDeviceId",
                table: "Measurement",
                column: "ReportingDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportingDevice_DeviceTypeId",
                table: "ReportingDevice",
                column: "DeviceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportingDevice_LocationId",
                table: "ReportingDevice",
                column: "LocationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Measurement");

            migrationBuilder.DropTable(
                name: "MeasurementType");

            migrationBuilder.DropTable(
                name: "ReportingDevice");

            migrationBuilder.DropTable(
                name: "DeviceType");

            migrationBuilder.DropTable(
                name: "Location");
        }
    }
}
