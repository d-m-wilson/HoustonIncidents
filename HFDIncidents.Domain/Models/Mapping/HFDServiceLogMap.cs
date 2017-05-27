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
    public class HFDServiceLogMap : EntityTypeConfiguration<HFDServiceLog>
    {
        public HFDServiceLogMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Source)
                .HasMaxLength(50);

            this.Property(t => t.Message)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("HFDServiceLog");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.LoggedDT).HasColumnName("LoggedDT");
            this.Property(t => t.Level).HasColumnName("Level");
            this.Property(t => t.Source).HasColumnName("Source");
            this.Property(t => t.Message).HasColumnName("Message");
            this.Property(t => t.ExceptionText).HasColumnName("ExceptionText");
        }
    }
}
