// HFD Incidents
// Copyright © 2016 David M. Wilson
// https://twitter.com/dmwilson_dev
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System.Data.Entity.ModelConfiguration;

namespace HFDIncidents.Domain.Models.Mapping
{
    public class ArchivedIncidentMap : EntityTypeConfiguration<ArchivedIncident>
    {
        public ArchivedIncidentMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Address)
                .HasMaxLength(255);

            this.Property(t => t.CrossStreet)
                .HasMaxLength(255);

            this.Property(t => t.KeyMap)
                .HasMaxLength(10);

            this.Property(t => t.Units)
                .HasMaxLength(255);

            this.Property(t => t.Status)
                .HasMaxLength(20);

            this.Property(t => t.Notes)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("ArchivedIncident");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.RetrievedDT).HasColumnName("RetrievedDT");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.CrossStreet).HasColumnName("CrossStreet");
            this.Property(t => t.KeyMap).HasColumnName("KeyMap");
            this.Property(t => t.Latitude).HasColumnName("Latitude");
            this.Property(t => t.Longitude).HasColumnName("Longitude");
            this.Property(t => t.CombinedResponse).HasColumnName("CombinedResponse");
            this.Property(t => t.CallTimeOpened).HasColumnName("CallTimeOpened");
            this.Property(t => t.IncidentTypeId).HasColumnName("IncidentTypeId");
            this.Property(t => t.AlarmLevel).HasColumnName("AlarmLevel");
            this.Property(t => t.NumberOfUnits).HasColumnName("NumberOfUnits");
            this.Property(t => t.Units).HasColumnName("Units");
            this.Property(t => t.LastSeenDT).HasColumnName("LastSeenDT");
            this.Property(t => t.ArchivedDT).HasColumnName("ArchivedDT");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.Notes).HasColumnName("Notes");

            // Relationships
            this.HasOptional(t => t.IncidentType)
                .WithMany(t => t.ArchivedIncidents)
                .HasForeignKey(d => d.IncidentTypeId);

        }
    }
}
