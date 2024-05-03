using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Demo_Test.Models;

[Table("students")]
public partial class Student
{
    [Key]
    [Column("rollno")]
    public int Rollno { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Column("classid")]
    public int? Classid { get; set; }

    [Column("age")]
    public int? Age { get; set; }

    [Column("pass")]
    public bool? Pass { get; set; }

    [ForeignKey("Classid")]
    [InverseProperty("Students")]
    public virtual StudentClass? Class { get; set; }
}
