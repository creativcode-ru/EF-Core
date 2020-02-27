﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsotoDbFirst.Models
{
    public partial class Department
    {
        public Department()
        {
            Course = new HashSet<Course>();
        }

        [Key]
        [Column("DepartmentID")]
        public int DepartmentId { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        [Column(TypeName = "money")]
        public decimal Budget { get; set; }
        public DateTime StartDate { get; set; }
        [Column("InstructorID")]
        public int? InstructorId { get; set; }
        public byte[] RowVersion { get; set; }

        [ForeignKey(nameof(InstructorId))]
        [InverseProperty("Department")]
        public virtual Instructor Instructor { get; set; }
        [InverseProperty("Department")]
        public virtual ICollection<Course> Course { get; set; }
    }
}